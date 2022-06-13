using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesi_Synchronizer.Models
{
 public class CheckListItemRegole
    {
        
        public int ID { get; set; }
        public int IDChecklistItem { get; set; }
        public int IDChecklistItemBind { get; set; }
        public int TipoRegola { get; set; }
        public bool Eliminato { get; set; }
    }
}
