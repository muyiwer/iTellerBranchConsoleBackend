using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class DraftIssuedChargesModel
    {
        public long TranId { get; set; }
        public int ChargeID { get; set; }
        public decimal ChargeAmount { get; set; }
        public bool Approved { get; set; }

    }
}
