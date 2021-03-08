using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
   public  class ReopenTillModel
    {
       public ReopenTill reopenTill { get; set; }
    }

    public class ReopenTill
    {
        public string TransactionBranch { get; set; }
        public string TellerId { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public string IsHeadTeller { get; set; }

        public string access_token { get; set; }

        public string User { get; set; }
    }
}
