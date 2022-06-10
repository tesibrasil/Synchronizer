using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;


namespace Synchronizer
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            _checkStatus = Enumerators.CheckStatus.Check;
            UpdateCheckUncheckButtonText();

            logger = new Logger(listBox1);
            Thread thread = new Thread(LogThread);
            thread.IsBackground = true;
            thread.Start();
        }

        //VARIAVEIS DO SISTEMA
        public static Logger logger;

        private Enumerators.CheckStatus _checkStatus;

        private Enumerators.TipoUsuaio _tipoUsuario;

        private string _nomeUsuario;

        private bool _usuarioLogado = false;

        private bool _operationSucess = false;

        private bool _syncing = false;

        DateTime _data = DateTime.Now;

        TimeSpan _tempoExecucao;

        List<string> failed = new List<string>();


        //METHODS  
        private void WritePerformance(int index)
        {
            string unidade = cklUnidades.Items[index].ToString();
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\performance.txt");
            string operacao = (lblAvisoUnidade.Text.Contains("ROLLBACK") ? lblAvisoUnidade.Text.Substring(0, lblAvisoUnidade.Text.IndexOf("ROLLBACK ") + 9) : lblAvisoUnidade.Text.Substring(0, lblAvisoUnidade.Text.IndexOf(" ")));

            if (File.Exists(path))
            {
                var valores = Configuration.GetPerformance();
                Dictionary<string, string[]> performance = new Dictionary<string, string[]>();
                foreach (var u in valores)
                {
                    var split = u.Split(',');
                    performance.Add(split[1] + split[2], new string[] { split[2], split[3], split[1] });
                }
                if (performance.ContainsKey(unidade + operacao))
                {
                    ReportBuilder.Get().Clear(path);
                    foreach (KeyValuePair<string, string[]> val in performance)
                    {
                        if (val.Key == unidade + operacao)
                        {
                            ReportBuilder.Get().Write(unidade + "," + operacao + "," + _tempoExecucao.ToString(), "\\performance.txt");
                        }
                        else
                        {
                            ReportBuilder.Get().Write(val.Value[2]  + "," + val.Value[0] + "," + val.Value[1], "\\performance.txt");
                        }
                    }
                }
                else
                {
                    ReportBuilder.Get().Write(unidade + "," + operacao + "," + _tempoExecucao.ToString(), "\\performance.txt");
                }
            }
            else
            {
                ReportBuilder.Get().Write(unidade + "," + operacao + "," + _tempoExecucao.ToString(), "\\performance.txt");
            }
        }

        private void UpdateCheckUncheckButtonText()
        {
            btnCheckUncheck.Text = _checkStatus == Enumerators.CheckStatus.Check ? "Check All" : "Uncheck All";
        }

        private void CallLogin()
        {
            //_usuarioLogado = true;
            //return;
            if (!_usuarioLogado)
            {
                Login login = new Login();
                login.ShowDialog();
                _usuarioLogado = login.usuarioLogado;
                _tipoUsuario = login.tipoUsuario;
                _nomeUsuario = login.nomeUsuario;
                if (_usuarioLogado)
                    logger.Log(Logger.Level.Verbose, "Login efetuado com o Usuário [" + _nomeUsuario + "]");
                ClearLog();
            }
        }

        private void RefreshCheckBoxList(Enumerators.Grupo grupo = Enumerators.Grupo.Todos)
        {
            cklUnidades.Items.Clear();
            string[,] unidades = Configuration.GetUnidades();
            for (var i = 0; i < unidades.GetLength(0); i++)
            //foreach (var u in Configuration.GetUnidades())
            {
                if (grupo == Enumerators.Grupo.Todos)
                    cklUnidades.Items.Add(unidades[i,1]);
                else if (grupo == Enumerators.Grupo.Fleury)
                {
                    if (unidades[i, 1].Contains("Fleury"))
                        cklUnidades.Items.Add(unidades[i, 1]);
                }
                else if (grupo == Enumerators.Grupo.Amais)
                {
                    if (unidades[i, 1].Contains("A+"))
                        cklUnidades.Items.Add(unidades[i, 1]);
                }
                else if (grupo == Enumerators.Grupo.Sp)
                {
                    if (unidades[i, 1].Contains(" SP "))
                        cklUnidades.Items.Add(unidades[i, 1]);
                }
                else if (grupo == Enumerators.Grupo.Rj)
                {
                    if (unidades[i, 1].Contains(" RJ "))
                        cklUnidades.Items.Add(unidades[i, 1]);
                }
                else if (grupo == Enumerators.Grupo.Ba)
                {
                    if (unidades[i, 1].Contains(" BA "))
                        cklUnidades.Items.Add(unidades[i, 1]);
                }
                else if (grupo == Enumerators.Grupo.Pe)
                {
                    if (unidades[i, 1].Contains(" PE "))
                        cklUnidades.Items.Add(unidades[i, 1]);
                }
                else if (grupo == Enumerators.Grupo.Hosp)
                {
                    if (unidades[i, 1].Contains(" HOSP "))
                        cklUnidades.Items.Add(unidades[i, 1]);
                }
            }
            _checkStatus = Enumerators.CheckStatus.Uncheck;
            UpdateCheckUncheckButtonText();
        }

        private void ClearLog(bool syncStarted = false, string closeReason = "")
        {
            var log = string.Empty;
            foreach (Synchronizer.Logger.LogEvent val in listBox1.Items)
            {
                log += val.EventTime + " " + val.Message + Environment.NewLine;
            }
            if (syncStarted)
                log += Environment.NewLine + "----------------------------" + Environment.NewLine + Environment.NewLine;
            if (!String.IsNullOrEmpty(closeReason))
                log += closeReason;

            string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string path = String.Format(@"{0}\logs\{1}.txt", dir, _data.Day.ToString().PadLeft(2, '0') + _data.Month.ToString().PadLeft(2, '0') + _data.Year
                + "-" + _data.Hour.ToString().PadLeft(2, '0') + _data.Minute.ToString().PadLeft(2, '0'));
            if (File.Exists(path))
                File.AppendAllText(path, log);
            else
            {
                Directory.CreateDirectory(dir+@"\logs\");
                File.Create(path).Close();
                File.AppendAllText(path, log);
            }
                
            listBox1.Items.Clear();
            listBox1.Refresh();
        }

        private void ChangeAllButtonsState(bool state, string unidade = "")
        {
            menuStrip1.Enabled = state;
            cklUnidades.Enabled = state;
            btnCheckUncheck.Enabled = state;
            btnSair.Enabled = state;
            btnSyncChecklist.Enabled = state;
            btnRollBackCheckList.Enabled = state;
            btnSyncFrases.Enabled = state;
            btnRollBackFrases.Enabled = state;
            cbxFiltro.Enabled = state;
            lblAviso.Text = state ? "" : "SINCRONIZANDO";
            lblAvisoUnidade.Text = state ? "" : unidade.ToUpper();
        }

        private void SetUnidadeStatus(int unidade)
        {
            if (!lblAvisoUnidade.Text.ToUpper().Contains("ROLLBACK"))
            {
                if (failed.Contains(lblAvisoUnidade.Text))
                {
                    if (_operationSucess)
                        failed.Remove(lblAvisoUnidade.Text);
                }
                else
                {
                    if (!_operationSucess)
                        failed.Add(lblAvisoUnidade.Text);
                }
            }

            ReportBuilder.Get().Write(_nomeUsuario + "," + cklUnidades.Items[unidade] + "," + (lblAvisoUnidade.Text.Contains("ROLLBACK") ? lblAvisoUnidade.Text.Substring(0, lblAvisoUnidade.Text.IndexOf("ROLLBACK ") + 9) : lblAvisoUnidade.Text.Substring(0, lblAvisoUnidade.Text.IndexOf(" "))) + "," + (_operationSucess ? "S" : "F"), "\\report.txt");

            WritePerformance(unidade);

            cklUnidades.SetItemChecked(unidade, !_operationSucess);
            if (cklUnidades.Items[unidade].ToString().Contains("[SUCESS] "))
                cklUnidades.Items[unidade] = cklUnidades.Items[unidade].ToString().Replace("[SUCESS] ", "");

            if (cklUnidades.Items[unidade].ToString().Contains("[FAIL] "))
                cklUnidades.Items[unidade] = cklUnidades.Items[unidade].ToString().Replace("[FAIL] ", "");

            cklUnidades.Items[unidade] = (_operationSucess ? "[SUCESS] " : "[FAIL] ") + cklUnidades.Items[unidade];
        }

        private void ParticionarCheckListItem()
        {
            //string keyFonte = Configuration.GetFonte();
            //string connStrFone = Configuration.GetValByKey(keyFonte);
            //PackageManager p = new PackageManager(ref logger);

            //if (p.CheckConnectionToFont(connStrFone))
            //{                
            //    p.Drop(connStrFone, p.GetNumeroTabelas(connStrFone), true);
            //    p.Part(connStrFone);
            //}
        }

        private void DropParticoesCheckListItem()
        {
            //string keyFonte = Configuration.GetFonte();
            //string connStrFone = Configuration.GetValByKey(keyFonte);
            //PackageManager p = new PackageManager(ref logger);

            //p.Drop(connStrFone, p.GetNumeroTabelas(connStrFone));
        }

        //THREADS
        private void LogThread()
        {
            logger.Log(Logger.Level.Verbose, "Program Started");
            while (true)
            {
                Thread.Sleep(2000);
            }
        }

        private void SyncChecklistThread(string unidade)
        {
            if (failed.Contains(lblAvisoUnidade.Text))
            {
                ChecklistRollback cr = new ChecklistRollback(unidade, ref logger);
                if (cr.sucesso)
                {
                    ChecklistSychronizer cs = new ChecklistSychronizer(unidade, ref logger);
                    _operationSucess = cs.sucesso;
                    _tempoExecucao = cs.t.Add(cr.t);
                }
                else
                {
                    _operationSucess = false;
                    _tempoExecucao = cr.t;
                    logger.Log(Logger.Level.Error, string.Format("Não será possivel Sincronizar CheckList da unidade {0} pois foi detectada falha na ultima tentativa de Sincronizar e não foi possivel restaurar Rollback para essa unidade", unidade));
                }
            }
            else
            {
                ChecklistSychronizer cs = new ChecklistSychronizer(unidade, ref logger);
                _operationSucess = cs.sucesso;
                _tempoExecucao = cs.t;
            }

        }

        private void SyncChecklistThreadControl(Dictionary<string, string> unidadesSelecionadas, bool firstExecution = false)
        {
            
            if (unidadesSelecionadas.Count == 0)
            {
                ChangeAllButtonsState(true);
                _syncing = false;
                _usuarioLogado = false;
                logger.Log(Logger.Level.Verbose, "Logoff efetuado com o Usuário [" + _nomeUsuario + "]");
                DropParticoesCheckListItem();
                return;
            }
            var unidade = unidadesSelecionadas.First();
            unidadesSelecionadas.Remove(unidade.Key);
            
            Action onThreadStart = () =>
            {
                ChangeAllButtonsState(false, "CHECKLIST DE " + unidade.Key);
            };
            Action onThreadComplete = () =>
            {
                SetUnidadeStatus(cklUnidades.FindString(unidade.Key));
                SyncChecklistThreadControl(unidadesSelecionadas);

            };
            Action Sync = () =>
            {
                //if (firstExecution) ParticionarCheckListItem();
                SyncChecklistThread(unidade.Key);
            };
            Thread thread = new Thread(() =>
            {
                try
                {
                    this.Invoke(onThreadStart);
                    Sync();
                }
                finally
                {
                    this.Invoke(onThreadComplete);
                }
            });
            thread.Name = unidade.Key;
            thread.IsBackground = true;
            thread.Start();
        }

        private void SyncFrasesThread(string unidade)
        {
            //if (failed.Contains(lblAvisoUnidade.Text))
            //{
            //    FrasesRollback fr = new FrasesRollback(unidade, ref logger);
            //    if (fr.sucesso)
            //    {
            //        FrasesSynchronizer fs = new FrasesSynchronizer(unidade, ref logger);
            //        _operationSucess = fs.sucesso;
            //        _tempoExecucao = fs.t.Add(fr.t);
            //    }
            //    else
            //    {
            //        _operationSucess = false;
            //        _tempoExecucao = fr.t;
            //        logger.Log(Logger.Level.Error, string.Format("Não será possivel Sincronizar Frases da unidade {0} pois foi detectada falha na ultima tentativa de Sincronizar e não foi possivel restaurar Rollback para essa unidade", unidade));
            //    }
            //}
            //else
            //{
            //    FrasesSynchronizer fs = new FrasesSynchronizer(unidade, ref logger);
            //    _operationSucess = fs.sucesso;
            //    _tempoExecucao = fs.t;
            //}
        }

        private void SyncFrasesThreadControl(Dictionary<string, string> unidadesSelecionadas)
        {
            //if (unidadesSelecionadas.Count == 0)
            //{
            //    ChangeAllButtonsState(true);
            //    _syncing = false;
            //    _usuarioLogado = false;
            //    logger.Log(Logger.Level.Verbose, "Logoff efetuado com o Usuário [" + _nomeUsuario + "]");
            //    return;
            //}
            //var unidade = unidadesSelecionadas.First();
            //unidadesSelecionadas.Remove(unidade.Key);

            //Action onThreadStart = () =>
            //{
            //    ChangeAllButtonsState(false, "FRASES DE " + unidade.Key);
            //};
            //Action onThreadComplete = () =>
            //{
            //    SetUnidadeStatus(cklUnidades.FindString(unidade.Key));
            //    SyncFrasesThreadControl(unidadesSelecionadas);
            //};
            //Action Sync = () =>
            //{
            //    SyncFrasesThread(unidade.Key);
            //};
            //Thread thread = new Thread(() =>
            //{
            //    try
            //    {
            //        this.Invoke(onThreadStart);
            //        Sync();
            //    }
            //    finally
            //    {
            //        this.Invoke(onThreadComplete);
            //    }
            //});
            //thread.Name = unidade.Key;
            //thread.IsBackground = true;
            //thread.Start();
        }

        private void RollbackChecklistThread(string unidade)
        {
            ChecklistRollback cr = new ChecklistRollback(unidade, ref logger);
            _operationSucess = cr.sucesso;
            _tempoExecucao = cr.t;
        }

        private void RollbackCheclistThreadControl(Dictionary<string, string> unidadesSelecionadas)
        {
            if (unidadesSelecionadas.Count == 0)
            {
                ChangeAllButtonsState(true);
                _syncing = false;
                _usuarioLogado = false;
                logger.Log(Logger.Level.Verbose, "Logoff efetuado com o Usuário [" + _nomeUsuario + "]");
                return;
            }
            var unidade = unidadesSelecionadas.First();
            unidadesSelecionadas.Remove(unidade.Key);

            Action onThreadStart = () =>
            {
                ChangeAllButtonsState(false, "CHECKLIST ROLLBACK DE " + unidade.Key);
            };
            Action onThreadComplete = () =>
            {
                SetUnidadeStatus(cklUnidades.FindString(unidade.Key));
                RollbackCheclistThreadControl(unidadesSelecionadas);
            };
            Action Roll = () =>
            {
                RollbackChecklistThread(unidade.Key);
            };
            Thread thread = new Thread(() =>
            {
                try
                {
                    this.Invoke(onThreadStart);
                    Roll();
                }
                finally
                {
                    this.Invoke(onThreadComplete);
                }
            });
            thread.Name = unidade.Key;
            thread.IsBackground = true;
            thread.Start();
        }

        private void RollbackFrasesThread(string unidade)
        {
            //FrasesRollback fr = new FrasesRollback(unidade, ref logger);
            //_operationSucess = fr.sucesso;
            //_tempoExecucao = fr.t;
        }

        private void RollbackFrasesThreadControl(Dictionary<string, string> unidadesSelecionadas)
        {
            //if (unidadesSelecionadas.Count == 0)
            //{
            //    ChangeAllButtonsState(true);
            //    _syncing = false;
            //    _usuarioLogado = false;
            //    logger.Log(Logger.Level.Verbose, "Logoff efetuado com o Usuário [" + _nomeUsuario + "]");
            //    return;
            //}
            //var unidade = unidadesSelecionadas.First();
            //unidadesSelecionadas.Remove(unidade.Key);

            //Action onThreadStart = () =>
            //{
            //    ChangeAllButtonsState(false, "FRASES ROLLBACK DE " + unidade.Key);
            //};
            //Action onThreadComplete = () =>
            //{
            //    SetUnidadeStatus(cklUnidades.FindString(unidade.Key));
            //    RollbackFrasesThreadControl(unidadesSelecionadas);
            //};
            //Action Roll = () =>
            //{
            //    RollbackFrasesThread(unidade.Key);
            //};
            //Thread thread = new Thread(() =>
            //{
            //    try
            //    {
            //        this.Invoke(onThreadStart);
            //        Roll();
            //    }
            //    finally
            //    {
            //        this.Invoke(onThreadComplete);
            //    }
            //});
            //thread.Name = unidade.Key;
            //thread.IsBackground = true;
            //thread.Start();
        }


        //MENUS
        private void configurarDestinosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CallLogin();
            if (_usuarioLogado && _tipoUsuario == Enumerators.TipoUsuaio.Admin)
            //if (_usuarioLogado)
            {
                logger.Log(Logger.Level.Verbose, "Menu [Configurar Destinos] Clicado");
                Configuracao configuracao = new Configuracao();
                configuracao.logger = logger;
                configuracao.configType = Configuracao.type.Destinos;
                configuracao.ShowDialog();
                RefreshCheckBoxList();
                _usuarioLogado = false;
                logger.Log(Logger.Level.Verbose, "Logoff efetuado com o Usuário [" + _nomeUsuario + "]");
                ClearLog();
            }
            if (_usuarioLogado && _tipoUsuario == Enumerators.TipoUsuaio.Fleury)
            {
                logger.Log(Logger.Level.Error, "Tentativa de Acesso as Configurações dos destinos com o usuário [Fleury]. Acesso Negado");
                MessageBox.Show("Este usuário não tem permissão para mudar o apontamento dos Destinos", "Aviso");
                _usuarioLogado = false;
                logger.Log(Logger.Level.Verbose, "Logoff efetuado com o Usuário [" + _nomeUsuario + "]");
                ClearLog();
            }
        }

        private void configurarFonteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CallLogin();
            if (_usuarioLogado && _tipoUsuario == Enumerators.TipoUsuaio.Admin)
            //if (_usuarioLogado)
            {
                logger.Log(Logger.Level.Verbose, "Menu [Configurar Fonte] Clicado");
                Configuracao configuracao = new Configuracao();
                configuracao.logger = logger;
                configuracao.configType = Configuracao.type.Fonte;
                configuracao.ShowDialog();
                RefreshCheckBoxList();
                _usuarioLogado = false;
                logger.Log(Logger.Level.Verbose, "Logoff efetuado com o Usuário [" + _nomeUsuario + "]");
                ClearLog();
            }
            if (_usuarioLogado && _tipoUsuario == Enumerators.TipoUsuaio.Fleury)
            {
                logger.Log(Logger.Level.Error, "Tentativa de Acesso as Configurações de fonte com o usuário [Fleury]. Acesso Negado");
                MessageBox.Show("Este usuário não tem permissão para mudar o apontamento da Fonte", "Aviso");
                _usuarioLogado = false;
                logger.Log(Logger.Level.Verbose, "Logoff efetuado com o Usuário [" + _nomeUsuario + "]");
                ClearLog();
            }
        }

        private void historicoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logger.Log(Logger.Level.Verbose, "Menu [Historico] Clicado");
            Configuracao configuracao = new Configuracao();
            configuracao.logger = logger;
            configuracao.configType = Configuracao.type.Historico;
            configuracao.ShowDialog();
        }

        private void performanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logger.Log(Logger.Level.Verbose, "Menu [Performance] Clicado");
            Configuracao configuracao = new Configuracao();
            configuracao.logger = logger;
            configuracao.configType = Configuracao.type.Performance;
            configuracao.ShowDialog();
        }



        //BOTOES
        private void btnSyncChecklist_Click(object sender, EventArgs e)
        {
            logger.Log(Logger.Level.Warning, "Botão [Sync Checklist] Clicado");
            Agendamento agenda = new Agendamento();
            agenda.ShowDialog();

            if (agenda.Cancel)
                return;

            CallLogin();
            if (_usuarioLogado)
            {
                if (agenda.Agendado)
                {
                    this.Enabled = false;
                    this.WindowState = FormWindowState.Minimized;
                    while (DateTime.Now < agenda.Hora)
                    {
                        Thread.Sleep(10000);
                    };
                    this.WindowState = FormWindowState.Maximized;
                    this.Enabled = true;
                }

                ClearLog(true);
                string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                logger.Log(Logger.Level.Verbose, "Sync Started");

                var unidadesSelecionadas = new Dictionary<string, string>();
                List<int> indexes = new List<int>();
                foreach (string unidade in cklUnidades.CheckedItems)
                {
                    if (unidade.Contains("[SUCESS] "))
                    {
                        unidadesSelecionadas.Add(unidade.Replace("[SUCESS] ", ""), Configuration.GetDestConnectionByKey(unidade.Replace("[SUCESS] ", "")));
                        indexes.Add(cklUnidades.FindString(unidade));
                    }
                    else if (unidade.Contains("[FAIL] "))
                    {
                        unidadesSelecionadas.Add(unidade.Replace("[FAIL] ", ""), Configuration.GetDestConnectionByKey(unidade.Replace("[FAIL] ", "")));
                        indexes.Add(cklUnidades.FindString(unidade));
                    }
                    else
                        unidadesSelecionadas.Add(unidade, Configuration.GetDestConnectionByKey(unidade));
                }
                foreach (int i in indexes)
                {
                    if (cklUnidades.Items[i].ToString().Contains("[SUCESS] "))
                        cklUnidades.Items[i] = cklUnidades.Items[i].ToString().Replace("[SUCESS] ", "");
                    if (cklUnidades.Items[i].ToString().Contains("[FAIL] "))
                        cklUnidades.Items[i] = cklUnidades.Items[i].ToString().Replace("[FAIL] ", "");
                }
                _syncing = true;
                SyncChecklistThreadControl(unidadesSelecionadas, true);
            }
        }

        private void btnSyncFrases_Click(object sender, EventArgs e)
        {
            //logger.Log(Logger.Level.Warning, "Botão [Sync Frases] Clicado");
            //Agendamento agenda = new Agendamento();
            //agenda.ShowDialog();

            //if (agenda.Cancel)
            //    return;


            //CallLogin();
            //if (_usuarioLogado)
            //{
            //    if (agenda.Agendado)
            //    {
            //        this.Enabled = false;
            //        this.WindowState = FormWindowState.Minimized;
            //        while (DateTime.Now < agenda.Hora)
            //        {
            //            Thread.Sleep(10000);
            //        };
            //        this.WindowState = FormWindowState.Maximized;
            //        this.Enabled = true;
            //    }

            //    ClearLog(true);
            //    string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //    logger.Log(Logger.Level.Verbose, "Sync Started");
            //    var unidadesSelecionadas = new Dictionary<string, string>();
            //    List<int> indexes = new List<int>();
            //    foreach (string unidade in cklUnidades.CheckedItems)
            //    {
            //        if (unidade.Contains("[SUCESS] "))
            //        {
            //            unidadesSelecionadas.Add(unidade.Replace("[SUCESS] ", ""), Configuration.GetValByKey(unidade.Replace("[SUCESS] ", "")));
            //            indexes.Add(cklUnidades.FindString(unidade));
            //        }
            //        else if (unidade.Contains("[FAIL] "))
            //        {
            //            unidadesSelecionadas.Add(unidade.Replace("[FAIL] ", ""), Configuration.GetValByKey(unidade.Replace("[FAIL] ", "")));
            //            indexes.Add(cklUnidades.FindString(unidade));
            //        }
            //        else
            //            unidadesSelecionadas.Add(unidade, Configuration.GetValByKey(unidade));
            //    }
            //    foreach (int i in indexes)
            //    {
            //        if (cklUnidades.Items[i].ToString().Contains("[SUCESS] "))
            //            cklUnidades.Items[i] = cklUnidades.Items[i].ToString().Replace("[SUCESS] ", "");
            //        if (cklUnidades.Items[i].ToString().Contains("[FAIL] "))
            //            cklUnidades.Items[i] = cklUnidades.Items[i].ToString().Replace("[FAIL] ", "");
            //    }
            //    _syncing = true;
            //    SyncFrasesThreadControl(unidadesSelecionadas);
            //}
        }

        private void btnRollBackCheckList_Click(object sender, EventArgs e)
        {
            logger.Log(Logger.Level.Warning, "Botão [Rollback Checklist] Clicado");
            Agendamento agenda = new Agendamento();
            agenda.ShowDialog();

            if (agenda.Cancel)
                return;


            CallLogin();
            if (_usuarioLogado)
            {
                if (agenda.Agendado)
                {
                    this.Enabled = false;
                    this.WindowState = FormWindowState.Minimized;
                    while (DateTime.Now < agenda.Hora)
                    {
                        Thread.Sleep(10000);
                    };
                    this.WindowState = FormWindowState.Maximized;
                    this.Enabled = true;
                }

                ClearLog(true);
                string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                logger.Log(Logger.Level.Verbose, "Rollback Started");
                var unidadesSelecionadas = new Dictionary<string, string>();
                List<int> indexes = new List<int>();
                foreach (string unidade in cklUnidades.CheckedItems)
                {
                    if (unidade.Contains("[SUCESS] "))
                    {
                        unidadesSelecionadas.Add(unidade.Replace("[SUCESS] ", ""), Configuration.GetDestConnectionByKey(unidade.Replace("[SUCESS] ", "")));
                        indexes.Add(cklUnidades.FindString(unidade));
                    }
                    else if (unidade.Contains("[FAIL] "))
                    {
                        unidadesSelecionadas.Add(unidade.Replace("[FAIL] ", ""), Configuration.GetDestConnectionByKey(unidade.Replace("[FAIL] ", "")));
                        indexes.Add(cklUnidades.FindString(unidade));
                    }
                    else
                        unidadesSelecionadas.Add(unidade, Configuration.GetDestConnectionByKey(unidade));
                }
                foreach (int i in indexes)
                {
                    if (cklUnidades.Items[i].ToString().Contains("[SUCESS] "))
                        cklUnidades.Items[i] = cklUnidades.Items[i].ToString().Replace("[SUCESS] ", "");
                    if (cklUnidades.Items[i].ToString().Contains("[FAIL] "))
                        cklUnidades.Items[i] = cklUnidades.Items[i].ToString().Replace("[FAIL] ", "");
                }
                _syncing = true;
                RollbackCheclistThreadControl(unidadesSelecionadas);
            }
        }

        private void btnRollBackFrases_Click(object sender, EventArgs e)
        {
            //logger.Log(Logger.Level.Warning, "Botão [RollbackFrases] Clicado");
            //Agendamento agenda = new Agendamento();
            //agenda.ShowDialog();

            //if (agenda.Cancel)
            //    return;


            //CallLogin();
            //if (_usuarioLogado)
            //{
            //    if (agenda.Agendado)
            //    {
            //        this.Enabled = false;
            //        this.WindowState = FormWindowState.Minimized;
            //        while (DateTime.Now < agenda.Hora)
            //        {
            //            Thread.Sleep(10000);
            //        };
            //        this.WindowState = FormWindowState.Maximized;
            //        this.Enabled = true;
            //    }

            //    ClearLog(true);
            //    string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //    logger.Log(Logger.Level.Verbose, "Rollback Started");
            //    var unidadesSelecionadas = new Dictionary<string, string>();
            //    List<int> indexes = new List<int>();
            //    foreach (string unidade in cklUnidades.CheckedItems)
            //    {
            //        if (unidade.Contains("[SUCESS] "))
            //        {
            //            unidadesSelecionadas.Add(unidade.Replace("[SUCESS] ", ""), Configuration.GetValByKey(unidade.Replace("[SUCESS] ", "")));
            //            indexes.Add(cklUnidades.FindString(unidade));
            //        }
            //        else if (unidade.Contains("[FAIL] "))
            //        {
            //            unidadesSelecionadas.Add(unidade.Replace("[FAIL] ", ""), Configuration.GetValByKey(unidade.Replace("[FAIL] ", "")));
            //            indexes.Add(cklUnidades.FindString(unidade));
            //        }
            //        else
            //            unidadesSelecionadas.Add(unidade, Configuration.GetValByKey(unidade));
            //    }
            //    foreach (int i in indexes)
            //    {
            //        if (cklUnidades.Items[i].ToString().Contains("[SUCESS] "))
            //            cklUnidades.Items[i] = cklUnidades.Items[i].ToString().Replace("[SUCESS] ", "");
            //        if (cklUnidades.Items[i].ToString().Contains("[FAIL] "))
            //            cklUnidades.Items[i] = cklUnidades.Items[i].ToString().Replace("[FAIL] ", "");
            //    }
            //    _syncing = true;
            //    RollbackFrasesThreadControl(unidadesSelecionadas);
            //}
        }

        private void btnCheckUncheck_Click(object sender, EventArgs e)
        {
            if (_checkStatus == Enumerators.CheckStatus.Check)
            {
                for (int i = 0; i < cklUnidades.Items.Count; i++)
                    cklUnidades.SetItemChecked(i, true);
                logger.Log(Logger.Level.Warning, "Botão [Check All] Clicado");
                ClearLog();
            }
            else
            {
                for (int i = 0; i < cklUnidades.Items.Count; i++)
                    cklUnidades.SetItemChecked(i, false);
                logger.Log(Logger.Level.Warning, "Botão [Uncheck All] Clicado");
                ClearLog();
            }

            _checkStatus = _checkStatus == Enumerators.CheckStatus.Check ? Enumerators.CheckStatus.Uncheck : Enumerators.CheckStatus.Check;
            UpdateCheckUncheckButtonText();
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        // EVENTOS        
        private void Main_Load(object sender, EventArgs e)
        {
            RefreshCheckBoxList();
            ChangeAllButtonsState(true);
            cbxFiltro.SelectedIndex = 0;
            btnCheckUncheck.PerformClick();
        }

        private void cklUnidades_SelectedIndexChanged(object sender, EventArgs e)
        {
            cklUnidades.ClearSelected();
        }

        private void cklUnidades_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var unidade = cklUnidades.Items[e.Index].ToString();
            var state = e.NewValue;
            logger.Log(Logger.Level.Warning, "Unidade " + unidade + (state == CheckState.Checked ? " Selecionada" : " Desselecionada"));
        }

        private void cbxFiltro_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Todos
            //Fleury
            //A +
            //São Paulo
            //Rio de Janeiro
            //Bahia
            //Pernambuco
            //Hospitais
            Enumerators.Grupo grupo = Enumerators.Grupo.Todos;
            if (cbxFiltro.SelectedIndex == 1)
            {
                grupo = Enumerators.Grupo.Fleury;
            }
            else if (cbxFiltro.SelectedIndex == 2)
            {
                grupo = Enumerators.Grupo.Amais;
            }
            else if (cbxFiltro.SelectedIndex == 3)
            {
                grupo = Enumerators.Grupo.Sp;
            }
            else if (cbxFiltro.SelectedIndex == 4)
            {
                grupo = Enumerators.Grupo.Rj;
            }
            else if (cbxFiltro.SelectedIndex == 5)
            {
                grupo = Enumerators.Grupo.Ba;
            }
            else if (cbxFiltro.SelectedIndex == 6)
            {
                grupo = Enumerators.Grupo.Pe;
            }
            else if (cbxFiltro.SelectedIndex == 7)
            {
                grupo = Enumerators.Grupo.Hosp;
            }

            logger.Log(Logger.Level.Warning, "Grupo [" + grupo.ToString().ToUpper() + "] Selecionado");
            RefreshCheckBoxList(grupo);
            ClearLog();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_syncing)
            {
                e.Cancel = true;
                return;
            }
            var closeReason = string.Format("{0} [Program Closing] - {1}", DateTime.Now.ToString(), e.CloseReason.ToString());
            ClearLog(false, closeReason);
        }
    }
}
