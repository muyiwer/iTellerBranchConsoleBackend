using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class ChequeStatus
    {
        public class ChequeRecord
        {
            [JsonProperty(PropertyName = "ACCOUNT.NUMBER")]
            public string ACCOUNT_NUMBER { get; set; }
            public string CHEQUE_NUMBER { get; set; }
            [JsonProperty(PropertyName = "IS.CHQ.VALID")]
            public string IS_CHQ_VALID { get; set; }
            [JsonProperty(PropertyName = "IS.CHQ.USED")]
            public string IS_CHQ_USED { get; set; }
            [JsonProperty(PropertyName = "IS.CHQ.POSTED")]
            public string IS_CHQ_POSTED { get; set; }
            [JsonProperty(PropertyName = "CHQ.STATUS")]
            public string CHQ_STATUS { get; set; }
            [JsonProperty(PropertyName = "CHQ.CCY")]
            public string CHQ_CCY { get; set; }
            [JsonProperty(PropertyName = "CHQ.AMOUNT")]
            public string CHQ_AMOUNT { get; set; }
            [JsonProperty(PropertyName = "CHQ.ORIGIN")]
            public string CHQ_ORIGIN { get; set; }
            [JsonProperty(PropertyName = "CHQ.ORIGIN.REF")]
            public string CHQ_ORIGIN_REF { get; set; }
            [JsonProperty(PropertyName = "CUSTOMER.NUMBER")]
            public string CUSTOMER_NUMBER { get; set; }
            [JsonProperty(PropertyName = "ALTERNATE.ACCOUNT.ID")]
            public string ALTERNATE_ACCOUNT_ID { get; set; }
            [JsonProperty(PropertyName = "DATE.STOPPED")]
            public object DATE_STOPPED { get; set; }
            [JsonProperty(PropertyName = "DATE.PRESENTED")]
            public string DATE_PRESENTED { get; set; }
        }

        public class ChequeStatusModel
        {
            public ChequeRecord Record { get; set; }
        }

        

    }

}
