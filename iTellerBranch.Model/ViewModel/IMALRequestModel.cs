using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class IMALRequestModel
    {
        public class AccountDetailsRequestModel
        {
            public string account { get; set; }
            public object accountNumber { get; set; }
            public string requestCode { get; set; }
            public string principalIdentifier { get; set; }
            public string referenceCode { get; set; }
            public string fromAccount { get; set; }
            public string toAccount { get; set; }
            public string beneficiaryName { get; set; }
            public string paymentReference { get; set; }
            public string access_token { get; set; }
        }

        public class LocalFT
        {
            public string fromAccount { get; set; }
            public string toAccount { get; set; }
            public string amount { get; set; }
            public string requestCode { get; set; }
            public string principalIdentifier { get; set; }
            public string referenceCode { get; set; }
            public string beneficiaryName { get; set; }
            public string paymentReference { get; set; }
        }
    }
}
