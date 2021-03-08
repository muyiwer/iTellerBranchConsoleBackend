using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class DepositClosureDetailsModel
    {
        public class DepositClosureDetails
        {
            public string CompCode { get; set; }
            public string ArrangementId { get; set; }
            public string ValueDate { get; set; }
            public string Currency { get; set; }
            public string SettlementAcct { get; set; }
        }

        public class DepositClosure 
        {
            public DepositClosureDetails Request { get; set; }
        }
    }
}
