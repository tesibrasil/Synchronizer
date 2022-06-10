using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Synchronizer
{
    public partial class Agendamento : Form
    {
        public bool Cancel = true;
        public bool Agendado = false;
        public DateTime Hora;

        public Agendamento()
        {
            InitializeComponent();
        }

        private void btnAgora_Click(object sender, EventArgs e)
        {
            Cancel = false;
            this.Close();
        }

        private void btnAgendar_Click(object sender, EventArgs e)
        {
            var d = dateTimePicker1.Value;
            var displayData = Convert.ToDateTime(d.Day + "/" + d.Month + "/" + d.Year + " " + d.Hour + ":" + d.Minute + ":00");
            if (MessageBox.Show("Tem certeza que quer agendar a execução para " + displayData + Environment.NewLine + "Lembre-se que após a confirmação desta mensagem e realização do login, o programa irá ficar minimizado e desabilitado até que a data e hora selecionada chegue." + Environment.NewLine + Environment.NewLine + "Caso queira encerrar o programa acesse o Task Manager e encerre o processo do programa...", "Confirmação necessaria", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Agendado = true;
                Hora = displayData;
                Cancel = false;
                this.Close();
            }
        }

        private void Agendamento_Load(object sender, EventArgs e)
        {
            dateTimePicker1.MinDate = DateTime.Now;            
        }
    }
}
