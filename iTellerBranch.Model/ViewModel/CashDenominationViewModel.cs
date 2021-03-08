using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class CashDenominationViewModel
    {
        public int ID { get; set; }
        public string SubunitName { get; set; }
        public Nullable<int> RelationshipUnit { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public string Currency { get; set; }
        public virtual ICollection<DenominationViewModel> Denomination { get; set; }
    }
}
