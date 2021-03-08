using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class TillBalanceModel
    {
        public string TellerId { get; set; }
        public int CurrencyId { get; set; }
        public string BranchCode { get; set; } 
        public string CurrencyCode { get; set; }
        public string CIFnumber { get; set; } 
        public string FromTeller { get; set; }
        public string ToTeller { get; set; } 
    }
}
