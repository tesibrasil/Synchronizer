using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesi_Synchronizer.Models
{
  public class Checklist
    {
        public int ID { get; set; }
        public string Codice { get; set; }
        public string  Descrizione { get; set; }
        public int Presentazione { get; set; }
        public bool ItemAlmenouno { get; set; }
        public bool ItemPiudiuno { get; set; }
        public int CampoCL { get; set; }
        public int Ordine { get; set; }
        public bool Eliminato { get; set; }
        public int UO { get; set; }

    }
}
