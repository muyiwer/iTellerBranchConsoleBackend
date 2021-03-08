using iTellerBranch.Model.ViewModel;
using iTellerBranch.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iTellerBranch.Model.ViewModel.AccountEnquiryModel;
using static iTellerBranch.Model.ViewModel.AccountFullInfoViewModel;
using static iTellerBranch.Model.ViewModel.PLStatementModel;

namespace iTellerBranch.BankService
{
    public class BankAccountFullInfoService
    {
        public static BankAccountFullInfo AccountEnquiry(string url, string accountNumber, string access_token)
        {
            try
            {
             
                string response = APIService.GET(url, access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    var jSonresult = JsonConvert.DeserializeObject<BankAccountFullInformationViewModel>(response);
                    
                    if (jSonresult != null)
                    {
                        Utils.Log("ACCOUNT ENQUIRY WAS SUCCESSFUL WITH DETAILS: " + response);
                       return jSonresult.BankAccountFullInfo;
                    // return ResponseModel<BankAccountFullInformationViewModel>.Success(jSonresult, "Success");
                    }
                    else
                    {
                        Utils.Log("ACCOUNT ENQUIRY FAILURE WITH DETAILS: " + response);
                        return null;
                       // return ResponseModel<BankAccountFullInformationViewModel>.Error("Error");

                    }
                }
                else
                {
                    Utils.Log("Empty Response from Client's API");
                    return null;
                  //  return ResponseModel<BankAccountFullInformationViewModel>.Error("Error");
                }

            }
            catch (System.Exception ex)
            {
                //todo: log ex
                return null;
               // return ResponseModel<BankAccountFullInformationViewModel>.Error("Error");
            }
        }

        //private static object GetCustomerAccountDetails(List<CustomerDetails> customerDetail)
        //{
        //    if(customerDetail != null)
        //    {
        //        foreach(var record in customerDetail)
        //        {
        //            var entity = 
        //        }
        //    }
        //}

        public static ResponseModel<AccountFullInformationModel> EnquiryByCustomerIdDemo()
        {
            return ResponseModel<AccountFullInformationModel>.Success(new AccountFullInformationModel
            {
                GetAccountFullInfo = new GetAccountFullInfoModel
                {
                    BankAccountFullInfo = new List<CustomerBankAccountFullInfoModel>
                    {
                        new CustomerBankAccountFullInfoModel
                        {
                            NUBAN = "0073420371",
                            BRA_CODE = "NG0020006",
                            DES_ENG = "HEAD OFFICE BRANCH",
                            CUS_NUM = "14308236",
                            CUR_CODE = "NGN",
                            LED_CODE = "3144",
                            CUS_SHO_NAME = "ADE",
                            AccountGroup = "CURRENT",
                            CustomerStatus = "6",
                            ADD_LINE1 = "11 BOLAJI BANWO STREET",
                            ADD_LINE2 = "11 BOLAJI BANWO STREET",
                            MOB_NUM = "08139610521",
                            email = "yemi2tu@yahoo.com",
                            ACCT_NO = "0073420371",
                            MAP_ACC_NO = "0073420371",
                            ACCT_TYPE = "Personal Loans",
                            ISO_ACCT_TYPE = null,
                            TEL_NUM = "08139610521",
                            DATE_OPEN = "20190716",
                            STA_CODE= "CURRENT",
                            CLE_BAL= "-1229978.37",
                            CRNT_BAL =  "-1229978.37",
                            TOT_BLO_FUND = "0.00",
                            INTRODUCER  = "1",
                            DATE_BAL_CHA= "20190930",
                            NAME_LINE1= "ADEYEMI",
                            NAME_LINE2= "ADEDEJI",
                            BVN ="22170674605",
                            REST_FLAG= "FALSE",
                            IsSMSSubscriber= "TRUE",
                            Alt_Currency= "566",
                            Currency_Code= "NGN",
                            T24_BRA_CODE= "NG0020006",
                            T24_CUS_NUM= "14308236",
                            T24_CUR_CODE= "566",
                            T24_LED_CODE= "3144",
                            OnlineActualBalance= "-1229978.37",
                            OnlineClearedBalance= "-1229978.37",
                            OpenActualBalance= "-1229978.37",
                            OpenClearedBalance= "-1229978.37",
                            WorkingBalance= "-1229978.37",
                            CustomerStatusCode= "6",
                            CustomerStatusDeecp= "Individual Staff",
                            LimitID= null,
                            LimitAmt= "1400000.00",
                            MinimumBal= "0",
                            UsableBal= "-1229978.37",
                            AccountDescp= "HRM.PERSONAL.LOANS",
                            CourtesyTitle= null,
                            AccountTitle= "ADE",
                            ManualRiskRating= null,
                            CalculatedRiskRating= null
                        },
                        new CustomerBankAccountFullInfoModel
                        {
                            NUBAN = "0073420371",
                            BRA_CODE = "NG0020006",
                            DES_ENG = "HEAD OFFICE BRANCH",
                            CUS_NUM = "14308236",
                            CUR_CODE = "NGN",
                            LED_CODE = "3144",
                            CUS_SHO_NAME = "ADE",
                            AccountGroup = "CURRENT",
                            CustomerStatus = "6",
                            ADD_LINE1 = "11 BOLAJI BANWO STREET",
                            ADD_LINE2 = "11 BOLAJI BANWO STREET",
                            MOB_NUM = "08139610521",
                            email = "yemi2tu@yahoo.com",
                            ACCT_NO = "0073420371",
                            MAP_ACC_NO = "0073420371",
                            ACCT_TYPE = "Personal Loans",
                            ISO_ACCT_TYPE = null,
                            TEL_NUM = "08139610521",
                            DATE_OPEN = "20190716",
                            STA_CODE= "CURRENT",
                            CLE_BAL= "-1229978.37",
                            CRNT_BAL =  "-1229978.37",
                            TOT_BLO_FUND = "0.00",
                            INTRODUCER  = "1",
                            DATE_BAL_CHA= "20190930",
                            NAME_LINE1= "ADEYEMI",
                            NAME_LINE2= "ADEDEJI",
                            BVN ="22170674605",
                            REST_FLAG= "FALSE",
                            IsSMSSubscriber= "TRUE",
                            Alt_Currency= "566",
                            Currency_Code= "NGN",
                            T24_BRA_CODE= "NG0020006",
                            T24_CUS_NUM= "14308236",
                            T24_CUR_CODE= "566",
                            T24_LED_CODE= "3144",
                            OnlineActualBalance= "-1229978.37",
                            OnlineClearedBalance= "-1229978.37",
                            OpenActualBalance= "-1229978.37",
                            OpenClearedBalance= "-1229978.37",
                            WorkingBalance= "-1229978.37",
                            CustomerStatusCode= "6",
                            CustomerStatusDeecp= "Individual Staff",
                            LimitID= null,
                            LimitAmt= "1400000.00",
                            MinimumBal= "0",
                            UsableBal= "-1229978.37",
                            AccountDescp= "HRM.PERSONAL.LOANS",
                            CourtesyTitle= null,
                            AccountTitle= "ADE",
                            ManualRiskRating= null,
                            CalculatedRiskRating= null
                        },
                        new CustomerBankAccountFullInfoModel{
                            NUBAN = "0073420371",
                            BRA_CODE = "NG0020006",
                            DES_ENG = "HEAD OFFICE BRANCH",
                            CUS_NUM = "14308236",
                            CUR_CODE = "NGN",
                            LED_CODE = "3144",
                            CUS_SHO_NAME = "ADE",
                            AccountGroup = "CURRENT",
                            CustomerStatus = "6",
                            ADD_LINE1 = "11 BOLAJI BANWO STREET",
                            ADD_LINE2 = "11 BOLAJI BANWO STREET",
                            MOB_NUM = "08139610521",
                            email = "yemi2tu@yahoo.com",
                            ACCT_NO = "0073420371",
                            MAP_ACC_NO = "0073420371",
                            ACCT_TYPE = "Personal Loans",
                            ISO_ACCT_TYPE = null,
                            TEL_NUM = "08139610521",
                            DATE_OPEN = "20190716",
                            STA_CODE= "CURRENT",
                            CLE_BAL= "-1229978.37",
                            CRNT_BAL =  "-1229978.37",
                            TOT_BLO_FUND = "0.00",
                            INTRODUCER  = "1",
                            DATE_BAL_CHA= "20190930",
                            NAME_LINE1= "ADEYEMI",
                            NAME_LINE2= "ADEDEJI",
                            BVN ="22170674605",
                            REST_FLAG= "FALSE",
                            IsSMSSubscriber= "TRUE",
                            Alt_Currency= "566",
                            Currency_Code= "NGN",
                            T24_BRA_CODE= "NG0020006",
                            T24_CUS_NUM= "14308236",
                            T24_CUR_CODE= "566",
                            T24_LED_CODE= "3144",
                            OnlineActualBalance= "-1229978.37",
                            OnlineClearedBalance= "-1229978.37",
                            OpenActualBalance= "-1229978.37",
                            OpenClearedBalance= "-1229978.37",
                            WorkingBalance= "-1229978.37",
                            CustomerStatusCode= "6",
                            CustomerStatusDeecp= "Individual Staff",
                            LimitID= null,
                            LimitAmt= "1400000.00",
                            MinimumBal= "0",
                            UsableBal= "-1229978.37",
                            AccountDescp= "HRM.PERSONAL.LOANS",
                            CourtesyTitle= null,
                            AccountTitle= "ADE",
                            ManualRiskRating= null,
                            CalculatedRiskRating= null
                        }
                    }
                }
            }, "Success");
        }
        
        public static object EnquiryByCustomerId(string url, string customerId, string access_token)
        {
            try
            {
               

                var response = APIService.GET(url, access_token);
                if (!string.IsNullOrEmpty(response))
                {
                 
                    var jsonResult = JsonConvert.DeserializeObject<AccountFullInformationModel>(response);
                    Utils.Log("EnquiryDetails: " + JsonConvert.SerializeObject(jsonResult));
                    return GetCustomerAccountDetail(jsonResult);
                    //if (jsonResult is JArray)
                    //{
                    //    var customerInfo = JsonConvert.DeserializeObject<AccountFullInformationModel>(response);
                    //    return GetCustomerAccountDetail(customerInfo);
                    //}

                }
                else
                {
                    Utils.Log("Empty Response from Client's API");
                    return null;
                }
               
            }
            catch (Exception ex)
            {
                //todo: log ex
                Utils.Log("EnquiryByCustomerId Error: " +  ex.Message);
                return null;
            }
        }

        private static object GetCustomerAccountDetail(AccountFullInformationModel customerDetails) 
        {
           // Utils.Log("AccountFullInformationModel: " + customerDetails.GetAccountFullInfo.BankAccountFullInfo);
            var cusDetails = customerDetails.GetAccountFullInfo.BankAccountFullInfo.FirstOrDefault();
            var accounts = customerDetails.GetAccountFullInfo.BankAccountFullInfo;
            //Utils.Log(JsonConvert.SerializeObject("accounts: " + accounts));
            CustomerDetailsViewModel customerDetailsViewModel = new CustomerDetailsViewModel();
            customerDetailsViewModel.Id = cusDetails.CUS_NUM;
            customerDetailsViewModel.CustomerStatus = cusDetails.CustomerStatus;
            customerDetailsViewModel.OwnerName = cusDetails.NAME_LINE1 + " " + cusDetails.NAME_LINE2;
            customerDetailsViewModel.CustomerId = cusDetails.CUS_NUM;
            customerDetailsViewModel.Branch = cusDetails.BRA_CODE;
            customerDetailsViewModel.Address = cusDetails.ADD_LINE1;
            customerDetailsViewModel.PhoneNumber = cusDetails.MOB_NUM;
            customerDetailsViewModel.NUBAN = cusDetails.NUBAN;
            customerDetailsViewModel.Bvn = cusDetails.BVN;
            customerDetailsViewModel.Accounts = new List<Account>();
            foreach(var account in accounts)
            {
                customerDetailsViewModel.Accounts.Add(new Account
                {
                    Abbrev = account.CUR_CODE,
                    AccountBalance = new AccountBalanceViewModel
                    {
                        LedgerBalance =  account.OpenActualBalance == null ? 0 : Convert.ToDecimal(account.OpenActualBalance),
                        ClearedBalance = account.OpenClearedBalance == null ? 0 : Convert.ToDecimal(account.OpenClearedBalance),
                        WorkingBalance = account.WorkingBalance == null ? 0 : Convert.ToDecimal(account.WorkingBalance),
                        UnauthorisedBalance = 0.00
                    },
                    AccountNumber = account.ACCT_NO,
                    AccountType = account.ACCT_TYPE,
                    AccountGroup = account.AccountGroup,
                    Accountofficer = account.INTRODUCER,
                    ArrangementDate = account.DATE_OPEN,
                    ProductName = account.AccountGroup,
                    Branch = account.BRA_CODE,
                    AvailableOverdraft = "",
                    LCY = account.CUR_CODE,
                    AccountStatus = account.STA_CODE,
                    Bvn = account.BVN,
                    AwaitingAprovals = "",
                    LockedFunds = account.TOT_BLO_FUND,
                    ER = "",
                    Statements = null
                });
                
            }
            Utils.Log("customerDetailsViewModel: " +  JsonConvert.SerializeObject(customerDetailsViewModel));
            return customerDetailsViewModel;
        }

        public static BankAccountFullInfo GetAccountEnquiryDemo()
        {

            BankAccountFullInfo BankAccountFullInfo = new BankAccountFullInfo
            {
                NUBAN = "0008522044",
                BRA_CODE = "NG0020006",
                DES_ENG = "HEAD OFFICE BRANCH",
                CUS_NUM = "247408",
                CUR_CODE = "NGN",
                LED_CODE = "1006",
                CUS_SHO_NAME = "OLADELE TOSIN OGUNNIRAN",
                AccountGroup = "CURRENT",
                CustomerStatus = "6",
                ADD_LINE1 = "14 OLUFEMI OSHOBU STREET SANGO OTTA",
                ADD_LINE2 = "14 OLUFEMI OSHOBU STREET SANGO OTTA",
                MOB_NUM = "08054245065",
                email = "tfeba@yahoo.com",
                ACCT_TYPE = "0008522044",
                MAP_ACC_NO = "0008522044",
                ISO_ACCT_TYPE = "CURRENT",
                TEL_NUM = "08054245065",
                DATE_OPEN = "08054245065",
                STA_CODE = "ACTIVE",
                CLE_BAL = "1275950.38",
                CRNT_BAL = "1275950.38",
                TOT_BLO_FUND = "0.00",
                INTRODUCER = null,
                DATE_BAL_CHA = "20191023",
                NAME_LINE1 = "OLADELE",
                NAME_LINE2 = "OGUNNIRAN",
                BVN = "22149271701",
                REST_FLAG = "FALSE",
                //RESTRICTION = new List<RESTRICTION>
                //{
                //    new RESTRICTION
                //    {
                //        RestrictionCode = null,
                //        RestrictionDescription = null
                //    }
                //},

                IsSMSSubscriber = "TRUE",
                Alt_Currency = "566",
                Currency_Code = "NGN",
                T24_BRA_CODE = "NG0020006",
                T24_CUS_NUM = "247408",
                T24_CUR_CODE = "566",
                T24_LED_CODE = "1006",
                OnlineActualBalance = "1275950.38",
                OnlineClearedBalance = "1275950.38",
                OpenActualBalance = "98053.42",
                OpenClearedBalance = "98053.42",
                WorkingBalance = "1275950.38",
                CustomerStatusCode = "6",
                CustomerStatusDeecp = "Individual Staff",
                LimitID = null,
                LimitAmt = "0.00",
                MinimumBal = "0.00",
                UsableBal = "1275950.38",
                AccountDescp = "STAFF.SALARY STAFF.SALARY",
                CourtesyTitle = "MR",
                AccountTitle = "OLADELE TOSIN OGUNNIRAN"
            };
            return BankAccountFullInfo;
            //return ResponseModel<BankAccountFullInformationViewModel>.Success(new BankAccountFullInformationViewModel
            //{
            //    BankAccountFullInfo = new BankAccountFullInfo
            //    {
            //        NUBAN = "0008522044",
            //        BRA_CODE = "NG0020006",
            //        DES_ENG = "HEAD OFFICE BRANCH",
            //        CUS_NUM = "247408",
            //        CUR_CODE = "NGN",
            //        LED_CODE = "1006",
            //        CUS_SHO_NAME = "OLADELE TOSIN OGUNNIRAN",
            //        AccountGroup = "CURRENT",
            //        CustomerStatus = "6",
            //        ADD_LINE1 = "14 OLUFEMI OSHOBU STREET SANGO OTTA",
            //        ADD_LINE2 = "14 OLUFEMI OSHOBU STREET SANGO OTTA",
            //        MOB_NUM = "08054245065",
            //        email = "tfeba@yahoo.com",
            //        ACCT_TYPE = "0008522044",
            //        MAP_ACC_NO = "0008522044",
            //        ISO_ACCT_TYPE = "CURRENT",
            //        TEL_NUM = "08054245065",
            //        DATE_OPEN = "08054245065",
            //        STA_CODE = "ACTIVE",
            //        CLE_BAL = "1275950.38",
            //        CRNT_BAL = "1275950.38",
            //        TOT_BLO_FUND = "0.00",
            //        INTRODUCER = null,
            //        DATE_BAL_CHA = "20191023",
            //        NAME_LINE1 = "OLADELE",
            //        NAME_LINE2 = "OGUNNIRAN",
            //        BVN= "22149271701",
            //        REST_FLAG= "FALSE",
            //        //RESTRICTION = new List<RESTRICTION>
            //        //{
            //        //    new RESTRICTION
            //        //    {
            //        //        RestrictionCode = null,
            //        //        RestrictionDescription = null
            //        //    }
            //        //},

            //        IsSMSSubscriber= "TRUE",
            //        Alt_Currency = "566",
            //        Currency_Code= "NGN",
            //        T24_BRA_CODE = "NG0020006",
            //        T24_CUS_NUM="247408",
            //        T24_CUR_CODE= "566",
            //        T24_LED_CODE =  "1006",
            //        OnlineActualBalance= "1275950.38",
            //        OnlineClearedBalance= "1275950.38",
            //        OpenActualBalance= "98053.42",
            //        OpenClearedBalance= "98053.42",
            //        WorkingBalance= "1275950.38",
            //        CustomerStatusCode= "6",
            //        CustomerStatusDeecp= "Individual Staff",
            //        LimitID= null,
            //        LimitAmt= "0.00",
            //        MinimumBal= "0.00",
            //        UsableBal= "1275950.38",
            //        AccountDescp= "STAFF.SALARY STAFF.SALARY",
            //        CourtesyTitle= "MR",
            //        AccountTitle= "OLADELE TOSIN OGUNNIRAN"
            //    }
            //}, "Success");
        }

        public static PLStatementDetails PlEntries(string url, string access_token)
        {
            try
            {
                var response = APIService.GET(url, access_token);
                if (!string.IsNullOrEmpty(response))
                {

                    var result = JsonConvert.DeserializeObject<PLStatementResponse>(response);
                   
                    if(result != null)
                    {
                        return result.PL_Statement;
                        Utils.Log("PLStatementResponse: " + JsonConvert.SerializeObject(result));
                    }
                    else
                    {
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
                //todo: log ex
                Utils.Log("PlEntries Error: " + ex.Message);
                return null;
            }
        }

        public static PLStatementDetails PlEntriesForLocal()
        {
            try
            {
                PLStatementResponse result = new PLStatementResponse();
                result.PL_Statement = new PLStatementDetails();
                result.PL_Statement.Statement_Header = new StatementHeader()
                {
                    Balance_At_Period_Start = "20,069,587,613.72",
                    Category = "51001",
                    Currency = "NGN",
                    BALANCE_INCLUDING_FORWARDS = "20,069,795,418.26"
                };
                result.PL_Statement.Statement = new List<StatementDetails>();
                result.PL_Statement.Statement.Add(new StatementDetails
                {
                    Description = "Interest Accruals",
                    Reference = "AAACT1929600FGR2YY",
                    Booking_date = "23 OCT 19",
                    Amount = "5,072.70",
                    Balance = "20,069,587,613.72"
                });
                result.PL_Statement.Statement.Add(new StatementDetails
                {
                    Description = "Interest Accruals",
                    Reference = "AAACT192960J1YC931",
                    Booking_date = "23 OCT 19",
                    Amount = "93.39",
                    Balance = "20,069,592,686.42"
                });
                result.PL_Statement.Statement.Add(new StatementDetails
                {
                    Description = "Interest Accruals",
                    Reference = "AAACT1929650KKCRVC",
                    Booking_date = "23 OCT 19",
                    Amount = "3,035.95",
                    Balance = "20,069,592,779.81"
                });
                return result.PL_Statement;
            }
            catch (Exception ex)
            {
                //todo: log ex
                Utils.Log("PlEntries Error: " + ex.Message);
                return null;
            }
        }


    }
}
