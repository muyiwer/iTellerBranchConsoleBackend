using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class ImalFundTransferReversal
    {
        public string requestCode { get; set; }
        public string principalIdentifier { get; set; }
        public string oldReferenceCode { get; set; }
        public string transactionFTref { get; set; }
        public string branchCode { get; set; }
    }

    public class ImalFundTransferReversalResponse
    {
        public string responseCode { get; set; }
        public string errorCode { get; set; }
        public string skipProcessing { get; set; }
        public string skipLog { get; set; }
    }
}
