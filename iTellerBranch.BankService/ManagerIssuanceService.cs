using iTellerBranch.Model.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iTellerBranch.Model.ViewModel.ManagerIssuanceModel;
using static iTellerBranch.Model.ViewModel.ManagerIssuanceResponseModel;

namespace iTellerBranch.BankService
{
    public class ManagerIssuanceService
    {
        public static ManagerIssuanceResponseDetails FundsTransferMcIssuanceStockNumber(string url, McIssuanceRequest request, string access_token)
        {
            try
            {
                string response = APIService.POST(url, request, access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<ManagerIssuanceResponse>(response);

                    if (result.FTResponse != null)
                    {
                        Utils.Log("POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result.FTResponse;
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
                Utils.Log("ManagerIssuanceResponseDetails error message: " + ex.Message);
                throw ex;
            }
        }

        public static ManagerIssuanceResponseDetails FundsTransferMcRepayment(string url, MCRepurchaseRequest request, string access_token)
        {
            try
            {
                string response = APIService.POST(url, request, access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<ManagerIssuanceResponse>(response);

                    if (result.FTResponse != null)
                    {
                        Utils.Log("POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result.FTResponse;
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
                Utils.Log("ManagerIssuanceResponseDetails error message: " + ex.Message);
                throw ex;
            }
        }

        public static MCRepurchaseResponseDetails McRepurchase(string url, string access_token) 
        {
            try
            {
                Utils.Log("Get Account Request access_token: " + access_token);
                string response = APIService.GET(url, access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    var jsonResult = JsonConvert.DeserializeObject<MCRepurchaseResponse>(response);
                    if (jsonResult != null)
                    {
                        Utils.Log("Get Account WAS SUCCESSFUL WITH DETAILS: " + response);
                       //please remove comma seprated

                        return jsonResult.McRepurchase.Record;
                    }
                    else
                    {

                        Utils.Log("POST FAILURE WITH DETAILS: " + jsonResult);
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
                Utils.Log("McRepurchase error message: " + ex.Message);
                throw ex;
            }

        }

       public static MCRepurchaseResponseDetails McRepurchaseDetails()
        {
            MCRepurchaseResponseDetails repurchase = new MCRepurchaseResponseDetails
            {
                Transaction_Reference = "MC.0053441488.785742",
                Draft_No= "785742",
                Draft_Ccy = "NGN",
                Draft_Amt= "20,000.00",
                Payee_Name= "WEST MIDLANDS COMMUNICATION LTD",
                Issue_Date= "06 APR 2018",
                Origin= "FUNDS.TRANSFER",
                Origin_Reference= "FT180966LYWM",
                Status= "ISSUED"

            };
            return repurchase;
        }

        public static OutwardChequeResponseDetails OutwardChequePosting(string url, OutwardChequeRequest request, string access_token)
        {
            try
            {
                string response = APIService.POST(url, request, access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<OutwardChequeResponse>(response);

                    if (result != null)
                    {
                        Utils.Log("POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result.FTResponse;
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
                Utils.Log("OutwardChequeResponseDetails error message: " + ex.Message);
                throw ex;
            }
        }


    }
}
