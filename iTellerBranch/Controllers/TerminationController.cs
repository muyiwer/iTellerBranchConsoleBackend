using iTellerBranch.BankService;
using iTellerBranch.Business.Transaction;
using iTellerBranch.Model;
using iTellerBranch.Model.ViewModel;
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
using static iTellerBranch.Model.ViewModel.DepositClosureDetailsModel;
using static iTellerBranch.Model.ViewModel.DiscountedDepositFlowModel;
using static iTellerBranch.Model.ViewModel.PartialWithdrawalFlowModel;
using static iTellerBranch.Model.ViewModel.ReverseLdContractFlowModel;
using static iTellerBranch.Model.ViewModel.TerminateDiscountedDepositModel;
using static iTellerBranch.Model.ViewModel.TreasuryRequestModel;

namespace iTellerBranch.Controllers
{
    public class TerminationController : ApiController
    {
        private readonly TreasuryService _treasuryService;
        private readonly TransactionService _transactionService;
        private readonly TransactionBusiness _transactionBusiness;
        private static string isDemo = System.Configuration.ConfigurationManager.AppSettings["isDemo"];
        public TerminationController()
        {
            _treasuryService = new TreasuryService();
            _transactionBusiness = new TransactionBusiness();
            _transactionService = new TransactionService();
        }

        [HttpPost, Route("api/GetDiscountProduct")]
        public IHttpActionResult GetDiscountProduct()
        {
            var result = _treasuryService.GetDiscountedTreasuryProduct();
            return Ok(result);
        }


        [HttpGet, Route("api/GetMatureDeals")]
        public IHttpActionResult GetMatureDeals()
        {
            var result = _treasuryService.GetMatureDeals();
            return Ok(result);
        }

        [HttpGet, Route("api/GetDiscountedDeals")]
        public IHttpActionResult GetDiscountedDeals() 
        {
            var result = _treasuryService.GetDiscountedDeals();
            return Ok(result);
        }


        [HttpPost, Route("api/Reverse/DiscountedTreasuryDeals")]
        public IHttpActionResult ReverseDiscountedDeal([FromBody]TreasuryDealsModel transMaster) 
        {
            try
            {
                object result = null;
                string url;
                Utils.LogNO("Saving Treasury trans for Approval Started");
                if (isDemo == "false")
                {
                    Utils.LogNO("Treasury deal details:" + JsonConvert.SerializeObject(transMaster));
                    ReverseLdContractFlowRequestModel reverseLdContract = new ReverseLdContractFlowRequestModel();
                    reverseLdContract.ReverseLdTransaction = new ReverseLdTransaction();
                    reverseLdContract.ReverseLdTransaction.LdId = transMaster.CBADealId;
                    reverseLdContract.ReverseLdTransaction.TransactionBranch = transMaster.BranchCode;
                    url = ConfigurationManager.AppSettings["ReverseLdContractFlow"];
                    var response = TreasuryCbaService.ReverseLdContractFlow(url, reverseLdContract, transMaster.access_token);
                    if (response.RespondCode != "1")
                    {
                        return Ok(new
                        {
                            success = false,
                            message = response.ResponseText
                        });
                    }
                    else
                    {
                        _treasuryService.ReverseDiscountedDeal(transMaster.DealId);
                        return Ok(new
                        {
                            success = true,
                            message = "Success",
                            TransRef = response.ResponseId
                        });
                    }
                }
                else
                {
                    _treasuryService.ReverseDiscountedDeal(transMaster.DealId);
                    return Ok(new
                    {
                        success = true,
                        message = "Success",
                        TransRef = transMaster.CBADealId
                    });

                    //var entries = _treasuryService.CreatDoubleEntriesForDeals(transMaster);
                    //_transactionService.CreateRangeTransaction(entries);

                }
                try
                {
                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed discounted Treasury Deal transaction  reversal of " + transMaster.CustomerId + " CustomerId"
                        , transMaster.UserId, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }

                // return Ok(result);

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


        [HttpPost, Route("api/Terminate/TreasuryDeals")]
        public IHttpActionResult TerminateDeal([FromBody]TreasuryDealsModel transMaster)
        {
            try
            {
                object result = null;
                string url;
                Utils.LogNO("Saving Treasury trans for Approval Started");
                if (isDemo == "false")
                {
                    Utils.LogNO("Treasury deal details:" + JsonConvert.SerializeObject(transMaster));
                    transMaster.CurrencyAbbrev = _treasuryService.GetCurrencyAbbrev(transMaster.CurrencyCode);
                    bool IsProductTypeTenure = _treasuryService.ProductTypeIsTenured(transMaster.ProductCode);
                    transMaster.CBAProductCode = _transactionService.GetTerminationCode
                                                (Convert.ToInt16(transMaster.TerminationInstructionCode));
                    DepositClosure depositClosure = new DepositClosure();
                    depositClosure.Request = new DepositClosureDetails();
                    depositClosure.Request.ArrangementId = transMaster.CBADealId;
                    depositClosure.Request.CompCode = transMaster.BranchCode;
                    depositClosure.Request.Currency = transMaster.CurrencyAbbrev;
                    depositClosure.Request.SettlementAcct = transMaster.PaymentAccount;
                    depositClosure.Request.ValueDate = ConvertToCBADate(transMaster.PreLiquidation.LiquidationDate);
                    url = ConfigurationManager.AppSettings["DepositClosureDetails"]; 
                    var response = TreasuryCbaService.DepositClosureDetails(url, depositClosure, transMaster.access_token);
                    if(response.RespondCode != "1")
                    {
                        return Ok(new
                        {
                            success = false,
                            message = response.ResponseText
                        });
                    }
                    else
                    {
                        transMaster.TransRef = response.ResponseId;
                        _treasuryService.TerminateDeal(transMaster);
                        return Ok(new
                        {
                            success = true,
                            message = "Success",
                            TransactionRef = response.ResponseId
                        });
                    }
                }
                else
                {
                    transMaster.TransRef = transMaster.DealId;
                    _treasuryService.TerminateDeal(transMaster);
                    return Ok(new
                    {
                        success = true,
                        message = "Success",
                        TransactionRef = transMaster.DealId
                    });

                    //var entries = _treasuryService.CreatDoubleEntriesForDeals(transMaster);
                    //_transactionService.CreateRangeTransaction(entries);

                }
                try
                {
                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed Treasury Deal transaction  Approval of " + transMaster.CustomerId + " CustomerId"
                        , transMaster.UserId, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }

               // return Ok(result);

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



        [HttpPost, Route("api/Create/TreasuryDeals")]
        public IHttpActionResult CreateTreasuryDeals([FromBody]TreasuryDealsModel transMaster)
        {
            try
            {
                transMaster.Posted = false;
                transMaster.CBA = "T24";
                object result = null;

                Utils.LogNO("Saving Treasury trans for Approval Started");
                bool IsProductTypeTenure = _treasuryService.ProductTypeIsTenured(transMaster.ProductCode);
                if (IsProductTypeTenure)
                {
                    if (Convert.ToInt16(transMaster.ProductCode) == Convert.ToInt16(TransType.ProductCode.CollaterizedCallDeposit))
                    {
                        transMaster.TerminationInstructionCode = 0;
                    }
                }
                else
                {
                    transMaster.TerminationInstructionCode = 0;
                }
                result = _transactionBusiness.CreateTreasuryDealsTransaction(transMaster);

                try
                {

                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed Treasury Deal transaction of " + transMaster.PrincipalAmount + " from " + transMaster.CustomerId + " CustomerId"
                        , transMaster.UserId, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }

                return Ok(result);

            }
            catch (Exception ex)
            {
                var result = _transactionBusiness.GetAllTreasuryTrans(false, ex.Message, ex);
                return Ok(result);
            }
        }

        [HttpPost, Route("api/Create/DiscountDeals")]
        public IHttpActionResult CreateDiscountDeals([FromBody]TreasuryDealsModel transMaster)
        {
            try
            {
                transMaster.Posted = false;
                transMaster.CBA = "T24";
                object result = null;

                Utils.LogNO("Saving Treasury trans for Approval Started");
                transMaster.TerminationInstructionCode = 0;
                result = _transactionBusiness.CreateTreasuryDealsTransaction(transMaster);
                try
                {

                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed Treasury Deal transaction of " + transMaster.PrincipalAmount + " from " + transMaster.CustomerId + " CustomerId"
                        , transMaster.UserId, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }

                return Ok(result);

            }
            catch (Exception ex)
            {
                var result = _transactionBusiness.GetAllTreasuryTrans(false, ex.Message, ex);
                return Ok(result);
            }
        }


        [HttpPost, Route("api/Update/TreasuryDeals")]
        public IHttpActionResult UpdateTreasuryDeals([FromBody]TreasuryDealsModel transMaster)
        {
            try
            {
                transMaster.Posted = false;
                transMaster.CBA = "T24";
                object result = null;

                Utils.LogNO("Saving Treasury trans for Approval Started");

                bool response = _transactionBusiness.DeleteTreasuryDeal(transMaster.Id);
                bool IsProductTypeTenure = _treasuryService.ProductTypeIsTenured(transMaster.ProductCode);
                if (IsProductTypeTenure)
                {
                    if (Convert.ToInt16(transMaster.ProductCode) == Convert.ToInt16(TransType.ProductCode.CollaterizedCallDeposit))
                    {
                        transMaster.TerminationInstructionCode = 0;
                    }
                }
                else
                {
                    transMaster.TerminationInstructionCode = 0;
                }
                if (response == true)
                {
                    Utils.LogNO("Deleted old record of treasury deal");
                    transMaster.ProcessStatus = 4;
                    Utils.LogNO("Treasury Details: "+ JsonConvert.SerializeObject(transMaster));
                    result = _transactionBusiness.CreateTreasuryDealsTransaction(transMaster);
                    Utils.LogNO("Created record of treasury deal");
                    try
                    {
                        transMaster.MachineName = DetermineCompName(GetIPAddress());
                        var IPAddress = GetIPAddress();
                        General.AuditLog("Performed Treasury Deal transaction of " + transMaster.PrincipalAmount + " from " + transMaster.CustomerId + " CustomerId"
                            , transMaster.UserId, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                    }
                    catch { }

                    return Ok(result);
                }
                else
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Treasury deal not successfully updated"
                    });
                }

            }
            catch (Exception ex)
            {
                var result = _transactionBusiness.GetAllTreasuryTrans(false, ex.Message, ex);
                return Ok(result);
            }
        }

        [HttpPost, Route("api/Approve/TreasuryDeals")]
        public IHttpActionResult ApproveTreasuryDeals([FromBody]TreasuryDealsModel transMaster)
        {
            try
            {
                object result = null;
                string url;
                Utils.LogNO("Saving Treasury trans for Approval Started");
                if (isDemo == "false")
                {
                    Utils.LogNO("Treasury deal details:" + JsonConvert.SerializeObject(transMaster));
                    transMaster.CurrencyAbbrev = _treasuryService.GetCurrencyAbbrev(transMaster.CurrencyCode);
                    bool IsProductTypeTenure = _treasuryService.ProductTypeIsTenured(transMaster.ProductCode);
                    transMaster.CBAProductCode = _transactionService.GetTerminationCode
                                                (Convert.ToInt16(transMaster.TerminationInstructionCode));
                    int discountDeposittype = _treasuryService.GetDiscountDepositType(transMaster.DealersReference);
                    Utils.LogNO("discountDeposittype:" + discountDeposittype);
                    if (discountDeposittype == 1)
                    {
                        result = PreLiquidateDepositForCBA(transMaster);
                    }
                    else
                    {
                        if (IsProductTypeTenure)
                        {
                            if (Convert.ToInt16(transMaster.ProductCode) == Convert.ToInt16(TransType.ProductCode.CollaterizedCallDeposit))
                            {
                                transMaster.TerminationInstructionCode = 0;
                            }
                            return ApproveTenureTreasuryDeal(transMaster, ref result, out url);
                        }
                        else
                        {
                            transMaster.TerminationInstructionCode = 0;
                            return ApproveNonTenureTreasuryDeal(transMaster, ref result, out url);
                        }
                    }
                    
                }
                else
                {
                    result = _transactionBusiness.ApproveTeasuryDeal(transMaster);
                   
                    //var entries = _treasuryService.CreatDoubleEntriesForDeals(transMaster);
                    //_transactionService.CreateRangeTransaction(entries);

                }
                try
                {
                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed Treasury Deal transaction  Approval of " + transMaster.CustomerId + " CustomerId"
                        , transMaster.UserId, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }

                return Ok(result);

            }
            catch (Exception ex)
            {
                var result = _transactionBusiness.GetAllTreasuryTrans(false, ex.Message, ex);
                return Ok(result);
            }
        }

        [HttpPost, Route("api/Approve/DiscountDeals")]
        public IHttpActionResult ApproveDiscountDeals([FromBody]TreasuryDealsModel transMaster) 
        {
            try
            {
                object result = null;
                DealOutPutResponse response = null;
                Utils.LogNO("Saving Treasury trans for Approval Started");
                if (isDemo == "false")
                {
                    Utils.LogNO("Treasury deal details:" + JsonConvert.SerializeObject(transMaster));

                    int discountDeposittype = _treasuryService.GetDiscountDepositType(transMaster.DealId);
                    if(discountDeposittype == 0)
                    {
                        response = CreateDiscountDepositForCBA(transMaster);
                    }
                    else if(discountDeposittype == 1)
                    {
                        response = PreLiquidateDiscountDepositForCBA(transMaster);
                    }
                    else
                    {
                        response = PartiallyPreLiquidateDiscountDepositForCBA(transMaster);
                    }

                    if (response.ResponseText == "Success")
                    {
                        transMaster.CBADealId = response.ResponseId;
                        transMaster.DealId = response.ResponseId;
                        transMaster.CBA = "T24";
                        transMaster.TransRef = response.ResponseId;
                        transMaster.DealersReference = response.ResponseId;
                        transMaster.DoubleEntrySuccessful = true;
                        result = _transactionBusiness.ApproveTeasuryDeal(transMaster);
                        return Ok(new
                        {
                            success = true,
                            message = "Approved successfully",
                            TransactionRef = response.ResponseId
                        });
                       
                    }
                    else
                    {
                        return Ok(new
                        {
                            success = false,
                            message = response.ResponseText
                        });

                    }
                }
                else
                {
                    result = _transactionBusiness.ApproveTeasuryDeal(transMaster);
                    //transMaster.TransRef = Guid.NewGuid().ToString();

                 //  var entries = _treasuryService.CreatDoubleEntriesForDiscountedDeals(transMaster);
                    //_transactionService.CreateRangeTransaction(entries);

                }
                try
                {
                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed Treasury Deal transaction  Approval of " + transMaster.CustomerId + " CustomerId"
                        , transMaster.UserId, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }

                return Ok(result);

            }
            catch (Exception ex)
            {
                var result = _transactionBusiness.GetAllTreasuryTrans(false, ex.Message, ex);
                return Ok(result);
            }
        }

        private string ConvertToCBADate(DateTime? dt)
        {
            var month = Convert.ToString(dt.Value.Month);
            month = month.Length == 1 ? "0" + month : month;
            var day = Convert.ToString(dt.Value.Day);
            day = day.Length == 1 ? "0" + day : day;
            return dt.Value.Year + month + day + "";
        }


        private IHttpActionResult ApproveTenureTreasuryDeal(TreasuryDealsModel transMaster, ref object result, out string url)
        {
            Utils.LogNO("ApproveTenureTreasuryDeal started");
            CollaterizedDepositModel collaterizedDepositModel = new CollaterizedDepositModel();
            collaterizedDepositModel.Request = new CollaterizedDepositDetail();
            collaterizedDepositModel.Request.AcctName = transMaster.CustomerName;
            collaterizedDepositModel.Request.Amt = Math.Round(Convert.ToDouble(transMaster.PrincipalAmount), 2) + "";
            collaterizedDepositModel.Request.CompCode = transMaster.BranchCode;
            collaterizedDepositModel.Request.Currency = transMaster.CurrencyAbbrev;
            collaterizedDepositModel.Request.CustId = transMaster.CustomerId;
            collaterizedDepositModel.Request.dao = transMaster.AccountOfficer;
            collaterizedDepositModel.Request.EffectiveDate = ConvertToCBADate(transMaster.ValueDate);
            collaterizedDepositModel.Request.PayinAcct = transMaster.InflowAccount;
            collaterizedDepositModel.Request.PayoutAcct = transMaster.PaymentAccount;
            collaterizedDepositModel.Request.ProductId = transMaster.CBAProductCode;
            collaterizedDepositModel.Request.Rate = transMaster.TreasuryInterest.FirstOrDefault().InterestRate + "";
            Utils.LogNO("CollaterizedCallDeposit: " + Convert.ToString(TransType.ProductCode.CollaterizedCallDeposit));
            if(Convert.ToInt16(transMaster.ProductCode) == Convert.ToInt16(TransType.ProductCode.CollaterizedCallDeposit))
            {
                url = ConfigurationManager.AppSettings["BookAADeposit"];
                collaterizedDepositModel.Request.Term = transMaster.Tenure + "D";
                collaterizedDepositModel.Request.ProductId =  _treasuryService.GetproductCode(transMaster.ProductCode);
            }
            else
            {
                if (transMaster.TerminationInstructionCode ==
                Convert.ToInt16(TransType.TerminationInstructionCode.NoRollOver))
                {
                    url = ConfigurationManager.AppSettings["BookAADeposit"];
                    collaterizedDepositModel.Request.Term = transMaster.Tenure + "D";
                }
                else
                {
                    url = ConfigurationManager.AppSettings["BookAADepositRollover"];
                    collaterizedDepositModel.Request.ChangePeriod = transMaster.Tenure + "D";
                }
            }
               
           
            TreasuryResponse treasury = TreasuryCbaService.CollaterizedDeposit
                                                             (url, collaterizedDepositModel, transMaster.access_token);
            if (treasury.response.RespondCode != "1")
            {
                return Ok(new
                {
                    success = false,
                    message = treasury.response.ResponseText
                });
            }
            else
            {
                transMaster.CBADealId = treasury.response.ArrangementId;
                transMaster.DealId = treasury.response.ArrangementId;
                transMaster.CBA = "T24";
                transMaster.TransRef = treasury.response.ResponseId;
                transMaster.DealersReference = treasury.response.ResponseId;
                //var entries = _treasuryService.CreatDoubleEntriesForDeals(transMaster);
                //bool isSuccessful = DoubleEntriesTransfer.TransferEntries(entries);  _transactionService.CreateRangeTransaction(entries);
                transMaster.DoubleEntrySuccessful = true;
                result = _transactionBusiness.ApproveTeasuryDeal(transMaster);
                return Ok(new
                {
                    success = true,
                    DoubleEntryFailed = false,
                    TransactionRef = treasury.response.ArrangementId,
                    message = "Approved successfully"
                });
            }
               
        }


        private IHttpActionResult ApproveNonTenureTreasuryDeal(TreasuryDealsModel transMaster, ref object result, out string url)
        {
            Utils.LogNO("ApproveTenureTreasuryDeal started");
            url = ConfigurationManager.AppSettings["CallDep"];
            CallDepositModel callDepositModel = new CallDepositModel();
            callDepositModel.Request = new CallDepositDetailsModel();
            callDepositModel.Request.AcctName = transMaster.CustomerName;
            callDepositModel.Request.amt = Math.Round(Convert.ToDouble(transMaster.PrincipalAmount), 2) + "";
            callDepositModel.Request.CompCode = transMaster.BranchCode;
            callDepositModel.Request.currency = transMaster.CurrencyAbbrev;
            callDepositModel.Request.custid = transMaster.CustomerId;
            callDepositModel.Request.dao = transMaster.AccountOfficer;
            callDepositModel.Request.effectivedate = ConvertToCBADate(transMaster.ValueDate); ;
            callDepositModel.Request.payinacct = transMaster.InflowAccount;
            callDepositModel.Request.payoutacct = transMaster.PaymentAccount;
            callDepositModel.Request.rate = transMaster.TreasuryInterest.FirstOrDefault().InterestRate + "";

            TreasuryResponse treasury = TreasuryCbaService.CallDeposit
                                                             (url, callDepositModel, transMaster.access_token);
            if (treasury.response.RespondCode != "1")
            {
                return Ok(new
                {
                    success = false,
                    message = treasury.response.ResponseText
                });
            }
            else
            {
                transMaster.CBADealId = treasury.response.ArrangementId;
                transMaster.DealId = treasury.response.ArrangementId;
                transMaster.CBA = "T24";
                transMaster.TransRef = treasury.response.ResponseId;
                transMaster.DealersReference = treasury.response.ResponseId;
                transMaster.DoubleEntrySuccessful = true;
                result = _transactionBusiness.ApproveTeasuryDeal(transMaster);
                return Ok(new
                {
                    success = true,
                    DoubleEntryFailed = false,
                    TransactionRef = treasury.response.ArrangementId,
                    message = "Approved successfully"
                });
               
                //var entries = _treasuryService.CreatDoubleEntriesForDeals(transMaster);
                //bool isSuccessful = DoubleEntriesTransfer.TransferEntries(entries);
                //if (isSuccessful)
                //{
                //    _transactionService.CreateRangeTransaction(entries);
                //    transMaster.DoubleEntrySuccessful = true;
                //    result = _transactionBusiness.ApproveTeasuryDeal(transMaster);
                //    return Ok(new
                //    {
                //        success = true,
                //        DoubleEntryFailed = false,
                //        message = "Approved successfully"
                //    });
                //}
                //else
                //{
                //    transMaster.DoubleEntrySuccessful = false;
                //    result = _transactionBusiness.ApproveTeasuryDeal(transMaster);
                //    return Ok(new
                //    {
                //        success = true,
                //        DoubleEntryFailed = true,
                //        message = "Approved successfully"
                //    });
                //}

            }
        }


        [HttpPost, Route("api/Disapprove/TreasuryDeals")]
        public IHttpActionResult DisapproveTreasuryDeals([FromBody]TreasuryDealsModel transMaster)
        {
            try
            {
                object result = null;
                Utils.LogNO("transMaster" + JsonConvert.SerializeObject(transMaster));
                Utils.LogNO("Saving Treasury trans for Approval Started");
                result = _transactionBusiness.DisapproveTeasuryDeal(transMaster);
                try
                {
                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed Treasury Deal disapproval of " + transMaster.PrincipalAmount + " from " + transMaster.CustomerId + " CustomerId"
                        , transMaster.UserId, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }

                return Ok(result);

            }
            catch (Exception ex)
            {
                var result = _transactionBusiness.GetAllTreasuryTrans(false, ex.Message, ex);
                return Ok(result);
            }
        }


        [HttpPost, Route("api/Liquidate/PreLiquidate")]
        public IHttpActionResult CreateLiquidation([FromBody]TreasuryDealsModel transMaster) 
        {
            try
            {
                transMaster.Posted = false;
                transMaster.CBA = "T24";
                _treasuryService.PreLiquidate(transMaster);
                //List<TransactionModel> transactionModels = _treasuryService.CreatDoubleEntries(transMaster);
                //_transactionService.CreateRangeTransaction(transactionModels);
                try
                {
                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed Treasury Deal transaction of " + transMaster.PrincipalAmount + " from " + transMaster.CustomerId + " CustomerId"
                        , transMaster.UserId, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }

                return Ok(new
                {
                    success =  true,
                    message = "Deal Pre Liquidated successfully sent for approval"
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


        [HttpPost, Route("api/Liquidate/LiquidateDiscount")]
        public IHttpActionResult CreateDiscountLiquidation([FromBody]TreasuryDealsModel transMaster)
        {
            try
            {
                transMaster.Posted = true;
                transMaster.CBA = "T24";
                if (isDemo == "false")
                {

                   // List<TransactionModel> transactionModels = _treasuryService.CreatDoubleEntries(transMaster);
                    //bool isSuccessful = DoubleEntriesTransfer.TransferEntries(transactionModels);
                    //if (isSuccessful)
                    //{
                    //    _transactionService.CreateRangeTransaction(transactionModels);
                    //}
                }
                else
                {
                    _treasuryService.CreatDoubleEntriesForDiscountLiquidation(transMaster);
                    List<TransactionModel> transactionModels = _treasuryService.CreatDoubleEntries(transMaster);
                    _transactionService.CreateRangeTransaction(transactionModels);
                }


                try
                {
                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed Treasury Deal transaction of " + transMaster.PrincipalAmount + " from " + transMaster.CustomerId + " CustomerId"
                        , transMaster.UserId, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }

                return Ok(new
                {
                    success = true,
                    message = "Deal Pre Liquidated successfully"
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

        [HttpPost, Route("api/Liquidate/PreLiquidateDiscount")]
        public IHttpActionResult CreateDiscountPreLiquidation([FromBody]TreasuryDealsModel transMaster)
        {
            try
            {
                transMaster.Posted = false; 
                transMaster.CBA = "T24";
                _treasuryService.PreLiquidate(transMaster);
                try
                {
                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed Treasury Deal transaction of " + transMaster.PrincipalAmount + " from " + transMaster.CustomerId + " CustomerId"
                        , transMaster.UserId, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }

                return Ok(new
                {
                    success = true,
                    message = "Deal Pre Liquidated successfully sent for approval"
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

        [HttpPost, Route("api/Liquidate/PartialPreLiquidateDiscount")]
        public IHttpActionResult CreateDiscountPreLiquidationPartially([FromBody]TreasuryDealsModel transMaster) 
        {
            try
            {
                transMaster.Posted = false;
                transMaster.CBA = "T24";
                _treasuryService.PreLiquidate(transMaster);
                _treasuryService.PreLiquidatePartially(transMaster);
              
                try
                {
                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed Treasury Deal transaction of " + transMaster.PrincipalAmount + " from " + transMaster.CustomerId + " CustomerId"
                        , transMaster.UserId, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }
                return Ok(new
                {
                    success = true,
                    message = "Deal Partially Pre-Liquidated successfully sent for approval"
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


        [HttpPost, Route("api/Liquidate/CallDeal")]
        public IHttpActionResult CreateCallDealLiquidation([FromBody]TreasuryDealsModel transMaster) 
        {
            try
            {
                transMaster.Posted = true;
                transMaster.CBA = "T24";
                if (isDemo == "false")
                {
                    List<TransactionModel> transactionModels = _treasuryService.CreatDoubleEntriesForCallDeposit(transMaster);
                    bool isSuccessful = DoubleEntriesTransfer.TransferEntries(transactionModels);
                    if (isSuccessful)
                    {
                        _transactionService.CreateRangeTransaction(transactionModels);
                    }
                }
                else
                {
                    _treasuryService.LiquidateCallDeal(transMaster);
                    List<TransactionModel> transactionModels = _treasuryService.CreatDoubleEntriesForCallDeposit(transMaster);
                    _transactionService.CreateRangeTransaction(transactionModels);
                }
               

                try
                {
                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed Treasury Deal transaction of " + transMaster.PrincipalAmount + " from " + transMaster.CustomerId + " CustomerId"
                        , transMaster.UserId, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }

                return Ok(new
                {
                    success = true,
                    message = "Deal Pre Liquidated successfully"
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

        private IHttpActionResult PreLiquidateDepositForCBA(TreasuryDealsModel transMaster)
        {
            string url = "";
            Utils.LogNO("Treasury deal details:" + JsonConvert.SerializeObject(transMaster));
            transMaster.CurrencyAbbrev = _treasuryService.GetCurrencyAbbrev(transMaster.CurrencyCode);
            bool IsProductTypeTenure = _treasuryService.ProductTypeIsTenured(transMaster.ProductCode);
            transMaster.CBAProductCode = _transactionService.GetTerminationCode
                                        (Convert.ToInt16(transMaster.TerminationInstructionCode));
            DepositClosure depositClosure = new DepositClosure();
            depositClosure.Request = new DepositClosureDetails();
            depositClosure.Request.ArrangementId = transMaster.DealId;
            depositClosure.Request.CompCode = transMaster.BranchCode;
            depositClosure.Request.Currency = transMaster.CurrencyAbbrev;
            depositClosure.Request.SettlementAcct = transMaster.PaymentAccount;
            depositClosure.Request.ValueDate = ConvertToCBADate(transMaster.MaturityDate);
            url = ConfigurationManager.AppSettings["DepositClosureDetails"];
            var response = TreasuryCbaService.DepositClosureDetails(url, depositClosure, transMaster.access_token);
            if(response.RespondCode != "1")
            {
                return Ok(new
                {
                    success = false,
                    message = response.ResponseText
                });
            }
            else
            {
                 _transactionBusiness.ApproveTeasuryDeal(transMaster);
                return Ok(new
                {
                    success = true,
                    message = "Approved successfully",
                    TransactionRef = response.ResponseId
                });
            }
        }

        private DealOutPutResponse PreLiquidateDiscountDepositForCBA(TreasuryDealsModel transMaster)
        {
            string url = ConfigurationManager.AppSettings["TerminateDiscountedDepositFlow"];
            TerminateDiscountedDeposit terminateDiscountedDeposit = new TerminateDiscountedDeposit();
            terminateDiscountedDeposit.TerminateLd = new TerminateLd();
            terminateDiscountedDeposit.TerminateLd.CompCode = transMaster.BranchCode;
            terminateDiscountedDeposit.TerminateLd.Date = ConvertToCBADate(transMaster.PreLiquidation == null ? DateTime.Now
                                                        : transMaster.PreLiquidation.LiquidationDate);
            terminateDiscountedDeposit.TerminateLd.DepositId = transMaster.DealId;
            return TreasuryCbaService.TerminateDiscountedDepositFlow(url, terminateDiscountedDeposit, transMaster.access_token);
        }

        private DealOutPutResponse PartiallyPreLiquidateDiscountDepositForCBA(TreasuryDealsModel transMaster)
        {
            string url = ConfigurationManager.AppSettings["PartialWithdrawalFlow"];
            PartialWithdrawalFlowRequestModel partialWithdrawalFlowRequestModel = new PartialWithdrawalFlowRequestModel();
            partialWithdrawalFlowRequestModel.PartialWithdrawal = new PartialWithdrawal();
            partialWithdrawalFlowRequestModel.PartialWithdrawal.Amount = Math.Round(Convert.ToDouble(transMaster.PreLiquidatedAmount), 2) + "";
            partialWithdrawalFlowRequestModel.PartialWithdrawal.LdId = transMaster.DealId;
            partialWithdrawalFlowRequestModel.PartialWithdrawal.TransactionBranch = transMaster.BranchCode;
            partialWithdrawalFlowRequestModel.PartialWithdrawal.WithdrawalDate = ConvertToCBADate(transMaster.PreLiquidation == null ? DateTime.Now
                                                        : transMaster.PreLiquidation.LiquidationDate);
            return TreasuryCbaService.PartialWithdrawalFlow(url, partialWithdrawalFlowRequestModel, transMaster.access_token);
        }

        private DealOutPutResponse CreateDiscountDepositForCBA(TreasuryDealsModel transMaster)
        {
            bool IsProductTypeTenure = _treasuryService.ProductTypeIsTenured(transMaster.ProductCode);
            transMaster.CurrencyAbbrev = _treasuryService.GetCurrencyAbbrev(transMaster.CurrencyCode);
            var productCode = _treasuryService.GetDiscountTreasuryProduct();
            string url = ConfigurationManager.AppSettings["CreateDiscountedDepositFlow"];
            DiscountedDepositFlow discountedDepositFlow = new DiscountedDepositFlow();
            discountedDepositFlow.Request = new DiscountedDepositFlowRequest();
            discountedDepositFlow.Request.Amount = Math.Round(Convert.ToDouble(transMaster.PrincipalAmount), 2) + "";
            discountedDepositFlow.Request.BusinessDayDef = "NG";
            discountedDepositFlow.Request.Category = "21015";
            discountedDepositFlow.Request.ChargLiqAcct = transMaster.WHTAccount;
            discountedDepositFlow.Request.CompCode = transMaster.BranchCode;
            discountedDepositFlow.Request.Currency = transMaster.CurrencyAbbrev;
            discountedDepositFlow.Request.CustId = transMaster.CustomerId;
            discountedDepositFlow.Request.DiscountValue = Math.Round(Convert.ToDouble(transMaster.PaymentAmount), 2) + "";
            discountedDepositFlow.Request.DrawnDownAcct = transMaster.InflowAccount;
            discountedDepositFlow.Request.FinMatDate = ConvertToCBADate(transMaster.MaturityDate);
            discountedDepositFlow.Request.InterestBasis = "C";
            discountedDepositFlow.Request.InterestRate = transMaster.TreasuryInterest.FirstOrDefault().InterestRate + "";
            discountedDepositFlow.Request.InterestRateKey = "";
            discountedDepositFlow.Request.IntLiqAcct = transMaster.InterestAccount;
            discountedDepositFlow.Request.LDAcctOfficer = transMaster.AccountOfficer;
            discountedDepositFlow.Request.MatureAtStartOfDay = "YES";
            discountedDepositFlow.Request.PrinLiqAcct = transMaster.PrincipalAccount;
            discountedDepositFlow.Request.ValueDate = ConvertToCBADate(transMaster.ValueDate);
            return TreasuryCbaService.CreateDiscountedDepositFlow(url, discountedDepositFlow, transMaster.access_token);
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
