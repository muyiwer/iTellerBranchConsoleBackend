using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class TellerReversalRequest
    {
        public TellerReversalModel TellerReversal { get; set; }
    }

    public class TellerReversalModel
    {
        public string TransactionBranch { get; set; }
        public string TTReference { get; set; }
        public string access_token { get; set; }
    }
}
