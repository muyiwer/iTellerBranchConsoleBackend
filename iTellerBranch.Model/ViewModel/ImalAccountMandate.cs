using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class ImalAccountMandate
    {
        public class ImalAccountMandateClientRequest
        {
            public string Type { get; set; }
            public string Number { get; set; }
            public string access_token { get; set; }
        }

        public class ImalAccountMandateCBARequest 
        {
            public string Type { get; set; }
            public string Number { get; set; }
        }

        public class ImalAccountMandateCBAResponse 
        {
            public string StatusCode { get; set; }
            public string StatusDesc { get; set; }
            public string currencyDescription { get; set; }
            public string signatureDescription { get; set; }
            public string signatureFile { get; set; }
            public string signatureInstruction { get; set; }
            public string status { get; set; }
            public string unlimited { get; set; }
        }


    }
}
