using iTellerBranch.BankService;
using iTellerBranch.Business.Setup;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static iTellerBranch.Model.ViewModel.PLStatementModel;

namespace iTellerBranch.Controllers
{
    public class BankAccountFullInfoController : ApiController
    {
        private readonly UserBusiness _userBusiness;
        private static string isDemo = System.Configuration.ConfigurationManager.AppSettings["isDemo"];
        public BankAccountFullInfoController()
        {
            _userBusiness = new UserBusiness();

        }


        [HttpPost, Route("api/AccountEnquiry/PLEntries")]
        public IHttpActionResult PlEntriesForLocal([FromBody] PLEntriesRequest PLEntriesRequest)
        {
            Utils.LogNO("api/AccountEnquiry/PLEntries:1:Currently Inside This End Point.");
            if (isDemo == "false") 
            {
                var PlEntriesUrl = ConfigurationManager.AppSettings["PlEntries"]; 
                var url = PlEntriesUrl + "/" + PLEntriesRequest.Branch + '/' + PLEntriesRequest.Category + '/' + PLEntriesRequest.Date;
                var result = BankAccountFullInfoService.PlEntries(url, PLEntriesRequest.access_token);
                return Ok(result);
            }
            else
            {
                return Ok(BankAccountFullInfoService.PlEntriesForLocal()); 
            }
        }

        [HttpGet, Route("api/AccountEnquiry/AccountDetails/{accountNumber}/{access_token}")]
        public IHttpActionResult GetAccountDetails(string accountNumber, string access_token)
        {
            Utils.LogNO("api/AccountEnquiry/AccountDetails/{accountNumber}/{access_token}:2:Currently Inside This End Point.");
            if (isDemo == "false")
            {
                var baseAccountEnquiryUrl = ConfigurationManager.AppSettings["AccountEnquiry"];
                var url = baseAccountEnquiryUrl + "/" + accountNumber;
                var result = BankAccountFullInfoService.AccountEnquiry(url,accountNumber, access_token);
                return Ok(result);
            }
            else
            {
                return Ok(BankAccountFullInfoService.GetAccountEnquiryDemo());
            }
        }

        [HttpGet, Route("api/AccountEnquiry/AccountByCustomerId/{customerId}/Token/{access_token}")]
        public IHttpActionResult GetCustomerAccountFullInfo([FromUri]string customerId, string access_token)
        {
            Utils.LogNO("api/AccountEnquiry/AccountByCustomerId/{customerId}/Token/{access_token}:3:Currently Inside This End Point.");
            if (isDemo == "false")
            {
                string baseAccountEnquiryUrl =ConfigurationManager.AppSettings["AccountByCustomerID"];
                var url = baseAccountEnquiryUrl + "/" + Int64.Parse(customerId);
                var result = BankAccountFullInfoService.EnquiryByCustomerId(url,customerId, access_token);
                return Ok(result);
                //return Ok();
            }
            else
            {
                var response = _userBusiness.GetCustomerAccountByIdDemo(customerId);
                return Ok(response);
            }
        }


        [HttpGet, Route("api/AccountEnquiry/AccountByAccountNumber/{acctNumber}/Token/{access_token}")]
        public IHttpActionResult GetCustomerAccountFullInfoByAccount([FromUri]string acctNumber, string access_token) 
        {
            Utils.LogNO("api/AccountEnquiry/AccountByAccountNumber/{acctNumber}/Token/{access_token}:3:Currently Inside This End Point.");
            if (isDemo == "false")
            {
                string url = ConfigurationManager.AppSettings["NameEnquiry"] + "/" + acctNumber;
                var result = BankAccountFullInfoService.EnquiryByCustomerId(url, acctNumber, access_token);
                return Ok(result);
                //return Ok();
            }
            else
            {
                var response = _userBusiness.GetCustomerAccountByIdDemo(acctNumber);
                return Ok(response);
            }
        }
    }

}
