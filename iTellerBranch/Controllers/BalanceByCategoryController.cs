using iTellerBranch.BankService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static iTellerBranch.Model.ViewModel.BalanceCategoryViewModel;

namespace iTellerBranch.Controllers
{
    public class BalanceByCategoryController : ApiController
    {
        private static string isDemo = System.Configuration.ConfigurationManager.AppSettings["isDemo"];
        public BalanceByCategoryController()
        {
        }

        [HttpGet, Route("api/getBalanceByCategory/{categoryCode}/{branchCode}/{access_token}")]
        public IHttpActionResult GetBalanceByCategory(string branchCode, string categoryCode, string access_token)
        {
            if (isDemo == "false")
            {
                string url = ConfigurationManager.AppSettings["BalanceByCategory"] + "/" + branchCode + "/" + categoryCode;
                var result = EnquiriesService.GetBalanceByCategory(url,branchCode, categoryCode, access_token);
                return Ok(result);
            }
            else
            {
                return Ok(EnquiriesService.GetBalanceByCategoryDemo());
            }
        }
    }
}
