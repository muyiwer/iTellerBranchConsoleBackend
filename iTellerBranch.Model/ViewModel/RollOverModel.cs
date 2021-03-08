using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class RollOverModel
    {
        public class BookADepositDetails 
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
        }

        public class BookADepositModel 
        {
            public BookADepositDetails Request { get; set; } 
        }

        public class BookAADepositRolloverDetails 
        {
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
            public string ChangePeriod { get; set; }
            public string dao { get; set; }
            public string AcctName { get; set; }
        }

        public class BookAADepositRolloverModel  
        {
            public BookAADepositRolloverDetails Request { get; set; } 
        }
    }
}
