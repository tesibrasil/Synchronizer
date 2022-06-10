using System;
using System.Windows.Forms;

namespace Synchronizer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var s1 = Encryption.Encrypt("6vkmmm8w", Encryption.Key);
            var s2 = Encryption.Encrypt("9cw72eyx", Encryption.Key);
            var s3 = Encryption.Encrypt("7x7zwlea", Encryption.Key);
            var s4 = Encryption.Encrypt("2qwjn7vo", Encryption.Key);
            var s5 = Encryption.Encrypt("8utlhp5p", Encryption.Key);
            var s6 = Encryption.Encrypt("5qo2pyl9", Encryption.Key);
            var s7 = Encryption.Encrypt("6qzp3fpi", Encryption.Key);
            var s8 = Encryption.Encrypt("14jz9osy", Encryption.Key);
            var s9 = Encryption.Encrypt("39jucr2r", Encryption.Key);
            var s10 = Encryption.Encrypt("7hfuz3x2", Encryption.Key);

            var text = s1 + "|" + s2 + "|" + s3 + "|" + s4 + "|" + s5 + "|" + s6 + "|" + s7 + "|" + s8 + "|" + s9 + "|" + s10;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
