using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class TreasuryRequestModel
    {
        public class CallDepositDetailsModel  
        {
            [JsonProperty("comp.code")]
            public string CompCode { get; set; }
            public string custid { get; set; }
            public string currency { get; set; }
            public string effectivedate { get; set; }
            public string amt { get; set; }
            public string rate { get; set; }
            public string payinacct { get; set; }
            public string payoutacct { get; set; }
            public string dao { get; set; }
            public string AcctName { get; set; }
        }

        public class CallDepositModel 
        {
            public CallDepositDetailsModel Request { get; set; }
        }

        public class TreasuryDetailsResponse  
        {
            public string RespondCode { get; set; }
            public string ResponseId { get; set; }
            public string ResponseText { get; set; }
            public string ArrangementId { get; set; }
        }

        public class TreasuryResponse
        {
            public TreasuryDetailsResponse response { get; set; }
        }

        public class CollaterizedDepositDetail 
        {
            [JsonProperty("Comp.code")]
            public string CompCode { get; set; }
            public string CustId { get; set; }
            public string Currency { get; set; }
            public string ProductId { get; set; }
            public string Amt { get; set; }
            public string Rate { get; set; }
            [JsonProperty("Effective.Date")]
            public string EffectiveDate { get; set; }
            public string PayinAcct { get; set; }
            public string PayoutAcct { get; set; }
            public string Term { get; set; }
            public string dao { get; set; }
            public string AcctName { get; set; }
            public string ChangePeriod { get; set; } 
        }

        public class CollaterizedDepositModel 
        {
            public CollaterizedDepositDetail Request { get; set; }
        }



    }
}
