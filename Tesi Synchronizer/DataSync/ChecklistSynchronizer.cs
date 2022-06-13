using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using Tesi_Synchronizer.Models;
using Dapper;
using System.Data;
using System.Linq;

namespace Synchronizer
{
    class ChecklistSychronizer
    {
        public Logger logger;
        private string keyFonte = Configuration.GetFonte("Fonte").Split('|')[0];
        private string connStrFonte;
        private string connStrDestino;
        private string catalog;
        public bool sucesso = false;
        public TimeSpan t;

        public IDbTransaction CurrentTransaction { get; private set; }

        public ChecklistSychronizer(string keyDestino, ref Logger list)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            logger = list;

            if (keyDestino.Contains("[SUCESS] "))
                keyDestino = keyDestino.Replace("[SUCESS] ", "");

            if (keyDestino.Contains("[FAIL] "))
                keyDestino = keyDestino.Replace("[FAIL] ", "");

            System.Reflection.Assembly executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(executingAssembly.Location);
            var version = fileVersionInfo.FileVersion;
            logger.Log(Logger.Level.Info, string.Format("Versão do Synchronizer {0}", version));

            connStrFonte = Configuration.GetSourceConnectionByKey("Fonte");

            connStrDestino = Configuration.GetDestConnectionByKey(keyDestino);

            catalog = connStrFonte.Split(';')[3].Split('=')[1].ToString();
            logger.Log(Logger.Level.Verbose, string.Format("Sincronização [Checklist] [{0} -> {1}]", keyFonte, keyDestino));


            ////////////////////////////////////////////////////////
            /// Testando se as connection string não está errada ///
            ////////////////////////////////////////////////////////
            try
            {
                SqlConnection connFonte = new SqlConnection(connStrFonte);
                SqlConnection connDestino = new SqlConnection(connStrDestino);
                connFonte.Close();
                connFonte.Dispose();
                connDestino.Close();
                connDestino.Dispose();
            }
            catch
            {
                logger.Log(Logger.Level.Critical, "A Connection String da Fonte, ou do Destino está configurada de forma errada. A Sincronização não será realizada");
                watch.Stop();
                t = watch.Elapsed;
                return;
            }

            ///////////////////////////////
            /// Iniciando Sincronização ///
            ///////////////////////////////
            try
            {
                ////////////////////////
                /// Abrindo Conexões ///
                //////////////////////// 

                logger.Log(Logger.Level.Verbose, string.Format("Testando Conexão com: {0}", keyFonte));
                bool connectionOk = OpenConnection(connStrFonte);
                logger.Log(connectionOk ? Logger.Level.Info : Logger.Level.Error, string.Format("Conxão com {0}: {1}", keyFonte, connectionOk ? "Sucess" : "Fail"));
                if (!connectionOk) //Se a Conexão não Abrir, tenta novamente
                {
                    logger.Log(Logger.Level.Debug, string.Format("Tentando estabelecer Conexão com {0} novamente", keyFonte));
                    connectionOk = OpenConnection(connStrFonte);
                    logger.Log(connectionOk ? Logger.Level.Info : Logger.Level.Error, string.Format("Conxão com {0}: {1}", keyFonte, connectionOk ? "Sucess" : "Fail"));
                    logger.Log(Logger.Level.Critical, "Não foi possivel Conectar com o Banco de Dados. A Sincronização não será realizada");
                    watch.Stop();
                    t = watch.Elapsed;
                    return;
                }

                logger.Log(Logger.Level.Verbose, string.Format("Testando Conexão com: {0}", keyDestino));
                connectionOk = OpenConnection(connStrDestino);
                logger.Log(connectionOk ? Logger.Level.Info : Logger.Level.Error, string.Format("Conxão com {0}: {1}", keyDestino, connectionOk ? "Sucess" : "Fail"));
                if (!connectionOk)//Se a Conexão não Abrir, tenta novamente
                {
                    logger.Log(Logger.Level.Debug, string.Format("Tentando estabelecer Conexão com {0} novamente", keyDestino));
                    connectionOk = OpenConnection(connStrDestino);
                    logger.Log(connectionOk ? Logger.Level.Info : Logger.Level.Error, string.Format("Conxão com {0}: {1}", keyDestino, connectionOk ? "Sucess" : "Fail"));
                    logger.Log(Logger.Level.Critical, "Não foi possivel Conectar com o Banco de Dados. A Sincronização não será realizada");
                    watch.Stop();
                    t = watch.Elapsed;
                    return;
                }


                ////////////////////////////
                /// CHECKLIST ///
                ////////////////////////////

                if (!CopyTable(
                    "CHECKLIST",
                    "ID, CODICE, DESCRIZIONE, PRESENTAZIONE, ITEMALMENOUNO, ITEMPIUDIUNO, CAMPOCL, ORDINE, ELIMINATO, UO",
                    connStrDestino, catalog, connStrFonte))
                {
                    logger.Log(Logger.Level.Critical, "Erro ao tentar Sincronizar [CHECKLIST]. A Sincronização será Encerrada");
                    new ChecklistRollback(keyDestino, ref list);
                    watch.Stop();
                    t = watch.Elapsed;
                    return;
                }

                //////////////////////////////
                ///// CHECKLISTITEMREGOLE ///
                //////////////////////////////

                if (!CopyTable(
                    "CHECKLISTITEMREGOLE",
                    "ID, IDCHECKLISTITEM, IDCHECKLISTITEMBIND, TIPOREGOLA, ELIMINATO",
                    connStrDestino, catalog, connStrFonte))
                {
                    logger.Log(Logger.Level.Critical, "Erro ao tentar Sincronizar [CHECKLISTITEMREGOLE]. A Sincronização será Encerrada");
                    new ChecklistRollback(keyDestino, ref list);
                    watch.Stop();
                    t = watch.Elapsed;
                    return;
                }

                ////////////////////////////////
                /////// CHECKLISTESAMI ///
                ////////////////////////////////

                if (!CopyTable(
                    "CHECKLISTESAMI",
                    "ID, IDCHECKLIST, IDTIPOESAME, ELIMINATO",
                    connStrDestino, catalog, connStrFonte))
                {
                    logger.Log(Logger.Level.Critical, "Erro ao tentar Sincronizar [CHECKLISTESAMI]. A Sincronização será Encerrada");
                    new ChecklistRollback(keyDestino, ref list);
                    watch.Stop();
                    t = watch.Elapsed;
                    return;
                }

                ////////////////////////////////
                /////// CODICICLASSIFICAZIONEDIAGNOST ///
                ////////////////////////////////

                if (!CopyTable(
                    "CODICICLASSIFICAZIONEDIAGNOST",
                    "ID, CODICE, DESCRIZIONE, CLASSIFICAZIONE, POSITIVITA, SCORE, IDTIPOESAME, ELIMINATO, UO",
                    connStrDestino, catalog, connStrFonte))
                {
                    logger.Log(Logger.Level.Critical, "Erro ao tentar Sincronizar [CODICICLASSIFICAZIONEDIAGNOST]. A Sincronização será Encerrada");
                    new ChecklistRollback(keyDestino, ref list);
                    watch.Stop();
                    t = watch.Elapsed;
                    return;
                }

                //////////////////////////////
                ///// CHECKLISTITEM ///
                //////////////////////////////

                if (!CopyTable(
                    "CHECKLISTITEM",
                    "ID, IDCHECKLIST, IDPADRE, ORDINE, TITOLO, TESTORTF, TESTOTXT, TESTONUMEROVARIABILI, ITEMALMENOUNO, ITEMPIUDIUNO, ELIMINATO, CLASSIFICAZIONEDIAGNOSI, IDORIGINALECLONATO",
                    connStrDestino, catalog, connStrFonte))
                {
                    logger.Log(Logger.Level.Critical, "Erro ao tentar Sincronizar [CHECKLISTITEM]. A Sincronização será Encerrada");
                    new ChecklistRollback(keyDestino, ref list);
                    watch.Stop();
                    t = watch.Elapsed;
                    return;
                }

                sucesso = true;
                watch.Stop();
                t = watch.Elapsed;
            }
            catch
            {
                logger.Log(Logger.Level.Critical, "Erro Fatal Durante Sincronização. A Sincronização será Encerrada");
                watch.Stop();
                t = watch.Elapsed;
            }
        }

        private bool OpenConnection(string connStr)
        {
            bool open = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    try
                    {
                        conn.Open();
                        open = true;
                    }
                    catch
                    {
                        open = false;
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                };
            }
            catch
            {
                open = false;
            }
            return open;
        }




        private bool CopyTable(string table, string columns, string connStrDestino, string catalog, string connStrFonte)
        {
            ///////

            var connSplit = connStrFonte.Split(';');
            string fonteServer = connSplit[0].Split('=')[1].ToString();
            string fonteUser = connSplit[1].Split('=')[1].ToString();
            string fontePass = connSplit[2].Split('=')[1].ToString();
            catalog = connSplit[3].Split('=')[1].ToString();



            var connSplit2 = connStrDestino.Split(';');
            string DestServer = connSplit2[0].Split('=')[1].ToString();
            string DestUser = connSplit2[1].Split('=')[1].ToString();
            string DestPass = connSplit2[2].Split('=')[1].ToString();
            string Destcatalog = connSplit2[3].Split('=')[1].ToString();

            ///////

            List<Checklist> checklists = null;
            List<CheckListItemRegole> checklistItemRegole = null;
            List<CheckListEsami> checklistEsami = null;
            List<CheckListItem> checklistItem = null;
            List<CodiciClassificazioneDiagnost> codiciClassificazioneDiagnost = null;


            string tempName = "#TEMP_" + table;
            string masterName = "MASTER" + table;
            string rollbackName = "ROLLBACK_" + table;

            bool copy = false;
            int affectedRows = 0;

            try
            {
                using (SqlConnection connFonte = new SqlConnection(connStrFonte))
                {
                    connFonte.Open();
                    // COPIA TABELA DA FONTE PARA MEMÓRIA //

                    try
                    {
                        logger.Log(Logger.Level.Verbose, $"Copiando {table} do Banco Fonte");

                        switch (table)
                        {
                            case "CHECKLIST":

                                checklists = connFonte.Query<Checklist>($"SELECT {columns} FROM {table}",
                                             transaction: CurrentTransaction,
                                             commandType: CommandType.Text).ToList();

                                affectedRows = checklists.Count();
                                logger.Log(Logger.Level.Info, affectedRows + " rows copiadas");

                                break;

                            case "CHECKLISTITEMREGOLE":

                                checklistItemRegole = connFonte.Query<CheckListItemRegole>($"SELECT {columns} FROM {table}",
                                                      transaction: CurrentTransaction,
                                                      commandType: CommandType.Text).ToList();

                                affectedRows = checklistItemRegole.Count();
                                logger.Log(Logger.Level.Info, affectedRows + " rows copiadas");

                                break;

                            case "CHECKLISTESAMI":

                                checklistEsami = connFonte.Query<CheckListEsami>($"SELECT {columns} FROM {table}",
                                                 transaction: CurrentTransaction,
                                                 commandType: CommandType.Text).ToList();

                                affectedRows = checklistEsami.Count();
                                logger.Log(Logger.Level.Info, affectedRows + " rows copiadas");

                                break;

                            case "CODICICLASSIFICAZIONEDIAGNOST":

                                codiciClassificazioneDiagnost = connFonte.Query<CodiciClassificazioneDiagnost>($"SELECT {columns} FROM {table}",
                                                 transaction: CurrentTransaction,
                                                 commandType: CommandType.Text).ToList();

                                affectedRows = codiciClassificazioneDiagnost.Count();
                                logger.Log(Logger.Level.Info, affectedRows + " rows copiadas");

                                break;

                            case "CHECKLISTITEM":

                                checklistItem = connFonte.Query<CheckListItem>($"SELECT {columns} FROM {table}",
                                                transaction: CurrentTransaction,
                                                commandType: CommandType.Text).ToList();

                                affectedRows = checklistItem.Count();
                                logger.Log(Logger.Level.Info, affectedRows + " rows copiadas");

                                break;
                        }

                    }
                    catch (Exception e)
                    {
                        logger.Log(Logger.Level.Error, "Ocorreu um erro durante a Sincronização da Tabela: " + e.Message);
                        logger.Log(Logger.Level.Error, e.StackTrace);

                        return false;
                    }
                }

                using (SqlConnection connDestino = new SqlConnection(connStrDestino))
                {
                    connDestino.Open();

                    // COPIA OS DADOS DA TABELA DE DESTINO PARA UMA TABELA TEMPORÁRIA //
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        logger.Log(Logger.Level.Verbose, "Copiando " + table);
                        cmd.Connection = connDestino;                        
                        cmd.CommandText = string.Format("SELECT * INTO {0} FROM (SELECT {1} FROM [{2}].[dbo].[{3}]) AS {4}",
                                                         tempName, columns, Destcatalog, table, masterName);

                        affectedRows = cmd.ExecuteNonQuery();
                        logger.Log(Logger.Level.Info, affectedRows + " rows copiadas");
                    };
                    // DELETA A ANTIGA TABELA DE ROLLBACK NO DESTINO //
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.Connection = connDestino;
                            cmd.CommandText = string.Format("DROP TABLE {0}", rollbackName);
                            cmd.ExecuteNonQuery();
                        };
                    }
                    catch
                    {
                        // CASO A TABELA JA TENHA SIDO DELETADA //
                    }

                    // CRIA UM NOVO ROLLBACK PARA A TABELA //
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        logger.Log(Logger.Level.Debug, "Criando tabela de Rollback");
                        cmd.Connection = connDestino;
                        cmd.CommandText = string.Format("SELECT * INTO {0} FROM (SELECT * FROM {1}) AS {0}", rollbackName, table);
                        affectedRows = cmd.ExecuteNonQuery();
                        logger.Log(Logger.Level.Info, affectedRows + " rows copiadas");
                    };

                    // APAGA OS DATOS ANTIGOS DA TABELA DE DESTINO //
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;

                        try
                        {
                            cmd.CommandText = string.Format("SET IDENTITY_INSERT {0} ON", table);
                            cmd.ExecuteNonQuery();
                            logger.Log(Logger.Level.Debug, "Identity ON");
                        }
                        catch (Exception e) { }

                        cmd.CommandText = string.Format("alter table CHECKLIST nocheck constraint all", table);
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = string.Format("alter table CHECKLISTITEMREGOLE nocheck constraint all", table);
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = string.Format("alter table CHECKLISTESAMI nocheck constraint all", table);
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = string.Format("alter table CODICICLASSIFICAZIONEDIAGNOST nocheck constraint all", table);
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = string.Format("alter table CHECKLISTITEM nocheck constraint all", table);
                        cmd.ExecuteNonQuery();
                        logger.Log(Logger.Level.Debug, "Nocheck ativado");

                        logger.Log(Logger.Level.Debug, "Apagando dados antigos da Tabela");
                        cmd.CommandText = string.Format("DELETE FROM {0}", table);
                        cmd.ExecuteNonQuery();

                        try
                        {
                            logger.Log(Logger.Level.Debug, $"Iniciando inserção de dados na tabela {table}...");

                            int count = 1;

                            string QueryText = $"INSERT INTO [{Destcatalog}].[dbo].[{table}] ({columns}) VALUES ";

                            int total = 0;
                            int pack = 0;
                            int progresso = 0;
                            int ultimoProgresso = 0;

                            switch (table)
                            {
                                case "CHECKLIST":

                                    total = checklists.Count;

                                    foreach (var item in checklists)
                                    {
                                        QueryText += $"\n({item.ID},'{item.Codice}','{item.Descrizione}',{item.Presentazione},{(item.ItemAlmenouno ? 1 : 0)},{(item.ItemPiudiuno ? 1 : 0)},{item.CampoCL},{item.Ordine},{(item.Eliminato ? 1 : 0)},{item.UO}),";
                                    
                                        if(count == 1000) 
                                        {
                                            QueryText = QueryText.Substring(0, (QueryText.Length - 1));

                                            connDestino.Query(QueryText,
                                                transaction: CurrentTransaction,
                                                commandType: CommandType.Text);

                                            count = 0;
                                            QueryText = $"INSERT INTO [{Destcatalog}].[dbo].[{table}] ({columns}) VALUES";

                                            pack++;
                                            progresso = (((pack * 1000) * 100) / total);

                                            if (progresso != ultimoProgresso)
                                                logger.Log(Logger.Level.Debug, $"Inserção de dados na tabela {table} em {progresso}%... ");

                                            ultimoProgresso = progresso;
                                        }

                                        count++;
                                    }

                                    break;

                                case "CHECKLISTITEMREGOLE":

                                    total = checklistItemRegole.Count;

                                    foreach (var item in checklistItemRegole)
                                    {
                                        QueryText += $"\n({item.ID},{item.IDChecklistItem},{item.IDChecklistItemBind},{item.TipoRegola},{(item.Eliminato ? 1 : 0)}),";

                                        if (count == 1000)
                                        {
                                            QueryText = QueryText.Substring(0, (QueryText.Length - 1));

                                            connDestino.Query(QueryText,
                                                transaction: CurrentTransaction,
                                                commandType: CommandType.Text);

                                            count = 0;
                                            QueryText = $"INSERT INTO [{Destcatalog}].[dbo].[{table}] ({columns}) VALUES";

                                            pack++;
                                            progresso = (((pack * 1000) * 100) / total);

                                            if (progresso != ultimoProgresso)
                                                logger.Log(Logger.Level.Debug, $"Inserção de dados na tabela {table} em {progresso}%... ");

                                            ultimoProgresso = progresso;
                                        }

                                        count++;
                                    }

                                    break;

                                case "CHECKLISTESAMI":

                                    total = checklistEsami.Count;

                                    foreach (var item in checklistEsami)
                                    {
                                        QueryText += $"\n({item.ID},{item.IDChecklist},{item.IDTipoEsame},{(item.Eliminato ? 1 : 0)}),";

                                        if (count == 1000)
                                        {
                                            QueryText = QueryText.Substring(0, (QueryText.Length - 1));

                                            connDestino.Query(QueryText,
                                                transaction: CurrentTransaction,
                                                commandType: CommandType.Text);

                                            count = 0;
                                            QueryText = $"INSERT INTO [{Destcatalog}].[dbo].[{table}] ({columns}) VALUES";

                                            pack++;
                                            progresso = (((pack * 1000) * 100) / total);

                                            if (progresso != ultimoProgresso)
                                                logger.Log(Logger.Level.Debug, $"Inserção de dados na tabela {table} em {progresso}%... ");

                                            ultimoProgresso = progresso;
                                        }

                                        count++;
                                    }

                                    break;

                                case "CODICICLASSIFICAZIONEDIAGNOST":

                                     total = codiciClassificazioneDiagnost.Count;

                                    foreach (var item in codiciClassificazioneDiagnost)
                                    {
                                        QueryText += $"\n({item.ID},'{item.Codice}','{item.Descrizione}','{item.Classificazione}',{(item.Positivita ? 1 : 0)},{item.Score},{item.IDTipoEsame},{(item.Eliminato ? 1 : 0)},{item.UO}),";
                                       
                                        if (count == 1000)
                                        {
                                            QueryText = QueryText.Substring(0, (QueryText.Length - 1));

                                            connDestino.Query(QueryText,
                                                transaction: CurrentTransaction,
                                                commandType: CommandType.Text);

                                            count = 0;
                                            QueryText = $"INSERT INTO [{Destcatalog}].[dbo].[{table}] ({columns}) VALUES";

                                            pack++;
                                            progresso = (((pack * 1000) * 100) / total);

                                            if (progresso != ultimoProgresso)
                                                logger.Log(Logger.Level.Debug, $"Inserção de dados na tabela {table} em {progresso}%... ");

                                            ultimoProgresso = progresso;
                                        }

                                        count++;
                                    }

                                    break;

                                case "CHECKLISTITEM":

                                     total = checklistItem.Count;
                                    
                                    foreach (var item in checklistItem)
                                    {
                                        QueryText += $"\n({item.ID},{item.IDCheckList},{item.IDPadre},{item.Ordine},'{(item.Titolo != null ? item.Titolo.Replace("'","''") : "") }','{(item.TestoRTF != null ? item.TestoRTF.Replace("'", "''"): "")}','{(item.TestoTXT != null ?item.TestoTXT.Replace("'", "''"):"")}',{item.TestoNumeroVariabili},{(item.ItemAlmenouno ? 1 : 0)},{(item.ItemPiudiuno ? 1 : 0)},{(item.Eliminato ? 1 : 0)},{item.ClassificazioneDiagnosi},{item.IDOriginaleClonato}),";


                                        if (count == 1000)
                                        {
                                            QueryText = QueryText.Substring(0, (QueryText.Length - 1));

                                            connDestino.Query(QueryText,
                                                transaction: CurrentTransaction,
                                                commandType: CommandType.Text);

                                            count = 0;
                                            QueryText = $"INSERT INTO [{Destcatalog}].[dbo].[{table}] ({columns}) VALUES ";
                                            
                                            pack++;
                                            progresso = (((pack *1000)*100)/ total);

                                            if(progresso != ultimoProgresso)
                                                 logger.Log(Logger.Level.Debug, $"Inserção de dados na tabela {table} em {progresso}%... ");

                                            ultimoProgresso = progresso;
                                        }

                                        count++;

                                    }

                                    break;
                            }

                            QueryText = QueryText.Substring(0, (QueryText.Length - 1));
                            if( QueryText.Length > 97) 
                            {
                                connDestino.Query(QueryText,
                                   transaction: CurrentTransaction,
                                   commandType: CommandType.Text);
                            }

                            logger.Log(Logger.Level.Debug, $"Fim da inserção de dados na tabela {table}.");

                            copy = true;

                        }
                        catch (Exception e)
                        {
                            logger.Log(Logger.Level.Error, "Ocorreu um erro durante a Sincronização da Tabela: " + e.Message);
                            logger.Log(Logger.Level.Error, e.StackTrace);
                            return false;
                        }


                       cmd.CommandText = string.Format("alter table CHECKLIST check constraint all", table);
                       cmd.ExecuteNonQuery();
                       cmd.CommandText = string.Format("alter table CHECKLISTITEMREGOLE check constraint all", table);
                       cmd.ExecuteNonQuery();
                       cmd.CommandText = string.Format("alter table CHECKLISTESAMI check constraint all", table);
                       cmd.ExecuteNonQuery();
                       cmd.CommandText = string.Format("alter table CODICICLASSIFICAZIONEDIAGNOST check constraint all", table);
                       cmd.ExecuteNonQuery();
                       cmd.CommandText = string.Format("alter table CHECKLISTITEM check constraint all", table);
                       cmd.ExecuteNonQuery();
                       logger.Log(Logger.Level.Debug, "Nocheck desativado");

                    }
                }
            }
            catch (Exception e)
            {
                logger.Log(Logger.Level.Error, "Ocorreu um erro durante a Sincronização da Tabela: " + e.Message);
                logger.Log(Logger.Level.Error, e.StackTrace);
                copy = false;
            }

            return copy;

        }
    }
}
