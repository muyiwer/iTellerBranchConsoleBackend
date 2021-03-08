using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class AccountClosureModel
    {
        public class CloseAccount
        {
            public string Account_Branch_Code { get; set; }
            public string Account_Number { get; set; }
            public string Account_Type { get; set; }
            public string Effective_Date { get; set; }
        }
        public class CloseAccountRequest 
        {
            public string Account_Branch_Code { get; set; }
            public string Account_Number { get; set; }
            public string Account_Type { get; set; }
            public string Effective_Date { get; set; }
            public string access_token { get; set; }
        }

        public class AccountClosure
        {
            public CloseAccount Close_Account { get; set; }
        }

        public class UpdateResponses
        {
            public string ResponseCode { get; set; }
            public string ResponseDescription { get; set; }
        }

        public class AccountClosureResponse 
        {
            public UpdateResponses UpdateResponses { get; set; }
        }
    }
}
