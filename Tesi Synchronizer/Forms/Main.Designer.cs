namespace Synchronizer
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.cklUnidades = new System.Windows.Forms.CheckedListBox();
            this.btnSyncChecklist = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.principalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configurarFonteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configurarDestinosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.historicoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.performanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCheckUncheck = new System.Windows.Forms.Button();
            this.btnSair = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.cbxFiltro = new System.Windows.Forms.ComboBox();
            this.lblAviso = new System.Windows.Forms.Label();
            this.btnSyncFrases = new System.Windows.Forms.Button();
            this.btnRollBackCheckList = new System.Windows.Forms.Button();
            this.btnRollBackFrases = new System.Windows.Forms.Button();
            this.lblAvisoUnidade = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cklUnidades
            // 
            this.cklUnidades.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cklUnidades.CheckOnClick = true;
            this.cklUnidades.ColumnWidth = 400;
            this.cklUnidades.FormattingEnabled = true;
            this.cklUnidades.HorizontalScrollbar = true;
            this.cklUnidades.Location = new System.Drawing.Point(12, 36);
            this.cklUnidades.MultiColumn = true;
            this.cklUnidades.Name = "cklUnidades";
            this.cklUnidades.Size = new System.Drawing.Size(582, 634);
            this.cklUnidades.TabIndex = 0;
            this.cklUnidades.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.cklUnidades_ItemCheck);
            this.cklUnidades.SelectedIndexChanged += new System.EventHandler(this.cklUnidades_SelectedIndexChanged);
            // 
            // btnSyncChecklist
            // 
            this.btnSyncChecklist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSyncChecklist.Location = new System.Drawing.Point(611, 641);
            this.btnSyncChecklist.Name = "btnSyncChecklist";
            this.btnSyncChecklist.Size = new System.Drawing.Size(127, 29);
            this.btnSyncChecklist.TabIndex = 1;
            this.btnSyncChecklist.Text = "Sync CheckList";
            this.btnSyncChecklist.UseVisualStyleBackColor = true;
            this.btnSyncChecklist.Click += new System.EventHandler(this.btnSyncChecklist_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip1.Font = new System.Drawing.Font("Arial", 10F);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.principalToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1184, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "SINCRONIZANDO";
            // 
            // principalToolStripMenuItem
            // 
            this.principalToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configurarFonteToolStripMenuItem,
            this.configurarDestinosToolStripMenuItem,
            this.historicoToolStripMenuItem,
            this.performanceToolStripMenuItem});
            this.principalToolStripMenuItem.Name = "principalToolStripMenuItem";
            this.principalToolStripMenuItem.Size = new System.Drawing.Size(74, 20);
            this.principalToolStripMenuItem.Text = "Principal";
            // 
            // configurarFonteToolStripMenuItem
            // 
            this.configurarFonteToolStripMenuItem.Name = "configurarFonteToolStripMenuItem";
            this.configurarFonteToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.configurarFonteToolStripMenuItem.Text = "Configurar Fonte";
            this.configurarFonteToolStripMenuItem.Click += new System.EventHandler(this.configurarFonteToolStripMenuItem_Click);
            // 
            // configurarDestinosToolStripMenuItem
            // 
            this.configurarDestinosToolStripMenuItem.Name = "configurarDestinosToolStripMenuItem";
            this.configurarDestinosToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.configurarDestinosToolStripMenuItem.Text = "Configurar Destinos";
            this.configurarDestinosToolStripMenuItem.Click += new System.EventHandler(this.configurarDestinosToolStripMenuItem_Click);
            // 
            // historicoToolStripMenuItem
            // 
            this.historicoToolStripMenuItem.Name = "historicoToolStripMenuItem";
            this.historicoToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.historicoToolStripMenuItem.Text = "Historico";
            this.historicoToolStripMenuItem.Click += new System.EventHandler(this.historicoToolStripMenuItem_Click);
            // 
            // performanceToolStripMenuItem
            // 
            this.performanceToolStripMenuItem.Name = "performanceToolStripMenuItem";
            this.performanceToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.performanceToolStripMenuItem.Text = "Performance";
            this.performanceToolStripMenuItem.Click += new System.EventHandler(this.performanceToolStripMenuItem_Click);
            // 
            // btnCheckUncheck
            // 
            this.btnCheckUncheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCheckUncheck.Location = new System.Drawing.Point(611, 36);
            this.btnCheckUncheck.Name = "btnCheckUncheck";
            this.btnCheckUncheck.Size = new System.Drawing.Size(115, 29);
            this.btnCheckUncheck.TabIndex = 3;
            this.btnCheckUncheck.Text = "c/u";
            this.btnCheckUncheck.UseVisualStyleBackColor = true;
            this.btnCheckUncheck.Click += new System.EventHandler(this.btnCheckUncheck_Click);
            // 
            // btnSair
            // 
            this.btnSair.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSair.Location = new System.Drawing.Point(1056, 36);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(115, 29);
            this.btnSair.TabIndex = 5;
            this.btnSair.Text = "Sair";
            this.btnSair.UseVisualStyleBackColor = true;
            this.btnSair.Click += new System.EventHandler(this.btnSair_Click);
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.ColumnWidth = 560;
            this.listBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalExtent = 3000;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.ItemHeight = 18;
            this.listBox1.Location = new System.Drawing.Point(611, 71);
            this.listBox1.Name = "listBox1";
            this.listBox1.ScrollAlwaysVisible = true;
            this.listBox1.Size = new System.Drawing.Size(561, 562);
            this.listBox1.TabIndex = 6;
            // 
            // cbxFiltro
            // 
            this.cbxFiltro.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxFiltro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxFiltro.FormattingEnabled = true;
            this.cbxFiltro.ItemHeight = 18;
            this.cbxFiltro.Items.AddRange(new object[] {
            "Todos",
            "Fleury",
            "A+",
            "São Paulo",
            "Rio de Janeiro",
            "Bahia",
            "Pernambuco",
            "Hospitais"});
            this.cbxFiltro.Location = new System.Drawing.Point(732, 36);
            this.cbxFiltro.Name = "cbxFiltro";
            this.cbxFiltro.Size = new System.Drawing.Size(318, 26);
            this.cbxFiltro.TabIndex = 7;
            this.cbxFiltro.SelectedIndexChanged += new System.EventHandler(this.cbxFiltro_SelectedIndexChanged);
            // 
            // lblAviso
            // 
            this.lblAviso.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblAviso.AutoSize = true;
            this.lblAviso.ForeColor = System.Drawing.Color.DarkRed;
            this.lblAviso.Location = new System.Drawing.Point(457, 9);
            this.lblAviso.Name = "lblAviso";
            this.lblAviso.Size = new System.Drawing.Size(137, 18);
            this.lblAviso.TabIndex = 8;
            this.lblAviso.Text = "SINCRONIZANDO";
            // 
            // btnSyncFrases
            // 
            this.btnSyncFrases.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSyncFrases.Location = new System.Drawing.Point(741, 641);
            this.btnSyncFrases.Name = "btnSyncFrases";
            this.btnSyncFrases.Size = new System.Drawing.Size(127, 29);
            this.btnSyncFrases.TabIndex = 9;
            this.btnSyncFrases.Text = "Sync Frases";
            this.btnSyncFrases.UseVisualStyleBackColor = true;
            this.btnSyncFrases.Visible = false;
            this.btnSyncFrases.Click += new System.EventHandler(this.btnSyncFrases_Click);
            // 
            // btnRollBackCheckList
            // 
            this.btnRollBackCheckList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRollBackCheckList.Location = new System.Drawing.Point(871, 641);
            this.btnRollBackCheckList.Name = "btnRollBackCheckList";
            this.btnRollBackCheckList.Size = new System.Drawing.Size(150, 29);
            this.btnRollBackCheckList.TabIndex = 10;
            this.btnRollBackCheckList.Text = "Rollback CheckList";
            this.btnRollBackCheckList.UseVisualStyleBackColor = true;
            this.btnRollBackCheckList.Click += new System.EventHandler(this.btnRollBackCheckList_Click);
            // 
            // btnRollBackFrases
            // 
            this.btnRollBackFrases.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRollBackFrases.Location = new System.Drawing.Point(1024, 641);
            this.btnRollBackFrases.Name = "btnRollBackFrases";
            this.btnRollBackFrases.Size = new System.Drawing.Size(150, 29);
            this.btnRollBackFrases.TabIndex = 11;
            this.btnRollBackFrases.Text = "Rollback Frases";
            this.btnRollBackFrases.UseVisualStyleBackColor = true;
            this.btnRollBackFrases.Visible = false;
            this.btnRollBackFrases.Click += new System.EventHandler(this.btnRollBackFrases_Click);
            // 
            // lblAvisoUnidade
            // 
            this.lblAvisoUnidade.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblAvisoUnidade.AutoSize = true;
            this.lblAvisoUnidade.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAvisoUnidade.ForeColor = System.Drawing.Color.DarkRed;
            this.lblAvisoUnidade.Location = new System.Drawing.Point(608, 9);
            this.lblAvisoUnidade.Name = "lblAvisoUnidade";
            this.lblAvisoUnidade.Size = new System.Drawing.Size(144, 19);
            this.lblAvisoUnidade.TabIndex = 12;
            this.lblAvisoUnidade.Text = "SINCRONIZANDO";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 682);
            this.Controls.Add(this.lblAvisoUnidade);
            this.Controls.Add(this.lblAviso);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.btnRollBackFrases);
            this.Controls.Add(this.btnRollBackCheckList);
            this.Controls.Add(this.btnSyncFrases);
            this.Controls.Add(this.btnCheckUncheck);
            this.Controls.Add(this.cbxFiltro);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.btnSair);
            this.Controls.Add(this.btnSyncChecklist);
            this.Controls.Add(this.cklUnidades);
            this.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1200, 720);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bem vindo ao Synchronizer - Tesi Elettronica e Sistemi Informativi";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox cklUnidades;
        private System.Windows.Forms.Button btnSyncChecklist;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem principalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configurarFonteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configurarDestinosToolStripMenuItem;
        private System.Windows.Forms.Button btnCheckUncheck;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ComboBox cbxFiltro;
        private System.Windows.Forms.Label lblAviso;
        private System.Windows.Forms.Button btnSyncFrases;
        private System.Windows.Forms.Button btnRollBackCheckList;
        private System.Windows.Forms.Button btnRollBackFrases;
        private System.Windows.Forms.Label lblAvisoUnidade;
        private System.Windows.Forms.ToolStripMenuItem historicoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem performanceToolStripMenuItem;
    }
}

