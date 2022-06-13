using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesi_Synchronizer.Models
{
   public class CheckListItem
    {
        public int ID { get; set; }
        public int IDCheckList { get; set; }
        public int IDPadre { get; set; }
        public int Ordine { get; set; }
        public string Titolo { get; set; }
        public string TestoRTF { get; set; }
        public string TestoTXT { get; set; }
        public int TestoNumeroVariabili { get; set; }
        public bool ItemAlmenouno { get; set; }
        public bool ItemPiudiuno { get; set; }
        public bool Eliminato { get; set; }
        public int ClassificazioneDiagnosi { get; set; }
        public int IDOriginaleClonato { get; set; }

    }
}
