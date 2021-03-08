using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class FundTransferReversalModel
    {
        public FTRequestFundReversal FT_Request { get; set; }
    }

    public class FTRequestFundReversal
    {
        public string TransactionBranch { get; set; }
        public string FTReference { get; set; }
    }
    public class FTResponseExt
    {
        public string ReferenceID { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseText { get; set; }
        public string Balance { get; set; }
        public string COMMAMT { get; set; }
        public string CHARGEAMT { get; set; }
        public string FTID { get; set; }
    }

    public class FTReversalResponse
    {
        public FTResponseExt FTResponseExt { get; set; }
    }
}