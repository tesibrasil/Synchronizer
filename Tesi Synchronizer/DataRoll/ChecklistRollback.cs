using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Synchronizer
{
    class ChecklistRollback
    {
        public Logger logger;
        private string connStrDestino;
        public bool sucesso = false;
        public TimeSpan t;

        public ChecklistRollback(string keyDestino, ref Logger list)
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
            connStrDestino = Configuration.GetDestConnectionByKey(keyDestino);
            logger.Log(Logger.Level.Verbose, string.Format("Rollback [Checklist] [{0}]", keyDestino));


            ////////////////////////////////////////////////////////
            /// Testando se as connection string não está errada ///
            ////////////////////////////////////////////////////////
            try
            {
                SqlConnection connDestino = new SqlConnection(connStrDestino);
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

                logger.Log(Logger.Level.Verbose, string.Format("Testando Conexão com: {0}", keyDestino));
                var connectionOk = OpenConnection(connStrDestino);
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

                /////////////////////////////
                /// PROCURA PELAS TABELAS ///
                /////////////////////////////

                //if (!DataBaseHasAllRollBackTables(connStrDestino))
                //{
                //    logger.Log(Logger.Level.Critical, "Não é Seguro realizar Operação de ROLLBACK. A Sincronização será Encerrada");
                //    return;
                //}


                ////////////////////////////
                /// CHECKLIST  ///
                ////////////////////////////

                if (!RollbackTable(
                    "CHECKLIST",
                    "ID, CODICE, DESCRIZIONE, PRESENTAZIONE, ITEMALMENOUNO, ITEMPIUDIUNO, CAMPOCL, ORDINE, ELIMINATO, UO",
                    connStrDestino))
                {
                    logger.Log(Logger.Level.Critical, "Erro ao tentar Restaurar Rollback de [CHECKLIST]. A Sincronização será Encerrada");
                    watch.Stop();
                    t = watch.Elapsed;
                    return;
                }
                ////////////////////////////
                /// CHECKLISTITEMREGOLE  ///
                ////////////////////////////

                if (!RollbackTable(
                    "CHECKLISTITEMREGOLE",
                    "ID, IDCHECKLISTITEM, IDCHECKLISTITEMBIND, TIPOREGOLA, ELIMINATO",
                    connStrDestino))
                {
                    logger.Log(Logger.Level.Critical, "Erro ao tentar Restaurar Rollback de [CHECKLISTITEMREGOLE]. A Sincronização será Encerrada");
                    watch.Stop();
                    t = watch.Elapsed;
                    return;
                }
                ////////////////////////////
                /// CHECKLISTESAMI  ///
                ////////////////////////////

                if (!RollbackTable(
                    "CHECKLISTESAMI",
                    "ID, IDCHECKLIST, IDTIPOESAME, ELIMINATO",
                    connStrDestino))
                {
                    logger.Log(Logger.Level.Critical, "Erro ao tentar Restaurar Rollback de [CHECKLISTESAMI]. A Sincronização será Encerrada");
                    watch.Stop();
                    t = watch.Elapsed;
                    return;
                }
                ////////////////////////////
                /// CODICICLASSIFICAZIONEDIAGNOST  ///
                ////////////////////////////

                if (!RollbackTable(
                    "CODICICLASSIFICAZIONEDIAGNOST",
                    "ID, CODICE, DESCRIZIONE, CLASSIFICAZIONE, POSITIVITA, SCORE, IDTIPOESAME, ELIMINATO, UO",
                    connStrDestino))
                {
                    logger.Log(Logger.Level.Critical, "Erro ao tentar Restaurar Rollback de [CODICICLASSIFICAZIONEDIAGNOST]. A Sincronização será Encerrada");
                    watch.Stop();
                    t = watch.Elapsed;
                    return;
                }
                ////////////////////////////
                /// CHECKLISTITEM  ///
                ////////////////////////////

                if (!RollbackTable(
                    "CHECKLISTITEM",
                    "ID, IDCHECKLIST, IDPADRE, ORDINE, TITOLO, TESTORTF, TESTOTXT, TESTONUMEROVARIABILI, ITEMALMENOUNO, ITEMPIUDIUNO, ELIMINATO, CLASSIFICAZIONEDIAGNOSI, IDORIGINALECLONATO",
                    connStrDestino))
                {
                    logger.Log(Logger.Level.Critical, "Erro ao tentar Restaurar Rollback de [CHECKLISTITEM]. A Sincronização será Encerrada");
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
                logger.Log(Logger.Level.Critical, "Erro Fatal Durante tentativa de Restauração de Rollback. A Sincronização será Encerrada");
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

        private bool RollbackTable(string table, string columns, string connStrDestino)
        {
            bool copy = false;
            int affectedRows = 0;
            try
            {
                using (SqlConnection connDestino = new SqlConnection(connStrDestino))
                {
                    connDestino.Open();

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM ROLLBACK_" + table;
                        if ((int)cmd.ExecuteScalar() <= 0)
                        {
                            //throw new Exception("TABELA [ROLLBACK_" + table + "] NÃO POSSUI REGISTROS");
                            logger.Log(Logger.Level.Debug, "TABELA [ROLLBACK_" + table + "] NÃO POSSUI REGISTROS");
                            return true;
                        }
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
                        cmd.Connection = connDestino;
                        cmd.CommandTimeout = 600;
                        cmd.CommandText = "DELETE FROM " + table;
                        cmd.ExecuteNonQuery();

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
                    };

                    // COPIA PARA A TABELA DE DESTINO OS NOVOS VALORES OBTINOS NA FONTE //
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        try
                        {
                            cmd.CommandText = string.Format("SET IDENTITY_INSERT {0} ON", table);
                            cmd.ExecuteNonQuery();
                            logger.Log(Logger.Level.Debug, "Identity ON");
                        }
                        catch (Exception e) { }

                        logger.Log(Logger.Level.Debug, "Sincronizando Tabelas");
                        cmd.Connection = connDestino;
                        cmd.CommandTimeout = 600;
                        cmd.CommandText = "INSERT INTO " + table + "(" + columns + ") SELECT " + columns + " FROM ROLLBACK_" + table;
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
                    connDestino.Close();
                    connDestino.Dispose();
                };
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

        private bool DataBaseHasAllRollBackTables(string connStrDestino)
        {
            bool has = false;
            try
            {
                using (SqlConnection connDestino = new SqlConnection(connStrDestino))
                {
                    connDestino.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM ANATOMIAFETALE";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [ANATOMIAFETALE] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM COMBOTABELLE";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [COMBOTABELLE] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM COMBOTABELLA";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [COMBOTABELLA] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM PROFILICURVE";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [PROFILICURVE] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM PROFILICURVE_CURVE";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [PROFILICURVE_CURVE] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM PARAMETRISHEETFETALI";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [PARAMETRISHEETFETALI] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM PARAMETRISHEETFETALIPOS";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [PARAMETRISHEETFETALIPOS] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM PARAMETRISHEETGINECOLOGICI";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [PARAMETRISHEETGINECOLOGICI] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM PARAMETRISHEETGINEPOS";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [PARAMETRISHEETGINEPOS] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM PARAMETRISHEETTITOLO";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [PARAMETRISHEETTITOLO] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM FRASI";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [FRASI] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM TIPOFRASI";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [TIPOFRASI] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM CODPATOLOGIA";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [CODPATOLOGIA] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM GRIGLIE_DIMENSIONE";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [GRIGLIE_DIMENSIONE] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM GRIGLIE_STATICHE_VERS_ORD";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [GRIGLIE_STATICHE_VERS_ORD] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM GRIGLIE_UTENTI_COMBO";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [GRIGLIE_UTENTI_COMBO] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM GRIGLIE_UTENTI_ESAMI";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [GRIGLIE_UTENTI_ESAMI] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM GRIGLIE_UTENTI_ESAMI_VERS";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [GRIGLIE_UTENTI_ESAMI_VERS] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM GRIGLIE_UTENTI_PAR";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [GRIGLIE_UTENTI_PAR] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM GRIGLIE_UTENTI_PAR_COLORE";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [GRIGLIE_UTENTI_PAR_COLORE] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM GRIGLIE_UTENTI_PAR_LISTAGI";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [GRIGLIE_UTENTI_PAR_LISTAGI] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM GRIGLIE_UTENTI_PAR_REGOLE";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [GRIGLIE_UTENTI_PAR_REGOLE] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM GRIGLIE_UTENTI_PAR_TIPOESA";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [GRIGLIE_UTENTI_PAR_TIPOESA] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM GRIGLIE_UTENTI_PAR_VERS";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [GRIGLIE_UTENTI_PAR_VERS] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM DIARIOGRAV_PAR_COMBO";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [DIARIOGRAV_PAR_COMBO] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM DIARIOGRAV_PAR_ESAMIRICH";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [DIARIOGRAV_PAR_ESAMIRICH] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM DIARIOGRAV_PAR_GRUPPOSANG";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [DIARIOGRAV_PAR_GRUPPOSANG] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM DIARIOGRAV_PAR_LIBERI";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [DIARIOGRAV_PAR_LIBERI] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM DIARIOGRAVIDANZA_COLORE";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [DIARIOGRAVIDANZA_COLORE] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM DIARIOGRAVIDANZA_PARAMETRI";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [DIARIOGRAVIDANZA_PARAMETRI] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM DIARIOGRAVIDANZA_REGOLE";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [DIARIOGRAVIDANZA_REGOLE] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM MODELLI";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [MODELLI] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM MODELLI_CAMPICANCELLABILI";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [MODELLI_CAMPICANCELLABILI] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM MODELLI_DEFAULT";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [MODELLI_DEFAULT] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM MODELLIMACRO";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [MODELLIMACRO] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM MODELLIMACRO_CAMPICANCELLABILI";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [MODELLIMACRO_CAMPICANCELLABILI] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM MODELLIMACRO_OPZIONICAMPI";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [MODELLIMACRO_OPZIONICAMPI] NÃO POSSUI REGISTROS");
                    };
                    /*
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM REPORTEX";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [REPORTEX] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM REPORTEX_ELEMENTO";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [REPORTEX_ELEMENTO] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM REPORTEX_ESAMI";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [REPORTEX_ESAMI] NÃO POSSUI REGISTROS");
                    };
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connDestino;
                        cmd.CommandText = "SELECT COUNT(*) FROM REPORTEX_HF";
                        if ((int)cmd.ExecuteScalar() <= 0)
                            throw new Exception("TABELA [REPORTEX_HF] NÃO POSSUI REGISTROS");
                    };
                    */
                    connDestino.Close();
                    connDestino.Dispose();
                };
                has = true;
            }
            catch (Exception er)
            {
                logger.Log(Logger.Level.Error, er.Message);
            }
            return has;
        }

    }
}
