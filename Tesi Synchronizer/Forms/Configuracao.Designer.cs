namespace Synchronizer
{
    partial class Configuracao
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Configuracao));
            this.gridConfig = new System.Windows.Forms.DataGridView();
            this.btnSalvar = new System.Windows.Forms.Button();
            this.btnSair = new System.Windows.Forms.Button();
            this.btnTestConn = new System.Windows.Forms.Button();
            this.btnAdicionarUnidade = new System.Windows.Forms.Button();
            this.btnEditarUnidade = new System.Windows.Forms.Button();
            this.btnDeletarUnidade = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridConfig)).BeginInit();
            this.SuspendLayout();
            // 
            // gridConfig
            // 
            this.gridConfig.AllowUserToAddRows = false;
            this.gridConfig.AllowUserToDeleteRows = false;
            this.gridConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridConfig.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.gridConfig.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.gridConfig.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridConfig.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.gridConfig.Location = new System.Drawing.Point(13, 53);
            this.gridConfig.Name = "gridConfig";
            this.gridConfig.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gridConfig.Size = new System.Drawing.Size(1159, 617);
            this.gridConfig.TabIndex = 0;
            this.gridConfig.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridConfig_CellClick);
            this.gridConfig.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridConfig_CellContentDoubleClick);
            this.gridConfig.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridConfig_CellDoubleClick);
            // 
            // btnSalvar
            // 
            this.btnSalvar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSalvar.Location = new System.Drawing.Point(1016, 12);
            this.btnSalvar.Name = "btnSalvar";
            this.btnSalvar.Size = new System.Drawing.Size(75, 35);
            this.btnSalvar.TabIndex = 1;
            this.btnSalvar.Text = "Salvar";
            this.btnSalvar.UseVisualStyleBackColor = true;
            this.btnSalvar.Click += new System.EventHandler(this.btnSalvar_Click);
            // 
            // btnSair
            // 
            this.btnSair.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSair.Location = new System.Drawing.Point(1097, 12);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(75, 35);
            this.btnSair.TabIndex = 2;
            this.btnSair.Text = "Sair";
            this.btnSair.UseVisualStyleBackColor = true;
            this.btnSair.Click += new System.EventHandler(this.btnSair_Click);
            // 
            // btnTestConn
            // 
            this.btnTestConn.Location = new System.Drawing.Point(13, 12);
            this.btnTestConn.Name = "btnTestConn";
            this.btnTestConn.Size = new System.Drawing.Size(140, 35);
            this.btnTestConn.TabIndex = 3;
            this.btnTestConn.Text = "Testar Connexão";
            this.btnTestConn.UseVisualStyleBackColor = true;
            this.btnTestConn.Click += new System.EventHandler(this.btnTestConn_Click);
            // 
            // btnAdicionarUnidade
            // 
            this.btnAdicionarUnidade.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnAdicionarUnidade.Location = new System.Drawing.Point(371, 12);
            this.btnAdicionarUnidade.Name = "btnAdicionarUnidade";
            this.btnAdicionarUnidade.Size = new System.Drawing.Size(146, 35);
            this.btnAdicionarUnidade.TabIndex = 4;
            this.btnAdicionarUnidade.Text = "Adicionar Unidade";
            this.btnAdicionarUnidade.UseVisualStyleBackColor = true;
            this.btnAdicionarUnidade.Click += new System.EventHandler(this.btnAdicionarUnidade_Click);
            // 
            // btnEditarUnidade
            // 
            this.btnEditarUnidade.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnEditarUnidade.Location = new System.Drawing.Point(523, 12);
            this.btnEditarUnidade.Name = "btnEditarUnidade";
            this.btnEditarUnidade.Size = new System.Drawing.Size(146, 35);
            this.btnEditarUnidade.TabIndex = 5;
            this.btnEditarUnidade.Text = "Editar Campo";
            this.btnEditarUnidade.UseVisualStyleBackColor = true;
            this.btnEditarUnidade.Click += new System.EventHandler(this.btnEditarUnidade_Click);
            // 
            // btnDeletarUnidade
            // 
            this.btnDeletarUnidade.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnDeletarUnidade.Location = new System.Drawing.Point(675, 12);
            this.btnDeletarUnidade.Name = "btnDeletarUnidade";
            this.btnDeletarUnidade.Size = new System.Drawing.Size(146, 35);
            this.btnDeletarUnidade.TabIndex = 6;
            this.btnDeletarUnidade.Text = "Deletar Unidade";
            this.btnDeletarUnidade.UseVisualStyleBackColor = true;
            this.btnDeletarUnidade.Click += new System.EventHandler(this.btnDeletarUnidade_Click);
            // 
            // Configuracao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 682);
            this.Controls.Add(this.btnDeletarUnidade);
            this.Controls.Add(this.btnEditarUnidade);
            this.Controls.Add(this.btnAdicionarUnidade);
            this.Controls.Add(this.btnTestConn);
            this.Controls.Add(this.btnSair);
            this.Controls.Add(this.btnSalvar);
            this.Controls.Add(this.gridConfig);
            this.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1200, 720);
            this.Name = "Configuracao";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuracao - Tesi Elettronica e Sistemi Informativi";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Configuracao_FormClosing);
            this.Load += new System.EventHandler(this.Configuracao_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridConfig)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridConfig;
        private System.Windows.Forms.Button btnSalvar;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.Button btnTestConn;
        private System.Windows.Forms.Button btnAdicionarUnidade;
        private System.Windows.Forms.Button btnEditarUnidade;
        private System.Windows.Forms.Button btnDeletarUnidade;
    }
}