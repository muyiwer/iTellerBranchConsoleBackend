using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class VaultDetailsModel
    {
        public class VaultDetails
        {
            [JsonProperty("TELLER.ID")]
            public string TELLERID { get; set; }
            [JsonProperty("TELLER.BRANCH")]
            public string TELLERBRANCH { get; set; }
            [JsonProperty("TELLER.NAME")]
            public string TELLERNAME { get; set; }
            [JsonProperty("BRANCH.NAME")]
            public string BRANCHNAME { get; set; }
        }

        public class GeTilid
        {
            public VaultDetails Record { get; set; }
        }

        public class VaultDetailRoot  
        {
            public GeTilid GeTilid { get; set; }
        }
    }
}
