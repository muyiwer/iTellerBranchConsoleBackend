using iTellerBranch.Model.ViewModel;
using iTellerBranch.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static iTellerBranch.Model.ViewModel.ChequeStatus;
using static iTellerBranch.Model.ViewModel.CustomerDetailsModel;
using static iTellerBranch.Model.ViewModel.VaultDetailsModel;

namespace iTellerBranch.BankService
{
    public static class Transaction
    {
        public static object GetAccountFullInfo(string url, string access_token)
        {
            try
            {
                Utils.Log("Get Account Request access_token: " + access_token);
                string response =   APIService.GET(url, access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    var jsonResult = JsonConvert.DeserializeObject<Response>(response);
                    var result = jsonResult.BankAccountFullInfo;
                    if (result != null)
                    {
                        Utils.Log("Get Account WAS SUCCESSFUL WITH DETAILS: " + response);
                        customerMaster customerDetail = new customerMaster();
                        customerDetail.AccountNo = result.ACCT_NO;
                       // customerDetail.AccountNo = result.AccountDescp;
                      //  customerDetail.AccountNo = result.AccountGroup;
                        customerDetail.AccountName = result.AccountTitle;
                        customerDetail.AccountType = result.ACCT_TYPE;
                       // customerDetail.AccountNo = result.ADD_LINE1;
                       // customerDetail.AccountNo = result.ADD_LINE2;
                       // customerDetail.AccountNo = result.Alt_Currency;
                        customerDetail.BranchCode = result.BRA_CODE;
                       // customerDetail.AccountNo = result.BVN;
                       // customerDetail.AccountNo = result.CourtesyTitle;
                      //  customerDetail.AccountNo = result.CRNT_BAL;
                        customerDetail.CurrencyCode = result.Currency_Code;
                        customerDetail.Currency = result.Currency_Code;
                        customerDetail.AccountStatus = result.STA_CODE;
                       // customerDetail.AccountNo = result.CustomerStatusCode;
                       // customerDetail.AccountNo = result.CustomerStatusDeecp;
                       // customerDetail.AccountNo = result.CUS_NUM;
                      //  customerDetail.AccountNo = result.CUS_SHO_NAME;
                       // customerDetail.AccountNo = result.DATE_BAL_CHA;
                        customerDetail.OpenDate = DateTime.ParseExact(result.DATE_OPEN, "yyyyMMdd", null);
                       // customerDetail.AccountNo = result.DES_ENG;
                        customerDetail.Email = result.email;
                       // customerDetail.AccountNo = result.INTRODUCER;
                       // customerDetail.AccountNo = result.ISO_ACCT_TYPE;
                       // customerDetail.AccountNo = result.IsSMSSubscriber;
                      //  customerDetail.AccountNo = result.LED_CODE;
                      //  customerDetail.AccountNo = result.LimitAmt;
                      //  customerDetail.AccountNo = result.LimitID;
                      //  customerDetail.AccountNo = result.MAP_ACC_NO;
                       // customerDetail.AccountNo = result.MinimumBal;
                        customerDetail.PhoneNo = result.MOB_NUM;
                       // customerDetail.AccountNo = result.NAME_LINE1;
                      //  customerDetail.AccountNo = result.NAME_LINE2;
                      //  customerDetail.AccountNo = result.NUBAN;
                     //   customerDetail.AccountNo = result.OnlineActualBalance;
                      //  customerDetail.AccountNo = result.OnlineClearedBalance;
                      //  customerDetail.AccountNo = result.OpenActualBalance;
                     //   customerDetail.AccountNo = result.OpenClearedBalance;
                      //  customerDetail.AccountNo = result.RESTRICTION;
                     //   customerDetail.AccountNo = result.REST_FLAG;
                       // customerDetail.AccountNo = result.STA_CODE;
                      //  customerDetail.BranchCode = result.T24_BRA_CODE;
                      //  customerDetail.AccountNo = result.T24_CUR_CODE;
                      //  customerDetail.AccountNo = result.T24_CUS_NUM;
                      //  customerDetail.AccountNo = result.T24_LED_CODE;
                        customerDetail.PhoneNo = result.TEL_NUM;
                        customerDetail.customerId = result.CUS_NUM;
                        //   customerDetail.AccountNo = result.TOT_BLO_FUND;
                        customerDetail.Abbrev = result.CUR_CODE;
                        customerDetail.Bookbalance = Convert.ToDecimal(result.CRNT_BAL);
                        customerDetail.Availablebalance = Convert.ToDecimal(result.UsableBal); //please remove comma seprated

                        return customerDetail;
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


        public static object GetCustomerInfo(string url, string access_token)
        {
            try
            {
                Utils.Log("Get Account Request access_token: " + access_token); 
                string response = APIService.GET(url, access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    // var jsonResult = JsonConvert.DeserializeObject<List<CustomerInfo>>(response);
                    object jsonResult = JsonConvert.DeserializeObject<object>(response);
                    if (jsonResult is JArray)
                    {
                        var customerInfo = JsonConvert.DeserializeObject<List<CustomerInfo>>(response);
                       return CustomerInfo(customerInfo.FirstOrDefault());
                    }
                    else
                    {
                        var customerInfo = JsonConvert.DeserializeObject<CustomerServiceModel>(response);
                       return CustomerInfo(customerInfo.Record);
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

        public static object CustomerInfo(CustomerInfo result)
        {
            if (result != null)
            {
                // Utils.Log("Get Account WAS SUCCESSFUL WITH DETAILS: " +JsonConvert.SerializeObject(result));
                customerMaster customerDetail = new customerMaster();
                customerDetail.AccountNo = "";
                customerDetail.AccountName = result.SHORTNAME;
                customerDetail.Accountofficer = result.ACCOUNTOFFICER;
                customerDetail.BranchCode = result.COMPANYBOOK;
                customerDetail.AccountStatus = result.CUSTOMERTYPE;

                return customerDetail;
            }
            else
            {
                Utils.Log("POST FAILURE WITH DETAILS: " + result);
                return null;
            }
        }


        public static string GetAccountCurrency(string url, string access_token)
        {
            try
            {
                Utils.Log("Get Account Request access_token: " + access_token);
                string response = APIService.GET(url, access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    var jsonResult = JsonConvert.DeserializeObject<Response>(response);
                    var result = jsonResult.BankAccountFullInfo;
                    if (result != null)
                    {
                        Utils.Log("Get Account WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result.CUR_CODE;
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

        public static OutputResponse CashDepLcy(string url, TellerRequest request)
        {
            try
            {
                string response = APIService.POST(url, request, request.Teller.access_token);
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

        public static OutputResponse CashDepositForeign(string url, TellerRequest request) 
        {
            try
            {
                string response = APIService.POST(url, request, request.Teller.access_token);
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

        public static List<CustomerImageRecord> GetCustImage(string url, string access_token) 
        {
            try
            {
                string response = APIService.GET(url,access_token); 
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<CustImage>(response);

                    if (result.GetCustImage != null)
                    {
                       

                            Utils.Log("POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result.GetCustImage.Record; 
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

        public static OutputResponse CashWithDrawal(string url, TellerRequest request)
        {
            try
            {
                string response = APIService.POST(url, request, request.Teller.access_token);
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

        public static OutputResponse FundTransferNarrExt(string url, FundTransferModel request, string access_token)
        {
            try
            {
                Utils.Log("calling FundTransferNarrExt api request dettails-> CreditAccountNo:" + 
                    request.FT_Request.CreditAccountNo + "DebitAcctNo" + request.FT_Request.DebitAcctNo);
                string response = APIService.POST(url, request, access_token);
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

        public static object LCYCashWithdrawalsWithCheq(string url, APIRequest request)
        {
            try
            {
                string response = APIService.POST(url, request, request.access_token);
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

        public static object CashWithdrawalCounterCheque(string url, APIRequest request) 
        {
            try
            {
                string response = APIService.POST(url, request, request.access_token);
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


        public static OutputResponse TellerReversal(string url, TellerReversalRequest request)
        {
            try
            {
                string response = APIService.POST(url, request, request.TellerReversal.access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<TellerReversalResponse>(response);

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

        public static UserRecord GetUserDetails(string url, string access_token) 
        {
            try
            {
                string response = APIService.GET(url, access_token); 
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<UserResponseModel>(response);

                    if (result.Response.Record != null)
                    {
                        Utils.Log("POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result.Response.Record;
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


        public static TillDetailsViewModel GetTellerDetails(string url, string access_token)
        {
            try
            {
                string response = APIService.GET(url, access_token); 
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<TillDetailsViewModel>(response);

                    if (result != null)
                    {
                        Utils.Log("POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result;
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

        public static VaultDetailRoot GetVaultDetails(string url, string access_token)  
        {
            try
            {
                string response = APIService.GET(url, access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<VaultDetailRoot>(response);

                    if (result != null)
                    {
                        Utils.Log("POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result;
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


        public static FTChqResponse PostInHouseCheque(string url, InHouseChequesViewModel request, string accessToken)
        {
            try
            {
                string response = APIService.POST(url, request, accessToken);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<FTOutputResponse>(response);

                    if (result.FTResponse != null)
                    {
                        Utils.Log("INHOUSE CHEQUES: POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
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
                Utils.Log("Post Inhouse Cheques error message: " + ex.Message);
                throw ex;
            }
        }

        public static CurrencyRateModel GetRate(string url, string access_token)
        {
            try
            {
                string response = APIService.GET(url, access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<CurrencyRateModel>(response);

                    if (result != null)
                    {
                        Utils.Log("POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result;
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
                Utils.Log("GetRate error message: " + ex.Message);
                throw ex;
            }
        }

        public static ChequeStatusModel GetChequeStatus(string url, string access_token)
        {
            string response = APIService.GET(url, access_token);
            try
            {
               
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<ChequeStatusModel>(response);

                    if (result != null)
                    {
                        Utils.Log("POSTING FOR CHEQUE STATUS WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result;
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
            catch 
            {
                Utils.Log("No records were found that matched the cheque credentials on request");

                try
                {
                    var result = JsonConvert.DeserializeObject<List<ChequeRecord>>(response);
                    ChequeStatusModel chequeStatus = new ChequeStatusModel();
                    chequeStatus.Record = new ChequeRecord();
                    chequeStatus.Record = result.FirstOrDefault();
                    Utils.Log("Cheque POSTing DETAILS: " + JsonConvert.SerializeObject(result));
                    return chequeStatus;
                }
                catch (Exception ex)
                {
                    Utils.Log("Error while calling CBA Get Cheque Status: " + ex.Message);
                    throw;
                }
            }
        }


        public static bool DisplayFileFromServer(Uri serverUri)
        {
            // The serverUri parameter should start with the ftp:// scheme.
            if (serverUri.Scheme != Uri.UriSchemeFtp)
            {
                return false;
            }
            // Get the object used to communicate with the server.
            WebClient request = new WebClient();

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com");
            try
            {
                byte[] newFileData = request.DownloadData(serverUri.ToString());
                string fileString = System.Text.Encoding.UTF8.GetString(newFileData);
                Console.WriteLine(fileString);
            }
            catch (WebException e)
            {
                Console.WriteLine(e.ToString());
            }
            return true;
        }
        public static string[] GetImagesFromDirectory(string url)
        {

            string[] fileLocations = Directory.GetFiles(url).Select(path => Path.GetFileName(path))
                                     .ToArray();
            return fileLocations;
        }

        public static byte[] ReadImagesFromDirectory(string path)
        {
            byte[] ImageByte = File.ReadAllBytes(path);
            return ImageByte;
        }

        public static byte[] ReadImageFromPath(string url, string access_token)
        {
            try
            {
                string response = APIService.GET(url, access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<object>(response);

                    if (result != null)
                    {
                        Utils.Log("POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
                        return (byte[])result;
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
                Utils.Log("GetRate error message: " + ex.Message);
                throw ex;
            }
        }
        public static FTReversalResponse FundTransferReversal(string url, FundTransferReversalModel request, string access_token)
        {
            try
            {
                string response = APIService.POST(url, request, access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<FTReversalResponse>(response);

                    if (result.FTResponseExt != null)
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
    }
}
