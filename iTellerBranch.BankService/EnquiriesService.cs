using iTellerBranch.Model;
using iTellerBranch.Model.ViewModel;
using iTellerBranch.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using static iTellerBranch.Model.ViewModel.AccountClosureModel;
using static iTellerBranch.Model.ViewModel.BalanceCategoryViewModel;
using static iTellerBranch.Model.ViewModel.CBACustomerDetailsModel;
using static iTellerBranch.Model.ViewModel.CustomerDetailsModel;

namespace iTellerBranch.BankService
{
    public class EnquiriesService
    {

        public static ResponseModel<object> GetHVTStatement(string baseHVTStatementUrl, HVTStatementModel statementModel, string access_token)
        {
            try
            {
                //var baseHVTStatementUrl = ""; //ConfigurationManager.AppSettings["GetHVTStatement"];
                var url = baseHVTStatementUrl + "/" + statementModel.BranchCode  + "/" + statementModel.AccountNo
                    + "/" + statementModel.DateFrom + "/" + statementModel.DateTo;
                
                string response = APIService.GET(url, access_token);
                Utils.Log("HVTstatement: " + JsonConvert.SerializeObject(response));
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<object>(response);
                    if (result != null)
                    {
                        Utils.Log("GET HVT STATEMENT WAS SUCCESSFUL WITH DETAILS: " + response);
                        return ResponseModel<object>.Success(result, "Success");
                    }
                    else
                    {
                        Utils.Log("GET HVT STATEMENT FAILURE WITH DETAILS: " + response);
                        return ResponseModel<object>.Error("Error");

                    }
                }
                else
                {
                    Utils.Log("Empty Response from Client's API");
                    return ResponseModel<object>.Error("Error");
                }

            }
            catch (System.Exception ex)
            {
                //todo: log ex
                return ResponseModel<object>.Error("Error");
            }
        }

        public static UpdateResponses CloseAccount(CloseAccountRequest CloseAccount, string url)
        {
           
            try
            {
                AccountClosure accountClosure = new AccountClosure();
                accountClosure.Close_Account = new CloseAccount();
                accountClosure.Close_Account.Account_Branch_Code = CloseAccount.Account_Branch_Code;
                accountClosure.Close_Account.Account_Number = CloseAccount.Account_Number;
                accountClosure.Close_Account.Account_Type = "ACCOUNTS";
                accountClosure.Close_Account.Effective_Date = CloseAccount.Effective_Date;
                Utils.Log("access_token" + CloseAccount.access_token);
                string response = APIService.POST(url, accountClosure, CloseAccount.access_token);
                Utils.Log("CloseAccountResponse: " + JsonConvert.SerializeObject(response));
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<AccountClosureResponse>(response);
                    if (result != null)
                    {
                        Utils.Log("GET HVT STATEMENT WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result.UpdateResponses;
                    }
                    else
                    {
                        Utils.Log("GET HVT STATEMENT FAILURE WITH DETAILS: " + response);
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
                Utils.Log("Close Account error: " + ex.Message);
                return null;
            }
        }

        public static ResponseModel<BalanceByCategoryModel> GetBalanceByCategoryDemo()
        {
            return ResponseModel<BalanceByCategoryModel>.Success(new BalanceByCategoryModel
            {
                EquiryResponse = new EnquiryResponseModel
                {
                   Record = new List<BalanceRecordModel>
                   {
                       new BalanceRecordModel
                       {
                           Account_Name = "EUR1001190062006",
                           Currency = "EUR",
                           Customer_No = null,
                           Account_Number = "Vault Cash",
                           Previous_Day_Bal = "-246,099.50",
                           Today_Bal = "-246,099.50",
                           Variance = "0.00",
                           Opening_Date = "26 APR 2016",
                           Date_Last_Updated = "09 OCT 2019",
                           Company_Code = "NG0020006",
                           Company_Name = "20, MARINA LAGOS"
                       },
                       new BalanceRecordModel
                       {
                           Account_Name = "EUR1001190062006",
                           Currency = "EUR",
                           Customer_No = null,
                           Account_Number = "Vault Cash",
                           Previous_Day_Bal = "-246,099.50",
                           Today_Bal = "-246,099.50",
                           Variance = "0.00",
                           Opening_Date = "26 APR 2016",
                           Date_Last_Updated = "09 OCT 2019",
                           Company_Code = "NG0020006",
                           Company_Name = "20, MARINA LAGOS"
                       },
                       new BalanceRecordModel
                       {
                            Account_Number = "GBP1001190062006",
                            Currency = "GBP",
                            Customer_No = null,
                            Account_Name = "VAULT CASH",
                            Previous_Day_Bal = "-27,036.00",
                            Today_Bal = "-27,036.00",
                            Variance = "0.00",
                            Opening_Date = "26 APR 2016",
                            Date_Last_Updated = "09 OCT 2019",
                            Company_Code = "NG0020006",
                            Company_Name = "20, MARINA LAGOS"
                       }
                   }
                },
               
            }, "Success");
        }
        public static ResponseModel<BalanceByCategoryModel> GetBalanceByCategory(string url,string branchCode, string categoryCode, string access_token)
        {
            try
            {
                // ConfigurationManager.AppSettings["BalanceByCategory"];
                string response = APIService.GET(url, access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<BalanceByCategoryModel>(response);

                    if (result != null)
                    {
                        Utils.Log("GET BALANCE BY CATEGORY CALL WAS SUCCESSFUL WITH DETAILS: " + response);
                        return ResponseModel<BalanceByCategoryModel>.Success(result, "Success");
                    }
                    else
                    {
                        Utils.Log("GET BALANCE BY CATEGORY CALL FAILURE WITH DETAILS: " + response);
                        return ResponseModel<BalanceByCategoryModel>.Error("Error");
                    }
                }
                else
                {
                    Utils.Log("Empty Response from Client's API");
                    return ResponseModel<BalanceByCategoryModel>.Error("Error");
                }

            }
            catch (System.Exception ex)
            {
                //todo: log ex
                return ResponseModel<BalanceByCategoryModel>.Error("Error");
            }
        }

        public static ResponseModel<AccountRecordModel> GetHVTStatementDemo()
        {
            return ResponseModel<AccountRecordModel>.Success(new AccountRecordModel
            {
                Account_Statement = new AccountStatementModel
                {
                    Statement = new StatementModel
                    {
                        Booking_Date = "20191023",
                        Reference = "FT192962K1SB\\SBN",
                        Description = "ADEYEMI AKINTAYO ADEDEJI ",
                        Value_Date = "20191023",
                        Debit = "0.0",
                        Credit = "20.00",
                        Closing_Balance = "14,326,346.69",
                        CUSTOM_HEADER = null
                    },
                    Statement_Header = new StatementHeaderModel
                    {
                        Account = "0037974939",
                        Customer_Id = "2120882",
                        Customer_Name = "NPF MICROFINANCE BANK PLC  NPF MICROFINANCE BANK PLC.",
                        Currency = "NGN",
                        Opening_Balance = "14326326.69",
                        Closing_Balance = "14326346.69"
                    }
                }
            }, "Success");
        }

        public static object GetCustomerDetail(string url, string access_token)
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
                        return GetCustomerDetail(customerInfo.FirstOrDefault());
                    }
                    else
                    {
                        var customerInfo = JsonConvert.DeserializeObject<CustomerServiceModel>(response);
                        return GetCustomerDetail(customerInfo.Record);
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

        public static object GetAccountFullInfo(string url, string access_token)
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
                       return GetCustomerDetailByAccount(result);
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

        public static object GetCustomerDetail(CustomerInfo result)
        {
            if (result != null)
            {
                // Utils.Log("Get Account WAS SUCCESSFUL WITH DETAILS: " +JsonConvert.SerializeObject(result));
                //customerMaster customerDetail = new customerMaster();
                //customerDetail.AccountNo = "";
                //customerDetail.AccountName = result.SHORTNAME;
                //customerDetail.Accountofficer = result.ACCOUNTOFFICER;
                //customerDetail.BranchCode = result.COMPANYBOOK;
                //customerDetail.AccountStatus = result.CUSTOMERTYPE;
                AccountDetailModel accountDetailModel = new AccountDetailModel();
                accountDetailModel.CustomerId = result._ID;
                accountDetailModel.OwnerName = result.SHORTNAME;
                
                accountDetailModel.Accounts = new List<AccountModel>();
                accountDetailModel.Accounts.Add(new AccountModel
                {
                    Bvn = result.BVN,
                    CustomerStatus = result.CUSTOMERSTATUS,
                    Branch = result.ACCTOFFICERBR,
                    Accountofficer = result.ACCOUNTOFFICER,
                });

                return accountDetailModel;
            }
            else
            {
                Utils.Log("POST FAILURE WITH DETAILS: " + result);
                return null;
            }
        }


        public static object GetCustomerDetailByAccount(BankAccountFullInfoViewModel result)
        {
            if (result != null)
            {
                // Utils.Log("Get Account WAS SUCCESSFUL WITH DETAILS: " +JsonConvert.SerializeObject(result));
                //customerMaster customerDetail = new customerMaster();
                //customerDetail.AccountNo = "";
                //customerDetail.AccountName = result.SHORTNAME;
                //customerDetail.Accountofficer = result.ACCOUNTOFFICER;
                //customerDetail.BranchCode = result.COMPANYBOOK;
                //customerDetail.AccountStatus = result.CUSTOMERTYPE;
                AccountDetailModel accountDetailModel = new AccountDetailModel();
                accountDetailModel.CustomerId = result.CUS_NUM;
                accountDetailModel.OwnerName = result.CUS_SHO_NAME;

                accountDetailModel.Accounts = new List<AccountModel>();
                accountDetailModel.Accounts.Add(new AccountModel
                {
                    Bvn = result.BVN,
                    CustomerStatus = result.CustomerStatusCode,
                    Branch = result.BRA_CODE,
                    Accountofficer = result.INTRODUCER,
                });

                return accountDetailModel;
            }
            else
            {
                Utils.Log("POST FAILURE WITH DETAILS: " + result);
                return null;
            }
        }



        public static object GetCustomerDetailInfo(CustomerInfo result)
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


    }
}
