using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{

    public class ActiveDirectoryModel
    {
       public AD_Credentials AD_Credentials { get; set; }
    }

    public class ADModel
    {
        public string userId { get; set; }

        public string password { get; set; }

        public string branchCode { get; set; }
        public string branchName { get; set; }
        public string sUserOriginalId { get; set; }
        public string sfullname { get; set; }
        public string sUserEmail { get; set; }
        public string Msg { get; set; }
    }

    public class AD_Credentials
    {
        public string AD_Username { get; set; }

        public string AD_Password { get; set; }
    }

    public class ActiveDirectoryResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
    }

    public class ADResponse
    {
        public string Status { get; set; }
        public ActiveDirectoryResponse Response { get; set; }
    }

    public class ACtiveDirectoryModel
    {
        public ADResponse AD_Response { get; set; }
    }
}

