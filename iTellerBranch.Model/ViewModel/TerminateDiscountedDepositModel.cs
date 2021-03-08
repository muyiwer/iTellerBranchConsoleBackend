using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class TerminateDiscountedDepositModel
    {
        public class TerminateLd
        {
            [JsonProperty("Comp.Code")]
            public string CompCode { get; set; }
            public string DepositId { get; set; }
            public string Date { get; set; }
        }

        public class TerminateDiscountedDeposit 
        {
            public TerminateLd TerminateLd { get; set; }
        }
    }
}
