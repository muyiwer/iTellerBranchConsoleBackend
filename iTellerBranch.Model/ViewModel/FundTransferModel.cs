using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class FundTransferModel
    {
       public FTRequest FT_Request { get; set; }
    }

    public class FTRequest
    {
        public string TransactionBranch { get; set; }
        public string TransactionType { get; set; }
        public string DebitAcctNo { get; set; }
        public string DebitCurrency { get; set; }
        public string CreditCurrency { get; set; }
        public string DebitAmount { get; set; }
        public string CreditAccountNo { get; set; }
        public string CommissionCode { get; set; }
        public string VtellerAppID { get; set; }
        public string narrations { get; set; }
        public string SessionId { get; set; }
        public string TrxnLocation { get; set; }
    }

    public class FTResponse
    {
        public string ReferenceID { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseText { get; set; }
        public string Balance { get; set; }
        public string COMMAMT { get; set; }
        public string CHARGEAMT { get; set; }
        public string FTID { get; set; }
    }

    public class FundTransferResponse 
    {
        public FTResponse FTResponse { get; set; }
    }
}
