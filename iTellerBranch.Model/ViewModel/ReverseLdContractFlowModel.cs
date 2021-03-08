using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class ReverseLdContractFlowModel
    {
        public class ReverseLdTransaction
        {
            public string TransactionBranch { get; set; }
            public string LdId { get; set; }
        }

        public class ReverseLdContractFlowRequestModel 
        {
            public ReverseLdTransaction ReverseLdTransaction { get; set; }
        }
    }
}
