using iTellerBranch.Model.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iTellerBranch.Model.ViewModel.BankerAcceptanceFlowModel;
using static iTellerBranch.Model.ViewModel.DepositClosureDetailsModel;
using static iTellerBranch.Model.ViewModel.DiscountedDepositFlowModel;
using static iTellerBranch.Model.ViewModel.LdInvestmentDetailsModel;
using static iTellerBranch.Model.ViewModel.PartialWithdrawalFlowModel;
using static iTellerBranch.Model.ViewModel.ReverseLdContractFlowModel;
using static iTellerBranch.Model.ViewModel.RollOverModel;
using static iTellerBranch.Model.ViewModel.TerminateDiscountedDepositModel;
using static iTellerBranch.Model.ViewModel.TreasuryRequestModel;

namespace iTellerBranch.BankService
{
    public class TreasuryCbaService
    {
        public static TreasuryResponse CallDeposit(string url, CallDepositModel request, string token) 
        {
            try
            {
                var requestObj = JsonConvert.SerializeObject(request);
                var obj = JsonConvert.DeserializeObject<object>(requestObj);
                string response = APIService.POSTDeal(url, requestObj, token);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<TreasuryResponse>(response); 

                    if (result.response != null)
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

        public static TreasuryResponse CollaterizedDeposit(string url, CollaterizedDepositModel request, string token) 
        {
            try
            {
                var requestObj = JsonConvert.SerializeObject(request);
                Utils.Log("requestObj: " + requestObj);
               // var obj = JsonConvert.DeserializeObject<object>(requestObj);
               // Utils.Log("obj: "+ obj);
                string response = APIService.POSTDeal(url, requestObj, token);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<TreasuryResponse>(response);

                    if (result.response != null)
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

        public static TreasuryResponse BookAADepositRollover(string url, BookAADepositRolloverModel request, string token)  
        {
            try
            {
                var requestObj = JsonConvert.SerializeObject(request);
                var obj = JsonConvert.DeserializeObject<object>(requestObj);
                string response = APIService.POSTDeal(url, requestObj, token);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<TreasuryResponse>(response);

                    if (result.response != null)
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

        public static DealOutPutResponse DepositClosureDetails(string url, DepositClosure request, string token)
        {
            try
            {
                string response = APIService.POST(url, request, token);
                if (!string.IsNullOrEmpty(response)) 
                {
                    var result = JsonConvert.DeserializeObject<DealResponse>(response);

                    if (result != null)
                    {
                        Utils.Log("POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result.Response;
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

        public static List<EnquiryResponse> GetLdInvestmentDetails(string url, string access_token)
        {
            try
            {
                Utils.Log("Get Account Request access_token: " + access_token);
                string response = APIService.GET(url, access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    var jsonResult = JsonConvert.DeserializeObject<LdInvestmentDetails>(response);
                    var result = jsonResult.EnquiryResponse;
                    if (result != null)
                    {
                        Utils.Log("Get GetLdInvestmentDetails WAS SUCCESSFUL WITH DETAILS: " + response);
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

        public static EnquiryCBAResponseDetails GetLdInvestmentDetailsById(string url, string access_token) 
        {
            try
            {
                Utils.Log("Get Account Request access_token: " + access_token);
                string response = APIService.GET(url, access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    var jsonResult = JsonConvert.DeserializeObject<EnquiryCBAResponseModel>(response); 
                    var result = jsonResult.EnquiryResponse;
                    if (result != null)
                    {
                        Utils.Log("Get GetLdInvestmentDetailsById WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result.Record;
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


        public static DealOutPutResponse CreateDiscountedDepositFlow(string url, DiscountedDepositFlow request, string token)
        {
            try
            {
                string response = APIService.POST(url, request, token);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<DealResponse>(response);

                    if (result.Response != null)
                    {
                        Utils.Log("POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result.Response;
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

        public static DealOutPutResponse CreateBankerAcceptanceFlow(string url, BankerAcceptanceRequestModel request, string token)
        {
            try
            {
                string response = APIService.POST(url, request, token);
                if (!string.IsNullOrEmpty(response))
                { 
                    var result = JsonConvert.DeserializeObject<DealResponse>(response);

                    if (result.Response != null)
                    {
                        Utils.Log("POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result.Response;
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

        public static DealOutPutResponse ReverseLdContractFlow(string url, ReverseLdContractFlowRequestModel request, string token)
        {
            try
            {
                string response = APIService.POST(url, request, token);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<DealResponse>(response); 

                    if (result.Response != null)
                    {
                        Utils.Log("POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result.Response;
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

        public static DealOutPutResponse PartialWithdrawalFlow(string url, PartialWithdrawalFlowRequestModel request, string token)
        {
            try
            {
                string response = APIService.POST(url, request, token);
                if (!string.IsNullOrEmpty(response))
                {  
                    var result = JsonConvert.DeserializeObject<DealResponse>(response);

                    if (result.Response != null)
                    {
                        Utils.Log("POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result.Response;
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



        public static DealOutPutResponse TerminateDiscountedDepositFlow(string url, TerminateDiscountedDeposit request, string token)
        {
            try
            {
                var requestObj = JsonConvert.SerializeObject(request);
                string response = APIService.POSTDeal(url, requestObj, token);  
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<DealResponse>(response);

                    if (result.Response != null)
                    {
                        Utils.Log("POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result.Response;
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
    }
}
