using System;
using System.Windows.Forms;

namespace Synchronizer
{
    public partial class Login : Form
    {
        
        public bool usuarioLogado;
        public Enumerators.TipoUsuaio tipoUsuario;
        public string nomeUsuario;

        public Login()
        {            
            InitializeComponent();
            usuarioLogado = false;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var users = Configuration.GetUser().Split('|');
            var passes = Configuration.GetPass().Split('|');

            var user1 = users[0];
            var user2 = users[1];
            var user3 = users[2];
            var user4 = users[3];
            var user5 = users[4];
            var user6 = users[5];
            var user7 = users[6];
            var user8 = users[7];
            var user9 = users[8];
            var user10 = users[9];

            var pass = passes[0];            
            var senha1 = Encryption.Decrypt(pass, Encryption.Key);            

            pass = passes[1];
            var senha2 = Encryption.Decrypt(pass, Encryption.Key);

            pass = passes[2];
            var senha3 = Encryption.Decrypt(pass, Encryption.Key);

            pass = passes[3];
            var senha4 = Encryption.Decrypt(pass, Encryption.Key);

            pass = passes[4];
            var senha5 = Encryption.Decrypt(pass, Encryption.Key);

            pass = passes[5];
            var senha6 = Encryption.Decrypt(pass, Encryption.Key);

            pass = passes[6];
            var senha7 = Encryption.Decrypt(pass, Encryption.Key);

            pass = passes[7];
            var senha8 = Encryption.Decrypt(pass, Encryption.Key);

            pass = passes[8];
            var senha9 = Encryption.Decrypt(pass, Encryption.Key);

            pass = passes[9];
            var senha10 = Encryption.Decrypt(pass, Encryption.Key);                       

            var isAdmin = (txtUsuario.Text.ToUpper() == user1.ToUpper() && txtSenha.Text == senha1);
            nomeUsuario = isAdmin ? user1 : "";

            var isFleury = false;
            if(!isAdmin)
            {
                if (txtUsuario.Text.ToUpper() == user2.ToUpper() && txtSenha.Text == senha2)
                {
                    isFleury = true;
                    nomeUsuario = user2;
                }
                else if (txtUsuario.Text.ToUpper() == user3.ToUpper() && txtSenha.Text == senha3)
                {
                    isFleury = true;
                    nomeUsuario = user3;
                }
                else if (txtUsuario.Text.ToUpper() == user4.ToUpper() && txtSenha.Text == senha4)
                {
                    isFleury = true;
                    nomeUsuario = user4;
                }
                else if (txtUsuario.Text.ToUpper() == user5.ToUpper() && txtSenha.Text == senha5)
                {
                    isFleury = true;
                    nomeUsuario = user5;
                }
                else if (txtUsuario.Text.ToUpper() == user6.ToUpper() && txtSenha.Text == senha6)
                {
                    isFleury = true;
                    nomeUsuario = user6;
                }
                else if (txtUsuario.Text.ToUpper() == user7.ToUpper() && txtSenha.Text == senha7)
                {
                    isFleury = true;
                    nomeUsuario = user7;
                }
                else if (txtUsuario.Text.ToUpper() == user8.ToUpper() && txtSenha.Text == senha8)
                {
                    isFleury = true;
                    nomeUsuario = user8;
                }
                else if (txtUsuario.Text.ToUpper() == user9.ToUpper() && txtSenha.Text == senha9)
                {
                    isFleury = true;
                    nomeUsuario = user9;
                }
                else if (txtUsuario.Text.ToUpper() == user10.ToUpper() && txtSenha.Text == senha10)
                {
                    isFleury = true;
                    nomeUsuario = user10;
                }
            }
            
            
            if (!isAdmin && !isFleury)
            {
                MessageBox.Show("Usuário ou senha incorretos");
                txtUsuario.Text = txtUsuario.Text.ToUpper();
                txtSenha.Text = string.Empty;
            }
            else
            {                
                usuarioLogado = true;
                tipoUsuario = isAdmin ? Enumerators.TipoUsuaio.Admin : Enumerators.TipoUsuaio.Fleury;
                this.Close();
            }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            usuarioLogado = false;
            this.Close();
        }
    }
}
