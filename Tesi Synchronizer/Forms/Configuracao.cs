using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Synchronizer
{
    public partial class Configuracao : Form
    {
        public Logger logger;
        public enum type { Destinos, Fonte, Historico, Performance };
        public type configType;
        private bool usuarioSalvouAlgumaCoisa = false;

        public Configuracao()
        {
            InitializeComponent();
        }

        private Dictionary<string, string[]> GetDataFromGridView()
        {
            Dictionary<string, string[]> keyvalue = new Dictionary<string, string[]>();
            var gridviewdata = (DataTable)gridConfig.DataSource;

            foreach (DataRow row in gridviewdata.Rows)
            {
                string[] editedConfig = new string[3];
                editedConfig[0] = row["Nome da Unidade"].ToString();
                editedConfig[1] = row["String de Conexão"].ToString();
                editedConfig[2] = row["Selecionar"].ToString();
                if (string.IsNullOrEmpty(row["key"].ToString().Trim()) &&
                    string.IsNullOrEmpty(row["Nome da Unidade"].ToString().Trim()) && string.IsNullOrEmpty(row["String de Conexão"].ToString().Trim()))
                    continue;

                bool newName = false;
                string keyName = row["Key"].ToString();
                while (!newName)
                {
                    try
                    {
                        keyvalue.Add(keyName, editedConfig);
                        newName = true;
                    }
                    catch
                    {
                        keyName += "-Copy";
                    }
                }
            }
            return keyvalue;
        }

        private void SetGridViewColumnWidth()
        {
            //if (configType == type.Historico)
            //{
            //    gridConfig.Columns[0].Width = 300;
            //    gridConfig.Columns[1].Width = 300;
            //    gridConfig.Columns[2].Width = 300;
            //    gridConfig.Columns[3].Width = 100;
            //    return;
            //}
            //gridConfig.Columns[2].Width = 200;
            //gridConfig.Columns[3].Width = 600;
            //gridConfig.Columns[4].Width = 215;
        }

        private void Configuracao_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            var valores = new List<string>();

            if (configType == type.Historico)
            {               
                btnAdicionarUnidade.Visible = false;
                btnDeletarUnidade.Visible = false;
                btnEditarUnidade.Visible = false;
                btnTestConn.Visible = false;
                btnSalvar.Visible = false;

                this.Text = "Histórico - Tesi Elettronica e Sistemi Informativi";
                dt.Columns.Add("Data", typeof(string));
                dt.Columns.Add("Usuário", typeof(string));
                dt.Columns.Add("Unidade", typeof(string));
                dt.Columns.Add("Operação", typeof(string));
                dt.Columns.Add("Status", typeof(string));

                valores = Configuration.GetHistorico();
                foreach (var u in valores)
                {
                    DataRow row = dt.NewRow();
                    var split = u.Split(',');
                    row["Data"] = split[0];
                    row["Usuário"] = split[1];
                    row["Unidade"] = split[2];
                    row["Operação"] = split[3];
                    row["Status"] = split[4];
                    dt.Rows.Add(row);
                }
                gridConfig.DataSource = dt;

                for (int i = 0; i < gridConfig.RowCount; i++)
                {
                    if (gridConfig.Rows[i].Cells[4].Value.ToString() == "S")
                    {
                        gridConfig.Rows[i].Cells[4].Style.BackColor = Color.Green;
                        gridConfig.Rows[i].Cells[4].Style.ForeColor = Color.Green;
                    }
                    else
                    {
                        gridConfig.Rows[i].Cells[4].Style.BackColor = Color.Red;
                        gridConfig.Rows[i].Cells[4].Style.ForeColor = Color.Red;
                    }
                }

                gridConfig.Columns[0].ReadOnly = true;
                gridConfig.Columns[1].ReadOnly = true;
                gridConfig.Columns[2].ReadOnly = true;
                gridConfig.Columns[3].ReadOnly = true;
                gridConfig.Columns[4].ReadOnly = true;

                gridConfig.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                gridConfig.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
                gridConfig.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
                gridConfig.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
                gridConfig.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;

                SetGridViewColumnWidth();
                gridConfig.Refresh();
            }

            else if (configType == type.Performance)
            {
                //gridConfig.Enabled = false;

                btnAdicionarUnidade.Visible = false;
                btnDeletarUnidade.Visible = false;
                btnEditarUnidade.Visible = false;
                btnTestConn.Visible = false;
                btnSalvar.Visible = false;

                this.Text = "Performance - Tesi Elettronica e Sistemi Informativi";
                dt.Columns.Add("Unidade", typeof(string));
                dt.Columns.Add("Operação", typeof(string));
                dt.Columns.Add("Último Tempo", typeof(string));

                valores = Configuration.GetPerformance();
                foreach (var u in valores)
                {
                    DataRow row = dt.NewRow();
                    var split = u.Split(',');
                    row["Unidade"] = split[1];
                    row["Operação"] = split[2];
                    row["Último Tempo"] = split[3];
                    dt.Rows.Add(row);
                }
                gridConfig.DataSource = dt;

                gridConfig.Columns[0].ReadOnly = true;
                gridConfig.Columns[1].ReadOnly = true;
                gridConfig.Columns[2].ReadOnly = true;                
                SetGridViewColumnWidth();
                gridConfig.Refresh();
            }

            else
            {
                dt.Columns.Add("Key", typeof(string));
                dt.Columns.Add("Selecionar", typeof(Boolean));
                dt.Columns.Add("Nome da Unidade", typeof(string));
                dt.Columns.Add("String de Conexão", typeof(string));
                dt.Columns.Add("Connection Status", typeof(string));

                if (configType == type.Destinos)
                {
                    string[,] unidades = Configuration.GetUnidades();
                    this.Text = "Configurar Destinos - Tesi Elettronica e Sistemi Informativi";
                    for (var i = 0; i < unidades.GetLength(0); i++)
                    {
                        DataRow row = dt.NewRow();
                        row["Key"] = unidades[i,1];
                        row["Nome da Unidade"] = unidades[i,1];
                        row["String de Conexão"] = (unidades[i, 2].IndexOf("Data Source=") >= 0 ? unidades[i, 2] : Encryption.Decrypt(unidades[i,2], Encryption.Key));
                        if (unidades[i, 2].IndexOf("Data Source=") >= 0) CfgHelper.Write("Main", "Destino" + (i + 1), unidades[i,0] + "|" + unidades[i, 1].Trim() + " |" + Encryption.Encrypt(unidades[i, 2], Encryption.Key));
                        row["Selecionar"] = false;
                        dt.Rows.Add(row);
                    }
                }

                else if (configType == type.Fonte)
                {
                    btnAdicionarUnidade.Visible = false;
                    btnDeletarUnidade.Visible = false; 
                    this.Text = "Configurar Fonte - Tesi Elettronica e Sistemi Informativi";
                    DataRow row = dt.NewRow();
                    var split = Configuration.GetFonte("Fonte").Split('|');
                    row["Key"] = split[0];
                    row["Nome da Unidade"] = split[1];
                    row["String de Conexão"] = (split[2].IndexOf("Data Source=") >= 0 ? split[2] : Encryption.Decrypt(split[2], Encryption.Key));
                    if (split[2].IndexOf("Data Source=") >= 0) CfgHelper.Write("Main", "Fonte", split[0] + "|" + split[1] + "|" + Encryption.Encrypt(split[2], Encryption.Key));
                    row["Selecionar"] = false;
                    dt.Rows.Add(row);
                }

                gridConfig.DataSource = dt;
                gridConfig.Columns[0].Visible = false;
                gridConfig.Columns[4].ReadOnly = true;
                gridConfig.Columns[2].ReadOnly = configType == type.Fonte;
                SetGridViewColumnWidth();
                gridConfig.Refresh();
            }
        }

        private void ChangeAllButtonsState(bool state)
        {
            btnAdicionarUnidade.Enabled = state;
            btnDeletarUnidade.Enabled = state;
            btnEditarUnidade.Enabled = state;
            btnSair.Enabled = state;
            btnSalvar.Enabled = state;
            btnTestConn.Enabled = state;
            btnTestConn.Text = state ? "Testar Conexão" : "Testando...";
            btnTestConn.Refresh();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            Configuration.DeleteAllDestinations();

            DialogResult dialog = MessageBox.Show("Deseja Realmente Salvar as Alterações no Arquivo de Configuração?", "Aviso", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
            {
                Dictionary<string, string[]> keyvalue = GetDataFromGridView();
                Dictionary<string, string[]> finalKeyValue = GetDataFromGridView();

                foreach (var c in keyvalue)
                {
                    if (c.Value[1].Trim() == string.Empty)
                    {
                        Configuration.DeleteConfiguration(c.Key);
                        finalKeyValue.Remove(c.Key);
                        logger.Log(Logger.Level.Info, "A Chave [" + c.Key + "] e suas Informações foram DELETADAS do arquivo de configuração");
                        usuarioSalvouAlgumaCoisa = true;
                    }
                }
                foreach (var c in finalKeyValue)
                {
                    if (c.Key != c.Value[0])
                    {
                        Configuration.UpdateConfiguration(c.Key, c.Value[0], c.Value[0].Trim() + "|" + c.Value[0].Trim() + " |" + c.Value[1], configType);
                        logger.Log(Logger.Level.Info, "A Chave [" + c.Key + "] foi ALTERADA para [" + c.Value[0] + "]");
                        usuarioSalvouAlgumaCoisa = true;
                    }
                    else if (c.Value[1] != Configuration.GetDestConnectionByKey(c.Key))
                    {
                        Configuration.UpdateConfiguration(c.Key, c.Value[0].Trim() + "|" + c.Value[0].Trim() + " |" + c.Value[1], configType);
                        logger.Log(Logger.Level.Info, "A Chave [" + c.Key + "] e suas informações foram ALTERADAS no arquivo de configuração");
                        usuarioSalvouAlgumaCoisa = true;
                    }
                }

                MessageBox.Show("Mudanças Salvas com Sucesso!");
            }
        }

        private void btnTestConn_Click(object sender, EventArgs e)
        {
            ChangeAllButtonsState(false);
            Dictionary<string, string[]> keyvalue = GetDataFromGridView();
            DataTable dt = new DataTable();
            dt.Columns.Add("Key", typeof(string));
            dt.Columns.Add("Selecionar", typeof(bool));
            dt.Columns.Add("Nome da Unidade", typeof(string));
            dt.Columns.Add("String de Conexão", typeof(string));
            dt.Columns.Add("Connection Status", typeof(string));

            //Caso nenhuma unidade tenha sido selecionada ele testa todas
            string testVal = "True";
            try
            {
                var test = keyvalue.First(x => x.Value[2] == "True").Key;
            }
            catch
            {
                testVal = "False";
            }

            foreach (var c in keyvalue)
            {
                DataRow row = dt.NewRow();
                var isSelected = c.Value[2] == testVal ? true : false;
                if (isSelected)
                {
                    try
                    {
                        row["Key"] = c.Key;
                        row["Nome da Unidade"] = c.Value[0];
                        row["String de Conexão"] = c.Value[1];
                        row["Selecionar"] = false;

                        using (SqlConnection conn = new SqlConnection(c.Value[1]))
                        {
                            try
                            {
                                conn.Open();
                                row["Connection Status"] = "Success";
                                logger.Log(Logger.Level.Info, "Teste de Conexão com " + row["Nome da Unidade"] + ": Sucess");
                            }
                            catch
                            {
                                row["Connection Status"] = "Fail";
                                logger.Log(Logger.Level.Error, "Teste de Conexão com " + row["Nome da Unidade"] + ": Fail");
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
                        row["Connection Status"] = "String em formato errado";
                        logger.Log(Logger.Level.Error, "Teste de Conexão com " + row["Nome da Unidade"] + ": String em formato Errado");
                    }
                    finally
                    {
                        dt.Rows.Add(row);
                    }
                }
                else
                {
                    row["Key"] = c.Key;
                    row["Nome da Unidade"] = c.Value[0];
                    row["String de Conexão"] = c.Value[1];
                    row["Selecionar"] = false;
                    dt.Rows.Add(row);
                }

            }

            gridConfig.DataSource = dt;
            gridConfig.Columns[0].Visible = false;
            gridConfig.Columns[2].ReadOnly = configType == type.Fonte;
            gridConfig.Columns[4].ReadOnly = true;
            SetGridViewColumnWidth();
            gridConfig.Refresh();
            ChangeAllButtonsState(true);

        }

        private void btnAdicionarUnidade_Click(object sender, EventArgs e)
        {
            DataTable data = (gridConfig.DataSource as DataTable);
            DataRow blankdata = data.NewRow();
            blankdata["Selecionar"] = false;
            blankdata["String de Conexão"] = "Data Source=srv-tesi,17001;User ID=;Password=;Initial Catalog=;";
            data.Rows.Add(blankdata);
            gridConfig.DataSource = data;
            gridConfig.Columns[0].Visible = false;
            gridConfig.Columns[4].ReadOnly = true;
            gridConfig.Columns[2].ReadOnly = configType == type.Fonte;
            SetGridViewColumnWidth();
            gridConfig.Refresh();
            logger.Log(Logger.Level.Warning, "Botão [Adicionar Unidade] Clicado");
        }

        private void btnEditarUnidade_Click(object sender, EventArgs e)
        {
            logger.Log(Logger.Level.Warning, "Botão [Editar Campo] Clicado");
            gridConfig.BeginEdit(true);
        }

        private void btnDeletarUnidade_Click(object sender, EventArgs e)
        {
            logger.Log(Logger.Level.Warning, "Botão [Deletar Unidade] Clicado");

            Dictionary<string, string[]> data = GetDataFromGridView();
            DataTable dt = new DataTable();
            dt.Columns.Add("Key", typeof(string));
            dt.Columns.Add("Selecionar", typeof(Boolean));
            dt.Columns.Add("Nome da Unidade", typeof(string));
            dt.Columns.Add("String de Conexão", typeof(string));
            dt.Columns.Add("Connection Status", typeof(string));

            foreach (var keyvalue in data)
            {
                if (Convert.ToBoolean(keyvalue.Value[2]))
                {
                    keyvalue.Value[0] = string.Empty;
                    keyvalue.Value[1] = string.Empty;
                }
                DataRow row = dt.NewRow();
                row["Key"] = keyvalue.Key;
                row["Nome da Unidade"] = keyvalue.Value[0];
                row["String de Conexão"] = keyvalue.Value[1];
                row["Selecionar"] = false;
                dt.Rows.Add(row);
            }

            gridConfig.DataSource = dt;
            gridConfig.Columns[0].Visible = false;
            gridConfig.Columns[4].ReadOnly = true;
            gridConfig.Columns[2].ReadOnly = configType == type.Fonte;
            SetGridViewColumnWidth();
            gridConfig.Refresh();
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void gridConfig_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                if (configType == type.Performance)
                    return;

                if (configType == type.Historico)
                {
                    if (gridConfig.Rows[e.RowIndex].Cells[4].Value.ToString() == "S")
                    {
                        gridConfig.Rows[e.RowIndex].Cells[4].Style.BackColor = Color.Green;
                        gridConfig.Rows[e.RowIndex].Cells[4].Style.ForeColor = Color.Green;
                    }
                    else
                    {
                        gridConfig.Rows[e.RowIndex].Cells[4].Style.BackColor = Color.Red;
                        gridConfig.Rows[e.RowIndex].Cells[4].Style.ForeColor = Color.Red;
                    }
                    return;
                }

                if (e.ColumnIndex == 1)
                {
                    gridConfig.Rows[e.RowIndex].Cells[1].Value = !Convert.ToBoolean(gridConfig.Rows[e.RowIndex].Cells[1].Value);
                    logger.Log(Logger.Level.Debug, "Linha:[" + e.RowIndex + "] [" + gridConfig.Rows[e.RowIndex].Cells[0].Value + "] Selecionada");
                }
                else
                {
                    logger.Log(Logger.Level.Debug, "Celula: [" + gridConfig.Rows[e.RowIndex].Cells[e.ColumnIndex].Value + "] Selecionada");
                }
            }
        }

        private void Configuracao_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (configType == type.Historico || configType == type.Performance)
                return;

            if (usuarioSalvouAlgumaCoisa)
                return;

            DialogResult dialog = MessageBox.Show("As informações alteradas não serão salvas", "Deseja Realmente Sair?", MessageBoxButtons.YesNo);
            e.Cancel = (dialog == DialogResult.No);
        }

        private void gridConfig_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (configType == type.Historico || configType == type.Performance)
                return;
            btnEditarUnidade.PerformClick();
        }

        private void gridConfig_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (configType == type.Historico || configType == type.Performance)
                return;
            btnEditarUnidade.PerformClick();
        }
    }
}
