using iTellerBranch.BankService;
using iTellerBranch.Business.Setup;
using iTellerBranch.Model;
using iTellerBranch.Model.ViewModel;
using iTellerBranch.Repository;
using iTellerBranch.Repository.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace iTellerBranch.Controllers
{
    public class TillController : ApiController
    {
        private readonly TillService _tillService;
        private readonly TillBusiness _tillBusiness;
        private static string isDemo = System.Configuration.ConfigurationManager.AppSettings["isDemo"];
        public TillController()
        {
            _tillService = new TillService();
            _tillBusiness = new TillBusiness();        }


        [HttpGet, Route("api/Till")]
        public IHttpActionResult GetTill()
        {
            var result = _tillService.GetTill(true, "");
            return Ok(result);
        }


        [HttpGet, Route("api/Till/GetCheckedTillBalance/{tellerid}")]
        public IHttpActionResult GetCheckedTillBalance([FromUri] string tellerId)
        {
            var result = _tillService.GetCheckedTillBalance(tellerId);
            return Ok(result);
        }
        

        [HttpGet, Route("api/Get/Approve/Till")]
        public IHttpActionResult GetTillApproval()
        {
            var result = _tillService.GetTillApproval(true, "");
            return Ok(result);
        }

        [HttpGet, Route("api/Giver/Till")]
        public IHttpActionResult GetGiverTill()
        {
            var result = _tillService.GetGiverTill();
            return Ok(result);
        }
        //[HttpGet, Route("api/Assign/Till")]
        //public IHttpActionResult GetAssignedTill()
        //{
        //    var result = _tillService.GetAssignedTill(true, "");
        //    return Ok(result);
        //}
        [HttpPost, Route("api/Approve/Till")]
        public IHttpActionResult ApproveTill([FromBody]TillManagement tillManagement)
        {
            try
            {
                var result = _tillService.ApproveTill(tillManagement);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _tillService.GetTillApproval(false, ex.Message, ex);
                return Ok(result);
            }
        }

        [HttpPost, Route("api/Disapprove/Till")]
        public IHttpActionResult DisapproveTill([FromBody]TillManagement tillManagement)
        {
            try
            {
                var result = _tillService.DisapproveTill(tillManagement);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _tillService.GetTillApproval(false, ex.Message, ex);
                return Ok(result);
            }
        }

        [HttpPost, Route("api/Open/Till")]
        public IHttpActionResult OpenTill([FromBody]TillManagement tillManagement)
        {
            try
            {
                var result = _tillService.OpenTill(tillManagement);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _tillService.GetTillApproval(false, ex.Message, ex);
                return Ok(result);
            }
        }

        [HttpPost, Route("api/Close/Till")]
        public IHttpActionResult CloseTill([FromBody]TillAssignmentModel tillManagement)
        {
            try
            {
                if (isDemo == "false")
                {
                    object result = null;
                    string url = System.Configuration.ConfigurationManager.AppSettings["CloseTill"];
                    TillClosureModel tillClosureRequest = new TillClosureModel();
                    tillClosureRequest.TillClosure = new TillClosure();
                    tillClosureRequest.TillClosure.access_token = tillManagement.access_token;
                    tillClosureRequest.TillClosure.Comment = tillManagement.Comments;
                    tillClosureRequest.TillClosure.TransactionBranch = tillManagement.TransactionBranch;
                    tillClosureRequest.TillClosure.IsHeadTeller = tillManagement.IsHeadTeller;
                    tillClosureRequest.TillClosure.TellerId = tillManagement.TellerId;
                    tillClosureRequest.TillClosure.Status = tillManagement.Status;
                    Utils.LogNO("Close Till event:  Calling Client Service for posting...");
                    OutputResponse response = TillAPIService.CloseTill(url, tillClosureRequest);
                    if (response.ResponseCode == "-1")
                    {
                        result = new
                        {
                            success = false,
                            message = "Operation not successful",
                        };
                        return Ok(result);
                    }
                    if (response.ResponseCode == "1")
                    {
                        tillManagement.CBAResponse = response.ResponseText;
                        _tillService.CloseTill(tillManagement);
                        result = new
                        {
                            success = true,
                            message = "Till closed successfully",
                            TransactionRef = response.ResponseId
                        };
                        return Ok(result);
                    }
                    else
                    {
                        result = new
                        {
                            success = false,
                            message = "Operation not successful."
                        };
                        return Ok(result);
                    }
                }
                else
                {
                    var result = _tillService.CloseTill(tillManagement);
                    return Ok(result);
                }
                
            }
            catch (Exception ex)
            {
                var result = _tillService.GetTillApproval(false, ex.Message, ex);
                return Ok(result);
            }
        }

        [HttpPost, Route("api/ConfirmBalance/Till")]
        public IHttpActionResult ConfirmTillBalance([FromBody]TillManagement tillManagement)
        {
            try
            { 
                var result = _tillService.ConfirmTillBalance(tillManagement);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _tillService.GetTillApproval(false, ex.Message, ex);
                return Ok(result);
            }
        }
        
        [HttpPost, Route("api/Create/Till")]
        public IHttpActionResult PostTill([FromBody] TillSetup tillSetup)
        {
            try
            {
                var result = _tillService.CreateTill(tillSetup);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _tillService.GetTill(false, ex.Message, ex);
                return Ok(result);
            }
        }

        [HttpPost, Route("api/Update/Till")]
        public IHttpActionResult PutTill([FromBody]TillSetup tillSetup)
        {
            try
            {
                var result = _tillService.UpdateTill(tillSetup);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _tillService.GetTill(false, ex.Message, ex);
                return Ok(result);
            }
        }

        [HttpGet, Route("api/User/Till")]
        public IHttpActionResult GetUserTill()
        {
            var result = _tillService.GetUserTill(true, "");
            return Ok(result);
        }

        [HttpPost, Route("api/Assign/Till")]
        public IHttpActionResult PostAssignTill([FromBody] TillAssignment tillAssign)
        {
            try
            {
                var result = _tillService.AssignTill(tillAssign);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _tillService.GetTill(false, ex.Message, ex);
                return Ok(result);
            }
        }

        [HttpPost, Route("api/Delete/Assign/Till")]
        public IHttpActionResult DeleteAssignTill([FromBody]List<int> ID)
        {
            try
            {
                var result = _tillService.DeleteTillAssignment(ID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _tillService.GetTill(false, ex.Message, ex);
                return Ok(result);
            }
        }

        [HttpPost, Route("api/Delete/Till")]
        public IHttpActionResult DeleteTill([FromBody]List<int> ID)
        {
            try
            {
                var result = _tillService.DeleteTill(ID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _tillService.GetTill(false, ex.Message, ex);
                return Ok(result);
            }
        }


        [HttpPost, Route("api/TellerTransaction/{tellerId}/Currency/{currencyId}")]
        public IHttpActionResult GetTillTransactions(string tellerId, int currencyId)
        {
            try
            {
                var result = _tillBusiness.GetTillTransactions(tellerId, currencyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // var result = _tillBusiness.GetTill(false, ex.Message, ex);
                throw ex;
            }
        }

        [HttpPost, Route("api/ImalTellerTransaction")]
        public IHttpActionResult GetImalTillTransactions(TillBalanceModel tillModel) 
        {
            try
            {
                tillModel.CurrencyCode = System.Configuration.ConfigurationManager.AppSettings["CurrencyCode"];
                tillModel.TellerId = tillModel.BranchCode + tillModel.CurrencyCode +
                System.Configuration.ConfigurationManager.AppSettings["SMLedgerCode"] + tillModel.CIFnumber +
                "000";
                tillModel.FromTeller = tillModel.BranchCode + tillModel.CurrencyCode +
                            System.Configuration.ConfigurationManager.AppSettings["CashSettlementLedgerCode"]
                        + "00000000" + "000";
                tillModel.ToTeller = tillModel.BranchCode + tillModel.CurrencyCode +
                           System.Configuration.ConfigurationManager.AppSettings["VaultLedgerCode"]
                       + "00000000" + "000";
                var result = _tillBusiness.GetIMALTillTransactions(tillModel);  
                return Ok(result);
            }
            catch (Exception ex)
            {
                // var result = _tillBusiness.GetTill(false, ex.Message, ex);
                throw ex;
            }
        }
    }
}
