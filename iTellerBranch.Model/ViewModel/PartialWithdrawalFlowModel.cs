using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class PartialWithdrawalFlowModel
    {
        public class PartialWithdrawal
        {
            public string TransactionBranch { get; set; }
            public string LdId { get; set; }
            public string Amount { get; set; }
            public string WithdrawalDate { get; set; }
        }

        public class PartialWithdrawalFlowRequestModel 
        {
            public PartialWithdrawal PartialWithdrawal { get; set; }
        }
    }
}
