using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class BankerAcceptanceFlowModel
    {
        public class BankerAcceptance
        {
            public string CompCode { get; set; }
            public string CustId { get; set; }
            public string Currency { get; set; }
            public string BusinessDayDef { get; set; }
            public string ValueDate { get; set; }
            public string FinMatDate { get; set; }
            public string InterestBasis { get; set; }
            public string LimitRef { get; set; }
            public string InterestRate { get; set; }
            public string InterestRateKey { get; set; }
            public string Amount { get; set; }
            public string IntPaymentMtd { get; set; }
            public string DrawnDownAcct { get; set; }
            public string PrinLiqAcct { get; set; }
            public string IntLiqAcct { get; set; }
            public string ChargLiqAcct { get; set; }
            public string FeePayAcct { get; set; }
            public string LDAcctOfficer { get; set; }
            public string MatureAtStartOfDay { get; set; }
            public string BAAssetID { get; set; }
            public string BAAssetAmt { get; set; }
            public string Capitalisation { get; set; }
            public string LiqMode { get; set; }
            public string PositionType { get; set; }
        }

        public class BankerAcceptanceRequestModel 
        {
            public BankerAcceptance BankerAcceptance { get; set; }
        }
    }
}
