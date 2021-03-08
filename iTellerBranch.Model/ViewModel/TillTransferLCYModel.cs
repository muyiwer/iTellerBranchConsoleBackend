using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class TillTransferLCYModel
    {
        public string TransactionBranch { get; set; }
        public string fromteller { get; set; }
        public string toteller { get; set; }
        public string txncurr { get; set; }
        public string amttotransfer { get; set; }
        public string Narrative { get; set; }
        public string access_token { get; set; }
    }
}
