using iTellerBranch.BankService;
using iTellerBranch.Business.Setup;
using iTellerBranch.Model;
using iTellerBranch.Model.ViewModel;
using iTellerBranch.Repository;
using iTellerBranch.Repository.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using static iTellerBranch.Model.ViewModel.ManagerIssuanceModel;

namespace iTellerBranch.Controllers
{
    public class IssuanceController : ApiController
    {
        private readonly IssuanceService _issuanceService;

        public IssuanceController()
        {
            _issuanceService = new IssuanceService();
        }

        private static string isDemo = ConfigurationManager.AppSettings["isDemo"];

        [HttpGet, Route("api/GetChequeTemplate")]
        public IHttpActionResult GetChequeTemplate()
        {
            var result = _issuanceService.GetChequeTemplates();
            return Ok(result);
        }

        [HttpPost, Route("api/Cheque/UpdateTransTemplate")]
        public IHttpActionResult UpdateTransTemplate([FromBody] ManagerChequeIssuanceDetailsModel transaction) 
        {
            try
            {
                _issuanceService.UpdateTransactionChequeTemplate(Convert.ToInt32(transaction.TranId), transaction.TemplateCode);
                return Ok(new
                {
                    success = true,
                    message= "Updated successfully"
                });
            }
            catch(Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = ex.Message
                });
                Utils.LogNO("UpdateTransTemplate Exception: " + ex.Message);
            }
           
        }

        [HttpGet,Route("api/MCAccount")]
        public IHttpActionResult  GetMCAccount()
        {
            var result = _issuanceService.GetMCAccounts();
            return Ok(result);
        }

        [HttpGet, Route("api/MCAccount/{accountNumber}")]
        public IHttpActionResult GetMCAccount(string accountNumber)
        {
            var result = _issuanceService.GetMCAccountsByAccountNumber(accountNumber);
            return Ok(result);
        }

        [HttpGet,Route("api/MCCharge")]
        public IHttpActionResult  GetMCCharge()
        {
            var result = _issuanceService.GetMCCharge();
            return Ok(result);
        }

        [HttpGet,Route("api/GetTransactionMaster")]
        public IHttpActionResult GetTransactionMaster()
        {
            var result = _issuanceService.GetTransactionMaster();
            return Ok(result);
        }


        [HttpGet, Route("api/GetApprovedDraft")]
        public IHttpActionResult GetApprovedDraft() 
        {
            var result = _issuanceService.GetApprovedDraft();
            return Ok(result);
        }

        [HttpGet, Route("api/GetDraftForRepurchase")]
        public IHttpActionResult GetDraftForRepurchase()
        {
            var result = _issuanceService.GetDraftForRepurchase(); 
            return Ok(result);
        }

        [HttpGet, Route("api/GetRepurchaseByDraftNumber/{draftNumber}/Token/{access_token}")]
        public IHttpActionResult GetRepurchaseByDraftNumber(string draftNumber, string access_token) 
        {
            if(isDemo == "false")
            {
                string url = ConfigurationManager.AppSettings["McRepurchase"];
                url = url + "/" + draftNumber;
                var result = ManagerIssuanceService.McRepurchase(url, access_token);
                return Ok(result);
            }
            else
            {
                var result = ManagerIssuanceService.McRepurchaseDetails();
                return Ok(result);
            }
           
           
        }

        [HttpGet, Route("api/GetOutwardCheque")]
        public IHttpActionResult GetOutwardCheque()
        {
            var result = _issuanceService.GetOutwardCheque();
            return Ok(result);
        }

        [HttpGet, Route("api/GetManagerIssuance/{tranId}")]
        public IHttpActionResult GetManagerIssuance(int tranId)
        {
            var result = _issuanceService.GetManagerIssuance(tranId); 
            return Ok(result);
        }

        [HttpPost, Route("api/Update/ManagerIssuance/{tranId}")]
        public IHttpActionResult UpdateManagerIssuanceForRepurchase(int tranId) 
        {
            _issuanceService.UpdateManagerIssuanceForRepurchase(tranId);
            return Ok(new
            {
                success = true,
                message = "Approved successfully",
                TransactionRef = tranId
            });
        }

        [HttpPost, Route("api/Approve/RepurchaseManagerIssuance")]
        public IHttpActionResult ApproveManagerIssuanceForRepurchase(ManagerChequeIssuanceModel transMaster) 
        {
            object result = null;
            Utils.LogNO("Request Details: " + JsonConvert.SerializeObject(transMaster));
            try
            {
                var MachineName = DetermineCompName(GetIPAddress());
                var IPAddress = GetIPAddress();
                General.AuditLog("Performed fund re-purchased Manager Issuance with stock number " + transMaster.DraftNumber
                    , transMaster.CreatedBy, MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
            }
            catch { }
            if (isDemo == "false")
            {
                string url = ConfigurationManager.AppSettings["FundsTransferMcRepayment"];
                Utils.LogNO("url: " + url);
                MCRepurchaseRequest MCRepurchaseRequest = new MCRepurchaseRequest();
                MCRepurchaseRequest.FTRequest = new MCRepurchaseRequestDetails();
                MCRepurchaseRequest.FTRequest.CreditAccount = transMaster.BeneficiaryAccount;
                MCRepurchaseRequest.FTRequest.creditCurrency = "NGN";
                MCRepurchaseRequest.FTRequest.DebitAccount = transMaster.AccountNumber;
                MCRepurchaseRequest.FTRequest.DebitValueDate = ConvertToCBADate(transMaster.DebitValueDate);
                MCRepurchaseRequest.FTRequest.DraftAmount = Math.Round(Convert.ToDouble(transMaster.Amount), 2) + "";
                MCRepurchaseRequest.FTRequest.narrations = transMaster.PaymentDetails;
                MCRepurchaseRequest.FTRequest.originalTransactionReference = transMaster.TransactionReference;
                MCRepurchaseRequest.FTRequest.PayeeName = transMaster.BeneficiaryName;
                MCRepurchaseRequest.FTRequest.SessionId = transMaster.access_token;
                MCRepurchaseRequest.FTRequest.StockNumber = transMaster.DraftNumber;
                MCRepurchaseRequest.FTRequest.TransactionBranch = transMaster.BranchCode;
                MCRepurchaseRequest.FTRequest.TrxnLocation = "1";
                MCRepurchaseRequest.FTRequest.VtellerAppID = ConfigurationManager.AppSettings["VtellerAppID"];

               var response =  ManagerIssuanceService.FundsTransferMcRepayment(url, MCRepurchaseRequest, transMaster.access_token);
                if(response.ResponseCode != "00")
                {
                    return Ok(new
                    {
                        success = false,
                        message =  response.ResponseText,
                    });
                }
                else
                {
                    transMaster.CBAResponse = response.ResponseText;
                    transMaster.CBAResponseCode = response.ResponseCode;
                    transMaster.TransactionReference = response.ReferenceID;
                    _issuanceService.ApproveManagerIssuanceForRepurchase(transMaster);
                    return Ok(new
                    {
                        success = true,
                        message = "Approved successfully",
                        TransactionRef = transMaster.TransactionReference
                    });
                }
            }
            else
            {
                _issuanceService.ApproveManagerIssuanceForRepurchase(transMaster);
                return Ok(new
                {
                    success = true,
                    message = "Approved successfully",
                    TransactionRef = transMaster.TransactionReference
                });
            }
           
        }


        [HttpPost, Route("api/Approve/ManagerIssuance")]
        public IHttpActionResult ApproveManagerIssuance(ManagerChequeIssuanceModel transMaster) 
        {
            try
            {
                Utils.LogNO("Request Details: " + JsonConvert.SerializeObject(transMaster));
                object result = null;
                try
                {
                    var MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed fund transfer Manager Issuance with stock number " + transMaster.DraftNumber
                        , transMaster.CreatedBy, MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }
                if (isDemo == "false")
                {
                    string url = ConfigurationManager.AppSettings["FundsTransferMcIssuanceStockNumber"];
                    Utils.LogNO("CBA url: " + url);
                    McIssuanceRequest mcIssuanceRequest = new McIssuanceRequest();
                    mcIssuanceRequest.FTRequest = new McIssuanceRequestDetails();
                    mcIssuanceRequest.FTRequest.ChequeType = "MC";
                    mcIssuanceRequest.FTRequest.CreditValueDate = ConvertToCBADate(transMaster.ValueDate);
                    mcIssuanceRequest.FTRequest.DebitAccount = transMaster.BeneficiaryAccount;
                    mcIssuanceRequest.FTRequest.DebitValueDate = ConvertToCBADate(transMaster.DebitValueDate);
                    mcIssuanceRequest.FTRequest.DraftAmount = Math.Round(Convert.ToDouble(transMaster.Amount), 2) + "";
                    mcIssuanceRequest.FTRequest.PayeeName = transMaster.BeneficiaryName;
                    mcIssuanceRequest.FTRequest.SessionId = transMaster.access_token;
                    mcIssuanceRequest.FTRequest.narations = transMaster.PaymentDetails;
                    mcIssuanceRequest.FTRequest.StockNumber = transMaster.DraftNumber;
                    mcIssuanceRequest.FTRequest.TransactionBranch = transMaster.BranchCode;
                    mcIssuanceRequest.FTRequest.ChargeAmt = transMaster.ChargeAmount != null ? transMaster.ChargeAmount > 0 ?
                                                          Math.Round(Convert.ToDecimal(transMaster.ChargeAmount), 0) + "0.00" : "0.00" : "0.00";
                    mcIssuanceRequest.FTRequest.TrxnLocation = "1";
                    mcIssuanceRequest.FTRequest.VtellerAppID = ConfigurationManager.AppSettings["VtellerAppID"];
                    var response = ManagerIssuanceService.FundsTransferMcIssuanceStockNumber(url, mcIssuanceRequest, transMaster.access_token);
                    if (response.ResponseCode != "00")
                    {
                        return Ok(new
                        {
                            success = false,
                            message = response.ResponseText,
                        });
                    }
                    else
                    {
                        transMaster.TransactionReference = response.ReferenceID;
                        transMaster.CBAResponse = response.ResponseText;
                        transMaster.CBAResponseCode = response.ResponseCode;
                        _issuanceService.ApproveManagerIssuance(transMaster);
                        return Ok(new
                        {
                            success = true,
                            message = "Approved successfully",
                            TransactionRef = response.ReferenceID
                        });
                    }
                }
                else
                {
                    transMaster.TransactionReference = Guid.NewGuid().ToString();
                    _issuanceService.ApproveManagerIssuance(transMaster);
                    return Ok(new
                    {
                        success = true,
                        message = "Approved successfully",
                        TransactionRef = transMaster.TransactionReference
                    });

                }
                
            }
            catch (Exception ex)
            {
                Utils.LogNO("ApproveManagerIssuance error: " + ex.Message);
                return Ok(new
                {
                    success = false,
                    message = ex.Message
                });
            }
           

               
           
        }


        [HttpPost, Route("api/Create/ManagerIssuance")]
        public IHttpActionResult CreateAndApproveManagerIssuance(TransactionModel transMaster)
        {
            try
            {
                Utils.LogNO("-----------------ApproveManagerIssuance started------------- ");
                Utils.LogNO("Details: " + JsonConvert.SerializeObject(transMaster));
                try
                {
                    var MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed  Manager Cheque Issuance with stock number "
                        + transMaster.ManagerChequeIssuanceDetailsModel.DraftNumber
                        , transMaster.InitiatorName, MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }
                object result = null;
                if (isDemo == "false")
                {
                    string url = ConfigurationManager.AppSettings["FundsTransferMcIssuanceStockNumber"];
                    Utils.LogNO("ApproveManagerIssuance url " + url);
                    McIssuanceRequest mcIssuanceRequest = new McIssuanceRequest();
                    mcIssuanceRequest.FTRequest = new McIssuanceRequestDetails();
                    mcIssuanceRequest.FTRequest.ChequeType = "MC";
                    mcIssuanceRequest.FTRequest.CreditValueDate = ConvertToCBADate(transMaster.ValueDate);
                    mcIssuanceRequest.FTRequest.DebitAccount = transMaster.AccountNo;
                    mcIssuanceRequest.FTRequest.DebitValueDate = ConvertToCBADate(transMaster.DebitValueDate);
                    mcIssuanceRequest.FTRequest.DraftAmount = Math.Round(Convert.ToDouble(transMaster.Amount), 2) + "";
                    mcIssuanceRequest.FTRequest.PayeeName = transMaster.AccountName;
                    mcIssuanceRequest.FTRequest.SessionId = transMaster.access_token;
                    mcIssuanceRequest.FTRequest.narations = transMaster.Remark;
                    mcIssuanceRequest.FTRequest.StockNumber = transMaster.ManagerChequeIssuanceDetailsModel.DraftNumber;
                    mcIssuanceRequest.FTRequest.TransactionBranch = transMaster.BranchCode;
                    mcIssuanceRequest.FTRequest.ChargeAmt = transMaster.CHARGEAMT != null ? transMaster.CHARGEAMT > 0 ?
                                                    Math.Round(Convert.ToDecimal(transMaster.CHARGEAMT), 0) + "" : "0.00" : "0.00";
                    mcIssuanceRequest.FTRequest.TrxnLocation = "1";
                    mcIssuanceRequest.FTRequest.VtellerAppID = ConfigurationManager.AppSettings["VtellerAppID"];
                    Utils.LogNO("web service begins");
                    var response = ManagerIssuanceService.FundsTransferMcIssuanceStockNumber(url, mcIssuanceRequest, transMaster.access_token);

                    if (response.ResponseCode != "00")
                    {
                        return Ok(new
                        {
                            success = false,
                            message = response.ResponseText,
                        });
                    }
                    else
                    {
                        transMaster.TransRef = response.ReferenceID;
                        transMaster.CBAResponse = response.ResponseText;
                        transMaster.CBACode = response.ResponseCode;
                        _issuanceService.CreateAndApproveManagerChequeIssuance(transMaster);
                        // _issuanceService.ApproveManagerIssuance(transMaster);
                        return Ok(new
                        {
                            success = true,
                            message = "Approved successfully",
                            TransactionRef = response.ReferenceID
                        });
                    }
                }
                else
                {
                    transMaster.TransRef  = Guid.NewGuid().ToString();
                    transMaster.CBAResponse = "Success";
                    transMaster.CBACode = "00";
                    _issuanceService.CreateAndApproveManagerChequeIssuance(transMaster);
                    return Ok(new
                    {
                        success = true,
                        message = "Approved successfully",
                        TransactionRef = transMaster.TransRef
                    });

                }
               
            }
            catch (Exception ex)
            {
                Utils.LogNO("ApproveManagerIssuance error: " + ex.Message);
                return Ok(new
                {
                    success = false,
                    message = ex.Message
                });
            }

        }

        [HttpPost, Route("api/Disapprove/ManagerIssuance")]
        public IHttpActionResult DisaApprove(MCApprovalModel transMaster)
        {
            try
            {
                _issuanceService.DissaproveManagerIssuance(transMaster);
                return Ok(new
                {
                    success = true,
                    message = "Disapproved successfully"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = ex.Message
                });
            }
           
        }


        [HttpPost, Route("api/CreateMCIssuance")]  
        public IHttpActionResult CreateransactionMaster(TransactionModel transactionModel) 
        {
            try
            {
                transactionModel.TransRef = Guid.NewGuid().ToString();
                _issuanceService.CreateManagerChequeIssuance(transactionModel);
                return Ok(new
                {
                    success = true,
                    message = "Save successfully"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = ex.Message
                });
            }
           
        }


        [HttpPost, Route("api/CreateOutwardCheque")]
        public IHttpActionResult CreateOutwardCheque(TransactionModel transactionModel)
        {
            try
            {
               // transactionModel.TransRef = Guid.NewGuid().ToString();
                _issuanceService.CreateOutwardChequeIssuance(transactionModel);
                return Ok(new
                {
                    success = true, 
                    message = "Save successfully"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = ex.Message
                });
            }

        }


        [HttpPost, Route("api/Approve/OutwardCheque")]
        public IHttpActionResult ApproveOutwardCheque(OutwardChequeDetailsModel transMaster)
        {
            try
            {
                Utils.LogNO("Request Details: " + JsonConvert.SerializeObject(transMaster));
                try
                {
                    var MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed Approved outward cheque with credit account number " + transMaster.CreditAccountNumber
                        , transMaster.ApprovedBy, MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }
                if (isDemo == "false")
                {
                    string url = ConfigurationManager.AppSettings["OutwardChequePosting"];
                    Utils.LogNO("url: " + url);
                    OutwardChequeRequest OutwardChequeRequest = new OutwardChequeRequest();
                    OutwardChequeRequest.FT_Request = new OutwardChequeRequestDetails();
                    OutwardChequeRequest.FT_Request.bankcode =transMaster.BankCode;
                    OutwardChequeRequest.FT_Request.beneficiaryAccount = transMaster.CreditAccountNumber;
                    OutwardChequeRequest.FT_Request.beneficiaryName = transMaster.CreditAccountName.Length <= 20 ? transMaster.CreditAccountName : transMaster.CreditAccountName.Substring(0, 20);
                    OutwardChequeRequest.FT_Request.ChequeNumber = transMaster.ChequeNumber;
                    OutwardChequeRequest.FT_Request.creditAmount = Math.Round(Convert.ToDouble(transMaster.CreditAmount), 2) + "";
                    OutwardChequeRequest.FT_Request.CreditValueDate = ConvertToCBADate(transMaster.DebitValueDate);
                    OutwardChequeRequest.FT_Request.debitAccount = transMaster.DebitAccountNumber;
                    OutwardChequeRequest.FT_Request.TransactionBranch = transMaster.BranchCode;
                    OutwardChequeRequest.FT_Request.SessionId = transMaster.access_token;
                    OutwardChequeRequest.FT_Request.TrxnLocation = "1";

                    var response = ManagerIssuanceService.OutwardChequePosting(url, OutwardChequeRequest, transMaster.access_token);
                    if (response.ResponseCode != "00")
                    {
                        return Ok(new
                        {
                            success = false,
                            message = response.ResponseText,
                        });
                    }
                    else
                    {
                        //  response.
                        transMaster.CBAResponse = response.ResponseText;
                        transMaster.CBAResponseCode = response.ResponseCode;
                        transMaster.TransactionReference = response.ReferenceID;
                        _issuanceService.ApproveOutwardCheque(transMaster);
                        return Ok(new
                        {
                            success = true,
                            message = "Approved successfully",
                            TransactionRef = response.ReferenceID
                        });
                    }
                }
                else
                {
                    Utils.LogNO("Approving...... ");
                    transMaster.CBAResponse = "SUCCESS";
                    transMaster.CBAResponseCode = "0";
                    transMaster.TransactionReference = Guid.NewGuid().ToString();
                    _issuanceService.ApproveOutwardCheque(transMaster);
                    return Ok(new
                    {
                        success = true,
                        message = "Approved successfully",
                        TransactionRef = Guid.NewGuid().ToString()
                    });
                }
                
            }
            catch (Exception ex)
            {
                Utils.LogNO("Error on Outward Cheque: " + ex.Message);
                return Ok(new
                {
                    success = false,
                    message = "Error occured while approving outward cheque.",
                    TransactionRef = ""
                });
            }
           
        }

        [HttpPost, Route("api/Create/Approve/OutwardCheque")]
        public IHttpActionResult CreateAndApproveOutwardCheque(TransactionModel transMaster)
        {
            try
            {
                Utils.LogNO("Request Details: " + JsonConvert.SerializeObject(transMaster));
                try
                {
                    var MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed Approved outward cheque with credit account number " + transMaster.AccountNo
                        , transMaster.ApprovedBy, MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }
                if (isDemo == "false")
                {
                    string url = ConfigurationManager.AppSettings["OutwardChequePosting"];
                    Utils.LogNO("url: " + url);
                    OutwardChequeRequest OutwardChequeRequest = new OutwardChequeRequest();
                    OutwardChequeRequest.FT_Request = new OutwardChequeRequestDetails();
                    OutwardChequeRequest.FT_Request.bankcode = transMaster.BankCode;
                    OutwardChequeRequest.FT_Request.beneficiaryAccount = transMaster.AccountNo;
                    OutwardChequeRequest.FT_Request.beneficiaryName = transMaster.AccountName.Length <= 20 ? transMaster.AccountName : transMaster.AccountName.Substring(0, 20);
                    OutwardChequeRequest.FT_Request.ChequeNumber = transMaster.ChequeNo;
                    OutwardChequeRequest.FT_Request.creditAmount = Math.Round(Convert.ToDouble(transMaster.TotalAmt), 2) + "";
                    OutwardChequeRequest.FT_Request.CreditValueDate = ConvertToCBADate(transMaster.DebitValueDate);
                    OutwardChequeRequest.FT_Request.debitAccount = transMaster.ManagerChequeIssuanceDetailsModel.AccountNumber;
                    OutwardChequeRequest.FT_Request.TransactionBranch = transMaster.BranchCode;
                    OutwardChequeRequest.FT_Request.SessionId = transMaster.access_token;
                    OutwardChequeRequest.FT_Request.TrxnLocation = "1";

                    var response = ManagerIssuanceService.OutwardChequePosting(url, OutwardChequeRequest, transMaster.access_token);
                    if (response.ResponseCode != "00")
                    {
                        return Ok(new
                        {
                            success = false,
                            message = response.ResponseText,
                        });
                    }
                    else
                    {
                        transMaster.CBAResponse = response.ResponseText;
                        transMaster.CBACode = response.ResponseCode;
                        transMaster.TransRef = response.ReferenceID;
                        _issuanceService.CreateAndApproveOutwardChequeIssuance(transMaster);
                        return Ok(new
                        {
                            success = true,
                            message = "Outward Cheque created and approved successfully",
                            TransactionRef = response.ReferenceID
                        });
                    }
                }
                else
                {
                    Utils.LogNO("Approving...... ");
                    transMaster.CBAResponse = "SUCCESS";
                    transMaster.CBACode = "0";
                    transMaster.TransRef = Guid.NewGuid().ToString();
                    _issuanceService.CreateAndApproveOutwardChequeIssuance(transMaster);
                    return Ok(new
                    {
                        success = true,
                        message = "Outward Cheque created and approved successfully",
                        TransactionRef = Guid.NewGuid().ToString()
                    });
                }

            }
            catch (Exception ex)
            {
                Utils.LogNO("Error on Outward Cheque: " + ex.Message);
                return Ok(new
                {
                    success = false,
                    message = "Error occured while approving outward cheque.",
                    TransactionRef = ""
                });
            }

        }


        [HttpPost, Route("api/Disapprove/OutwardCheque")]
        public IHttpActionResult DisapproveOutwardCheque(OutwardChequeDetailsModel transMaster)
        {
            try
            {
                try
                {
                    var MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed Approved outward cheque with credit account number " + transMaster.CreditAccountNumber
                        , transMaster.ApprovedBy, MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }

                    Utils.LogNO("Disapproving...... ");
                    _issuanceService.ApproveOutwardCheque(transMaster);
                    return Ok(new
                    {
                        success = true,
                        message = "Approved successfully",
                        TransactionRef = Guid.NewGuid().ToString()
                    });

            }
            catch (Exception ex)
            {
                Utils.LogNO("Error on Outward Cheque: " + ex.Message);
                return Ok(new
                {
                    success = false,
                    message = "Error occured while approving outward cheque.",
                    TransactionRef = ""
                });
            }

        }



        //[HttpPost, Route("api/Disapprove/ManagerIssuance")]
        //public IHttpActionResult DisaApproveMCCheque([FromBody] MCApprovalModel mCApprovalModel)
        //{
        //    _issuanceService.DissaproveOutwardCheque(mCApprovalModel);
        //    return Ok(new
        //    {
        //        success = true,
        //        message = "Disapproved successfully"
        //    });
        //}

        private string ConvertToCBADate(DateTime? dt)
        {
            var month = Convert.ToString(dt.Value.Month);
            month = month.Length == 1 ? "0" + month : month;
            var day = Convert.ToString(dt.Value.Day);
            day = day.Length == 1 ? "0" + day : day;
            return dt.Value.Year + month + day + "";
        }
        public static string DetermineCompName(string IP)
        {
            try
            {
                IPAddress myIP = IPAddress.Parse(IP);
                IPHostEntry GetIPHost = Dns.GetHostEntry(myIP);
                List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
                return compName.First();
            }
            catch { return ""; }
        }

        public string GetIPAddress()
        {
            try
            {

                System.Web.HttpContext context = System.Web.HttpContext.Current;
                string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    string[] addresses = ipAddress.Split(',');
                    if (addresses.Length != 0)
                    {
                        return addresses[0];
                    }
                }

                return context.Request.ServerVariables["REMOTE_ADDR"];
            }
            catch
            {
                return HttpContext.Current.Request.Params["HTTP_CLIENT_IP"] ?? HttpContext.Current.Request.UserHostAddress;

            }

        }


    }
}
