using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class TokenRequest
    {
        public string client_id { get; set; }
         public  string client_secret { get; set; }
    }

    public class OtpValidation
    {
        public string otp { get; set; }
        public string username { get; set; }
        public string hashkey { get; set; }
    }

    public class TokenBody
    {
        public OtpValidation OtpValidation { get; set; }
    }

    public class OTPtoken
    {
        public TokenBody Body { get; set; } 
    }
}
