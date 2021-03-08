using iTellerBranch.BankService;
using iTellerBranch.Business.Setup;
using iTellerBranch.Model;
using iTellerBranch.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static iTellerBranch.Model.ViewModel.AccountClosureModel;

namespace iTellerBranch.Controllers
{
    public class EnquiriesController : ApiController
    {
        private readonly UserBusiness _userBusiness;
        private static string isDemo = System.Configuration.ConfigurationManager.AppSettings["isDemo"];
        public EnquiriesController()
        {
            _userBusiness = new UserBusiness();
        }

        [HttpPost, Route("api/GetHVTStatement/HVTStatement")]
        public IHttpActionResult GetHVTStatement([FromBody]HVTStatementModel statementModel)
        {
            if (isDemo == "false")
            {
                string url = ConfigurationManager.AppSettings["GetHVTStatement"];
                var result = EnquiriesService.GetHVTStatement(url,statementModel, statementModel.access_token);
                return Ok(result);
            }
            else
            {
                return Ok(EnquiriesService.GetHVTStatementDemo());
            }
        }

        [HttpPost, Route("api/Account/Close")]
        public IHttpActionResult CloseAccount([FromBody]CloseAccountRequest CloseAccount)
        {
            try
            {
                if (isDemo == "false")
                {
                    string url = ConfigurationManager.AppSettings["CloseAccount"];
                    var result = EnquiriesService.CloseAccount(CloseAccount, url);
                        if (result == null)
                        {
                            return Ok(new
                            {
                                success = false,
                                message = "Server Error please contact the admin"
                            });
                        }
                        else if (result.ResponseCode == "1")
                        {
                            return Ok(new
                            {
                                success = true,
                                message = "Account Closed successfully"
                            });
                        }
                        else
                        {
                            return Ok(new
                            {
                                success = false,
                                message = result.ResponseDescription
                            });
                        }
                }
                else
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Account Closed successfully"
                    });
                }
            }
            catch (Exception ex)
            {
                Utils.LogNO("Close Account: " +  ex.Message);
                return Ok(new
                {
                    success = false,
                    message = ex.Message
                });
            }
           
        }

        [HttpGet, Route("api/GetCustomerDetails/{CustId}/Token/{access_token}")]
        public IHttpActionResult GetCustomerDetailsById([FromUri] string CustId, string access_token) // can u achieve it- yes ook. When I m done I willc all ub 
        {
            try
            {
                object response = null;
                if (isDemo == "true")
                {
                    response = _userBusiness.GetCustomerById(CustId);
                    return Json(response);
                }
                else
                {
                    string url = System.Configuration.ConfigurationManager.AppSettings["GetCustomerInfo"] + "/" + CustId;
                    response = EnquiriesService.GetCustomerDetail(url, access_token);
                    return Json(response);
                }

            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }


        [HttpGet, Route("api/GetCustomerDetailsByAccount/{acctNumber}/Token/{access_token}")]
        public IHttpActionResult GetCustomerDetailsByAccountNumber([FromUri] string acctNumber, string access_token) // can u achieve it- yes ook. When I m done I willc all ub 
        {
            try
            {
                object response = null; 
                if (isDemo == "true")
                {
                    response = _userBusiness.GetCustomerByAccount(acctNumber); 
                    return Json(response);
                }
                else
                {
                    string url = ConfigurationManager.AppSettings["NameEnquiry"] + "/" + acctNumber;
                    response = EnquiriesService.GetAccountFullInfo(url, access_token);
                    return Json(response);
                }

            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
    }
}
