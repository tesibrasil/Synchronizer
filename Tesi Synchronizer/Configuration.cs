using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Synchronizer
{
    static class Configuration
    {
        public static string GetUser()
        {
            return CfgHelper.Read("Main", "user", "admin|fleury1|fleury2|fleury3|fleury4|fleury5|fleury6|fleury7|fleury8|fleury9");
        }
        public static string GetPass()
        {
            return CfgHelper.Read("Main", "pass", "fs0nYcwVPVYoDBEK5Gnb7ZdDWsNwE2BmmNfFBUwCkzXrrl86Gs2csQ26+oFSGrMUJ37DXqUkme3h1thgHQ26a79q5dCDT7RP9mRAxBaEbTm5vo7tvTB7FPeoasKIgmrt|fs0nYcwVPVYoDBEK5Gnb7ZdDWsNwE2BmmNfFBUwCkzXrrl86Gs2csQ26+oFSGrMUJ37DXqUkme3h1thgHQ26a79q5dCDT7RP9mRAxBaEbTm5vo7tvTB7FPeoasKIgmrt|fs0nYcwVPVYoDBEK5Gnb7ZdDWsNwE2BmmNfFBUwCkzXrrl86Gs2csQ26+oFSGrMUJ37DXqUkme3h1thgHQ26a79q5dCDT7RP9mRAxBaEbTm5vo7tvTB7FPeoasKIgmrt|fs0nYcwVPVYoDBEK5Gnb7ZdDWsNwE2BmmNfFBUwCkzXrrl86Gs2csQ26+oFSGrMUJ37DXqUkme3h1thgHQ26a79q5dCDT7RP9mRAxBaEbTm5vo7tvTB7FPeoasKIgmrt|fs0nYcwVPVYoDBEK5Gnb7ZdDWsNwE2BmmNfFBUwCkzXrrl86Gs2csQ26+oFSGrMUJ37DXqUkme3h1thgHQ26a79q5dCDT7RP9mRAxBaEbTm5vo7tvTB7FPeoasKIgmrt|fs0nYcwVPVYoDBEK5Gnb7ZdDWsNwE2BmmNfFBUwCkzXrrl86Gs2csQ26+oFSGrMUJ37DXqUkme3h1thgHQ26a79q5dCDT7RP9mRAxBaEbTm5vo7tvTB7FPeoasKIgmrt|fs0nYcwVPVYoDBEK5Gnb7ZdDWsNwE2BmmNfFBUwCkzXrrl86Gs2csQ26+oFSGrMUJ37DXqUkme3h1thgHQ26a79q5dCDT7RP9mRAxBaEbTm5vo7tvTB7FPeoasKIgmrt|fs0nYcwVPVYoDBEK5Gnb7ZdDWsNwE2BmmNfFBUwCkzXrrl86Gs2csQ26+oFSGrMUJ37DXqUkme3h1thgHQ26a79q5dCDT7RP9mRAxBaEbTm5vo7tvTB7FPeoasKIgmrt|fs0nYcwVPVYoDBEK5Gnb7ZdDWsNwE2BmmNfFBUwCkzXrrl86Gs2csQ26+oFSGrMUJ37DXqUkme3h1thgHQ26a79q5dCDT7RP9mRAxBaEbTm5vo7tvTB7FPeoasKIgmrt|fs0nYcwVPVYoDBEK5Gnb7ZdDWsNwE2BmmNfFBUwCkzXrrl86Gs2csQ26+oFSGrMUJ37DXqUkme3h1thgHQ26a79q5dCDT7RP9mRAxBaEbTm5vo7tvTB7FPeoasKIgmrt");
        }
        public static string GetSourceConnectionByKey(string key)
        {
            var split = CfgHelper.Read("Main", key).Split('|');
            var connection = split[2].IndexOf("Data Source=") >= 0 ? split[2] : Encryption.Decrypt(split[2], Encryption.Key);
            if (split[2].IndexOf("Data Source=") >= 0) CfgHelper.Write("Main", "Fonte", split[0] + "|" + split[1] + "|" + Encryption.Encrypt(split[2], Encryption.Key));
            return connection;
        }

        public static string GetDestConnectionByKey(string key)
        {
            var qtde = GetQuantity();
            var count = 0;

            for (var i = 0; i < 100; i++)
            {
                if (qtde == count)
                    break;
                var split = CfgHelper.Read("Destinos", "Destino" + (i + 1)).Split('|');
                if (split.Length == 3)
                {
                    if (split[1] == key)
                    {
                        count++;
                        var connection = split[2].IndexOf("Data Source=") >= 0 ? split[2] : Encryption.Decrypt(split[2], Encryption.Key);
                        if (split[2].IndexOf("Data Source=") >= 0) CfgHelper.Write("Destinos", "Destino" + (i + 1), split[0] + "|" + split[1].Trim() + " |" + Encryption.Encrypt(connection, Encryption.Key));
                        return connection;
                    }
                }
            }

            return "";
        }
        public static string GetFonte(string key)
        {
            return CfgHelper.Read("Main", key);
        }
        public static string[,] GetUnidades()
        {
            var qtde = GetQuantity();
            var count = 0;
            string[,] unidades = new string[qtde, 3];

            for (var i = 0; i < 100; i++)
            {
                if (qtde == count)
                    break;
                var key = "Destino" + (i + 1);
                var split = CfgHelper.Read("Destinos", key).Split('|');
                if (split.Length == 3)
                {
                    unidades[count, 0] = split[0];
                    unidades[count, 1] = split[1];
                    unidades[count, 2] = split[2];
                    count++;
                }
            }
        
            return unidades;
        }
        public static List<string> GetHistorico()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\report.txt");
            List<string> h = new List<string>();
            if (File.Exists(path))
            {
                foreach (string line in File.ReadAllLines(path))
                {
                    h.Add(line);
                }
            }
            return h;
        }
        public static List<string> GetPerformance()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\performance.txt");
            List<string> p = new List<string>();
            if (File.Exists(path))
            {
                foreach (string line in File.ReadAllLines(path))
                {
                    p.Add(line);
                }
            }
            return p;
        }

        public static void AddConfiguration(string key, string value)
        {
            //System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //config.AppSettings.Settings.Add(key, Encryption.Encrypt(value, Encryption.Key));
            //config.Save(ConfigurationSaveMode.Modified, true);
            //ConfigurationManager.RefreshSection("appSettings");            

            //CfgHelper.Write("Main", key, Encryption.Encrypt(value, Encryption.Key));
        }
        public static void UpdateConfiguration(string key, string value, Configuracao.type type)
        {
            var split = value.Split('|');
            if (type == Configuracao.type.Destinos)
            {
                var qtde = GetQuantity() == -1 ? 0 : GetQuantity();
                var newKey = GetNextKey();
                

                CfgHelper.Write("Destinos", newKey, split[0] + "|" + split[1] + "|" + Encryption.Encrypt(split[2], Encryption.Key));
                CfgHelper.Write("Destinos", "Quantidade", (qtde + 1).ToString());
            }
            else
            {
                CfgHelper.Write("Main", "Fonte", split[0] + "|" + split[1] + "|" + Encryption.Encrypt(split[2], Encryption.Key));
            }
        }
        public static void UpdateConfiguration(string oldKey, string newKey, string value, Configuracao.type type)
        {
            var split = value.Split('|');

            if (type == Configuracao.type.Destinos)
            {
                var key = GetNextKey();
                var qtde = GetQuantity() == -1 ? 0 : GetQuantity();

                var oldValues = GetDestValue(oldKey);
                if (oldValues.Length < 1)
                {
                    CfgHelper.Write("Destinos", key, split[0] + "|" + split[1] + "|" + Encryption.Encrypt(split[2], Encryption.Key));
                    CfgHelper.Write("Destinos", "Quantidade", (qtde + 1).ToString());
                }
                else
                {
                    CfgHelper.Write("Destinos", oldKey, split[0] + "|" + split[1] + "|" + Encryption.Encrypt(split[2], Encryption.Key));
                }
            }
            else
            {
                CfgHelper.Write("Main", "Fonte", split[0] + "|" + split[1] + "|" + Encryption.Encrypt(split[2], Encryption.Key));
            }
        }
        public static void DeleteConfiguration(string key)
        {
            //CfgHelper.Write("Destinos", "Quantidade", (GetQuantity() - 1).ToString());

            CfgHelper.Write("Destinos", GetDestKey(key), null);
        }

        private static string[] GetDestValue(string key)
        {
            for (var i = 0; i < 100; i++)
            {
                var split = CfgHelper.Read("Destinos", "Destino" + (i + 1)).Split('|');
                if (split.Length == 3)
                {
                    if (split[1] == key)
                    {
                        return split;
                    }
                }
            }

            return new string[0];
        }

        private static int GetQuantity()
        {
            return Convert.ToInt32(CfgHelper.Read("Destinos", "Quantidade"));
        }

        private static string GetDestKey(string key)
        {
            for (var i = 1; i < 100; i++)
            {
                var split = CfgHelper.Read("Destinos", "Destino" + i).Split('|');
                if (split.Length == 3 && split[1] == key) return "Destino" + i;
            }

            return "";
        }

       private static string GetNextKey()
        {
            for (var i = 1; i < 100; i++)
            {
                var split = CfgHelper.Read("Destinos", "Destino" + i).Split('|');
                if (split.Length != 3)
                    return "Destino" + i;
            }

            return "";
        }
        public static void DeleteAllDestinations()
        {
            CfgHelper.Write("Destinos", null, null);
            CfgHelper.Write("Destinos", "Quantidade", "0");

            //CfgHelper.Write("Main", GetDestKey(key), null);
        }
    }
}
