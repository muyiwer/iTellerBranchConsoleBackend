using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
        public class InHouseChequesViewModel
        {
            public FT_Request FT_Request { get; set; }
        }

        public class FT_Request
        {
        public string TransactionBranch { get; set; }
        public string TransactionType { get; set; }
        public string DebitAcctNo { get; set; }
        public string DebitCurrency { get; set; }
        public string CreditCurrency { get; set; }
        public string DebitAmount { get; set; }
        public string CreditAccountNo { get; set; }
        public string VtellerAppID { get; set; }
        public string narrations { get; set; }
        public string ChequeNumber { get; set; }
        public string SessionId { get; set; }
        public string TrxnLocation { get; set; }
    }
    
}
