using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class DenominationViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> Value { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<int> CashDenominationID { get; set; }
        public virtual CashDenominationViewModel CashDenomination { get; set; }
    }
}
