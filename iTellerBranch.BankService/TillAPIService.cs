using iTellerBranch.Model.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.BankService
{
    public static class TillAPIService
    {
        public static OutputResponse TILLTRANSFERLOCAL(string url, TillTransferRequest request) 
        {
            try
            {
                string response = APIService.POST(url, request, request.TiiTransferLCY.access_token); 
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<Response>(response);

                    if (result.OutputResponse != null)
                    {
                        Utils.Log("POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result.OutputResponse;
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

        public static OutputResponse TILLTRANSFERForeign(string url, TillTransferRequestForeign request)
        {
            try
            {
                Utils.Log("FOREIGN CURRENCY POSTING: AMT- " + request.TiiTransferFCY.amttotransfer+", FroTeller"+ request.TiiTransferFCY.fromteller+", CURR - "+ request.TiiTransferFCY+", Branch - "+ request.TiiTransferFCY.TransactionBranch+", toTeller - "+ request.TiiTransferFCY.toteller);

                string response = APIService.POST(url, request, request.TiiTransferFCY.access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<Response>(response);

                    if (result.OutputResponse != null)
                    {
                        Utils.Log("POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result.OutputResponse;
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

        public static OutputResponse CloseTill(string url, TillClosureModel request) 
        {
            try
            {
                string response = APIService.POST(url, request, request.TillClosure.access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<Response>(response);

                    if (result.OutputResponse != null)
                    {
                        Utils.Log("CLOSE TILL: POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result.OutputResponse;
                    }
                    else
                    {

                        Utils.Log("CLOSE TILL: POST FAILURE WITH DETAILS: " + result);
                        return null;

                    }
                }
                else
                {
                    Utils.Log("CLOSE TILL: Empty Response from Client's API");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Utils.Log("CLOSE TILL error message: " + ex.Message);
                throw ex;
            }
        }

        public static OutputResponse ReOpenTill(string url, ReopenTillModel request)
        {
            try
            {
                string response = APIService.POST(url, request, request.reopenTill.access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<Response>(response);

                    if (result.OutputResponse != null)
                    {
                        Utils.Log("OPEN TILL: POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result.OutputResponse;
                    }
                    else
                    {

                        Utils.Log("OPEN TILL: POST FAILURE WITH DETAILS: " + result);
                        return null;

                    }
                }
                else
                {
                    Utils.Log("OPEN TILL: Empty Response from Client's API");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Utils.Log("OPEN TILL error message: " + ex.Message);
                throw ex;
            }
        }
    }
}
