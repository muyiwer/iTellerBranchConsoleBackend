using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace iTellerBranch.BankService
{
    public class CurrencyRateModel
    {
        public GetRates GetRates { get; set; }
    }

    public class Record
    {
        [JsonProperty(PropertyName = "CCY.CODE")]
        public string CCY_CODE { get; set; }
        [JsonProperty(PropertyName = "NUM.CCY.CODE")]
        public string NUM_CCY_CODE { get; set; }
        [JsonProperty(PropertyName = "CCY.NAME")]
        public string CCY_NAME { get; set; }
        [JsonProperty(PropertyName = "CCY.MARKET")]
        public string CCY_MARKET { get; set; }
        [JsonProperty(PropertyName = "CCY.MID.RATE")]
        public string CCY_MID_RATE { get; set; }
        [JsonProperty(PropertyName = "CCY.BUY.RATE")]
        public string CCY_BUY_RATE { get; set; }
        [JsonProperty(PropertyName = "CCY.SELL.RATE")]
        public string CCY_SELL_RATE { get; set; }
    }
    public class GetRates
    {
        public Record Record { get; set; }
    }
    
}
