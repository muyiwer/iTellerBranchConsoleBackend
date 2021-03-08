using iTellerBranch.Model.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.BankService
{
    public class ActiveDirectoryService
    {
        public static ACtiveDirectoryModel ValidateUser(string url, string request)
        {
            try
            {
                string response = APIService.POSTAD(url, request);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<ACtiveDirectoryModel>(response);

                    if (result != null)
                    {
                        Utils.Log("ADSERVICE CALL WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result;
                    }
                    else
                    {
                        Utils.Log("ADSERVICE CALL FAILURE WITH DETAILS: " + result);
                        return null;
                    }
                }
                else
                {
                    Utils.Log("Empty Response from Client's API");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Utils.Log("VALIDATING USER ON AD error message: " + ex.Message);
                throw ex;
            }
        }

        public static UserADdetailsModel GetDetailsByUsername(string url, string access_token)  
        {
            try
            {
                string response = APIService.GET(url, access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<UserADdetailsModel>(response);

                    if (result != null)
                    {
                        Utils.Log("POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result;
                    }
                    else
                    {
                        Utils.Log("POST FAILURE WITH DETAILS: " + result);
                        return null;
                    }
                }
                else
                {
                    Utils.Log("Empty Response from Client's API");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Utils.Log("GetAccountFullInfo error message: " + ex.Message);
                throw ex;
            }
        }


        public static string EncryptCredentials(string url, object request)
        {
            try
            {
                
                string response = APIService.POSTAD(url, request);// fire
                if (!string.IsNullOrEmpty(response))
                {
                    return response;
                }
                else
                {
                    Utils.Log("Empty Response from Client's API");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Utils.Log("GetAccountFullInfo error message: " + ex.Message);
                throw ex;
            }
        }

    }
}
