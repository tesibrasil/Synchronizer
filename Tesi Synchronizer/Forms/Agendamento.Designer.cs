namespace Synchronizer
{
    partial class Agendamento
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
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.btnAgora = new System.Windows.Forms.Button();
            this.btnAgendar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 96);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(179, 18);
            this.label2.TabIndex = 1;
            this.label2.Text = "Agendar Execução Para";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CustomFormat = "dd-MM-yyyy HH:mm";
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(19, 126);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(241, 26);
            this.dateTimePicker1.TabIndex = 2;
            // 
            // btnAgora
            // 
            this.btnAgora.Location = new System.Drawing.Point(19, 54);
            this.btnAgora.Name = "btnAgora";
            this.btnAgora.Size = new System.Drawing.Size(241, 32);
            this.btnAgora.TabIndex = 3;
            this.btnAgora.Text = "Iniciar Agora";
            this.btnAgora.UseVisualStyleBackColor = true;
            this.btnAgora.Click += new System.EventHandler(this.btnAgora_Click);
            // 
            // btnAgendar
            // 
            this.btnAgendar.Location = new System.Drawing.Point(19, 158);
            this.btnAgendar.Name = "btnAgendar";
            this.btnAgendar.Size = new System.Drawing.Size(241, 32);
            this.btnAgendar.TabIndex = 4;
            this.btnAgendar.Text = "Agendar";
            this.btnAgendar.UseVisualStyleBackColor = true;
            this.btnAgendar.Click += new System.EventHandler(this.btnAgendar_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(237, 18);
            this.label1.TabIndex = 5;
            this.label1.Text = "Escolha uma forma de Execução";
            // 
            // Agendamento
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(278, 203);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnAgendar);
            this.Controls.Add(this.btnAgora);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Agendamento";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Agendamento - Tesi Elettronica e Sistemi Informativi";            
            this.Load += new System.EventHandler(this.Agendamento_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Button btnAgora;
        private System.Windows.Forms.Button btnAgendar;
        private System.Windows.Forms.Label label1;
    }
}