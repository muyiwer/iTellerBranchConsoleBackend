using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class TillDetailsViewModel
    {
        public GeTilid GeTilid { get; set; }
    }

    public class RecordModel
    {
        [JsonProperty(PropertyName = "TELLER.ID")]
        public string TELLER_ID { get; set; }
        [JsonProperty(PropertyName = "TELLER.BRANCH")]
        public string TELLER_BRANCH { get; set; }
        [JsonProperty(PropertyName = "TELLER.NAME")]
        public string TELLER_NAME { get; set; }
        [JsonProperty(PropertyName = "BRANCH.NAME")]
        public string BRANCH_NAME { get; set; }
    }
    public class GeTilid
    {
        public RecordModel Record { get; set; }
    }

}
