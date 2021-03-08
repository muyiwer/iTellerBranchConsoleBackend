using iTellerBranch.BankService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static iTellerBranch.Model.ViewModel.StatementViewModel;

namespace iTellerBranch.Controllers
{
    public class UserAccountStatementController : ApiController
    {
        private static string isDemo = System.Configuration.ConfigurationManager.AppSettings["isDemo"];
        public UserAccountStatementController()
        {

        }

        //[HttpGet, Route("api/AccountStatement/Statement/{accountNumber}/{dateFrom}/{dateTo}")]
        //public IHttpActionResult GetUserAccountStatement([FromUri]UserStatementModel statementModel, string access_token)
        //{
        //    if (isDemo == "false")
        //    {
        //        var result = UserAccountStatementService.AccountEnquiry(statementModel, access_token);
        //        return Ok(result);
        //    }
        //    else
        //    {
        //        return Ok(UserAccountStatementService.GetAccountStatementDemo());
        //    }
        //}

        [HttpGet, Route("api/AccountStatement/Statement/{accountNumber}/{dateFrom}/{dateTo}/{access_token}")] 
        public IHttpActionResult GetUserAccountStatement(string accountNumber, string dateFrom, string dateTo, string access_token)
        {
            if (isDemo == "false")
            {
                string url = ConfigurationManager.AppSettings["AccountStatement"] + "/" 
                                        + accountNumber + "/" + dateFrom + "/" + dateTo;
                var result = UserAccountStatementService.AccountEnquiry(url, access_token); 
                return Ok(result);
            }
            else
            {
                return Ok(UserAccountStatementService.GetAccountStatementDemo());
            }
        }
    }
}
