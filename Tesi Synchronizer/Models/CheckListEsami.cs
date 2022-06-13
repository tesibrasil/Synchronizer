using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesi_Synchronizer.Models
{
public class CheckListEsami
    {
        public int ID { get; set; }
        public int IDChecklist { get; set; }
        public int IDTipoEsame { get; set; }
        public bool Eliminato { get; set; }
    }
}
