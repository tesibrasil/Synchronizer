using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Synchronizer
{
    class ReportBuilder
    {
        private static ReportBuilder m_ctrl = new ReportBuilder();
        public static ReportBuilder Get() { return m_ctrl; }

        public void Write(string strLog, string file)
        {
            Console.WriteLine(strLog);
            DateTime dateTimeNow = DateTime.Now;
            string strDate = String.Format("{0:d2}/{1:d2}/{2:d2} {3:d2}:{4:d2}:{5:d2},", dateTimeNow.Day, dateTimeNow.Month, dateTimeNow.Year, dateTimeNow.Hour, dateTimeNow.Minute, dateTimeNow.Second);
            try
            {
                string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + file);
                StreamWriter stream = File.AppendText(filePath);
                stream.WriteLine(strDate + strLog);
                stream.Close();
            }
            catch (Exception)
            {
                //
            }
        }
        public void Clear(string filePath)
        {
            File.Delete(filePath);
        }
    }
}
