using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Synchronizer
{

    public class CfgHelper
    {
        public static string sCfgFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\TesiSynchronizer.cfg";

        [DllImport("Kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("Kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public static long Write(string secao, string chave, string valor)
        {
            if (!File.Exists(sCfgFile)) File.Create(sCfgFile);
            return WritePrivateProfileString(secao, chave, valor, sCfgFile);
        }

        public static string Read(string secao, string chave)
        {
            if (!File.Exists(sCfgFile)) 
                File.Create(sCfgFile);

            var strRetVal = new StringBuilder(2048);
            GetPrivateProfileString(secao, chave, "", strRetVal, int.MaxValue, sCfgFile);
            //if(GetPrivateProfileString(secao, chave, "", strRetVal, int.MaxValue, sCfgFile) != 0)
            //{
            //    return strRetVal.ToString();
            //}
            //else
            //{
            //    // Verifica o último erro Win32.
            //    int err = Marshal.GetLastWin32Error();
            //    return null;
            //}


            return strRetVal.ToString();
        }

        public static string Read(string secao, string chave, string padrao)
        {
            if (!File.Exists(sCfgFile)) 
                File.Create(sCfgFile);

            var strRetVal = new StringBuilder(2048);
            GetPrivateProfileString(secao, chave, padrao, strRetVal, int.MaxValue, sCfgFile);

            if (strRetVal.ToString() == padrao || strRetVal.Length == 0) Write(secao, chave, padrao);

            return strRetVal.ToString();
        }

        public static string ReadAllSections()
        {
            if (!File.Exists(sCfgFile)) File.Create(sCfgFile);

            var strRetVal = new StringBuilder(2048);
            byte[] buffer = new byte[2048];
            GetPrivateProfileString(null, null, "", strRetVal, int.MaxValue, sCfgFile);
            
            buffer = Encoding.ASCII.GetBytes(strRetVal.ToString());

            string[] tmp = Encoding.ASCII.GetString(buffer).Trim('\0').Split('\0');

            string result = string.Empty;

            foreach (string entry in tmp)
            {
                result += (result.Length == 0 ? "" : ";") + entry.Split('=')[0];
            }

            return result;

            //return strRetVal.ToString();
            
        }
    }
}