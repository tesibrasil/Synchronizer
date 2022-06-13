using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesi_Synchronizer.Models
{
   public class CodiciClassificazioneDiagnost
    {

        public int ID { get; set; }
        public string Codice { get; set; }
        public string Descrizione { get; set; }
        public int Classificazione { get; set; }
        public bool Positivita { get; set; }
        public int Score { get; set; }
        public int IDTipoEsame { get; set; }
        public bool Eliminato { get; set; }
        public int UO { get; set; }
    }
}
