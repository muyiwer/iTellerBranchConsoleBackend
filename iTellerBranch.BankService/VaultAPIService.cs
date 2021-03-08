using iTellerBranch.Model.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.BankService
{
    public static class VaultAPIService
    {
        public static OutputResponse VaultOutLocal(string url, TillTransferRequest request) 
        {
            try
            {
                string response = APIService.POST(url, request, request.TiiTransferLCY.access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<Response>(response);

                    if (result.OutputResponse.ResponseCode == "1")
                    {
                        Utils.Log("POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result.OutputResponse;
                    }
                    else
                    {
                        Utils.Log("POST FAILURE WITH DETAILS: " + response);
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

        public static OutputResponse VaultOutFCY(string url, TillTransferRequestForeign request, string access_token) 
        {
            try
            {
                string response = APIService.POST(url, request, access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<Response>(response);

                    if (result.OutputResponse.ResponseCode == "1")
                    {
                        Utils.Log("POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result.OutputResponse;
                    }
                    else
                    {
                        Utils.Log("POST FAILURE WITH DETAILS: " + response);
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

    }
}
