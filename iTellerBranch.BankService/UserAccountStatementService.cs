using iTellerBranch.Model;
using iTellerBranch.Model.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iTellerBranch.Model.ViewModel.StatementViewModel;

namespace iTellerBranch.BankService
{
    public class UserAccountStatementService
    {

        public static AccountOfficerModel RMDetails(string url, string access_token)
        {
            try
            {

                string response = APIService.GET(url, access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<AccountOfficerModel>(response); 

                    if (result != null)
                    {
                        Utils.Log("ACCOUNT STATEMENT QUERY  WAS SUCCESSFUL WITH DETAILS: " + response);
                        return result;
                        //  return ResponseModel<UserAccountStatement>.Success(result, "Success");
                    }
                    else
                    {
                        Utils.Log("ACCOUNT STATEMENT QUERY FAILURE WITH DETAILS: " + response);
                        return null;

                    }
                }
                else
                {
                    Utils.Log("Empty Response from Client's API");
                    return null;
                }

            }
            catch (System.Exception ex)
            {
                //todo: log ex
                Utils.Log("AccountEnquiry Statement Error: " + ex.Message);
                return null;
            }
        }


        public static List<AccountStatementDetails> AccountEnquiry(string url, string access_token)
        {
            try
            {
               
                string response = APIService.GET(url, access_token);
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<UserAccountStatement>(response);

                    if (result != null)
                    {
                        Utils.Log("ACCOUNT STATEMENT QUERY  WAS SUCCESSFUL WITH DETAILS: " + response);
                        return GetStatement(result);
                      //  return ResponseModel<UserAccountStatement>.Success(result, "Success");
                    }
                    else
                    {
                        Utils.Log("ACCOUNT STATEMENT QUERY FAILURE WITH DETAILS: " + response);
                        return null;

                    }
                }
                else
                {
                    Utils.Log("Empty Response from Client's API");
                    return null;
                }

            }
            catch (System.Exception ex)
            {
                //todo: log ex
                Utils.Log("AccountEnquiry Statement Error: " + ex.Message);
                return null;
            }
        }

        public static List<AccountStatementDetails> GetAccountStatementDemo()
        {
            UserAccountStatement statement = new UserAccountStatement();
            statement.AccountStatement = new IndividualAccountStatementModel();
            statement.AccountStatement.Statement = new List<Statement>();
            statement.AccountStatement.Statement.Add(new Statement
            {
                Closing_Balance = "202,585.92",
                Debit = "10,000,000.00",
                Credit = "",
                Booking_Date = "23 OCT 19",
                Reference = "FT19296NGJ5D\\SBN",
                Description = "iTeller/147813/23627/OLADELE.TO/CLE ARING.S",
                Value_Date = "23 OCT 19",
                DebitCreditIndicator = "1"

            });
            return GetStatement(statement);
            //return ResponseModel<UserAccountStatement>.Success(new UserAccountStatement
            //{
            //    AccountStatement = new IndividualAccountStatementModel
            //    {
            //        Statement = new List<Statement>
            //        {
            //            new Statement
            //            {
            //                CUSTOM_HEADER = "CUSTOM",
            //                Closing_Balance = "204,105.92",
            //                Booking_Date = "23 OCT 19",
            //                Reference = "FT19296LJY1K\\SBN",
            //                Description = "Transfer In- ADEYEMI AKINTAYO ADEDEJI",
            //                Value_Date = "23 OCT 19",
            //                DebitCreditIndicator = "2",
            //                Debit = " ",
            //                Credit = "100.00"
            //            },
            //            new Statement
            //            {
            //                Booking_Date = "23 OCT 19",
            //                Reference = "FT19296LJY1K\\SBN",
            //                Description = "Transfer In- ADEYEMI AKINTAYO ADEDEJI",
            //                Value_Date = "23 OCT 19",
            //                DebitCreditIndicator = "2",
            //                Debit = " ",
            //                Credit = "100.00",
            //                CUSTOM_HEADER = "CUSTOM",
            //                Closing_Balance = "204,105.92",
            //            }
            //        },
            //        StatementHeader = new StatementHeader
            //        {
            //            Account = "0008522044",
            //            Customer_Id = "247408",
            //            Customer_Name = "OLADELE TOSIN OGUNNIRAN",
            //            Currency = "NGN",
            //            Opening_Balance = "204,005.92",
            //            Closing_Balance = "1,275,950.38"
            //        },
            //        Status = "TRUE"
            //    }

            //}, "Success");
        }

        public static List<AccountStatementDetails> GetStatement(UserAccountStatement statements)
        {
            List<AccountStatementDetails> accountStatementDetails = new List<AccountStatementDetails>();
            if(statements.AccountStatement != null)
            {
                foreach (var statement in statements.AccountStatement.Statement)
                {
                    accountStatementDetails.Add(new AccountStatementDetails
                    {
                        Approved = true,
                        Balance = statement.Closing_Balance == "" ? 0.00 : Convert.ToDouble(statement.Closing_Balance.Replace(",", "")),
                        Credit = statement.Credit == "" ? 0.00 : Convert.ToDouble(statement.Credit.Replace(",", "")),
                        Debit = statement.Debit == "" ? 0.00 : Convert.ToDouble(statement.Debit.Replace(",", "")),
                        Date = statement.Booking_Date,
                        ValDate = statement.Value_Date,
                        Narration = statement.Description,
                        ExpenseLine = ""
                    });
                }
            }

            return accountStatementDetails;
        }

    }
}
