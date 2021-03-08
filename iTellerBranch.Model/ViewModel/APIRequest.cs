using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
   public class APIRequest
    {
        public Teller Teller { get; set; }
        public TillTransferLCYModel TiilTransferLCY { get; set; }
        public TellerReversal TellerReversal { get; set; }
        public string access_token { get; set; }
    }

    public class TellerReversal
    {
        public string TransactionBranch { get; set; }
        public string TReference { get; set; }
        public string access_token { get; set; }
        public int TranId { get; set; }
    }

    public class Teller
    {
        public string TransactionBranch { get; set; }
        public string TellerId { get; set; }
        public string Amt { get; set; }
        public string CustAcct { get; set; }
        public string Narration { get; set; }
        public string TransferParty { get; set; }
        public string InitiatorName { get; set; }
        public string TxnCurr { get; set; }
        public string Rate { get; set; }
        public string WaiveCharge { get; set; }
        public string ChargeCode { get; set; }
        public string access_token { get; set; }
        public string ChequeNo { get; set; }
    }

  
}
