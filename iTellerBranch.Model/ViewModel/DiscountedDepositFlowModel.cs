using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class DiscountedDepositFlowModel
    {
        public class DiscountedDepositFlowRequest
        {
            public string CompCode { get; set; }
            public string CustId { get; set; }
            public string Currency { get; set; }
            public string Category { get; set; }
            public string BusinessDayDef { get; set; }
            public string ValueDate { get; set; }
            public string FinMatDate { get; set; }
            public string InterestBasis { get; set; }
            public string InterestRate { get; set; }
            public string InterestRateKey { get; set; }
            public string Amount { get; set; }
            public string DiscountValue { get; set; }
            public string DrawnDownAcct { get; set; }
            public string PrinLiqAcct { get; set; }
            public string IntLiqAcct { get; set; }
            public string ChargLiqAcct { get; set; }
            public string LDAcctOfficer { get; set; }
            public string MatureAtStartOfDay { get; set; }
        }

        public class DiscountedDepositFlow
        {
            public DiscountedDepositFlowRequest Request { get; set; }
        }
    }
}
