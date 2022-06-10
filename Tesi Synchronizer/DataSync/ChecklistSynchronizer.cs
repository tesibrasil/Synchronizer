using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

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
            //connStrFonte = "Data Source=LOCALHOST;User ID=sa;Password=nautilus;Initial Catalog=ECOPLUS_CHECKLIST;";
            connStrDestino = Configuration.GetDestConnectionByKey(keyDestino);
            //connStrDestino = "Data Source=LOCALHOST;User ID=sa;Password=nautilus;Initial Catalog=ECOPLUS_REP2;";
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

                //////////////////////////////
                /// Deltando Linked Server ///             
                //////////////////////////////

                DropLinkedServer(connStrDestino, true);

                /////////////////////////////
                /// Criando Linked Server ///
                /////////////////////////////

                logger.Log(Logger.Level.Verbose, "Configurando LinkedServer");
                if (!CreateLinkedServer(connStrFonte, connStrDestino))
                {
                    logger.Log(Logger.Level.Critical, "Não Foi Possivel Estabelecer uma conexão LinkedServer. A Sincronização não será realizada");
                    new ChecklistRollback(keyDestino, ref list);
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

                ////////////////////////////
                /// CHECKLISTITEMREGOLE ///
                ////////////////////////////

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

                ////////////////////////////
                /// CHECKLISTESAMI ///
                ////////////////////////////

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

                ////////////////////////////
                /// CODICICLASSIFICAZIONEDIAGNOST ///
                ////////////////////////////

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

                ////////////////////////////
                /// CHECKLISTITEM ///
                ////////////////////////////

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

                //////////////////////////////
                /// Deltando Linked Server ///             
                //////////////////////////////

                DropLinkedServer(connStrDestino);
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

        private bool CreateLinkedServer(string connStrFonte, string connStrDestino)
        {
            bool ok = false;
            //   var connSplit = connStrFonte.Split(';');
            var connSplit = connStrDestino.Split(';');
            string fonteServer = connSplit[0].Split('=')[1].ToString();
            string fonteUser = connSplit[1].Split('=')[1].ToString();
            string fontePass = connSplit[2].Split('=')[1].ToString();
            try
            {
                using (SqlConnection connFonte = new SqlConnection(connStrFonte))
                //using (SqlConnection connDestino = new SqlConnection(connStrDestino))
                {

                    try
                    {
                        //  string sp_addlinkedserver = "EXEC sp_addlinkedserver @server='SRV'";
                        string sp_addlinkedserver = $"EXEC sp_addlinkedserver @server='{fonteServer}'";
                        //    using (SqlCommand cmd = new SqlCommand(sp_addlinkedserver, connDestino))
                        using (SqlCommand cmd = new SqlCommand(sp_addlinkedserver, connFonte))
                        {
                            cmd.Connection.Open();
                            cmd.ExecuteNonQuery();
                            cmd.Connection.Close();
                        };
                        logger.Log(Logger.Level.Debug, "LinkedServer Criado");

                        string sp_setnetname = string.Format($"EXEC sp_setnetname '{fonteServer}','{fonteServer}'");
                        //       string sp_setnetname = string.Format("EXEC sp_setnetname 'SRV','{0}'", fonteServer);
                        using (SqlCommand cmd = new SqlCommand(sp_setnetname, connFonte))
                        {
                            cmd.Connection.Open();
                            cmd.ExecuteNonQuery();
                            cmd.Connection.Close();
                        };
                        string sp_addlinkedsrvlogin = string.Format($"sp_addlinkedsrvlogin '{fonteServer}', 'false', null, '{fonteUser}', '{fontePass}'");
                        //   string sp_addlinkedsrvlogin = string.Format("sp_addlinkedsrvlogin 'SRV', 'false', null, '{0}', '{1}'", fonteUser, fontePass);
                        logger.Log(Logger.Level.Debug, "Servidor Registrado");

                        using (SqlCommand cmd = new SqlCommand(sp_addlinkedsrvlogin, connFonte))
                        {
                            cmd.Connection.Open();
                            cmd.ExecuteNonQuery();
                            cmd.Connection.Close();
                        };
                        logger.Log(Logger.Level.Debug, "Credenciais Registradas");

                        ok = true;

                    }
                    catch (Exception e)
                    {
                        logger.Log(Logger.Level.Error, "Erro Durante Registro de LinkedServer");
                        DropLinkedServer(connStrDestino);
                        ok = false;
                    }
                    connFonte.Close();
                    connFonte.Dispose();
                    //connDestino.Close();
                    //connDestino.Dispose();
                };
            }
            catch
            {
                logger.Log(Logger.Level.Error, "Não Foi Possivel Conectar ao Servidor de Destino");
                ok = false;
            }
            return ok;
        }

        private void DropLinkedServer(string connStr, bool inicio = false)
        {
            try
            {
                string sp_addlinkedserver = "EXEC sp_dropserver 'SRV', 'droplogins'";
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    using (SqlCommand cmd = new SqlCommand(sp_addlinkedserver, conn))
                    {
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    };
                    conn.Close();
                    conn.Dispose();
                };
                logger.Log(Logger.Level.Debug, "LinkedServer Deletado");
            }
            catch (Exception e)
            {
                if (!inicio)
                    logger.Log(Logger.Level.Error, "Erro ao Deletar LinkedServer");
            }
        }

        private bool CopyTable(string table, string columns, string connStrDestino, string catalog, string fonte)
        {
            ///////

            var connSplit = fonte.Split(';');
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


            string tempName = "#TEMP_" + table;
            string masterName = "MASTER" + table;
            string rollbackName = "ROLLBACK_" + table;

            bool copy = false;
            int affectedRows = 0;
            try
            {
                using (SqlConnection connDestino = new SqlConnection(connStrDestino))
                //  using (SqlConnection connDestino = new SqlConnection(fonte))
                {
                    connDestino.Open();
                    // COPIA TABELA DA FONTE PARA TABELA TEMPORARIA NO DESTINO //
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        logger.Log(Logger.Level.Verbose, "Copiando " + table);
                        cmd.Connection = connDestino;
                        //          cmd.CommandText = string.Format("SELECT * INTO {0} FROM (SELECT {1} FROM [SRV].[{2}].[dbo].[{3}]) AS {4}",
                        cmd.CommandText = string.Format("SELECT * INTO {0} FROM (SELECT {1} FROM [{2}].[dbo].[{3}]) AS {4}",
                                      tempName,
                            columns,
                          //  catalog,
                          Destcatalog,
                            table,
                            masterName
                            );
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

                        //cmd.CommandText = string.Format("alter table CHECKLIST check constraint all", table);
                        //cmd.ExecuteNonQuery();
                        //cmd.CommandText = string.Format("alter table CHECKLISTITEMREGOLE check constraint all", table);
                        //cmd.ExecuteNonQuery();
                        //cmd.CommandText = string.Format("alter table CHECKLISTESAMI check constraint all", table);
                        //cmd.ExecuteNonQuery();
                        //cmd.CommandText = string.Format("alter table CODICICLASSIFICAZIONEDIAGNOST check constraint all", table);
                        //cmd.ExecuteNonQuery();
                        //cmd.CommandText = string.Format("alter table CHECKLISTITEM check constraint all", table);
                        //cmd.ExecuteNonQuery();
                        //logger.Log(Logger.Level.Debug, "Nocheck desativado");

                        try
                        {
                            cmd.CommandText = string.Format("SET IDENTITY_INSERT {0} OFF", table);
                            logger.Log(Logger.Level.Debug, "Identity OFF");
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception e) { }
                    };

                    using (SqlConnection connFonte = new SqlConnection(fonte))
                    {

                        connFonte.Open();
                        //fechar conexao
                        // COPIA PARA A TABELA DE DESTINO OS NOVOS VALORES OBTINOS NA FONTE //
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.Connection = connFonte;
                            cmd.CommandTimeout = 0;
                            try
                            {
                                cmd.CommandText = string.Format("SET IDENTITY_INSERT {0} ON", table);
                                cmd.ExecuteNonQuery();
                                logger.Log(Logger.Level.Debug, "Identity ON");
                            }
                            catch (Exception e) { }

                            logger.Log(Logger.Level.Debug, "Sincronizando Tabelas");
                            cmd.CommandText = string.Format($"INSERT INTO [{DestServer}].[{Destcatalog}].[dbo].[{table}] SELECT {columns} FROM {table}");
                            affectedRows = cmd.ExecuteNonQuery();
                            logger.Log(Logger.Level.Info, affectedRows + " rows copiadas");

                            try
                            {
                                cmd.CommandText = string.Format("SET IDENTITY_INSERT {0} OFF", table);
                                logger.Log(Logger.Level.Debug, "Identity OFF");
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception e) { }


                            logger.Log(Logger.Level.Info, table + " Sincronizada com sucesso");
                        };
                        connFonte.Close();
                        connFonte.Dispose();
                    };

                    //connDestino.Close();
                    //connDestino.Dispose();

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        try
                        {
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
                        catch (Exception e) { }
                    }
                }

               
                copy = true;

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
