using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class ManagerIssuanceModel
    {
        public class McIssuanceRequestDetails 
        {
            public string TransactionBranch { get; set; }
            public string DraftAmount { get; set; }
            public string DebitAccount { get; set; }
            public string StockNumber { get; set; }
            public string DebitValueDate { get; set; }
            public string CreditValueDate { get; set; }
            public string ChequeType { get; set; }
            public string PayeeName { get; set; }
            public string VtellerAppID { get; set; }
            public string narations { get; set; }
            public string SessionId { get; set; }
            public string TrxnLocation { get; set; }
            public string ChargeAmt { get; set; }
        }

        public class McIssuanceRequest
        {
            public McIssuanceRequestDetails FTRequest { get; set; } 
        }

        public class MCRepurchaseRequestDetails
        {
            public string TransactionBranch { get; set; }
            public string DraftAmount { get; set; }
            public string DebitAccount { get; set; }
            public string CreditAccount { get; set; }
            public string originalTransactionReference { get; set; }
            public string creditCurrency { get; set; }
            public string StockNumber { get; set; }
            public string DebitValueDate { get; set; }
            public string PayeeName { get; set; }
            public string VtellerAppID { get; set; }
            public string narrations { get; set; }
            public string SessionId { get; set; }
            public string TrxnLocation { get; set; }
        }

        public class MCRepurchaseRequest
        {
            public MCRepurchaseRequestDetails FTRequest { get; set; }
        }

        public class OutwardChequeRequestDetails 
        {
            public string TransactionBranch { get; set; }
            public string bankcode { get; set; }
            public string beneficiaryAccount { get; set; }
            public string beneficiaryName { get; set; }
            public string CreditValueDate { get; set; }
            public string creditAmount { get; set; }
            public string ChequeNumber { get; set; }
            public string debitAccount { get; set; }
            public string SessionId { get; set; }
            public string TrxnLocation { get; set; }
        }

        public class OutwardChequeRequest
        {
            public OutwardChequeRequestDetails FT_Request { get; set; }
        }

    }
}
