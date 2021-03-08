using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class TokenResponseModel
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string expires_in { get; set; } 
    }

    public class OtpValidationResponse
    {
        public string OtpValidationResult { get; set; }
    }

    public class TokenResponseBody  
    {
        public OtpValidationResponse OtpValidationResponse { get; set; }
    }

    public class TokenResponseEnvelope 
    {
        public TokenResponseBody Body { get; set; }
    }

    public class OTPtokenResponse 
    {
        public TokenResponseEnvelope Envelope { get; set; }
    }
}
