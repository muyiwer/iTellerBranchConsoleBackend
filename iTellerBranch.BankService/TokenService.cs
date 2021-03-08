using iTellerBranch.Model.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.BankService
{
    public static class TokenService
    {
        public static TokenResponseModel Validate(string url, TokenRequest token)
        {
            try
            {
                Utils.Log("Generate Token URL " + url);
                string response = APIService.POSTtoken(url, token);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<TokenResponseModel>(response);

                    if (result != null)
                    {
                        Utils.Log("GENERATE TOKEN WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result;
                    }
                    else
                    {

                        Utils.Log("Generate Token Error Message: " + result);
                        return null;

                    }
                }
                else
                {
                    Utils.Log("Generate Token Empty Response from Client's API");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Utils.Log("Generate Token error message: " + ex.Message);
                throw ex;
            }
        }

        public static OTPtokenResponse otpvalidation(string url, OTPtoken otp_token, string access_token)
        {
            try 
            {
                string response = APIService.POST(url, otp_token, access_token);  
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<OTPtokenResponse>(response);

                    if (result != null)
                    {
                        Utils.Log("GENERATE TOKEN WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result;
                    }
                    else
                    {

                        Utils.Log("Generate Token Error Message: " + result);
                        return null;

                    }
                }
                else
                {
                    Utils.Log("Generate Token Empty Response from Client's API");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Utils.Log("Generate Token error message: " + ex.Message);
                throw ex;
            }
        }

    }
}
