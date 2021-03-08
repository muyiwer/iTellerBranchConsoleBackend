using iTellerBranch.BankService;
using iTellerBranch.Business.Transaction;
using iTellerBranch.Model;
using iTellerBranch.Model.ViewModel;
using iTellerBranch.Repository;
using iTellerBranch.Repository.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Web;
using System.Web.Http;
using static iTellerBranch.Model.ViewModel.IMALRequestModel;
using TransMaster = iTellerBranch.Repository.TransMaster;

namespace iTellerBranch.Controllers
{
    public class TransactionController : ApiController
    {
        private readonly TransactionBusiness _transactionBusiness;
        private readonly TransactionService _transactionService;
        private readonly TillTransferService _tillTransferService;

        private static string isDemo = System.Configuration.ConfigurationManager.AppSettings["isDemo"];
        public TransactionController()
        {
            _transactionBusiness = new TransactionBusiness();
            _tillTransferService = new TillTransferService();
            _transactionService = new TransactionService();
        }


        [HttpGet, Route("api/CashDeposit")]
        public IHttpActionResult GetDepositTrans()
        {
            Utils.LogNO("api/CashDeposit:1:Currently Inside This End Point.");
            var result = _transactionBusiness.GetTransactionDeposit(true, "");
            return Ok(result);
        }

        [HttpGet, Route("api/Branch/GetBranchAccounts/{branchCode}")]
        public IHttpActionResult GetBranchAccounts(string branchCode) 
        {
            Utils.LogNO("api/Branch/GetBranchAccounts/{branchCode}:2:Currently Inside This End Point.");
            var result = _transactionService.BranchAccounts(branchCode);
            return Ok(result);
        }

        //[HttpPost, Route("api/Create/CashDeposit")]
        //public IHttpActionResult PostCashDeposit([FromBody]TransMaster transMaster)
        //{
        //    try
        //    {
        //        var MachineName = DetermineCompName(GetIPAddress());
        //        transMaster.MachineName = MachineName;
        //        var result = _transactionBusiness.CreateTransactionDeposit(transMaster);

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        var result = _transactionBusiness.GetWithdrawalTrans(false, ex.Message, ex);
        //        return Ok(result);
        //    }


        //}


        [HttpPost, Route("api/ImalTransaction/Create/CashTrans")]
        public IHttpActionResult CreateImalDepositWithdrawalTrans([FromBody]TransactionModel transMaster)
        {
            Utils.LogNO("api/ImalTransaction/Create/CashTrans:3:Currently Inside This End Point.");
            try
            {
                transMaster.CBA = "IMAL";
                transMaster.Status = 2;
                var transType = (int)TransType.TransactionType.Deposit;
                transMaster.TransType = transType.ToString();
                object result = null;
                if (isDemo == "false")
                {
                    var currencyCode = System.Configuration.ConfigurationManager.AppSettings["CurrencyCode"];
                    string url = null;
                    OutputResponse response = null;
                    APIRequest request = new APIRequest();
                    request.access_token = transMaster.access_token;
                    var narration = _transactionBusiness.BuildNarration(transMaster.ChequeNo,
                        transMaster.Beneficiary, "", transMaster.Narration, transMaster.TransName, 3, transMaster.TransType);
                    transMaster.TellerId = transMaster.BranchCode + currencyCode +
                    System.Configuration.ConfigurationManager.AppSettings["SMLedgerCode"] + transMaster.SMCIFNumber + "000";
                    url = System.Configuration.ConfigurationManager.AppSettings["ImalLocalFT"];
                    LocalFT accountDetailsRequestModel = new LocalFT();
                    accountDetailsRequestModel.fromAccount = transMaster.TellerId; 
                    accountDetailsRequestModel.paymentReference = transMaster.Remark;//ook
                    accountDetailsRequestModel.principalIdentifier = System.Configuration.ConfigurationManager.AppSettings["principalIdentifier"];
                    accountDetailsRequestModel.referenceCode = System.Configuration.ConfigurationManager.AppSettings["referenceCode"];
                    accountDetailsRequestModel.requestCode = System.Configuration.ConfigurationManager.AppSettings["requestCode"];
                    accountDetailsRequestModel.toAccount = transMaster.CustomerAcctNos;
                    accountDetailsRequestModel.amount = Math.Round(transMaster.TotalAmt, 2) + "";
                    accountDetailsRequestModel.beneficiaryName = transMaster.Beneficiary;
                    var responseImal = ImalService.LocalFT(url, accountDetailsRequestModel, transMaster.access_token);
                    try
                    {
                        transMaster.MachineName = DetermineCompName(GetIPAddress());
                        var IPAddress = GetIPAddress();
                        General.AuditLog("Posting/Creating transaction on IMAL", transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                    }
                    catch { }
                    if (responseImal.responseCode == "-1214")
                    {
                        result = new
                        {
                            success = false,
                            message = "Amount Exceeds Available Balance in Account"
                          //  TransactionRef = responseImal.transactionID
                        };
                        return Ok(result);
                    }else if (responseImal.responseCode == "0")
                    {
                        transMaster.CBA = "IMAL";
                        transMaster.TransRef = responseImal.iMALTransactionCode;
                        transMaster.CBAResponse = responseImal.responseMessage;
                        transMaster.CBACode = responseImal.responseCode;
                        transMaster.Posted = true;
                        transMaster.MachineName = "";
                        transMaster.Approved = true;
                        transMaster.ApprovedBy = transMaster.TransName;
                        transMaster.Status = 2;
                        transMaster.BranchCode = transMaster.Branch;
                        transMaster.Remarks = BuildNarration("", transMaster.Beneficiary, "", transMaster.Narration, transMaster.Beneficiary, 2);
                        result = _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,3);
                        try
                        {
                            var IPAddress = GetIPAddress();
                            General.AuditLog("Posting/Creating transaction on IMAL", transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                        }
                        catch { }

                        return Ok(result);
                    }
                    else
                    {
                        result = new
                        {
                            success = false,
                            message = response.ResponseText
                           // TransactionRef = responseImal.transactionID
                        };
                        return Ok(result);
                    }
                }
                else
                {
                    //var MachineName = DetermineCompName(GetIPAddress());
                    transMaster.CBA = "IMAL";
                    transMaster.MachineName = "";
                    transMaster.Approved = true;
                    transMaster.Status = 2;
                    transMaster.TransRef = Guid.NewGuid().ToString();
                    result = _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,3);
                }


                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _transactionBusiness.GetAllTrans(false, ex.Message, ex);
                return Ok(result);
            }


        }


        [HttpPost, Route("api/Create/CashTrans")]
        public IHttpActionResult CreateDepositWithdrawalTrans([FromBody]TransactionModel transMaster)
        {
            Utils.LogNO("api/Create/CashTrans:4:Currently Inside This End Point.");
            try
            {
                transMaster.CBA = "T24";
                var transType = (int)TransType.TransactionType.Deposit;
                transMaster.TransType = transType.ToString();
                object result = null;
                try
                {
                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Posting/Creating transaction on T24", transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }
                if (isDemo == "false")
                {
                    OutputResponse response = CreateCBADeposit(transMaster);
                    if (response.ResponseCode == "-1")
                    {
                        result = new
                        {
                            success = false,
                            message = "Invalid teller ID or branch for company",
                            TransactionRef = ""
                        };
                        return Ok(result);
                    }

                    if (response != null)
                    {
                        //var MachineName = DetermineCompName(GetIPAddress());
                        transMaster.MachineName = "";
                        transMaster.TransRef = response.ResponseId;
                        transMaster.TransRef = response.ResponseId;
                        transMaster.CBAResponse = response.ResponseText;
                        transMaster.CBACode = response.ResponseCode;
                        transMaster.Approved = true;
                        transMaster.ApprovedBy = transMaster.TransName;
                        transMaster.Posted = true;
                        transMaster.Status = 2;
                        result = _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,5);

                        return Ok(result);
                    }
                    else
                    {
                        result = new
                        {
                            success = false,
                            message = "transaction not successfull.",
                            TransactionRef = result
                        };
                        return Ok(result);
                    }
                }
                else
                {
                    //var MachineName = DetermineCompName(GetIPAddress());
                    transMaster.MachineName = "WorkLoad";
                    transMaster.TransRef = Guid.NewGuid().ToString();
                    transMaster.Approved = true;
                    transMaster.Status = 2;
                    result = _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,5);
                }


                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _transactionBusiness.GetAllTrans(false, ex.Message, ex);
                return Ok(result);
            }


        }


        private OutputResponse CreateCBADeposit(TransactionModel transMaster)
        {
            string url = null;
            OutputResponse response = null;
            TellerRequest request = new TellerRequest();
            request.Teller = new Teller();
            request.Teller.access_token = transMaster.access_token;
  
            var narration = transMaster.Remark;
            if (transMaster.CurrCode != "NGN")
            {
                narration = _transactionBusiness.BuildNarration(transMaster.ChequeNo,
                transMaster.Beneficiary, "", transMaster.Narration, transMaster.TransName, 6);
            }
            else
            {
                narration = _transactionBusiness.BuildNarration(transMaster.ChequeNo,
               transMaster.Beneficiary, "", transMaster.Narration, transMaster.TransName, 4);
            }
            request.Teller.Amt = String.Format("{0:0.00}", transMaster.Amount); 
            request.Teller.InitiatorName = transMaster.InitiatorName;
            request.Teller.CustAcct = transMaster.CustomerAcctNos;
            request.Teller.Narration = narration.Length <= 34 ? narration : narration.Substring(0, 34);
            request.Teller.TellerId = transMaster.TellerId;
            request.Teller.TransactionBranch = transMaster.Branch;//"NG0020006";
            request.Teller.TxnCurr = transMaster.CurrCode;
            request.Teller.TransferParty = transMaster.TransactionParty;//"A";
            request.Teller.Rate = "";
            if (transMaster.CurrCode == "NGN")
            {
                Utils.LogNO("CBA DEPOSIT - NGN");
                Utils.LogNO("Details " + request.Teller.TxnCurr + " " + request.Teller.TellerId + " " +
                    request.Teller.CustAcct + " ");
                url = System.Configuration.ConfigurationManager.AppSettings["CashDepositLCY"];
                response = Transaction.CashDepLcy(url, request);
            }
            else
            {
                Utils.LogNO("CBA DEPOSIT - FCY");
                url = System.Configuration.ConfigurationManager.AppSettings["CashDepositForeign"];
                response = Transaction.CashDepositForeign(url, request);
            }

            return response;
        }

        [HttpGet, Route("api/getCashTrans")]
        public IHttpActionResult GetAllTrans()
        {
            Utils.LogNO("api/getCashTrans:5:Currently Inside This End Point.");
            var result = _transactionBusiness.GetAllTrans(true, "");
            return Ok(result);
        }

        [HttpGet, Route("api/getUnApprovedTrans")]
        public IHttpActionResult GetUnApprovedTrans()
        {
            Utils.LogNO("api/getUnApprovedTrans:6:Currently Inside This End Point.");
            var result = _transactionService.GetUnApprovedTrans(true, ""); 
            return Ok(result);
        }

        [HttpGet, Route("api/getInHouseTransferTransaction")]
        public IHttpActionResult GetInHouseTransferTransaction() 
        {
            Utils.LogNO("api/getInHouseTransferTransaction:7:Currently Inside This End Point.");
            var result = _transactionService.GetInHouseTransferTransaction(true, ""); 
            return Ok(result);
        }

        [HttpGet, Route("api/getProductDetails")]
        public IHttpActionResult GetProductCodeDetails()
        {
            Utils.LogNO("api/getProductDetails:8:Currently Inside This End Point.");
            var result = _transactionService.GetTreasuryProductDetails(true, "");
            return Ok(result);
        }

        [HttpGet, Route("api/getTerminationInstruction")]
        public IHttpActionResult GetAllTerminationInstructions()
        {
            Utils.LogNO("api/getTerminationInstruction:9:Currently Inside This End Point.");
            var result = _transactionService.GetTerminationInstruction(true, "");
            return Ok(result);
        }


        [HttpPost, Route("api/getTransactionFile")]
        public IHttpActionResult GetTransacctionFile([FromBody] TransactionFiles files)
        {
            Utils.LogNO("api/getTransactionFile:10:Currently Inside This End Point.");
            var result = _transactionService.GetTransactionFile(files);
            if(result == null)
            {
                return Ok(new
                {
                    success = false,
                    details = result
                });
            }
            return Ok(new
            {
                success = true,
                result.FileName
            });
        }

        [HttpGet, Route("api/getTreasuryDeals")]
        public IHttpActionResult GetAllTreasuryDeals()
        {

            var result = _transactionBusiness.GetAllTreasuryTrans(true, "");
            return Ok(result);
        }

        [HttpGet, Route("api/getTreasuryTrans/{TranId}")]
        public IHttpActionResult GetAllTreasuryDealsById([FromUri] int TranId)
        {
            Utils.LogNO("api/getTreasuryTrans/{TranId}:11:Currently Inside This End Point.");
            var result = _transactionBusiness.GetAllTreasuryTransById(TranId);
            return Ok(result);

        }

        [HttpGet, Route("api/CashWithdrawal")]
        public IHttpActionResult GetWithdrawalTrans()
        {
            Utils.LogNO("api/CashWithdrawal:12:Currently Inside This End Point.");
            var result = _transactionBusiness.GetWithdrawalTrans(true, "");
            return Ok(result);
        }


        [HttpPost, Route("api/ImalTransaction/Create/CashWithdrawal")]
        public IHttpActionResult PostImalCash([FromBody]TransactionModel transMaster)
        {
            Utils.LogNO("api/ImalTransaction/Create/CashWithdrawal:13:Currently Inside This End Point.");
            try
            {
                transMaster.CBA = "IMAL";
                object result = null;
                string url = null;
                if (transMaster.ChequeNo.Length > 0 )
                {
                    url = System.Configuration.ConfigurationManager.AppSettings["CashWithdrawalWithCheq"];
                    var transType = (int)TransType.TransactionType.ChequeLodgement;
                    transMaster.TransType = transType.ToString();
                }
                else
                {
                    url = System.Configuration.ConfigurationManager.AppSettings["CashWithDrawal"];
                    var transType = (int)TransType.TransactionType.Withdrawal;
                    transMaster.TransType = transType.ToString();
                }

                if (isDemo == "false")
                {
                    var currencyCode = System.Configuration.ConfigurationManager.AppSettings["CurrencyCode"];
                    OutputResponse response = null;
                    APIRequest request = new APIRequest();
                    request.access_token = transMaster.access_token;
                    var narration = _transactionBusiness.BuildNarration(transMaster.ChequeNo,
                        transMaster.Beneficiary, "", transMaster.Narration, transMaster.TransName, 3, transMaster.TransType);
                    transMaster.TellerId = transMaster.BranchCode + currencyCode +
                    System.Configuration.ConfigurationManager.AppSettings["SMLedgerCode"] + transMaster.SMCIFNumber + "000";
                    url = System.Configuration.ConfigurationManager.AppSettings["ImalLocalFT"];
                    LocalFT accountDetailsRequestModel = new LocalFT();
                    accountDetailsRequestModel.fromAccount = transMaster.CustomerAcctNos;
                    accountDetailsRequestModel.paymentReference = transMaster.Remark;//ook
                    accountDetailsRequestModel.principalIdentifier = System.Configuration.ConfigurationManager.AppSettings["principalIdentifier"];
                    accountDetailsRequestModel.referenceCode = System.Configuration.ConfigurationManager.AppSettings["referenceCode"];
                    accountDetailsRequestModel.requestCode = System.Configuration.ConfigurationManager.AppSettings["requestCode"];
                    accountDetailsRequestModel.toAccount = transMaster.TellerId;
                    accountDetailsRequestModel.amount = transMaster.Amount + "";
                    accountDetailsRequestModel.beneficiaryName = "";
                    var imalResponse = ImalService.LocalFT(url, accountDetailsRequestModel, transMaster.access_token);
                    Utils.LogNO("Executed CBA: " + JsonConvert.SerializeObject(imalResponse));
                    try
                    {
                        transMaster.MachineName = DetermineCompName(GetIPAddress());
                        var IPAddress = GetIPAddress();
                        General.AuditLog("Posting/Creating transaction on IMAL", transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                    }
                    catch { }
                    if(imalResponse == null)
                    {
                        result = new
                        {
                            success = false,
                            message = "Service Error from CBA. Please contact the admin",
                            TransactionRef = ""
                        };
                        return Ok(result);
                    }
                    if (imalResponse.responseCode == "0")
                    {
                        Utils.LogNO("Executed CBA successfully");
                        //var MachineName = DetermineCompName(GetIPAddress()); go on //System.Configuration.ConfigurationManager.AppSettings["principalIdentifier"];System.Configuration.ConfigurationManager.AppSettings["referenceCode"]; // ask sterlingSystem.Configuration.ConfigurationManager.AppSettings["requestCode"]; // ask sterling
                        transMaster.TransRef = imalResponse.iMALTransactionCode;
                        transMaster.MachineName = "";
                       // transMaster.TransRef = response.ResponseId;
                        transMaster.CBAResponse = imalResponse.responseMessage;
                        transMaster.CBACode = imalResponse.responseCode;
                        transMaster.Posted = true;
                        transMaster.Status = 2;
                        transMaster.Approved = true;
                        transMaster.BranchCode = transMaster.Branch;
                        transMaster.Remarks = BuildNarration("",transMaster.Beneficiary,"",transMaster.Remark,transMaster.Beneficiary,3);
                        Utils.LogNO("TransMaster" + JsonConvert.SerializeObject(transMaster));
                        result = _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,3);
                        result = new
                        {
                            success = true,
                            message = "Transaction successful",
                            TransactionRef = imalResponse.iMALTransactionCode
                        };
                        return Ok(result);
                    }
                    else if (imalResponse.responseCode == "-1214")
                    {
                        result = new
                        {
                            success = false,
                            message = "Amount Exceeds Available Balance in Account. Kindly check tellers' available balance",
                            TransactionRef = ""
                        };
                        return Ok(result);
                    }
                    else
                    {
                        result = new
                        {
                            success = false,
                            message = response.ResponseText,
                            TransactionRef = ""
                        };
                        return Ok(result);
                    }

                }
                else
                {
                    var MachineName = DetermineCompName(GetIPAddress());
                    transMaster.MachineName = MachineName;
                    transMaster.TransRef = Guid.NewGuid().ToString();
                    transMaster.Approved = true; transMaster.Approved = true;
                    transMaster.Status = 2;
                    result = _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,3);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _transactionBusiness.GetWithdrawalTrans(false, ex.Message, ex);
                return Ok(result);
            }


        }

        public string BuildNarration(string SerialNo, string Beneficiary, string transRef, string remarks, string depositor, int status, string transType="") //ok lets factor what MD said as per narration here
        {
            string narration = string.Empty;
            Utils.LogNO("Building Narration inside Controller. TransRef:"+ transRef+", serialNo:"+ SerialNo+", status:"+ status+", transtype:"+ transType);
            //if (status == 1)//cheque withdrawal
            //    narration = @" " + transRef + @" CASH WTD B/O " + @" " + Beneficiary + @" CHQ" + @" " +
            //                SerialNo + @" " + remarks;
            //if (status == 2)//cheque deposit
            //    narration = @" " + transRef + @" CASH DEPOSIT B/O " + @" " + depositor + @" CHQ" + @" " +
            //            SerialNo + @" " + remarks;
            //if (status == 3)//Pure cash withdrawal
            //    narration = @" " + transRef + @" CASH WTD B/O " + @" " + depositor + @" " + remarks;
            //if (status == 4)//Pure cash deposit
            //    narration = @" " + transRef + @" CASH DEPOSIT B/O " + @" " + depositor + @" " + remarks;

            //string narration = string.Empty;
            if (status == 1)//cheque withdrawal
                narration = @" " + transRef + @" CASH WTD B/O " + @" " + Beneficiary + @" CASH" + @" " +
                            SerialNo + @" " + remarks;
            if (status == 2)//cheque deposit
                narration = transRef + @"CHEQUE DEPOSIT B/O " + depositor + @" CHQ" + @" " +
                        SerialNo + @" " + remarks;
            if (status == 3)//Pure cash withdrawal
                narration = @" " + transRef + @" CASH WTD B/O " + @" " + depositor + @" " + remarks;
            if (status == 4)//Pure cash deposit
                narration = @" " + transRef + @" CASH DEPOSIT B/O " + @" " + depositor + @" " + remarks;
            if (status == 5)//Pure cash deposit
                narration = @" " + transRef + @" CASH DEPOSIT B/O " + @" " + depositor + @" " + remarks;

            if (status == 6)//Pure cash deposit
                narration = @" " + transRef + @" FCY DEPOSIT B/O " + @" " + depositor + @" " + remarks;

            if ((status == 3 && transType == "13") && (status==3 && transType=="3"))
            {
                narration = @" " + transRef + @" CASH WTD CHQ B/O" + @" " + depositor + @" " + remarks;
            }


            Utils.LogNO("naration " + narration);
            return narration;
        }


        [HttpPost, Route("api/ImalTransaction/Approve/CashTrans")]
        public IHttpActionResult PostApproveImalCash([FromBody]TransactionModel transMaster)
        {
            Utils.LogNO("api/ImalTransaction/Approve/CashTrans:14:Currently Inside This End Point.");
            try
            {
                transMaster.CBA = "IMAL";
                object result = null; 
                string url = null;
                if (isDemo == "false")
                {
                    var currencyCode = System.Configuration.ConfigurationManager.AppSettings["CurrencyCode"];
                    OutputResponse response = null;
                    APIRequest request = new APIRequest();
                    request.access_token = transMaster.access_token;
                    var narration = _transactionBusiness.BuildNarration(transMaster.ChequeNo,
                        transMaster.Beneficiary, "", transMaster.Narration, transMaster.TransName, 3, transMaster.TransType);
                    //transMaster.TellerId = transMaster.BranchCode + currencyCode +
                    //System.Configuration.ConfigurationManager.AppSettings["SMLedgerCode"] + transMaster.SMCIFNumber + "000";
                    url = System.Configuration.ConfigurationManager.AppSettings["ImalLocalFT"];
                    LocalFT accountDetailsRequestModel = new LocalFT();
                    if (Convert.ToInt32(transMaster.TransType ) != 1)
                    {
                        accountDetailsRequestModel.fromAccount = transMaster.TellerId;
                        accountDetailsRequestModel.paymentReference = transMaster.Remarks;//ook
                        accountDetailsRequestModel.principalIdentifier = System.Configuration.ConfigurationManager.AppSettings["principalIdentifier"];
                        accountDetailsRequestModel.referenceCode = System.Configuration.ConfigurationManager.AppSettings["referenceCode"];
                        accountDetailsRequestModel.requestCode = System.Configuration.ConfigurationManager.AppSettings["requestCode"];
                        accountDetailsRequestModel.toAccount = transMaster.AccountNo; // transMaster.TellerId;
                        accountDetailsRequestModel.amount = Math.Round(transMaster.TotalAmount, 2) + "";
                        accountDetailsRequestModel.beneficiaryName = "";
                    }
                    else
                    {
                        accountDetailsRequestModel.fromAccount = transMaster.AccountNo;
                        accountDetailsRequestModel.paymentReference = transMaster.Remarks;//ook
                        accountDetailsRequestModel.principalIdentifier = System.Configuration.ConfigurationManager.AppSettings["principalIdentifier"];
                        accountDetailsRequestModel.referenceCode = System.Configuration.ConfigurationManager.AppSettings["referenceCode"];
                        accountDetailsRequestModel.requestCode = System.Configuration.ConfigurationManager.AppSettings["requestCode"];
                        accountDetailsRequestModel.toAccount = transMaster.TellerId;
                        accountDetailsRequestModel.amount = Math.Round(transMaster.TotalAmount, 2) + "";
                        accountDetailsRequestModel.beneficiaryName = transMaster.Beneficiary;
                    }
                    
                    var imalResponse = ImalService.LocalFT(url, accountDetailsRequestModel, transMaster.access_token);
                    Utils.LogNO("Executed CBA: " + JsonConvert.SerializeObject(imalResponse));
                    try
                    {
                        transMaster.MachineName = DetermineCompName(GetIPAddress());
                        var IPAddress = GetIPAddress();
                        General.AuditLog("Posting/Creating transaction on IMAL", transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                    }
                    catch { }
                    if (imalResponse.responseCode == "0")
                    {
                        Utils.LogNO("Executed CBA successfully");
                        Utils.LogNO("TransMaster" + JsonConvert.SerializeObject(transMaster));
                        TransactionApprovalModel tresponse = new TransactionApprovalModel();
                        tresponse.TReference = imalResponse.iMALTransactionCode;
                        tresponse.TranId = transMaster.TranId;
                        tresponse.ApprovedBy = transMaster.ApprovedBy;
                        result = _transactionBusiness.ApproveImalTransaction(tresponse, imalResponse.iMALTransactionCode);
                        return Ok(result);
                    }
                    else if (imalResponse.responseCode == "-1214")
                    {
                        result = new
                        {
                            success = false,
                            message = "Amount Exceeds Available Balance in Account",
                            TransactionRef = ""
                        };
                        return Ok(result);
                    }
                    else
                    {
                        result = new
                        {
                            success = false,
                            message = response.ResponseText,
                            TransactionRef = ""
                        };
                        return Ok(result);
                    }

                }
                else
                {
                    Utils.LogNO("Executed CBA successfully");
                    Utils.LogNO("TransMaster" + JsonConvert.SerializeObject(transMaster));
                    TransactionApprovalModel tresponse = new TransactionApprovalModel();
                    tresponse.TReference = Guid.NewGuid().ToString();
                    tresponse.TranId = transMaster.TranId;
                    tresponse.ApprovedBy = transMaster.ApprovedBy;
                    result = _transactionBusiness.ApproveTransaction(tresponse, tresponse.TReference);
                    return Ok(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _transactionBusiness.GetWithdrawalTrans(false, ex.Message, ex);
                return Ok(result);
            }


        }

        [HttpPost, Route("api/Approve/ImalTransaction/TillTransfer")]
        public IHttpActionResult ApproveImalTillTransfer([FromBody]TransactionModel transMaster)
        {
            Utils.LogNO("api/Approve/ImalTransaction/TillTransfer:15:Currently Inside This End Point.");
            try
            {
                transMaster.CBA = "IMAL";
                object result = null;
                string url = null;
                if (isDemo == "false")
                {
                    
                    url = System.Configuration.ConfigurationManager.AppSettings["ImalLocalFT"];
                    LocalFT accountDetailsRequestModel = new LocalFT();
                    accountDetailsRequestModel.fromAccount = transMaster.TellerId;
                    accountDetailsRequestModel.amount = Math.Round(Convert.ToDouble(transMaster.Amount) ,2) + "" ;
                    accountDetailsRequestModel.beneficiaryName = "";
                    accountDetailsRequestModel.paymentReference = transMaster.Remark;//ook
                    accountDetailsRequestModel.principalIdentifier = System.Configuration.ConfigurationManager.AppSettings["principalIdentifier"];
                    accountDetailsRequestModel.referenceCode = System.Configuration.ConfigurationManager.AppSettings["referenceCode"];
                    accountDetailsRequestModel.requestCode = System.Configuration.ConfigurationManager.AppSettings["requestCode"];
                    accountDetailsRequestModel.toAccount = transMaster.ToTellerId;
                    var imalResponse = ImalService.LocalFT(url, accountDetailsRequestModel, transMaster.access_token);
                    //var MachineName = DetermineCompName(GetIPAddress()); go on //System.Configuration.ConfigurationManager.AppSettings["principalIdentifier"];System.Configuration.ConfigurationManager.AppSettings["referenceCode"]; // ask sterlingSystem.Configuration.ConfigurationManager.AppSettings["requestCode"]; // ask sterling
                    try
                    {
                        transMaster.MachineName = DetermineCompName(GetIPAddress());
                        var IPAddress = GetIPAddress();
                        General.AuditLog("Posting/Creating Till Transfer on IMAL", transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                    }
                    catch { }
                    if (imalResponse.responseCode == "0")
                    {
                        var transType = (int)TransType.TransactionType.TillTransfer;
                        transMaster.TransType = transType.ToString();
                        transMaster.CBAResponse = imalResponse.responseMessage;
                        transMaster.CBACode = imalResponse.responseCode;
                        transMaster.Posted = true;
                        transMaster.CBA = "IMAL";
                        transMaster.NeededApproval = false;
                        transMaster.TransRef = imalResponse.iMALTransactionCode;
                        transMaster.BranchCode = transMaster.Branch;
                        Utils.LogNO("Saving imal details to local db");
                        transMaster.Approved = true;
                        result = _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,3);
                        var response = _tillTransferService.AcceptImalTillTransfer(transMaster.TillTransferID);

                        result = new
                        {
                            success = true,
                            message = "Transaction successful",
                            TransactionRef = imalResponse.iMALTransactionCode
                        };
                        return Ok(result);
                    }
                    else if (imalResponse.responseCode == "-1214")
                    {
                        result = new
                        {
                            success = false,
                            message = "Amount Exceeds the Available Balance in the Account!",
                            TransactionRef = ""
                        };
                        return Ok(result);
                    }

                }
                else
                {
                    //var MachineName = DetermineCompName(GetIPAddress());
                    // transMaster.MachineName = MachineName;
                    transMaster.CBAResponse = "";
                    transMaster.CBACode = "";
                    transMaster.Posted = true;
                    transMaster.Approved = true;
                    transMaster.TransRef = Guid.NewGuid().ToString();
                    var transType = (int)TransType.TransactionType.TillTransfer;
                    transMaster.TransType = transType.ToString();
                    result = _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,3);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _transactionBusiness.GetWithdrawalTrans(false, ex.Message, ex);
                return Ok(result);
            }

        }


        [HttpPost, Route("api/ImalTransaction/TillTransfer")]
        public IHttpActionResult PostImalTillTransfer([FromBody]TransactionModel transMaster)
        {
            Utils.LogNO("api/ImalTransaction/TillTransfer:16:Currently Inside This End Point.");
            try
            {
                transMaster.CBA = "IMAL";
                object result = null;
                string url = null;
                if (isDemo == "false")
                {
                    var currencyCode = System.Configuration.ConfigurationManager.AppSettings["CurrencyCode"];
                    transMaster.TellerId = transMaster.BranchCode + currencyCode +
                        System.Configuration.ConfigurationManager.AppSettings["SMLedgerCode"] + transMaster.TellerId +
                        "000";
                    transMaster.ToTellerId = transMaster.BranchCode + currencyCode +
                        System.Configuration.ConfigurationManager.AppSettings["SMLedgerCode"] + transMaster.ToTellerId +
                        "000";
                    url = System.Configuration.ConfigurationManager.AppSettings["ImalLocalFT"];
                    LocalFT accountDetailsRequestModel = new LocalFT();
                    accountDetailsRequestModel.fromAccount = transMaster.TellerId;
                    accountDetailsRequestModel.amount = transMaster.Amount + "";
                    accountDetailsRequestModel.beneficiaryName = "";
                    accountDetailsRequestModel.paymentReference = transMaster.Remark;//ook
                    accountDetailsRequestModel.principalIdentifier = System.Configuration.ConfigurationManager.AppSettings["principalIdentifier"];
                    accountDetailsRequestModel.referenceCode = System.Configuration.ConfigurationManager.AppSettings["referenceCode"];
                    accountDetailsRequestModel.requestCode = System.Configuration.ConfigurationManager.AppSettings["requestCode"];
                    accountDetailsRequestModel.toAccount = transMaster.ToTellerId;
                    var imalResponse = ImalService.LocalFT(url, accountDetailsRequestModel, transMaster.access_token);
                    //var MachineName = DetermineCompName(GetIPAddress()); go on //System.Configuration.ConfigurationManager.AppSettings["principalIdentifier"];System.Configuration.ConfigurationManager.AppSettings["referenceCode"]; // ask sterlingSystem.Configuration.ConfigurationManager.AppSettings["requestCode"]; // ask sterling
                    try
                    {
                        transMaster.MachineName = DetermineCompName(GetIPAddress());
                        var IPAddress = GetIPAddress();
                        General.AuditLog("Posting/Creating Till Transfer on IMAL", transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                    }
                    catch { }
                    if (imalResponse.responseCode == "0")
                    {
                        var transType = (int)TransType.TransactionType.TillTransfer;
                        transMaster.TransType = transType.ToString();
                        transMaster.CBAResponse = imalResponse.responseMessage;
                        transMaster.CBACode = imalResponse.responseCode;
                        transMaster.Posted = true;
                        transMaster.CBA = "IMAL";
                        transMaster.NeededApproval = false;
                        transMaster.BranchCode = transMaster.Branch;
                        Utils.LogNO("Saving imal details to local db");
                        transMaster.Approved = true;
                        result = _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,3);
                        result = new
                        {
                            success = true,
                            message = "Transaction successful",
                            TransactionRef = imalResponse.iMALTransactionCode
                        };
                        return Ok(result);
                    }
                    else if (imalResponse.responseCode == "-1214")
                    {
                        result = new
                        {
                            success = false,
                            message = "Amount Exceeds the Available Balance in the Account!",
                            TransactionRef = ""
                        };
                        return Ok(result);
                    }

                }
                else
                {
                    //var MachineName = DetermineCompName(GetIPAddress());
                    // transMaster.MachineName = MachineName;
                    transMaster.CBAResponse = "";
                    transMaster.CBACode = "";
                    transMaster.Posted = true;
                    transMaster.Approved = true;
                    transMaster.TransRef = Guid.NewGuid().ToString();
                    var transType = (int)TransType.TransactionType.TillTransfer;
                    transMaster.TransType = transType.ToString();
                    result = _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,3);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _transactionBusiness.GetWithdrawalTrans(false, ex.Message, ex);
                return Ok(result);
            }

        }


        [HttpPost, Route("api/Create/CashWithdrawal")]
        public IHttpActionResult PostCash([FromBody]TransactionModel transMaster)
        {
            Utils.LogNO("api/Create/CashWithdrawal:17:Currently Inside This End Point.");
            try
            {
                Utils.LogNO("Type A- cashwithdrawal with or without cheque HERE...... TransType:" + transMaster.TransType+", ChargeType: "+ transMaster.ChargeType+", CheNo:"+ transMaster.ChequeNo);
                transMaster.Approved = true;
                transMaster.CBA = "T24";
                object result = null;
                string url = null;
                Utils.LogNO("Post Cash Started");
                if (transMaster.ChequeNo.Length == 8)
                {
                    if (transMaster.TransType == "Counter")
                    {
                        url = System.Configuration.ConfigurationManager.AppSettings["CashWithdrawalWithCounterCheque"];
                        var ctransType = (int)TransType.TransactionType.CashWithDrawalCounter;
                        transMaster.TransType = ctransType.ToString();
                    }
                    else
                    {
                        url = System.Configuration.ConfigurationManager.AppSettings["CashWithdrawalWithCheq"];
                        var transType = (int)TransType.TransactionType.ChequeLodgement;
                        transMaster.TransType = transType.ToString();
                    }
                    if (transMaster.TransType == "3")
                    {
                        Utils.LogNO("cashwithdrawal with cheque. TransType:"+ transMaster.TransType);
                        var transType = (int)TransType.TransactionType.ChequeLodgement;
                        transMaster.TransType = transType.ToString();
                    }
                }
                else
                {
                    url = System.Configuration.ConfigurationManager.AppSettings["CashWithDrawal"];
                    var transType = (int)TransType.TransactionType.Withdrawal;
                    transMaster.TransType = transType.ToString();
                    transMaster.ChequeNo = null;
                    Utils.LogNO("cashwithdrawal without cheque");
                    if (transMaster.CurrCode != "NGN")
                        url = System.Configuration.ConfigurationManager.AppSettings["CashWithDrawalFCY"];
                }

                if (isDemo == "false")
                {
                    OutputResponse response = null;
                    APIRequest request = new APIRequest();
                    request.access_token = transMaster.access_token;
                    var narration = _transactionBusiness.BuildNarration(transMaster.ChequeNo,
                        transMaster.Beneficiary, "", transMaster.Narration, transMaster.TransName, 3, transMaster.TransType);
                    Utils.LogNO("Executed narration " + narration);
                    if (transMaster.IsTillTransfer)
                    {
                        Utils.LogNO("TransactionModel 1 (Serialized): " + JsonConvert.SerializeObject(transMaster));
                        if (transMaster.CurrCode == "NGN")
                        {
                            url = System.Configuration.ConfigurationManager.AppSettings["TILLTRANSFERLOCAL"];
                            TillTransferRequest tillTransferRequest = new TillTransferRequest();
                            tillTransferRequest.TiiTransferLCY = new TillTransferLCYModel();
                            tillTransferRequest.TiiTransferLCY.amttotransfer = Convert.ToString(transMaster.Amount);
                            tillTransferRequest.TiiTransferLCY.fromteller = transMaster.TellerId;
                            tillTransferRequest.TiiTransferLCY.Narrative = transMaster.Remark;
                            tillTransferRequest.TiiTransferLCY.toteller = transMaster.ToTellerId;
                            tillTransferRequest.TiiTransferLCY.TransactionBranch = transMaster.Branch; //"NG0020006";
                            tillTransferRequest.TiiTransferLCY.txncurr = transMaster.CurrCode;
                            tillTransferRequest.TiiTransferLCY.access_token = transMaster.access_token;
                            response = TillAPIService.TILLTRANSFERLOCAL(url, tillTransferRequest);
                        }
                        else
                        {
                            url = System.Configuration.ConfigurationManager.AppSettings["TILLTRANSFERFCY"];
                            TillTransferRequestForeign tillTransferRequestFCY = new TillTransferRequestForeign();
                            tillTransferRequestFCY.TiiTransferFCY = new TillTransferLCYModel();
                            tillTransferRequestFCY.TiiTransferFCY.amttotransfer = Convert.ToString(transMaster.Amount);
                            tillTransferRequestFCY.TiiTransferFCY.fromteller = transMaster.TellerId;
                            tillTransferRequestFCY.TiiTransferFCY.Narrative = transMaster.Remark;
                            tillTransferRequestFCY.TiiTransferFCY.toteller = transMaster.ToTellerId;
                            tillTransferRequestFCY.TiiTransferFCY.TransactionBranch = transMaster.Branch; //"NG0020006";
                            tillTransferRequestFCY.TiiTransferFCY.txncurr = transMaster.CurrCode;
                            tillTransferRequestFCY.TiiTransferFCY.access_token = transMaster.access_token;
                            response = TillAPIService.TILLTRANSFERForeign(url, tillTransferRequestFCY);
                        }
                        Utils.LogNO("Is till transfer" + response);
                        try
                        {
                            transMaster.MachineName = DetermineCompName(GetIPAddress());
                            var IPAddress = GetIPAddress();
                            General.AuditLog("Posting/Creating transaction on T24", transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                        }
                        catch { }

                        if (response == null)
                        {
                            result = new
                            {
                                success = false,
                                message = "till not transfered successfully",
                                TransactionRef = ""
                            };
                            return Ok(result);
                        }
                        if (response.ResponseCode == "-1")
                        {
                            result = new
                            {
                                success = false,
                                message = "Invalid requester teller ID or branch for company",
                                TransactionRef = ""
                            };
                            return Ok(result);
                        }

                        if (response.ResponseCode == "1")
                        {
                            var transType = (int)TransType.TransactionType.TillTransfer;
                            transMaster.TransType = transType.ToString();
                            transMaster.MachineName = "";
                            transMaster.TransRef = response.ResponseId;
                            transMaster.CBAResponse = response.ResponseText;
                            transMaster.CBACode = response.ResponseCode;
                            transMaster.Status = 2;
                            transMaster.Posted = true;
                            transMaster.TransactionParty = "";
                            transMaster.ApprovedBy = "";
                            Utils.LogNO("TransactionModel 2 (Serialized): " + JsonConvert.SerializeObject(transMaster));
                            Utils.LogNO("Executing local transaction.....");
                            _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,3);
                                                    
                            Utils.LogNO("Executed local transaction successfully");
                            return Ok(new
                            {
                                success = true,
                                message = "transaction saved successfully",
                                TransactionRef = response.ResponseId,
                                Result = result
                            });
                        }
                        else
                        {
                            result = new
                            {
                                success = false,
                                message = "Till has been transferred but transfer not successful",
                                TransactionRef = ""
                            };
                            return Ok(result);
                        }
                    }
                    else
                    {
                        response = CreateCBACashWithdrawal(transMaster, url, narration);
                        if (response != null)
                        {
                            if (response.ResponseId == null)
                            {
                                result = new
                                {
                                    success = false,
                                    message = response.ResponseText,
                                    TransactionRef = ""
                                };
                                return Ok(result);
                            }
                            if (response.ResponseCode == "-1")
                            {
                                result = new
                                {
                                    success = false,
                                    message = "Balance too low",
                                    TransactionRef = ""
                                };
                                return Ok(result);
                            }
                            // var MachineName = DetermineCompName(GetIPAddress());
                            //To TellerId will be used as cheque number  if it is a cheque transaction
                            transMaster.ToTellerId = transMaster.ChequeNo == null ? transMaster.ToTellerId : transMaster.ChequeNo;
                            transMaster.TransRef = response.ResponseId;
                            transMaster.MachineName = "";
                            transMaster.Posted = true;
                            transMaster.Approved = true;
                            transMaster.Status = 2;
                            transMaster.CBA = "T24";
                            transMaster.ApprovedBy = transMaster.CashierID;
                            transMaster.CBACode = response.ResponseCode;
                            transMaster.CBAResponse = response.ResponseText;
                            result = _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,3);
                        }
                        else
                        {
                            result = new
                            {
                                success = false,
                                message = "Till has been transfered but transfer not successful",
                                TransactionRef = ""
                            };
                            return Ok(result);
                        }
                    }
                }
                else
                {
                    //var MachineName = DetermineCompName(GetIPAddress());
                    transMaster.Posted = true;
                    transMaster.TransRef = Guid.NewGuid().ToString();
                    transMaster.Status = 2;
                    if (transMaster.IsTillTransfer == true)
                    {
                        var transType = (int)TransType.TransactionType.TillTransfer;
                        transMaster.TransType = transType.ToString();
                       // result = _tillTransferService.AcceptTillTransfer(transMaster.TillTransferID);
                        transMaster.MachineName = "Workload";

                        result = _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,3);
                    }
                    else
                    {
                        transMaster.MachineName = "Workload";
                       result =  _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,3);
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _transactionBusiness.GetWithdrawalTrans(false, ex.Message, ex);
                return Ok(result);
            }


        }

        private FundTransferResponse CreateTransferTransactionForGL(TransactionModel transMaster, long result, string url, BranchAccounts GLAccount)
        {
            var Response = new FundTransferResponse();

            try
            {//MUST: GET THE 2 GL ACCOUNTS FOR THIS POSTING
                Utils.LogNO(string.Format("CreateTransferTransactionforGL: TranID: {0}", result));
                Utils.LogNO(string.Format("Transaction Master: {0}", JsonConvert.SerializeObject(transMaster)));
                transMaster.TranId = result;
                Utils.LogNO("Processing of transfer Started");
                TransactionModel transactionModel = new TransactionModel();
                //var response = _transactionBusiness.GetAllTransById(Convert.ToInt32(transMaster.TranId));
                //Utils.LogNO(string.Format("Currency: {0}", response.Currency));
                // Utils.LogNO(string.Format("Transaction Master: {0}", JsonConvert.SerializeObject(response, new JsonSerializerSettings
                // { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })));

                transactionModel.CurrCode = transMaster.CurrencyAbbrev; //_transactionBusiness.GetCurrencyAbbrev(response.Currency);
                //Utils.LogNO("Serialized data: ..." + JsonConvert.SerializeObject(response));
                FundTransferModel fundTransferModel = new FundTransferModel();
                    fundTransferModel.FT_Request = new FTRequest();
                    fundTransferModel.FT_Request.TransactionBranch = transMaster.Branch;//ook
                    fundTransferModel.FT_Request.TransactionType = transMaster.ChargeType;/// transMaster.TransType; //"AVCE";
                    fundTransferModel.FT_Request.DebitAcctNo = transMaster.AccountNo;//GLAccount==null?"": GLAccount.BranchDBSuspenceAccount;//transMaster.AccountNo;//GL Here
                    fundTransferModel.FT_Request.CreditAccountNo = GLAccount == null ? "" : GLAccount.BranchCRSuspenceAccount;//transMaster.AccountNo;//GL Here
                    fundTransferModel.FT_Request.DebitCurrency = transMaster.CurrencyAbbrev;
                    fundTransferModel.FT_Request.CreditCurrency = "NGN";
                    fundTransferModel.FT_Request.DebitAmount = "" + Math.Round(Convert.ToDecimal(transMaster.Amount), 2);
                    fundTransferModel.FT_Request.CommissionCode = "";
                    fundTransferModel.FT_Request.narrations = transMaster.Narration;
                    fundTransferModel.FT_Request.SessionId = transMaster.access_token;
                    fundTransferModel.FT_Request.TrxnLocation = "1";
                    //post here
                    var cbaResponse = ImalService.FundTransferNar(url, fundTransferModel, transMaster.access_token);

                Response = cbaResponse;
            }
            catch (Exception ex)
            {
                Utils.LogNO("Error posting the transfer details: " + ex.Message);
            }

            return Response;
        }

        private FundTransferResponse CreateTransferTransactionForTermination(TransactionModel transMaster, long result, string url)
        {
            var Response = new FundTransferResponse();

            try
            {//MUST: GET THE 2 GL ACCOUNTS FOR THIS POSTING
                Utils.LogNO(string.Format("CreateTransferTransaction4GL: TranID: {0}", result));
                Utils.LogNO(string.Format("Transaction Master: {0}", JsonConvert.SerializeObject(transMaster)));
                transMaster.TranId = result;
                Utils.LogNO("Processing of transfer Started");
                TransactionModel transactionModel = new TransactionModel();
                //var response = _transactionBusiness.GetAllTransById(Convert.ToInt32(transMaster.TranId));
                //Utils.LogNO(string.Format("Currency: {0}", response.Currency));
                // Utils.LogNO(string.Format("Transaction Master: {0}", JsonConvert.SerializeObject(response, new JsonSerializerSettings
                // { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })));

                transactionModel.CurrCode = transMaster.CurrencyAbbrev; //_transactionBusiness.GetCurrencyAbbrev(response.Currency);
                //Utils.LogNO("Serialized data: ..." + JsonConvert.SerializeObject(response));
                FundTransferModel fundTransferModel = new FundTransferModel();
                fundTransferModel.FT_Request = new FTRequest();
                fundTransferModel.FT_Request.TransactionBranch = transMaster.Branch;//ook
                fundTransferModel.FT_Request.TransactionType = transMaster.ChargeType;/// transMaster.TransType; //"AVCE";
                fundTransferModel.FT_Request.DebitAcctNo = transMaster.AccountNo;//GLAccount==null?"": GLAccount.BranchDBSuspenceAccount;//transMaster.AccountNo;//GL Here
                fundTransferModel.FT_Request.CreditAccountNo = transMaster.CustomerAcctNos;//transMaster.AccountNo;//GL Here
                fundTransferModel.FT_Request.DebitCurrency = transMaster.CurrencyAbbrev;
                fundTransferModel.FT_Request.CreditCurrency = transMaster.CurrencyAbbrev;
                fundTransferModel.FT_Request.DebitAmount = "" + Math.Round(Convert.ToDecimal(transMaster.TotalAmt), 2);
                fundTransferModel.FT_Request.CommissionCode = "";
                fundTransferModel.FT_Request.narrations = transMaster.Narration;
                fundTransferModel.FT_Request.SessionId = transMaster.access_token;
                fundTransferModel.FT_Request.TrxnLocation = "1";
                //post here
                var cbaResponse = ImalService.FundTransferNar(url, fundTransferModel, transMaster.access_token);

                Response = cbaResponse;
            }
            catch (Exception ex)
            {
                Utils.LogNO("Error posting the transfer details: " + ex.Message);
            }

            return Response;
        }

        private TransactionResponseMessages CreateSingleTransferTransaction(TransactionModel transMaster, TransferDetails Beneficiary, ref long result, string url)
        {
            var Response = new TransactionResponseMessages();

            try
            {//InitiatorName Branch TransactionParty CurrencyAbbrev

                Utils.LogNO("CreateSingleTransferTranssaction: Processing of transfer Started...");
                TransactionModel transactionModel = new TransactionModel();
                var transHeader = new TransferHeader();
              //  transactionModel.CurrCode = transMaster.CurrencyAbbrev;
               // Utils.LogNO("Serialized data: ..." + JsonConvert.SerializeObject(transMaster));
                //Utils.LogNO("Serialized data: ..." + JsonConvert.SerializeObject(response));
                FundTransferModel fundTransferModel = new FundTransferModel();
                fundTransferModel.FT_Request = new FTRequest();
                fundTransferModel.FT_Request.TransactionBranch = transMaster.Branch;//ook
                fundTransferModel.FT_Request.TransactionType = transMaster.ChargeType; //"AVCE";
                fundTransferModel.FT_Request.DebitAcctNo = transMaster.AccountNo;
                fundTransferModel.FT_Request.CreditAccountNo = Beneficiary.BenAccountNumber;
                fundTransferModel.FT_Request.DebitCurrency = transMaster.CurrCode; // currcode same as debit currency for only this case
                fundTransferModel.FT_Request.CreditCurrency = transMaster.CurrencyAbbrev;
                fundTransferModel.FT_Request.DebitAmount = "" + Math.Round(Convert.ToDouble(Beneficiary.Amount), 2);
                fundTransferModel.FT_Request.CommissionCode = "";
                
                //finding a way to link individual posting with the main using the tranId
                fundTransferModel.FT_Request.narrations =  Beneficiary.Narration;
                fundTransferModel.FT_Request.SessionId = transMaster.access_token;
                fundTransferModel.FT_Request.TrxnLocation = "1";
                //post here
                  var cbaResponse = ImalService.FundTransferNar(url, fundTransferModel, transMaster.access_token);
                //FundTransferResponse cbaResponse = new FundTransferResponse();
                //cbaResponse.FTResponse = new FTResponse();
                //cbaResponse.FTResponse.ReferenceID = "";
                //cbaResponse.FTResponse.ReferenceID="";
                // cbaResponse.FTResponse.ResponseText="Test";
                //cbaResponse.FTResponse.ResponseCode = "00";
                //cbaResponse.FTResponse.CHARGEAMT = "0.0";
                                             
                if (cbaResponse != null)
                {
                    Response.rowNumber = Beneficiary.Id;
                    Response.msgId = cbaResponse.FTResponse.ReferenceID;
                    Response.responseCode = cbaResponse.FTResponse.ResponseCode;
                    Response.responseMessage = cbaResponse.FTResponse.ResponseText + "(" + cbaResponse.FTResponse.ResponseCode + ")";
                    Response.isSuccessful = cbaResponse.FTResponse.ResponseCode == "00" ? true : false;
                    Response.AccountNumber = Beneficiary.BenAccountNumber;
                    Response.RefId = cbaResponse.FTResponse.ReferenceID;
                    Response.ChargeAmt = Convert.ToDecimal(cbaResponse.FTResponse.CHARGEAMT);

                }
                else
                {
                    Response.rowNumber = Beneficiary.Id;
                    Response.msgId = "";
                    Response.responseMessage = ": No response from CBA!";
                    Response.isSuccessful =  false;
                    Response.AccountNumber = Beneficiary.BenAccountNumber;
                    Response.RefId = "";
                    Response.ChargeAmt = 0;
                }




            }
            catch (Exception ex)
            {
                Utils.LogNO("Single: Error posting the transfer details: " + ex.Message);
            }

            return Response;
        }

        private TransactionResponseMessages CreateTransferTransaction(TransactionModel transMaster, TransferDetails Beneficiary, ref long result, string url, BranchAccounts GLAccounts)
        {
            var Response = new TransactionResponseMessages();

            try
            {//InitiatorName Branch TransactionParty CurrencyAbbrev
             
                Utils.LogNO("CreateTransferTransaction 2: Processing of transfer Started...");
                TransactionModel transactionModel = new TransactionModel();
                var transHeader = new TransferHeader();
               // transactionModel.CurrCode = transMaster.CurrencyAbbrev;

                Utils.LogNO("Serialized data: ..." + JsonConvert.SerializeObject(transMaster));
                FundTransferModel fundTransferModel = new FundTransferModel();
                fundTransferModel.FT_Request = new FTRequest();
                fundTransferModel.FT_Request.TransactionBranch = transMaster.Branch;//ook
                fundTransferModel.FT_Request.TransactionType = transMaster.ChargeType; //"AVCE";
                fundTransferModel.FT_Request.DebitAcctNo = GLAccounts.BranchCRSuspenceAccount;
                fundTransferModel.FT_Request.CreditAccountNo = Beneficiary.BenAccountNumber;
                fundTransferModel.FT_Request.DebitCurrency = "NGN";
                fundTransferModel.FT_Request.CreditCurrency = transMaster.CurrencyAbbrev;
                fundTransferModel.FT_Request.DebitAmount = "" + Math.Round(Convert.ToDecimal(Beneficiary.Amount), 2);
                fundTransferModel.FT_Request.CommissionCode = "";
                //finding a way to link individual posting with the main using the tranId
                fundTransferModel.FT_Request.narrations = result+":"+Beneficiary.Narration;
                fundTransferModel.FT_Request.SessionId = transMaster.access_token;
                fundTransferModel.FT_Request.TrxnLocation = "1";
                //post here
                var cbaResponse = ImalService.FundTransferNar(url, fundTransferModel, transMaster.access_token);
                      

                if (cbaResponse != null)
                {
                    Response.rowNumber = Beneficiary.Id;
                    Response.msgId = cbaResponse.FTResponse.ReferenceID;
                    Response.responseCode = cbaResponse.FTResponse.ResponseCode;
                    Response.responseMessage = cbaResponse.FTResponse.ResponseText+ "("+ cbaResponse.FTResponse.ResponseCode+")";
                    Response.isSuccessful = cbaResponse.FTResponse.ResponseCode=="00" ? true:false;
                    Response.AccountNumber = Beneficiary.BenAccountNumber;
                    Response.RefId = cbaResponse.FTResponse.ReferenceID;
                    Response.ChargeAmt =Convert.ToDecimal(cbaResponse.FTResponse.CHARGEAMT);
                        
                }
                else
                {
                    Response.responseCode = cbaResponse.FTResponse.ResponseCode;
                    Response.rowNumber = Beneficiary.Id;
                    Response.msgId = "";
                    Response.responseMessage = "No response from CBA!";
                    Response.isSuccessful = false;
                    Response.AccountNumber = Beneficiary.BenAccountNumber;
                    Response.RefId = "";
                    Response.ChargeAmt = 0;
                }

                

               
            }
            catch(Exception ex)
            {
                Utils.LogNO("Error posting the transfer details: "+ ex.Message);
            }
                                          
            return Response;
        }

        [HttpPost, Route("api/PostTransferDetails/TransDeposit")]
        public IHttpActionResult PostTransferDetails([FromBody]TransactionModel transMaster)
        {
            Utils.LogNO("api/PostTransferDetails/TransDeposit:18:Currently Inside This End Point.");
            List<TransactionResponseMessages> transactionResponseMessages = new List<TransactionResponseMessages>();
            List<TransactionBeneficiaries> TransactionBeneficiary = new List<TransactionBeneficiaries>();
            List<TransferDetails> Beneficiaries = new List<TransferDetails>();
            try
            {
                transMaster.CBA = transMaster.CBA;
                long result = 0;
                string url = null;
                int updateResponse = 0;
                int count = 0;

                Utils.LogNO("Post Transfer details Started....");
                Utils.LogNO("BranchCode: " + transMaster.BranchCode);
                url = System.Configuration.ConfigurationManager.AppSettings["INHOUSETRANSFER"];

                var transType = (int)TransType.TransactionType.InHouseTransfer;
                transMaster.TransType = transType.ToString();
                var GLAccounts = _transactionBusiness.FetchGLAccounts(transMaster.BranchCode);
                try
                {
                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Posting/Creating Inhouse Transfer transaction on T24", transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }
                if (isDemo == "false")
                {
                    transMaster.IsBulkTran = true;
                    result = _transactionBusiness.CreateTransaction(transMaster,3);

                    
                    Beneficiaries = _transactionBusiness.RetrieveBeneficiaries(result);
                    if (Beneficiaries.Count() == 1)
                    {
                            var rResult = SingleBeneficiaryPostingToCBA
                                            (Beneficiaries.FirstOrDefault(), transMaster, result, url);
                        if(rResult.isSuccessful == true)
                        {
                            var Response = new TransactionResponseMessages()
                            {
                                rowNumber = count++,
                                msgId = result + "",
                                responseMessage = "Successful",
                                isSuccessful = true,
                                AccountNumber = "",
                                RefId = rResult.RefId
                            };
                            transactionResponseMessages.Add(Response);
                            return Ok(new
                            {
                                success = true,
                                message = "",
                                TransRef = rResult.RefId,
                                result = transactionResponseMessages
                            });
                        }
                        else
                        {
                            return Ok(new
                            {
                                success = false,
                                message = rResult.responseMessage,
                                TransRef = "",
                                result = transactionResponseMessages
                            });
                        }
                    }
                    else
                    {
                        //2. Second Leg Posting to Individuals
                        var responseObj = CreateTransferTransactionForGL(transMaster, result, url, GLAccounts);
                        if (responseObj.FTResponse.ResponseCode == "00")
                        {
                            transMaster.Posted = true;
                            transMaster.Status = 2;
                            transMaster.CBACode = responseObj.FTResponse.ResponseCode;
                            transMaster.CBAResponse = responseObj.FTResponse.ResponseText;
                            transMaster.TransRef = responseObj.FTResponse.ReferenceID;
                            transMaster.CHARGEAMT = Convert.ToDecimal(responseObj.FTResponse.CHARGEAMT);
                            transMaster.TranId = result;
                            var rUpdate = _transactionBusiness.UpdateStatusForMasterTrans(transMaster);
                            //retrieve beneficiaries from Db cos the ID will be needed for update
                            Beneficiaries = _transactionBusiness.RetrieveBeneficiaries(result);
                            Utils.LogNO("Beneficiaries count : " + Beneficiaries.Count());
                            if (Beneficiaries != null)
                            {
                                    System.Threading.ThreadStart tAPIStarts =
                                       delegate
                                       {
                                           var rResult = BeneficiaryPostingToCBA(Beneficiaries, transMaster, result, url, GLAccounts);
                                       };
                                    new System.Threading.Thread(tAPIStarts).Start();

                                //transactionResponseMessages = ProcessBeneficiaryPosting(Beneficiaries, transMaster, result, url, GLAccounts);

                            }

                            var Response = new TransactionResponseMessages()
                            {
                                rowNumber = count++,
                                msgId = result + "",
                                responseMessage = "Successful",
                                isSuccessful = true,
                                AccountNumber = "",
                                RefId = responseObj.FTResponse.ReferenceID
                            };


                            transactionResponseMessages.Add(Response);

                            return Ok(new
                            {
                                success = true,
                                message = "",
                                TransRef = responseObj.FTResponse.ReferenceID,
                                result = transactionResponseMessages
                            });

                        }
                        else
                        {
                            int removalResponse = _transactionBusiness.RemoveAlreadyCreatedTransaction(result);

                            var Response = new TransactionResponseMessages()
                            {
                                rowNumber = 1,
                                msgId = result + "",
                                responseMessage = responseObj.FTResponse.ResponseText + "(" + responseObj.FTResponse.ResponseText + ")",
                                isSuccessful = false,
                                AccountNumber = responseObj.FTResponse.FTID,
                                RefId = responseObj.FTResponse.ReferenceID
                            };
                            transactionResponseMessages.Add(Response);
                            return Ok(new
                            {
                                success = false,
                                message = "GL Posting Failed",
                                TransRef = "",
                                result = transactionResponseMessages
                            });
                        }
                    }
                }
                else
                {
                    //isDemo        
                    transMaster.TransRef = Guid.NewGuid().ToString();
                    result = _transactionBusiness.CreateTransaction(transMaster,3);

                    //1. First Leg Posting to Central Accounts

                    //2. Second Leg Posting to Individuals
                    if (result > 0)
                    {
                        //retrieve beneficiaries from Db cos the ID will be needed for update
                        //Pass to background thread
                        Beneficiaries = _transactionBusiness.RetrieveBeneficiaries(result);
                        foreach (var beneficiary in Beneficiaries)
                        {
                            // var responseObj2 = CreateTransferTransaction(transMaster, beneficiary, ref result, url);
                            var isValid = _transactionBusiness.ValidateAccountNumber(beneficiary.BenAccountNumber);
                            if (isValid)
                            {
                                beneficiary.Posted = true;
                                beneficiary.TransRef = Guid.NewGuid().ToString();
                                beneficiary.CBAResponse = "Successful";
                                beneficiary.CBAResponseCode = "00";
                                // beneficiary.ApplicableCharge = responseObj2.ChargeAmt;

                                updateResponse = _transactionBusiness.UpdateBeneficiaryForPostStatus(beneficiary);

                                var Response2 = new TransactionResponseMessages()
                                {
                                    rowNumber = count++,
                                    msgId = result + "",
                                    responseMessage = "Successful",
                                    isSuccessful = true,
                                    AccountNumber = beneficiary.BenAccountNumber,
                                    RefId = ""
                                };


                                transactionResponseMessages.Add(Response2);

                            }
                            else
                            {
                                beneficiary.Posted = false;
                                beneficiary.TransRef = Guid.NewGuid().ToString();
                                beneficiary.CBAResponse = "Failed";
                                beneficiary.CBAResponseCode = "x03";
                                // beneficiary.ApplicableCharge = responseObj2.ChargeAmt;

                                updateResponse = _transactionBusiness.UpdateBeneficiaryForPostStatus(beneficiary);

                                var Response2 = new TransactionResponseMessages()
                                {
                                    rowNumber = count++,
                                    msgId = result + "",
                                    responseMessage = "Failed",
                                    isSuccessful = false,
                                    AccountNumber = beneficiary.BenAccountNumber,
                                    RefId = ""
                                };
                                transactionResponseMessages.Add(Response2);
                            }

                            //var newbeneficiary = new TransactionBeneficiaries()
                            //{
                            //    SN = 
                            //};
                            //TransactionBeneficiary.Add(newbeneficiary);
                        }
                        return Ok(new
                        {
                            success = true,
                            message = "",
                            TransRef = result,
                            result = transactionResponseMessages
                        });


                    }
                    else
                    {
                        int removalResponse = _transactionBusiness.RemoveAlreadyCreatedTransaction(result);

                        var Response2 = new TransactionResponseMessages()
                        {
                            rowNumber = 1,
                            msgId = result + "",
                            responseMessage = "Failed",
                            isSuccessful = false,
                            AccountNumber = "",
                            RefId = ""
                        };
                        transactionResponseMessages.Add(Response2);
                        return Ok(new
                        {
                            success = false,
                            message = "GL posting Failed",
                            TransRef = "",
                            result = transactionResponseMessages
                        });
                    }

                }

            }
            catch (Exception ex)
            {
                Utils.LogNO("Transfer Processing Error: " + ex.Message);
                return Ok(new
                {
                    success = false,
                    message = ex.Message,
                    TransRef = "",
                    result = transactionResponseMessages
                });
            }
        }

        [HttpPost, Route("api/PostTransferDetails/Approve")]
        public IHttpActionResult PostTransferDetailsOnApproval([FromBody]TransactionModel transMaster)
        {
            Utils.LogNO("api/PostTransferDetails/Approve:19:Currently Inside This End Point.");
            List<TransactionResponseMessages> transactionResponseMessages = new List<TransactionResponseMessages>();
            List<TransactionBeneficiaries> TransactionBeneficiary = new List<TransactionBeneficiaries>();
            List<TransferDetails> Beneficiaries = new List<TransferDetails>();
            try
            {
                
                    long result = 0;
                    string url = null;
                    int updateResponse = 0;
                    int count = 0;

                    Utils.LogNO("Post Transfer details Approval Started....");

                    url = System.Configuration.ConfigurationManager.AppSettings["INHOUSETRANSFER"];
                   



                    var GLAccounts = _transactionBusiness.FetchGLAccounts(transMaster.BranchCode);
                    try
                    {
                        transMaster.MachineName = DetermineCompName(GetIPAddress());
                        var IPAddress = GetIPAddress();
                        General.AuditLog("Posting/Creating Inhouse Transfer transaction on T24 During Approval", transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                    }
                    catch { }
                    if (isDemo == "false")
                    {
                    result = transMaster.TranId;
                    // result = _transactionBusiness.CreateTransaction(transMaster);

                    Beneficiaries = _transactionBusiness.RetrieveBeneficiaries(result);
                    if (Beneficiaries.Count() == 1)
                    {
                        var rResult = SingleBeneficiaryPostingToCBA
                                        (Beneficiaries.FirstOrDefault(), transMaster, result, url);
                        if (rResult.isSuccessful == true)
                        {
                            TransactionApprovalModel Transaction = new TransactionApprovalModel();
                            Transaction.TranId = transMaster.TranId;
                            Transaction.ApprovedBy = transMaster.ApprovedBy;
                            Transaction.TReference = rResult.RefId;
                            var results = _transactionService.ApproveInHouseTransactionTransaction(Transaction, Transaction.TReference);
                            return Ok(results);
                        }
                        else
                        {
                            int removalResponse = _transactionBusiness.RemoveAlreadyCreatedTransaction(result);
                            return Ok(new
                            {
                                success = false,
                                message = rResult.responseMessage,
                                TransRef = "",
                                result = transactionResponseMessages
                            });
                        }
                    }
                    else
                    {
                        //1. First Leg Posting to Central Accounts
                        var transHeader = _transactionBusiness.RetrieveTransferHeader(result);
                        transMaster.ChargeType = transHeader.ChargeType;
                        var responseObj = CreateTransferTransactionForGL(transMaster, result, url, GLAccounts);


                        //2. Second Leg Posting to Individuals
                        if (responseObj.FTResponse.ResponseCode == "00")
                        {
                            transMaster.Posted = true;
                            transMaster.Status = 2;

                            transMaster.CBACode = responseObj.FTResponse.ResponseCode;
                            transMaster.CBAResponse = responseObj.FTResponse.ResponseText;
                            transMaster.TransRef = responseObj.FTResponse.ReferenceID;
                            transMaster.CHARGEAMT = Convert.ToDecimal(responseObj.FTResponse.CHARGEAMT);



                            var rUpdate = _transactionBusiness.UpdateStatusForMasterTrans(transMaster);
                            //retrieve beneficiaries from Db cos the ID will be needed for update
                            Beneficiaries = _transactionBusiness.RetrieveBeneficiaries(result);

                            if (Beneficiaries != null)
                            {
                                //transactionResponseMessages = ProcessBeneficiaryPosting(Beneficiaries, transMaster, result, url, GLAccounts);
                                System.Threading.ThreadStart tAPIStarts =
                                delegate
                                {
                                    var rResult = BeneficiaryPostingToCBA(Beneficiaries, transMaster, result, url, GLAccounts);
                                };

                                new System.Threading.Thread(tAPIStarts).Start();
                            }

                            var Response = new TransactionResponseMessages()
                            {
                                rowNumber = count++,
                                msgId = result + "",
                                responseMessage = "Successful",
                                isSuccessful = true,
                                AccountNumber = "",
                                RefId = responseObj.FTResponse.ReferenceID
                            };


                            transactionResponseMessages.Add(Response);
                            TransactionApprovalModel Transaction = new TransactionApprovalModel();
                            Transaction.TranId = transMaster.TranId;
                            Transaction.ApprovedBy = transMaster.ApprovedBy;
                            Transaction.TReference = responseObj.FTResponse.ReferenceID;

                            var results = _transactionService.ApproveInHouseTransactionTransaction(Transaction, Transaction.TReference);
                            return Ok(results);

                        }
                        else
                        {
                            int removalResponse = _transactionBusiness.RemoveAlreadyCreatedTransaction(result);

                            var Response = new TransactionResponseMessages()
                            {
                                rowNumber = 1,
                                msgId = result + "",
                                responseMessage = responseObj.FTResponse.ResponseText + "(" + responseObj.FTResponse.ResponseText + ")",
                                isSuccessful = false,
                                AccountNumber = responseObj.FTResponse.FTID,
                                RefId = responseObj.FTResponse.ReferenceID
                            };
                            transactionResponseMessages.Add(Response);
                            return Ok(new
                            {
                                success = false,
                                message = "GL Posting Failed",
                                TransRef = "",
                                result = transactionResponseMessages
                            });
                        }
                    }
                }
                    else
                    {
                        //isDemo        
                        transMaster.TransRef = Guid.NewGuid().ToString();
                        transMaster.Approved = true;
                        result = _transactionBusiness.CreateTransaction(transMaster,3);

                        //1. First Leg Posting to Central Accounts

                        //2. Second Leg Posting to Individuals
                        if (result > 0)
                        {
                            //retrieve beneficiaries from Db cos the ID will be needed for update
                            //Pass to background thread
                            Beneficiaries = _transactionBusiness.RetrieveBeneficiaries(result);
                            foreach (var beneficiary in Beneficiaries)
                            {
                                // var responseObj2 = CreateTransferTransaction(transMaster, beneficiary, ref result, url);
                                var isValid = _transactionBusiness.ValidateAccountNumber(beneficiary.BenAccountNumber);
                                if (isValid)
                                {
                                    beneficiary.Posted = true;
                                    beneficiary.TransRef = Guid.NewGuid().ToString();
                                    beneficiary.CBAResponse = "Successful";
                                    beneficiary.CBAResponseCode = "00";
                                    // beneficiary.ApplicableCharge = responseObj2.ChargeAmt;

                                    updateResponse = _transactionBusiness.UpdateBeneficiaryForPostStatus(beneficiary);

                                    var Response2 = new TransactionResponseMessages()
                                    {
                                        rowNumber = count++,
                                        msgId = result + "",
                                        responseMessage = "Successful",
                                        isSuccessful = true,
                                        AccountNumber = beneficiary.BenAccountNumber,
                                        RefId = ""
                                    };


                                    transactionResponseMessages.Add(Response2);

                                TransactionApprovalModel Transactions = new TransactionApprovalModel();
                                Transactions.TranId = transMaster.TranId;
                                Transactions.ApprovedBy = transMaster.ApprovedBy;
                                Transactions.TReference = Guid.NewGuid().ToString();

                                var resultss = _transactionService.ApproveInHouseTransactionTransaction(Transactions, Transactions.TReference);
                                return Ok(resultss);

                            }
                                else
                                {
                                    beneficiary.Posted = false;
                                    beneficiary.TransRef = Guid.NewGuid().ToString();
                                    beneficiary.CBAResponse = "Failed";
                                    beneficiary.CBAResponseCode = "x03";
                                    // beneficiary.ApplicableCharge = responseObj2.ChargeAmt;

                                    updateResponse = _transactionBusiness.UpdateBeneficiaryForPostStatus(beneficiary);

                                    var Response2 = new TransactionResponseMessages()
                                    {
                                        rowNumber = count++,
                                        msgId = result + "",
                                        responseMessage = "Failed",
                                        isSuccessful = false,
                                        AccountNumber = beneficiary.BenAccountNumber,
                                        RefId = ""
                                    };
                                    transactionResponseMessages.Add(Response2);
                                }

                                //var newbeneficiary = new TransactionBeneficiaries()
                                //{
                                //    SN = 
                                //};
                                //TransactionBeneficiary.Add(newbeneficiary);
                            }
                        TransactionApprovalModel Transaction = new TransactionApprovalModel();
                        Transaction.TranId = transMaster.TranId;
                        Transaction.ApprovedBy = transMaster.ApprovedBy;
                        Transaction.TReference = Guid.NewGuid().ToString();

                        var results = _transactionService.ApproveInHouseTransactionTransaction(Transaction, Transaction.TReference);
                        return Ok(results);


                    }
                    else
                        {
                            int removalResponse = _transactionBusiness.RemoveAlreadyCreatedTransaction(result);

                            var Response2 = new TransactionResponseMessages()
                            {
                                rowNumber = 1,
                                msgId = result + "",
                                responseMessage = "Failed",
                                isSuccessful = false,
                                AccountNumber = "",
                                RefId = ""
                            };
                            transactionResponseMessages.Add(Response2);
                            return Ok(new
                            {
                                success = false,
                                message = "GL posting Failed",
                                TransRef = "",
                                result = transactionResponseMessages
                            });
                        }

                    }
                
            }
            catch (Exception ex)
            {
                Utils.LogNO("Transfer Processing Error: " + ex.Message);
                return Ok(new
                {
                    success = false,
                    message = ex.Message,
                    TransRef = "",
                    result = transactionResponseMessages
                });
            }

            Utils.LogNO("Post Transfer details Ended!");
        }

        [HttpPost, Route("api/PostTransferDetails/Single/TransDeposit")]
        public IHttpActionResult PostSingleTransferDetails([FromBody]TransactionModel transMaster)
        {
            Utils.LogNO("api/PostTransferDetails/Single/TransDeposit:20:Currently Inside This End Point.");
            List<TransactionResponseMessages> transactionResponseMessages = new List<TransactionResponseMessages>();
            List<TransactionBeneficiaries> TransactionBeneficiary = new List<TransactionBeneficiaries>();
            List<TransferDetails> Beneficiaries = new List<TransferDetails>();
            TransferHeader transferHeader = new TransferHeader();
            try
            {
                transMaster.CBA = transMaster.CBA;
                long result = 0;
                string url = null;
                int updateResponse = 0;
                int count = 0;

                Utils.LogNO("Post Transfer details Started....");

                url = System.Configuration.ConfigurationManager.AppSettings["INHOUSETRANSFER"];

                var transType = (int)TransType.TransactionType.InHouseTransfer;
                transMaster.TransType = transType.ToString();
               
                try
                {
                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Posting/Creating Single Inhouse Transfer transaction on T24", transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }
                if (isDemo == "false")
                {
                        var GLAccounts = _transactionBusiness.FetchGLAccounts(transMaster.BranchCode);
                        var responseObj = CreateTransferTransactionForGL(transMaster, result, url, GLAccounts);
                        
                        transMaster.IsBulkTran = false;
                        transMaster.Status = 2;
                        result = _transactionBusiness.CreateTransaction(transMaster, 2);

                        
                        var rUpdate = _transactionBusiness.UpdateStatusForMasterTrans(transMaster);
                        //retrieve beneficiaries from Db cos the ID will be needed for update
                    Beneficiaries = _transactionBusiness.RetrieveBeneficiaries(result);
                    if (string.IsNullOrEmpty(transMaster.ChargeType))
                    {
                        transferHeader = _transactionBusiness.RetrieveTransferHeader(result);
                        transMaster.ChargeType = transferHeader.ChargeType;
                    }


                    foreach (var beneficiary in Beneficiaries)
                    {
                        var responseObj2 = CreateSingleTransferTransaction(transMaster, beneficiary, ref result, url);
                        
                        if (responseObj2.responseCode=="00")
                        {
                            if(count < 1)
                            {
                                transMaster.Posted = true;
                                transMaster.CBACode = "00";
                                transMaster.CBAResponse = "Successful";
                                transMaster.TransRef = transMaster.TranId.ToString();
                                transMaster.CHARGEAMT = Convert.ToDecimal(responseObj2.ChargeAmt);

                                var rsUpdate = _transactionBusiness.UpdateStatusForMasterTrans(transMaster);

                            }


                            beneficiary.Posted = true;
                            //beneficiary.ApplicableCharge = responseObj2.RefId;
                            beneficiary.TransRef = responseObj2.RefId;
                            beneficiary.CBAResponse = responseObj2.responseMessage;
                            beneficiary.CBAResponseCode = responseObj2.responseCode;
                            beneficiary.ApplicableCharge = responseObj2.ChargeAmt;
                            
                            

                            updateResponse = _transactionBusiness.UpdateBeneficiaryForPostStatus(beneficiary);

                            var Response2 = new TransactionResponseMessages()
                            {
                                rowNumber = count++,
                                msgId = result + "",
                                responseMessage = responseObj2.responseMessage,
                                isSuccessful = true,
                                AccountNumber = beneficiary.BenAccountNumber,
                                RefId = ""
                            };


                            transactionResponseMessages.Add(Response2);

                        }
                        else
                        {
                            beneficiary.Posted = false;
                            beneficiary.TransRef = responseObj2.RefId;
                            beneficiary.CBAResponse = responseObj2.responseMessage;
                            beneficiary.CBAResponseCode = responseObj2.responseCode;
                            // beneficiary.ApplicableCharge = responseObj2.ChargeAmt;

                            updateResponse = _transactionBusiness.UpdateBeneficiaryForPostStatus(beneficiary);

                            var Response2 = new TransactionResponseMessages()
                            {
                                rowNumber = count++,
                                msgId = result + "",
                                responseMessage = "Failed",
                                isSuccessful = false,
                                AccountNumber = beneficiary.BenAccountNumber,
                                RefId = ""
                            };
                            transactionResponseMessages.Add(Response2);
                        }

                        //var newbeneficiary = new TransactionBeneficiaries()
                        //{
                        //    SN = 
                        //};
                        //TransactionBeneficiary.Add(newbeneficiary);
                    }
                    return Ok(new
                    {
                        success = true,
                        message = "",
                        TransRef = result,
                        result = transactionResponseMessages
                    });


                }
                else
                {
                    //isDemo        
                    transMaster.TransRef = Guid.NewGuid().ToString();
                    result = _transactionBusiness.CreateTransaction(transMaster,3);

                    //1. First Leg Posting to Central Accounts


                    //2. Second Leg Posting to Individuals
                    if (result > 0)
                    {
                        //retrieve beneficiaries from Db cos the ID will be needed for update
                        //Pass to background thread
                        Beneficiaries = _transactionBusiness.RetrieveBeneficiaries(result);
                        foreach (var beneficiary in Beneficiaries)
                        {
                            // var responseObj2 = CreateTransferTransaction(transMaster, beneficiary, ref result, url);
                            var isValid = _transactionBusiness.ValidateAccountNumber(beneficiary.BenAccountNumber);
                            if (isValid)
                            {
                                beneficiary.Posted = true;
                                beneficiary.TransRef = Guid.NewGuid().ToString();
                                beneficiary.CBAResponse = "Successful";
                                beneficiary.CBAResponseCode = "00";
                                // beneficiary.ApplicableCharge = responseObj2.ChargeAmt;

                                updateResponse = _transactionBusiness.UpdateBeneficiaryForPostStatus(beneficiary);

                                var Response2 = new TransactionResponseMessages()
                                {
                                    rowNumber = count++,
                                    msgId = result + "",
                                    responseMessage = "Successful",
                                    isSuccessful = true,
                                    AccountNumber = beneficiary.BenAccountNumber,
                                    RefId = ""
                                };


                                transactionResponseMessages.Add(Response2);

                            }
                            else
                            {
                                beneficiary.Posted = false;
                                beneficiary.TransRef = Guid.NewGuid().ToString();
                                beneficiary.CBAResponse = "Failed";
                                beneficiary.CBAResponseCode = "x03";
                                // beneficiary.ApplicableCharge = responseObj2.ChargeAmt;

                                updateResponse = _transactionBusiness.UpdateBeneficiaryForPostStatus(beneficiary);

                                var Response2 = new TransactionResponseMessages()
                                {
                                    rowNumber = count++,
                                    msgId = result + "",
                                    responseMessage = "Failed",
                                    isSuccessful = false,
                                    AccountNumber = beneficiary.BenAccountNumber,
                                    RefId = ""
                                };
                                transactionResponseMessages.Add(Response2);
                            }

                            //var newbeneficiary = new TransactionBeneficiaries()
                            //{
                            //    SN = 
                            //};
                            //TransactionBeneficiary.Add(newbeneficiary);
                        }
                        return Ok(new
                        {
                            success = true,
                            message = "",
                            TransRef = result,
                            result = transactionResponseMessages
                        });


                    }
                    else
                    {
                        int removalResponse = _transactionBusiness.RemoveAlreadyCreatedTransaction(result);

                        var Response2 = new TransactionResponseMessages()
                        {
                            rowNumber = 1,
                            msgId = result + "",
                            responseMessage = "Failed",
                            isSuccessful = false,
                            AccountNumber = "",
                            RefId = ""
                        };
                        transactionResponseMessages.Add(Response2);
                        return Ok(new
                        {
                            success = false,
                            message = "GL posting Failed",
                            TransRef = "",
                            result = transactionResponseMessages
                        });
                    }

                }

            }
            catch (Exception ex)
            {
                Utils.LogNO("Transfer Processing Error: " + ex.Message);
                return Ok(new
                {
                    success = false,
                    message = ex.Message,
                    TransRef = "",
                    result = transactionResponseMessages
                });
            }

            Utils.LogNO("Post Transfer details Ended!");
        }

        [HttpPost, Route("api/PostTransferDetails/Single/Approval")]
        public IHttpActionResult PostSingleTransferDetailsOnApproval([FromBody]TransactionModel transMaster)
        {
            Utils.LogNO("api/PostTransferDetails/Single/Approval:21:Currently Inside This End Point.");
            List<TransactionResponseMessages> transactionResponseMessages = new List<TransactionResponseMessages>();
            List<TransactionBeneficiaries> TransactionBeneficiary = new List<TransactionBeneficiaries>();
            List<TransferDetails> Beneficiaries = new List<TransferDetails>();
            try
            {
                //transMaster.CBA = transMaster.CBA;
                long result = 0;
                string url = null;
                int updateResponse = 0;
                int count = 0;

                Utils.LogNO("Post Transfer details Started on Approval....");

                url = System.Configuration.ConfigurationManager.AppSettings["INHOUSETRANSFER"];

                var transType = (int)TransType.TransactionType.InHouseTransfer;
                transMaster.TransType = transType.ToString();


                try
                {
                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Posting/Creating Single Inhouse Transfer transaction on T24 During Approval", transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }
                if (isDemo == "false")
                {
                    // result = _transactionBusiness.CreateTransaction(transMaster);

                    result = transMaster.TranId;

                   // var rUpdate = _transactionBusiness.UpdateStatusForMasterTrans(transMaster);
                    //retrieve beneficiaries from Db cos the ID will be needed for update
                    Beneficiaries = _transactionBusiness.RetrieveBeneficiaries(result);
                    transMaster.Approved = true;
                    var transRef = "";
                    foreach (var beneficiary in Beneficiaries)
                    {
                        var responseObj2 = CreateSingleTransferTransaction(transMaster, beneficiary, ref result, url);
                        
                        if (responseObj2.responseCode == "00")
                        {
                            if (count < 1)
                            {
                                transMaster.Posted = true;
                                transRef = responseObj2.RefId;
                                transMaster.Status = 2;

                                transMaster.CBACode = "00";
                                transMaster.CBAResponse = "Successful";
                                transMaster.TransRef = transMaster.TranId.ToString();
                                transMaster.CHARGEAMT = Convert.ToDecimal(responseObj2.ChargeAmt);

                                var rsUpdate = _transactionBusiness.UpdateStatusForMasterTrans(transMaster);

                            }


                            beneficiary.Posted = true;
                            
                            beneficiary.TransRef = responseObj2.RefId;
                            beneficiary.CBAResponse = responseObj2.responseMessage;
                            beneficiary.CBAResponseCode = responseObj2.responseCode;
                            // beneficiary.ApplicableCharge = responseObj2.ChargeAmt;

                            updateResponse = _transactionBusiness.UpdateBeneficiaryForPostStatus(beneficiary);

                            var Response2 = new TransactionResponseMessages()
                            {
                                rowNumber = count++,
                                msgId = result + "",
                                responseMessage = responseObj2.responseMessage,
                                isSuccessful = true,
                                AccountNumber = beneficiary.BenAccountNumber,
                                RefId = ""
                            };


                            transactionResponseMessages.Add(Response2);

                        }
                        else
                        {
                            beneficiary.Posted = false;
                            beneficiary.TransRef = responseObj2.RefId;
                            beneficiary.CBAResponse = responseObj2.responseMessage;
                            beneficiary.CBAResponseCode = responseObj2.responseCode;
                            // beneficiary.ApplicableCharge = responseObj2.ChargeAmt;

                            updateResponse = _transactionBusiness.UpdateBeneficiaryForPostStatus(beneficiary);

                            var Response2 = new TransactionResponseMessages()
                            {
                                rowNumber = count++,
                                msgId = result + "",
                                responseMessage = "Failed",
                                isSuccessful = false,
                                AccountNumber = beneficiary.BenAccountNumber,
                                RefId = ""
                            };
                            transactionResponseMessages.Add(Response2);
                        }

                        //var newbeneficiary = new TransactionBeneficiaries()
                        //{
                        //    SN = 
                        //};
                        //TransactionBeneficiary.Add(newbeneficiary);
                    }
                    TransactionApprovalModel Transaction = new TransactionApprovalModel();
                    Transaction.TranId = transMaster.TranId;
                    Transaction.ApprovedBy = transMaster.ApprovedBy;
                    Transaction.TReference = transRef;

                    var results = _transactionBusiness.ApproveTransaction(Transaction, transRef);
                    return Ok(results);
                    //return Ok(new
                    //{
                    //    success = true,
                    //    message = "Successful",
                    //    TransactionRef = Transaction.TReference,
                    //    data = results
                    //});



                }
                else
                {
                    //isDemo        
                    transMaster.TransRef = Guid.NewGuid().ToString();
                    transMaster.Approved = true;
                    result = _transactionBusiness.CreateTransaction(transMaster,3);

                    //1. First Leg Posting to Central Accounts

                    //2. Second Leg Posting to Individuals
                    if (result > 0)
                    {
                        //retrieve beneficiaries from Db cos the ID will be needed for update
                        //Pass to background thread
                        Beneficiaries = _transactionBusiness.RetrieveBeneficiaries(result);
                        foreach (var beneficiary in Beneficiaries)
                        {
                            // var responseObj2 = CreateTransferTransaction(transMaster, beneficiary, ref result, url);
                            var isValid = _transactionBusiness.ValidateAccountNumber(beneficiary.BenAccountNumber);
                            if (isValid)
                            {
                                beneficiary.Posted = true;
                                beneficiary.TransRef = Guid.NewGuid().ToString();
                                beneficiary.CBAResponse = "Successful";
                                beneficiary.CBAResponseCode = "00";
                                // beneficiary.ApplicableCharge = responseObj2.ChargeAmt;

                                updateResponse = _transactionBusiness.UpdateBeneficiaryForPostStatus(beneficiary);

                                var Response2 = new TransactionResponseMessages()
                                {
                                    rowNumber = count++,
                                    msgId = result + "",
                                    responseMessage = "Successful",
                                    isSuccessful = true,
                                    AccountNumber = beneficiary.BenAccountNumber,
                                    RefId = ""
                                };


                                transactionResponseMessages.Add(Response2);

                            }
                            else
                            {
                                beneficiary.Posted = false;
                                beneficiary.TransRef = Guid.NewGuid().ToString();
                                beneficiary.CBAResponse = "Failed";
                                beneficiary.CBAResponseCode = "x03";
                                // beneficiary.ApplicableCharge = responseObj2.ChargeAmt;

                                updateResponse = _transactionBusiness.UpdateBeneficiaryForPostStatus(beneficiary);

                                var Response2 = new TransactionResponseMessages()
                                {
                                    rowNumber = count++,
                                    msgId = result + "",
                                    responseMessage = "Failed",
                                    isSuccessful = false,
                                    AccountNumber = beneficiary.BenAccountNumber,
                                    RefId = ""
                                };
                                transactionResponseMessages.Add(Response2);
                            }

                            //var newbeneficiary = new TransactionBeneficiaries()
                            //{
                            //    SN = 
                            //};
                            //TransactionBeneficiary.Add(newbeneficiary);
                        }
                        TransactionApprovalModel Transaction = new TransactionApprovalModel();
                        Transaction.TranId = transMaster.TranId;
                        Transaction.ApprovedBy = transMaster.ApprovedBy;
                        Transaction.TReference = Guid.NewGuid().ToString();

                        var results = _transactionBusiness.ApproveTransaction(Transaction, Transaction.TReference);
                        return Ok(results);



                    }
                    else
                    {
                        int removalResponse = _transactionBusiness.RemoveAlreadyCreatedTransaction(result);

                        var Response2 = new TransactionResponseMessages()
                        {
                            rowNumber = 1,
                            msgId = result + "",
                            responseMessage = "Failed",
                            isSuccessful = false,
                            AccountNumber = "",
                            RefId = ""
                        };
                        transactionResponseMessages.Add(Response2);
                        return Ok(new
                        {
                            success = false,
                            message = "GL posting Failed",
                            TransRef = "",
                            result = transactionResponseMessages
                        });
                    }

                }

            }
            catch (Exception ex)
            {
                Utils.LogNO("Transfer Processing Error: " + ex.Message);
                return Ok(new
                {
                    success = false,
                    message = ex.Message,
                    TransRef = "",
                    result = transactionResponseMessages
                });
            }

            Utils.LogNO("Post Transfer details Ended!");
        }


        [HttpPost, Route("api/PostTransferDetailsOld/TransDeposit")]
        public IHttpActionResult PostTransferDetailsOld([FromBody]TransactionModel transMaster)
        {
            Utils.LogNO("api/PostTransferDetailsOld/TransDeposit:21:Currently Inside This End Point.");
            List<TransactionResponseMessages> transactionResponseMessages = new List<TransactionResponseMessages>();
            List<TransactionBeneficiaries> TransactionBeneficiary = new List<TransactionBeneficiaries>();
            List<TransferDetails> Beneficiaries = new List<TransferDetails>();
            try
            {
                transMaster.CBA = transMaster.CBA;
                long result = 0;
                string url = null;
                int updateResponse = 0;
                int count = 0;

                Utils.LogNO("Post Transfer details Started....");

                url = System.Configuration.ConfigurationManager.AppSettings["INHOUSETRANSFER"];

                var transType = (int)TransType.TransactionType.InHouseTransfer;
                transMaster.TransType = transType.ToString();
                var GLAccounts = _transactionBusiness.FetchGLAccounts(transMaster.BranchCode);
                try
                {
                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Posting/Creating Inhouse Transfer transaction on T24", transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }
                if (isDemo == "false")
                {
                    result = _transactionBusiness.CreateTransaction(transMaster,3);

                    //1. First Leg Posting to Central Accounts
                    var responseObj = CreateTransferTransactionForGL(transMaster, result, url, GLAccounts);

                    
                    //2. Second Leg Posting to Individuals
                    if (responseObj.FTResponse.ResponseCode == "00")
                    {
                        transMaster.Posted = true;
                        transMaster.CBACode = responseObj.FTResponse.ResponseCode;
                        transMaster.CBAResponse = responseObj.FTResponse.ResponseText;
                        transMaster.TransRef = responseObj.FTResponse.ReferenceID;

                        var rUpdate = _transactionBusiness.UpdateStatusForMasterTrans(transMaster);
                        //retrieve beneficiaries from Db cos the ID will be needed for update
                        Beneficiaries = _transactionBusiness.RetrieveBeneficiaries(result);

                        if (Beneficiaries != null)
                        {
                            //transactionResponseMessages = ProcessBeneficiaryPosting(Beneficiaries, transMaster, result, url, GLAccounts);
                            System.Threading.ThreadStart tAPIStarts =
                            delegate
                            {
                                var rResult = BeneficiaryPostingToCBA(Beneficiaries, transMaster, result, url, GLAccounts);
                            };

                            new System.Threading.Thread(tAPIStarts).Start();
                        }

                        var Response = new TransactionResponseMessages()
                        {
                            rowNumber = count++,
                            msgId = result + "",
                            responseMessage = "Successful",
                            isSuccessful = true,
                            AccountNumber = "",
                            RefId = responseObj.FTResponse.ReferenceID
                        };


                        transactionResponseMessages.Add(Response);

                        return Ok(new
                        {
                            success = true,
                            message = "",
                            TransRef = responseObj.FTResponse.ReferenceID,
                            result = transactionResponseMessages
                        });

                    }
                    else
                    {
                        int removalResponse = _transactionBusiness.RemoveAlreadyCreatedTransaction(result);
                        
                        var Response = new TransactionResponseMessages()
                        {
                            rowNumber = 1,
                            msgId = result+"",
                            responseMessage = responseObj.FTResponse.ResponseText+"("+ responseObj.FTResponse.ResponseText + ")",
                            isSuccessful = false,
                            AccountNumber = responseObj.FTResponse.FTID,
                            RefId = responseObj.FTResponse.ReferenceID
                        };
                        transactionResponseMessages.Add(Response);
                        return Ok(new
                        {
                            success = false,
                            message = "GL Posting Failed",
                            TransRef = "",
                            result = transactionResponseMessages
                        });
                    }
                }
                else
                {
                    //isDemo        
                    transMaster.TransRef = Guid.NewGuid().ToString();
                    result = _transactionBusiness.CreateTransaction(transMaster,3);

                    //1. First Leg Posting to Central Accounts
                    
                    //2. Second Leg Posting to Individuals
                    if (result > 0)
                    {
                        //retrieve beneficiaries from Db cos the ID will be needed for update
                        //Pass to background thread
                        Beneficiaries = _transactionBusiness.RetrieveBeneficiaries(result);
                        foreach (var beneficiary in Beneficiaries)
                        {
                            // var responseObj2 = CreateTransferTransaction(transMaster, beneficiary, ref result, url);
                            var isValid = _transactionBusiness.ValidateAccountNumber(beneficiary.BenAccountNumber);
                            if (isValid)
                            {
                                beneficiary.Posted = true;
                                beneficiary.TransRef = Guid.NewGuid().ToString();
                                beneficiary.CBAResponse = "Successful";
                                beneficiary.CBAResponseCode = "00";
                               // beneficiary.ApplicableCharge = responseObj2.ChargeAmt;

                                updateResponse = _transactionBusiness.UpdateBeneficiaryForPostStatus(beneficiary);

                                var Response2 = new TransactionResponseMessages()
                                {
                                    rowNumber = count++,
                                    msgId = result + "",
                                    responseMessage = "Successful",
                                    isSuccessful = true,
                                    AccountNumber = beneficiary.BenAccountNumber,
                                    RefId = ""
                                };


                                transactionResponseMessages.Add(Response2);
                                
                            }
                            else
                            {
                                beneficiary.Posted = false;
                                beneficiary.TransRef = Guid.NewGuid().ToString();
                                beneficiary.CBAResponse = "Failed";
                                beneficiary.CBAResponseCode = "x03";
                               // beneficiary.ApplicableCharge = responseObj2.ChargeAmt;

                                updateResponse = _transactionBusiness.UpdateBeneficiaryForPostStatus(beneficiary);

                                var Response2 = new TransactionResponseMessages()
                                {
                                    rowNumber = count++,
                                    msgId = result + "",
                                    responseMessage = "Failed",
                                    isSuccessful = false,
                                    AccountNumber = beneficiary.BenAccountNumber,
                                    RefId = ""
                                };
                                transactionResponseMessages.Add(Response2);
                            }
                           
                            //var newbeneficiary = new TransactionBeneficiaries()
                            //{
                            //    SN = 
                            //};
                            //TransactionBeneficiary.Add(newbeneficiary);
                        }
                        return Ok(new
                        {
                            success = true,
                            message = "",
                            TransRef = result,
                            result = transactionResponseMessages
                        });


                    }
                    else
                    {
                        int removalResponse = _transactionBusiness.RemoveAlreadyCreatedTransaction(result);

                        var Response2 = new TransactionResponseMessages()
                        {
                            rowNumber = 1,
                            msgId = result + "",
                            responseMessage = "Failed",
                            isSuccessful = false,
                            AccountNumber = "",
                            RefId = ""
                        };
                        transactionResponseMessages.Add(Response2);
                        return Ok(new
                        {
                            success = false,
                            message = "GL posting Failed",
                            TransRef = "",
                            result = transactionResponseMessages
                        });
                    }

                }
              
             }
            catch (Exception ex)
            {
                Utils.LogNO("Transfer Processing Error: " + ex.Message);
                return Ok(new
                {
                    success = false,
                    message = ex.Message,
                    TransRef = "",
                    result = transactionResponseMessages
                });
            }

            Utils.LogNO("Post Transfer details Ended!");
        }

        private List<TransactionResponseMessages> ProcessBeneficiaryPosting(List<TransferDetails> Beneficiaries, TransactionModel transMaster, long result, string url, BranchAccounts GLAccounts)
        {
            List<TransactionResponseMessages> results = new List<TransactionResponseMessages>();
            System.Threading.ThreadStart tAPIStarts =
                    delegate
                    {
                        results = BeneficiaryPostingToCBA(Beneficiaries, transMaster, result, url, GLAccounts);
                    };

            new System.Threading.Thread(tAPIStarts).Start();


            return results;

        }

        private TransactionResponseMessages SingleBeneficiaryPostingToCBA(TransferDetails beneficiary, TransactionModel transMaster,
            long result, string url) 
        {
            TransactionResponseMessages transactionResponseMessage =new TransactionResponseMessages();
            List<TransactionBeneficiaries> TransactionBeneficiary = new List<TransactionBeneficiaries>();
            var transHeader = new TransferHeader();

            if (string.IsNullOrEmpty(transMaster.ChargeType))
            {
                transHeader = _transactionBusiness.RetrieveTransferHeader(result);
                transMaster.ChargeType = transHeader.ChargeType;
            }
            //need to batch into branches to process
                Utils.LogNO("SINGLE BEN POSTING 2CBA: Beneficiary Details: " + JsonConvert.SerializeObject(beneficiary));
                var accountUrl = System.Configuration.ConfigurationManager.AppSettings["NameEnquiry"] + "/" + beneficiary.BenAccountNumber;
                transMaster.CurrencyAbbrev = Transaction.GetAccountCurrency(accountUrl, transMaster.access_token);
                var responseObj2 = CreateSingleTransferTransaction(transMaster, beneficiary, ref result, url);
                Utils.LogNO("SINGLE BEN POSTING 2CBA - Executed transaction: ");
                Utils.LogNO("Transaction Response: " + JsonConvert.SerializeObject(responseObj2));
                if (responseObj2.isSuccessful == true)
                {
                    beneficiary.Posted = true;
                    beneficiary.TransRef = responseObj2.RefId;
                    beneficiary.CBAResponse = responseObj2.responseMessage;
                    beneficiary.CBAResponseCode = responseObj2.responseCode;
                    beneficiary.ApplicableCharge = responseObj2.ChargeAmt;

                    int updateResponse = _transactionBusiness.UpdateBeneficiaryForPostStatus(beneficiary);
                    transMaster.Posted = true;
                    transMaster.Status = 2;
                    transMaster.CBACode = responseObj2.responseCode;
                    transMaster.CBAResponse = responseObj2.responseMessage;
                    transMaster.TransRef = responseObj2.RefId;
                    transMaster.CHARGEAMT = Convert.ToDecimal(responseObj2.ChargeAmt);
                    transMaster.TranId = result;
                  var rUpdate = _transactionBusiness.UpdateStatusForMasterTrans(transMaster);
                 }
                else
                {
                    beneficiary.Posted = false;
                    beneficiary.TransRef = responseObj2.RefId;
                    beneficiary.CBAResponse = responseObj2.responseMessage;
                    beneficiary.CBAResponseCode = responseObj2.responseCode;
                    beneficiary.ApplicableCharge = responseObj2.ChargeAmt;

                    int updateResponse = _transactionBusiness.UpdateBeneficiaryForPostStatus(beneficiary);
                }
            transactionResponseMessage = responseObj2;

              //  transactionResponseMessages.Add(responseObj2);

            return transactionResponseMessage;
        }

        private List<TransactionResponseMessages> BeneficiaryPostingToCBA(List<TransferDetails> Beneficiaries, TransactionModel transMaster,
            long result, string url, BranchAccounts GLAccounts)
        {
            List<TransactionResponseMessages> transactionResponseMessages = new List<TransactionResponseMessages>();
            List<TransactionBeneficiaries> TransactionBeneficiary = new List<TransactionBeneficiaries>();
            var transHeader = new TransferHeader();
          
            if (string.IsNullOrEmpty(transMaster.ChargeType))
            {
                transHeader = _transactionBusiness.RetrieveTransferHeader(result);
                transMaster.ChargeType = transHeader.ChargeType;
            }
            //need to batch into branches to process
            foreach (var beneficiary in Beneficiaries)
            {
                Utils.LogNO("MULTI BEN POSTING 2CBA: Beneficiary Details: " + JsonConvert.SerializeObject(beneficiary));
                var accountUrl = System.Configuration.ConfigurationManager.AppSettings["NameEnquiry"] + "/" + beneficiary.BenAccountNumber;
                transMaster.CurrencyAbbrev = Transaction.GetAccountCurrency(accountUrl, transMaster.access_token);
                var responseObj2 = CreateTransferTransaction(transMaster, beneficiary, ref result, url, GLAccounts);
                Utils.LogNO("Executed transaction: ");
                Utils.LogNO("Transaction Response: " + JsonConvert.SerializeObject(responseObj2));
                if(responseObj2.isSuccessful == true)
                {
                    beneficiary.Posted = true;
                    beneficiary.TransRef = responseObj2.RefId;
                    beneficiary.CBAResponse = responseObj2.responseMessage;
                    beneficiary.CBAResponseCode = responseObj2.responseCode;
                    beneficiary.ApplicableCharge = responseObj2.ChargeAmt;

                    int updateResponse = _transactionBusiness.UpdateBeneficiaryForPostStatus(beneficiary);
                }
                else
                {
                    beneficiary.Posted = false;
                    beneficiary.TransRef = responseObj2.RefId;
                    beneficiary.CBAResponse = responseObj2.responseMessage;
                    beneficiary.CBAResponseCode = responseObj2.responseCode;
                    beneficiary.ApplicableCharge = responseObj2.ChargeAmt;

                    int updateResponse = _transactionBusiness.UpdateBeneficiaryForPostStatus(beneficiary);
                }
               

                transactionResponseMessages.Add(responseObj2);

              
            }

            return transactionResponseMessages;
        }

        //InHouseChequesViewModel
        [HttpPost, Route("api/PostInHouseCheques/TransDeposit")]
        public IHttpActionResult PostInHouseChequesWithdrawal([FromBody]TransactionModel transMaster)
        {
            Utils.LogNO("api/PostInHouseCheques/TransDeposit:21:Currently Inside This End Point.");
            try
            {
                transMaster.CBA = transMaster.CBA;
                object result = null;
                string url = null;
               
                Utils.LogNO("Post Inhouse Cheques Started");
              
                    url = System.Configuration.ConfigurationManager.AppSettings["INHOUSECHEQUES"];
                   
                    var transType = (int)TransType.TransactionType.InHouseChequesDeposit;
                    transMaster.TransType = transType.ToString();

                try
                {
                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Posting/Creating Inhouse cheque Deposit on T24", transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }

                if (transMaster.ChequeNo.Length !=8)
                {
                    result = new
                    {
                        success = false,
                        message = "This transaction is not valid for cheque operation!",
                        TransactionRef = ""
                    };
                    return Ok(result);
                }
              
                if (isDemo == "false")
                {
                    var narration = _transactionBusiness.BuildNarration(transMaster.ChequeNo,
                        transMaster.AccountName, "", transMaster.Narration, transMaster.AccountName, 2);
                    Utils.LogNO("Executed narration " + narration);
                    var tresponse = new FTChqResponse();
                    //Creating and posting InHouse Cheques
                     tresponse = CreateCBAInHouseCheques(transMaster, url, narration);
                    if (tresponse.ResponseCode == "00")
                    {
                        transMaster.Posted = true;
                        // var MachineName = DetermineCompName(GetIPAddress());
                        transMaster.TransRef = tresponse.ReferenceID;
                        transMaster.CBACode = tresponse.ResponseCode;
                        transMaster.CBAResponse = tresponse.ResponseText;
                        transMaster.Status = 2;
                        transMaster.MachineName = "";
                        transMaster.Approved = true;
                        transMaster.CHARGEAMT = Convert.ToDecimal(tresponse.CHARGEAMT);
                        result = _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster, 2);
                        result = new
                        {
                            success = true,
                            message = "Successful",
                            TransactionRef = tresponse.ReferenceID
                        };
                        return Ok(result);
                    }
                    else
                    {
                        if (tresponse.ResponseCode == "x03")
                        {
                            string errorMsg = tresponse.ResponseText;
                            errorMsg = errorMsg.ToLower().Contains("cheque was already cleared") ? "Cheque was already cleared" : errorMsg;
                            result = new
                            {
                                success = false,
                                message = errorMsg,
                                TransactionRef = "",
                                Result = result
                            };
                            return Ok(result);

                        }
                        result = new
                        {
                            success = false,
                            message = tresponse.ResponseText,
                            TransactionRef = "",
                            Result = result
                        };
                        return Ok(result);
                    }  
                    }
                else
                {
                    transMaster.Posted = true;
                    transMaster.TransRef = Guid.NewGuid().ToString();
                    transMaster.Approved = true;
                    transMaster.Status = 2;
                    // var MachineName = DetermineCompName(GetIPAddress());
                    transMaster.MachineName = "";
                    result = _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,2);
                    return Ok(new
                    {
                        success = true,
                        message = "Transaction was committed successfully!",
                        TransactionRef = transMaster.TransRef,
                        Result = result
                    });
                }
            }
            catch (Exception ex)
            {
                Utils.LogNO(string.Format("Error Message for Inhouse Cheque: {0}", ex));
                return Ok(new
                {
                    success = false,
                    message = ex.Message,
                    TransactionRef = ""
                });
            }

        }


       




        private static OutputResponse CreateCBACashWithdrawal(TransactionModel transMaster, string url, string narration)
        {
            
            OutputResponse response;
            TellerRequest tellerRequest = new TellerRequest();
            Utils.LogNO("Create CBA CashWithdrawal.........");
            Utils.LogNO("Not till transfer");
            Utils.LogNO("Amount " + transMaster.Amount);
            tellerRequest.Teller = new Teller();
            tellerRequest.Teller.ChequeNo = transMaster.ChequeNo;
            tellerRequest.Teller.access_token = transMaster.access_token;
            tellerRequest.Teller.Amt = String.Format("{0:0.00}", transMaster.Amount);
            Utils.LogNO("Executed Amount 50");
            tellerRequest.Teller.InitiatorName = transMaster.InitiatorName;
            Utils.LogNO("Executed InitiatorName " + transMaster.InitiatorName);
            tellerRequest.Teller.CustAcct = transMaster.CustomerAcctNos;
            Utils.LogNO("Executed AccountNo " + transMaster.CustomerAcctNos);
            tellerRequest.Teller.Narration = transMaster.Remark;
            Utils.LogNO("Executed narration " + narration);
            tellerRequest.Teller.TellerId = transMaster.TellerId;
            Utils.LogNO("Executed TellerId " + transMaster.TellerId);
            tellerRequest.Teller.TransactionBranch = transMaster.Branch; //"NG0020006";//transMaster.Branch;
            Utils.LogNO("Executed Branch " + transMaster.Branch);
            tellerRequest.Teller.TxnCurr = transMaster.CurrCode;
            Utils.LogNO("Executed CurrCode " + transMaster.CurrCode);
            tellerRequest.Teller.Rate = "";
            tellerRequest.Teller.TransferParty = transMaster.TransactionParty; //"A";
            response = Transaction.CashWithDrawal(url, tellerRequest);
            return response;
        }

        private static FTChqResponse CreateCBAInHouseCheques(TransactionModel transMaster, string url, string narration)
        {
            // FTOutputResponse response;
            Utils.LogNO("CreateCBAInHouseChq....");
            InHouseChequesViewModel inHouseCheques = new InHouseChequesViewModel();
            inHouseCheques.FT_Request = new FT_Request();
            // response = null;
            url = System.Configuration.ConfigurationManager.AppSettings["INHOUSECHEQUES"];
            string InHouseChequeTranType = System.Configuration.ConfigurationManager.AppSettings["InHouseChequeTranType"];
            inHouseCheques.FT_Request.TransactionBranch = transMaster.Branch; //"NG0020006";
            inHouseCheques.FT_Request.TransactionType = InHouseChequeTranType;//transMaster.TransType;
            inHouseCheques.FT_Request.DebitAcctNo = transMaster.AccountNo;
            inHouseCheques.FT_Request.DebitCurrency = transMaster.CurrencyAbbrev;
            inHouseCheques.FT_Request.CreditCurrency = transMaster.CurrencyAbbrev;

            inHouseCheques.FT_Request.DebitAmount = transMaster.TotalAmt+"";
            inHouseCheques.FT_Request.CreditAccountNo = transMaster.CustomerAcctNos;
            inHouseCheques.FT_Request.VtellerAppID = System.Configuration.ConfigurationManager.AppSettings["VtellerAppID"];
            inHouseCheques.FT_Request.narrations = narration.Length <= 34 ? narration : narration.Substring(0, 34);
            inHouseCheques.FT_Request.ChequeNumber = transMaster.ChequeNo;
            inHouseCheques.FT_Request.SessionId = transMaster.access_token;
            inHouseCheques.FT_Request.TrxnLocation = string.IsNullOrEmpty(transMaster.TransationLocation) ? "Lagos" : transMaster.TransationLocation;
            inHouseCheques.FT_Request.SessionId = transMaster.access_token;

            var tResponse = Transaction.PostInHouseCheque(url, inHouseCheques, transMaster.access_token);
            return tResponse;
        }

        [HttpPost, Route("api/Create/TempCashWithdrawal")]
        public IHttpActionResult PostCashWithdrawalTemp([FromBody]TransactionModel transMaster)
        {
            Utils.LogNO("api/Create/TempCashWithdrawal:22:Currently Inside This End Point.");
            try
            {
                Utils.LogNO("Type B- cashwithdrawal with or without cheque HERE...... TransType:" + transMaster.TransType + ", ChargeType: " + transMaster.ChargeType + ", CheNo:" + transMaster.ChequeNo+ ", amount:"+ transMaster.Amount);
                var transType = (int)TransType.TransactionType.Withdrawal;
               
               
                    if (transMaster.TransType == "Counter")
                    {
                        transType = (int)TransType.TransactionType.CashWithDrawalCounter;
                    }
                    if (transMaster.ChequeNo.Trim().Length == 8)
                    {
                        if (transMaster.TransType == "Counter")
                        {

                        transType = (int)TransType.TransactionType.CashWithDrawalCounter;
                            transMaster.TransType = transType.ToString();
                        Utils.LogNO("Counter Here: With counter!");

                        }
                        else
                        {

                        Utils.LogNO("Counter Here: With Cheque only!");
                        transMaster.TransType = "";
                            try
                            {
                            // var ctransType = (int)TransType.TransactionType.ChequeLodgement;
                            transType = ((int)TransType.TransactionType.ChequeLodgement);
                            }catch(Exception ex)
                            {
                                Utils.LogNO("Error: Counter Here: With Cheque only!"+ ex.Message);
                            }
                        }

                    
                }
                transMaster.TransType = transType.ToString();
                transMaster.CBAResponse = "";
                transMaster.Posted = false;
                transMaster.CBA = "T24";
                transMaster.ApprovedBy = "";
                transMaster.Status = 1;
                object result = null;
                transMaster.NeededApproval = true;
                Utils.LogNO("Saving Cash Withdrawal- trans for Approval Started");
                result = _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,3);

                try
                {
                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed Cashwithdrawal transaction of " + transMaster.TotalAmt + " from " + transMaster.CustomerAcctNos + " account"
                        , transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }

                return Ok(result);

            }
            catch (Exception ex)
            {
                var result = _transactionBusiness.GetWithdrawalTrans(false, ex.Message, ex);
                return Ok(result);
            }
        }

        [HttpPost, Route("api/Create/Imal/TempCashWithdrawal")]
        public IHttpActionResult PostImalCashWithdrawalTemp([FromBody]TransactionModel transMaster) 
        {
            Utils.LogNO("api/Create/Imal/TempCashWithdrawal:23:Currently Inside This End Point.");
            try
            {
                var transType = (int)TransType.TransactionType.Withdrawal;
                if(transMaster.ChequeNo.Length > 0)
                {
                    transType = (int)TransType.TransactionType.ChequeLodgement;
                }
                var currencyCode = System.Configuration.ConfigurationManager.AppSettings["CurrencyCode"];
                transMaster.TransType = transType.ToString();
                transMaster.CBAResponse = "";
                transMaster.Posted = false;
                transMaster.CBA = "IMAL";
                transMaster.ApprovedBy = "";
                transMaster.Status = 1;
                object result = null;
                transMaster.NeededApproval = true;
                transMaster.TellerId = transMaster.BranchCode + currencyCode +
                    System.Configuration.ConfigurationManager.AppSettings["SMLedgerCode"] + transMaster.SMCIFNumber + "000";
                transMaster.BranchCode = transMaster.Branch;
                transMaster.Remarks = BuildNarration("", transMaster.Beneficiary, "", transMaster.Remark, transMaster.Beneficiary, 3);
                Utils.LogNO("Saving Cash Withdrawal- trans for Approval Started");
                result = _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,3);

                try
                {
                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed Cashwithdrawal transaction of " + transMaster.TotalAmt + " from " + transMaster.CustomerAcctNos + " account"
                        , transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }

                return Ok(result);

            }
            catch (Exception ex)
            {
                var result = _transactionBusiness.GetWithdrawalTrans(false, ex.Message, ex);
                return Ok(result);
            }
        }

        [HttpPost, Route("api/Create/TempInHouseTransfer")]
        public IHttpActionResult PostCashInHouseTransferTemp([FromBody]TransactionModel transMaster)
        {
            Utils.LogNO("api/Create/TempInHouseTransfer:23:Currently Inside This End Point.");
            try
            {
                var transType = (int)TransType.TransactionType.InHouseTransfer;
                transMaster.TransType = transType.ToString();
                transMaster.CBAResponse = "";
                transMaster.Posted = false;
                transMaster.CBA = "T24";
                transMaster.ApprovedBy = "";
                transMaster.Status = 1;
                object result = null;
                transMaster.NeededApproval = true;
                Utils.LogNO("Saving Inhouse Transfer- trans pending for Approval Started");
                result = _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,1);

                try
                {
                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed Inhouse Transfer transaction of " + transMaster.TotalAmt + " from " + transMaster.CustomerAcctNos + " account"
                        , transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }

                return Ok(result);

            }
            catch (Exception ex)
            {
                var result = _transactionBusiness.GetWithdrawalTrans(false, ex.Message, ex);
                return Ok(result);
            }
        }

        [HttpPost, Route("api/Create/TempInHouseChequeTransfer")]
        public IHttpActionResult PostCashInHouseChequeTransferTemp([FromBody]TransactionModel transMaster)
        {
            Utils.LogNO("api/Create/TempInHouseChequeTransferr:24:Currently Inside This End Point.");
            try
            {
            
                var transType = (int)TransType.TransactionType.InHouseChequesDeposit;
                transMaster.TransType = transType.ToString();
                transMaster.CBAResponse = "";
                transMaster.Posted = false;
                transMaster.CBA = "T24";
                transMaster.ApprovedBy = "";
                transMaster.Status = 1;
                object result = null;
                transMaster.NeededApproval = true;
                Utils.LogNO("Saving Inhouse CCheque- trans pending for Approval Started");
                result = _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,2);

                try
                {
                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed Inhouse Cheque Deposit transaction of " + transMaster.TotalAmt + " from " + transMaster.CustomerAcctNos + " account"
                        , transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }

                return Ok(result);

            }
            catch (Exception ex)
            {
                var result = _transactionBusiness.GetWithdrawalTrans(false, ex.Message, ex);
                return Ok(result);
            }
        }


        [HttpPost, Route("api/Create/TempCashDeposit")]
        public IHttpActionResult PostCashDepositTemp([FromBody]TransactionModel transMaster)
        {
            try
            {
                Utils.LogNO("api/Create/TempCashDeposit:25:Currently Inside This End Point.");
                int transType = 0;

                if (transMaster.ChequeNo.Length == 8)
                {
                    transType = (int)TransType.TransactionType.ChequeLodgement;
                    transMaster.TransType = transType.ToString();
                }
                else
                {
                    transType = (int)TransType.TransactionType.Deposit;
                    transMaster.TransType = transType.ToString();
                }

                transMaster.CBAResponse = "";
                transMaster.Posted = false;
                transMaster.Status = 1;
                transMaster.CBA = transMaster.CBA == "IMAL" ? "IMAL" : "T24";
                transMaster.Status = 1;
                object result = null;
                transMaster.NeededApproval = true;
                Utils.LogNO("Saving Cash Deposit- trans for Approval Started");
                result = _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,5);
                try
                {
                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed Cash deposit transaction of " + transMaster.TotalAmt + " to " + transMaster.CustomerAcctNos + " account"
                        , transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }
                return Ok(result);

            }
            catch (Exception ex)
            {
                var result = _transactionBusiness.GetWithdrawalTrans(false, ex.Message, ex);
                return Ok(result);
            }
        }

        [HttpPost, Route("api/Create/IMAL/TempCashDeposit")]
        public IHttpActionResult PostIMALCashDepositTemp([FromBody]TransactionModel transMaster) 
        {
            Utils.LogNO("api/Create/IMAL/TempCashDeposit:26:Currently Inside This End Point.");
            try
            {
                int transType = 0;

                if (transMaster.ChequeNo.Length == 8)
                {
                    transType = (int)TransType.TransactionType.ChequeLodgement;
                    transMaster.TransType = transType.ToString();
                }
                else
                {
                    transType = (int)TransType.TransactionType.Deposit;
                    transMaster.TransType = transType.ToString();
                }
                var currencyCode = System.Configuration.ConfigurationManager.AppSettings["CurrencyCode"];
                transMaster.CBAResponse = "";
                transMaster.Posted = false;
                transMaster.Status = 1;
                transMaster.CBA = "IMAL" ;
                transMaster.Status = 1;
                transMaster.TellerId = transMaster.BranchCode + currencyCode +
                   System.Configuration.ConfigurationManager.AppSettings["SMLedgerCode"] + transMaster.SMCIFNumber + "000";
                transMaster.BranchCode = transMaster.Branch;
                object result = null;
                transMaster.NeededApproval = true;
                transMaster.Remarks = BuildNarration("", transMaster.Beneficiary, "", transMaster.Remark, transMaster.Beneficiary, 2);
                Utils.LogNO("Saving Cash Deposit- trans for Approval Started");
                result = _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,5);
                try
                {
                    transMaster.MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed Cash deposit transaction of " + transMaster.TotalAmt + " to " + transMaster.CustomerAcctNos + " account"
                        , transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }
                return Ok(result);

            }
            catch (Exception ex)
            {
                var result = _transactionBusiness.GetWithdrawalTrans(false, ex.Message, ex);
                return Ok(result);
            }
        }


        [HttpPost, Route("api/ImalTransaction/Create/VaultIn")]
        public IHttpActionResult VaultInImal([FromBody]TransactionModel transMaster)
        {
            Utils.LogNO("api/ImalTransaction/Create/VaultIn:26:Currently Inside This End Point.");
            try
            {
                string  branchCode = transMaster.BranchCode + "";
                Utils.LogNO("details " + JsonConvert.SerializeObject(transMaster));
                transMaster.CBA = "IMAL";
                var transType = (int)TransType.TransactionType.vaultIn;
                transMaster.TransType = transType.ToString();
                object result = null;
                int transID = 0;
                int secondTranID = 0;
                TellerReversal tellerReversal = new TellerReversal();
                if (isDemo == "false")
                {
                    string url = null;
                    var currencyCode = System.Configuration.ConfigurationManager.AppSettings["CurrencyCode"];
                    url = System.Configuration.ConfigurationManager.AppSettings["FundTransferNarrExt"];
                    FundTransferReversalModel request = new FundTransferReversalModel();
                    request.FT_Request = new FTRequestFundReversal();
                    request.FT_Request.TransactionBranch = transMaster.Branch;

                    ImalFundTransferReversal imalFundTransferReversal = new ImalFundTransferReversal();
                    imalFundTransferReversal.branchCode = "1";
                    imalFundTransferReversal.oldReferenceCode = System.Configuration.ConfigurationManager.AppSettings["referenceCode"];
                    imalFundTransferReversal.principalIdentifier = System.Configuration.ConfigurationManager.AppSettings["principalIdentifier"];
                    imalFundTransferReversal.requestCode = System.Configuration.ConfigurationManager.AppSettings["requestCode"];

                    //1. A. Movement of Cash from T24 to IMAL VAULT
                    transMaster.TransType = System.Configuration.ConfigurationManager.AppSettings["IMALTransactionType"];
                   // transMaster.VaultAccount = transMaster.CurrencyAbbrev + transMaster.VaultAccount;
                   // transMaster.NIBCashSettlement = transMaster.CurrencyAbbrev + transMaster.NIBCashSettlement;
                    var fundTransferModel = _transactionBusiness.ConvertToFundTransferModelForVaultIn(transMaster);
                    fundTransferModel.FT_Request.VtellerAppID = System.Configuration.ConfigurationManager.AppSettings["VtellerAppID"];
                    Utils.LogNO("************Movement of Cash from T24 to IMAL VAULT transaction Begins****************");
                    var imalResponse = ImalService.FundTransferNar(url, fundTransferModel, transMaster.access_token);
                    if (imalResponse.FTResponse.ResponseCode == "00")
                    {
                        Utils.LogNO("**********Movement of Cash from T24 to IMAL VAULT transaction END****************");
                        IMALResponse.IMALLocalFTResponse iMALLocalFTResponse = new IMALResponse.IMALLocalFTResponse();
                        iMALLocalFTResponse.responseMessage = imalResponse.FTResponse.ResponseText;
                        iMALLocalFTResponse.iMALTransactionCode = imalResponse.FTResponse.ResponseCode;
                        iMALLocalFTResponse.responseCode = imalResponse.FTResponse.ResponseCode;
                        iMALLocalFTResponse.transactionID = imalResponse.FTResponse.FTID;
                       var trannsType = (int)TransType.TransactionType.vaultIn;
                        transMaster.TransType = trannsType.ToString();
                        transMaster.AccountNo = transMaster.NIBCashSettlement;
                       
                        transID = AddTransactionToDB(transMaster, iMALLocalFTResponse);
                        //2. 2nd leg.. Cash movement from T24 to IMAL Vault
                        url = System.Configuration.ConfigurationManager.AppSettings["ImalLocalFT"];
                        Utils.LogNO("Branch " + branchCode);
                        transMaster.TellerId = branchCode + currencyCode +  
                            System.Configuration.ConfigurationManager.AppSettings["CashSettlementLedgerCode"]
                        + "00000000" + "000";
                        transMaster.ToTellerId = branchCode + currencyCode +
                        System.Configuration.ConfigurationManager.AppSettings["VaultLedgerCode"]
                        + "00000000" + "000";
                        url = System.Configuration.ConfigurationManager.AppSettings["ImalLocalFT"];
                        LocalFT accountDetailsRequestModel = new LocalFT();
                        accountDetailsRequestModel.fromAccount = transMaster.TellerId.Replace("NG", "");
                        accountDetailsRequestModel.paymentReference = transMaster.Remark;//ook
                        accountDetailsRequestModel.amount = Math.Round(Convert.ToDecimal(transMaster.Amount) , 2) + "";
                        accountDetailsRequestModel.principalIdentifier = System.Configuration.ConfigurationManager.AppSettings["principalIdentifier"];
                        accountDetailsRequestModel.referenceCode = System.Configuration.ConfigurationManager.AppSettings["referenceCode"];
                        accountDetailsRequestModel.requestCode = System.Configuration.ConfigurationManager.AppSettings["requestCode"];
                        accountDetailsRequestModel.toAccount = transMaster.ToTellerId.Replace("NG", "");
                        Utils.LogNO("************Cash movement from T24 to IMAL Vault transaction Begins****************");
                        var responseImal = ImalService.LocalFT(url, accountDetailsRequestModel, transMaster.access_token);

                        if (responseImal.responseCode == "0")
                        {
                            Utils.LogNO("************Cash movement from T24 to IMAL Vault transaction ENDS****************");
                            trannsType = (int)TransType.TransactionType.vaultIn;
                            transMaster.TransType = trannsType.ToString();
                            transMaster.AccountNo = transMaster.NIBCashSettlement;
                            secondTranID = AddTransactionToDB(transMaster, responseImal);

                            //3.Debit IMAL vault Account, Credit SM Till
                            transMaster.TellerId = branchCode+ currencyCode +
                            System.Configuration.ConfigurationManager.AppSettings["SMLedgerCode"] +
                            transMaster.SMCIFNumber + "000";

                            transMaster.ToTellerId = branchCode + currencyCode +
                            System.Configuration.ConfigurationManager.AppSettings["VaultLedgerCode"]
                            + "00000000" + "000";

                            accountDetailsRequestModel.fromAccount = transMaster.TellerId.Replace("NG", "");
                            accountDetailsRequestModel.toAccount = transMaster.ToTellerId.Replace("NG", "");
                            Utils.LogNO("************.Debit IMAL vault Account, Credit SM Till Begins****************");
                            var response2Imal = ImalService.LocalFT(url, accountDetailsRequestModel, transMaster.access_token);
                            if (response2Imal.responseCode == "0")
                            {
                                Utils.LogNO("************.Debit IMAL vault Account, Credit SM Till End****************");
                                trannsType = (int)TransType.TransactionType.vaultIn;
                                transMaster.TransType = trannsType.ToString();
                                transMaster.AccountNo = transMaster.NIBCashSettlement;
                                AddTransactionToDB(transMaster, response2Imal);
                                result = new
                                {
                                    success = true,
                                    message = "Transaction successful",
                                    TransactionRef = responseImal.iMALTransactionCode
                                };
                                return Ok(result);
                            }
                            else
                            {
                                url = System.Configuration.ConfigurationManager.AppSettings["FTReversal"];
                                imalFundTransferReversal.transactionFTref = responseImal.iMALTransactionCode;
                                var response = ImalService.FTReversal(url, imalFundTransferReversal, transMaster.access_token);
                                Utils.LogNO("************REVERSING Movement of Cash from T24 to IMAL VAULT transaction****************");
                                if (response.errorCode == "0")
                                {
                                    Utils.LogNO("************REVERSED Movement of Cash from T24 to IMAL VAULT transaction****************");
                                    tellerReversal.TranId = transID;
                                    _transactionBusiness.ReverseTransaction(tellerReversal);

                                }
                                else
                                {
                                    var errormessage = "";
                                    errormessage = response2Imal.responseCode == "-1214" ? "Amount Exceeds Available Balance in Account" : "";
                                    result = new
                                    {
                                        success = false,
                                        message = errormessage,
                                        TransactionRef = ""
                                    };
                                    return Ok(result);
                                }
                                url = System.Configuration.ConfigurationManager.AppSettings["FundTransferReversal"];
                                request.FT_Request.FTReference = imalResponse.FTResponse.ReferenceID;
                                var responseFT = Transaction.FundTransferReversal(url, request, transMaster.access_token);
                                Utils.LogNO("************REVERSING Cash movement from T24 to IMAL Vault transaction****************");
                                if (responseFT.FTResponseExt.ResponseCode != "-1")
                                {
                                    Utils.LogNO("************REVERSED Cash movement from T24 to IMAL Vault transaction****************");
                                    tellerReversal.TranId = secondTranID;
                                    _transactionBusiness.ReverseTransaction(tellerReversal);
                                    var errormessage2= response2Imal.responseCode == "-1214" ? "Amount Exceeds Available Balance in Account" : response2Imal.responseCode;
                                    result = new
                                    {
                                        success = false,
                                        message = errormessage2,
                                        TransactionRef = ""
                                    };
                                    return Ok(result);

                                }
                                else
                                {
                                    result = new
                                    {
                                        success = false,
                                        message = responseFT.FTResponseExt.ResponseText,
                                        TransactionRef = ""
                                    };
                                    return Ok(result);
                                }

                            }
                        }
                        else
                        {
                            var errormessage = "";
                            errormessage = responseImal.responseCode == "-1214" ? "Amount Exceeds Available Balance in Account" : responseImal.responseCode;
                            url = System.Configuration.ConfigurationManager.AppSettings["FundTransferReversal"];
                            request.FT_Request.FTReference = imalResponse.FTResponse.ReferenceID;
                            var responseFT = Transaction.FundTransferReversal(url, request, transMaster.access_token);
                            Utils.LogNO("************REVERSING Movement of Cash from T24 to IMAL VAULT transaction****************");
                            var response = Transaction.FundTransferReversal(url, request, transMaster.access_token);
                            if (response.FTResponseExt.ResponseCode != "00")
                            {
                                Utils.LogNO("************REVERSED Movement of Cash from T24 to IMAL VAULT transaction****************");
                                tellerReversal.TranId = transID;
                                _transactionBusiness.ReverseTransaction(tellerReversal);
                            }
                            result = new
                            {
                                success = false,
                                message = errormessage,
                                TransactionRef = ""
                            };
                            return Ok(result);
                        }
                    }
                    else if (imalResponse.FTResponse.ResponseCode == "x03")
                    {
                        string error = null;
                        if (imalResponse.FTResponse.ResponseText.Contains("1/NO,OVERRIDE:1:1="))
                            error = error.Replace("1/NO,OVERRIDE:1:1=", "");
                        else error = imalResponse.FTResponse.ResponseText;
                        result = new
                        {
                            success = false,
                            message = error,
                            TransactionRef = ""
                        };
                        return Ok(result);
                    }
                    else
                    {
                        result = new
                        {
                            success = false,
                            message = imalResponse.FTResponse.ResponseText,
                            TransactionRef = ""
                        };
                        return Ok(result);
                    }
                }
                else
                {
                    transMaster.CBAResponse = "";
                    transMaster.CBACode = "";
                    transMaster.Posted = true;
                    transMaster.CBA = "IMAL";
                    transMaster.Approved = true;
                    transMaster.NeededApproval = false;
                    //IMALResponse.IMALLocalFTResponse iMAL = new IMALResponse.IMALLocalFTResponse();
                    //iMAL.iMALTransactionCode = "12345";
                    //iMAL.transactionID = "12345";
                    //iMAL.responseMessage = "SUCCESS";
                    //iMAL.responseCode = "00";
                    result = _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,3); // AddTransactionToDB(transMaster, iMAL);   //
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                var result = _transactionBusiness.GetAllTrans(false, ex.Message, ex);
                return Ok(result);
            }
        }
      


        [HttpPost, Route("api/ImalTransaction/Create/VaultOut")]
        public IHttpActionResult VaultOutImal([FromBody]TransactionModel transMaster)
        {
            Utils.LogNO("api/ImalTransaction/Create/VaultOut:27:Currently Inside This End Point.");
            try
            {
                var branchCode = transMaster.BranchCode;
                Utils.LogNO("details " + JsonConvert.SerializeObject(transMaster));
                transMaster.CBA = "IMAL";
                var transType = (int)TransType.TransactionType.VaultOut;
                transMaster.TransType = transType.ToString();
                object result = null;
                int transID = 0;
                int secondTranID = 0;
                TellerReversal tellerReversal = new TellerReversal();
                if (isDemo == "false")
                {
                    string url = null;
                    var currencyCode = System.Configuration.ConfigurationManager.AppSettings["CurrencyCode"];
                    url = System.Configuration.ConfigurationManager.AppSettings["FundTransferNarrExt"];
                    FundTransferReversalModel request = new FundTransferReversalModel();
                    request.FT_Request = new FTRequestFundReversal();
                    request.FT_Request.TransactionBranch = transMaster.Branch;
                    ImalFundTransferReversal imalFundTransferReversal = new ImalFundTransferReversal();
                    imalFundTransferReversal.branchCode = "1";
                    imalFundTransferReversal.oldReferenceCode = System.Configuration.ConfigurationManager.AppSettings["referenceCode"];
                    imalFundTransferReversal.principalIdentifier = System.Configuration.ConfigurationManager.AppSettings["principalIdentifier"];
                    imalFundTransferReversal.requestCode = System.Configuration.ConfigurationManager.AppSettings["requestCode"];

                    //1. A. Movement of Cash from T24 to IMAL VAULT
                    transMaster.TransType = System.Configuration.ConfigurationManager.AppSettings["IMALTransactionType"];
                   // transMaster.VaultAccount = transMaster.CurrencyAbbrev + transMaster.VaultAccount;
                    //transMaster.NIBCashSettlement = transMaster.CurrencyAbbrev + transMaster.NIBCashSettlement;
                    var fundTransferModel = _transactionBusiness.ConvertToFundTransferModel(transMaster);
                    fundTransferModel.FT_Request.VtellerAppID = System.Configuration.ConfigurationManager.AppSettings["VtellerAppID"];
                    Utils.LogNO("************Movement of Cash from T24 to IMAL VAULT transaction Begins****************");
                    var imalResponse = ImalService.FundTransferNar(url, fundTransferModel, transMaster.access_token);
                    if (imalResponse.FTResponse.ResponseCode == "00")
                    {
                        Utils.LogNO("**********Movement of Cash from T24 to IMAL VAULT transaction END****************");
                        IMALResponse.IMALLocalFTResponse iMALLocalFTResponse = new IMALResponse.IMALLocalFTResponse();
                        iMALLocalFTResponse.responseMessage = imalResponse.FTResponse.ResponseText;
                        iMALLocalFTResponse.iMALTransactionCode = imalResponse.FTResponse.ResponseCode;
                        iMALLocalFTResponse.responseCode = imalResponse.FTResponse.ResponseCode;
                        iMALLocalFTResponse.transactionID = imalResponse.FTResponse.FTID;
                      var trannsType = (int)TransType.TransactionType.VaultOut;
                        transMaster.TransType = trannsType.ToString();
                        transMaster.AccountNo = transMaster.NIBCashSettlement;
                        transID = AddTransactionToDB(transMaster, iMALLocalFTResponse);
                        if(transID == 0)
                        {
                            result = new
                            {
                                success = false,
                                message = "Transaction failed to commit",
                                TransactionRef = ""
                            };
                            return Ok(result);
                        }
                        //2. 2nd leg.. Cash movement from T24 to IMAL Vault
                        url = System.Configuration.ConfigurationManager.AppSettings["ImalLocalFT"];
                        Utils.LogNO("Branch " + transMaster.BranchCode);
                        transMaster.ToTellerId = transMaster.BranchCode + currencyCode +
                        System.Configuration.ConfigurationManager.AppSettings["VaultLedgerCode"] + "00000000" +
                        "000";
                        transMaster.TellerId = transMaster.BranchCode + currencyCode +
                        System.Configuration.ConfigurationManager.AppSettings["CashSettlementLedgerCode"]
                        + "00000000" + "000";
                        url = System.Configuration.ConfigurationManager.AppSettings["ImalLocalFT"];
                        LocalFT accountDetailsRequestModel = new LocalFT();
                        accountDetailsRequestModel.fromAccount = transMaster.TellerId.Replace("NG", ""); 
                        accountDetailsRequestModel.amount = transMaster.Amount + "";
                        accountDetailsRequestModel.beneficiaryName = "";
                        accountDetailsRequestModel.paymentReference = transMaster.Remark;//ook
                        accountDetailsRequestModel.principalIdentifier = System.Configuration.ConfigurationManager.AppSettings["principalIdentifier"];
                        accountDetailsRequestModel.referenceCode = System.Configuration.ConfigurationManager.AppSettings["referenceCode"];
                        accountDetailsRequestModel.requestCode = System.Configuration.ConfigurationManager.AppSettings["requestCode"];
                        accountDetailsRequestModel.toAccount = transMaster.ToTellerId.Replace("NG", "");
                        Utils.LogNO("************Cash movement from T24 to IMAL Vault transaction Begins****************");
                        var responseImal = ImalService.LocalFT(url, accountDetailsRequestModel, transMaster.access_token);
                        if (responseImal.responseCode == "0")
                        {
                            Utils.LogNO("************Cash movement from T24 to IMAL Vault transaction ENDS****************");
                             trannsType = (int)TransType.TransactionType.VaultOut;
                            transMaster.TransType = trannsType.ToString();
                         
                            secondTranID = AddTransactionToDB(transMaster, responseImal);

                            //3.Debit IMAL vault Account, Credit SM Till
                            transMaster.ToTellerId = branchCode + currencyCode +
                            System.Configuration.ConfigurationManager.AppSettings["SMLedgerCode"] +
                            transMaster.SMCIFNumber + "000";

                            transMaster.TellerId = branchCode + currencyCode +
                            System.Configuration.ConfigurationManager.AppSettings["VaultLedgerCode"]
                            + "00000000" + "000";

                            accountDetailsRequestModel.fromAccount = transMaster.TellerId.Replace("NG", "");
                            accountDetailsRequestModel.toAccount = transMaster.ToTellerId.Replace("NG", "");
                            Utils.LogNO("************.Debit IMAL vault Account, Credit SM Till Begins****************");
                            var response2Imal = ImalService.LocalFT(url, accountDetailsRequestModel, transMaster.access_token);
                            if (response2Imal.responseCode == "0")
                            {
                                Utils.LogNO("************.Debit IMAL vault Account, Credit SM Till End****************");
                                trannsType = (int)TransType.TransactionType.VaultOut;
                                transMaster.TransType = trannsType.ToString();
                                AddTransactionToDB(transMaster, response2Imal);
                                result = new
                                {
                                    success = true,
                                    message = "Transaction successful",
                                    TransactionRef = responseImal.iMALTransactionCode
                                };
                                return Ok(result);
                            }
                            else
                            {
                                url = System.Configuration.ConfigurationManager.AppSettings["FTReversal"];
                                imalFundTransferReversal.transactionFTref = responseImal.iMALTransactionCode;
                                var response = ImalService.FTReversal(url, imalFundTransferReversal, transMaster.access_token);
                                Utils.LogNO("************REVERSING Movement of Cash from T24 to IMAL VAULT transaction****************");
                                if (response.errorCode == "0")
                                {
                                    Utils.LogNO("************REVERSED Movement of Cash from T24 to IMAL VAULT transaction****************");
                                    tellerReversal.TranId = transID;
                                    _transactionBusiness.ReverseTransaction(tellerReversal);

                                }
                                else
                                {
                                    result = new
                                    {
                                        success = false,
                                        message = "Amount Exceeds Available Balance in Account",
                                        TransactionRef = ""
                                    };
                                    return Ok(result);
                                }
                                url = System.Configuration.ConfigurationManager.AppSettings["FundTransferReversal"];
                                request.FT_Request.FTReference = imalResponse.FTResponse.ReferenceID;
                                var responseFT = Transaction.FundTransferReversal(url, request, transMaster.access_token);
                                Utils.LogNO("************REVERSING Cash movement from T24 to IMAL Vault transaction****************");
                                if (responseFT.FTResponseExt.ResponseCode != "00")
                                {
                                    Utils.LogNO("************REVERSED Cash movement from T24 to IMAL Vault transaction****************");
                                    tellerReversal.TranId = secondTranID;
                                    _transactionBusiness.ReverseTransaction(tellerReversal);
                                }
                                result = new
                                {
                                    success = false,
                                    message = response2Imal.responseCode == "-1214" ? "Amount Exceeds the Available Balance in the Account!" : response2Imal.responseMessage,
                                    TransactionRef = ""
                                };
                                return Ok(result);
                            }
                        }
                        else
                        {
                            url = System.Configuration.ConfigurationManager.AppSettings["FundTransferReversal"];
                            request.FT_Request.FTReference = imalResponse.FTResponse.ReferenceID;
                            var responseFT = Transaction.FundTransferReversal(url, request, transMaster.access_token);
                            Utils.LogNO("************REVERSING Movement of Cash from T24 to IMAL VAULT transaction****************");
                            var response = Transaction.FundTransferReversal(url, request, transMaster.access_token);
                            if (response.FTResponseExt.ResponseCode != "00")
                            {
                                Utils.LogNO("************REVERSED Movement of Cash from T24 to IMAL VAULT transaction****************");
                                tellerReversal.TranId = transID;
                                _transactionBusiness.ReverseTransaction(tellerReversal);
                            }
                            result = new
                            {
                                success = false,
                                message = responseImal.responseCode == "-1214" ? "Amount Exceeds the Available Balance in the Account!" : responseImal.responseMessage,
                                TransactionRef = ""
                            };
                            return Ok(result);
                        }
                    }
                    else if (imalResponse.FTResponse.ResponseCode == "x03")
                    {
                        //string error = null;
                        //if (imalResponse.FTResponse.ResponseText.Contains("1/NO,OVERRIDE:1:1="))
                        // error = error.Replace("1/NO,OVERRIDE:1:1=", "");
                        //else error = imalResponse.FTResponse.ResponseText;
                        result = new
                        {
                            success = false,
                            message = imalResponse.FTResponse.ResponseText,
                            TransactionRef = ""
                        };
                        return Ok(result);
                    }
                    else
                    {
                        result = new
                        {
                            success = false,
                            message = imalResponse.FTResponse.ResponseCode,
                            TransactionRef = ""
                        };
                        return Ok(result);
                    }
                }
                else
                {
                    transMaster.CBAResponse = "";
                    transMaster.CBACode = "";
                    transMaster.Posted = true;
                    transMaster.CBA = "IMAL";
                    transMaster.NeededApproval = false;
                    transMaster.Approved = true;
                    transMaster.TransRef = Guid.NewGuid().ToString();
                    result = _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,3);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                var result = _transactionBusiness.GetAllTrans(false, ex.Message, ex);
                return Ok(result);
            }
        }
        private int AddTransactionToDB(TransactionModel transMaster, IMALResponse.IMALLocalFTResponse response2Imal)
        {
            int result;
            // transMaster.BranchCode = transMaster.Branch;
            Utils.LogNO("Add Trans 2DB...");
            transMaster.CBAResponse = response2Imal.responseMessage;
            transMaster.CBACode = string.IsNullOrEmpty(response2Imal.responseCode)?"00": response2Imal.responseCode;
            transMaster.Posted = true;
            transMaster.CBA = "IMAL";
            transMaster.Approved = true;
            transMaster.NeededApproval = false;
            transMaster.TransRef = response2Imal.iMALTransactionCode;
            result = _transactionBusiness.CreateTransactionForImal(transMaster);
            return result;
        }

        [HttpPost, Route("api/Create/VaultOut")]
        public IHttpActionResult VaultOut([FromBody]TransactionModel transMaster)
        {
            Utils.LogNO("api/Create/VaultOut:28:Currently Inside This End Point.");
            try
            {
                OutputResponse response = null;
                transMaster.CBA = "T24";
                if (transMaster.IsVaultIn == true)
                {
                    var transType = (int)TransType.TransactionType.vaultIn;
                    transMaster.TransType = transType.ToString();
                }
                else
                {
                    var transType = (int)TransType.TransactionType.VaultOut;
                    transMaster.TransType = transType.ToString();
                }
                var random = new Random(12);

                object result = null;
                if (isDemo == "false")
                {
                    if (transMaster.Approved)
                    {
                        string url = null;
                        if (transMaster.CurrCode == "NGN")
                        {
                            url = System.Configuration.ConfigurationManager.AppSettings["VaultOutLocal"];
                            TillTransferRequest request = new TillTransferRequest();
                            request.TiiTransferLCY = new TillTransferLCYModel();
                            request.TiiTransferLCY.access_token = transMaster.access_token;
                            if (transMaster.IsVaultIn)
                            {
                                request.TiiTransferLCY.fromteller = transMaster.TellerId;
                                request.TiiTransferLCY.toteller = transMaster.ToTellerId;
                            }
                            else
                            {
                                request.TiiTransferLCY.fromteller = transMaster.ToTellerId;
                                request.TiiTransferLCY.toteller = transMaster.TellerId;
                            }
                            request.TiiTransferLCY.TransactionBranch = transMaster.Branch; //"NG0020006";
                            request.TiiTransferLCY.txncurr = transMaster.CurrCode;
                            request.TiiTransferLCY.Narrative = transMaster.Remark;
                            request.TiiTransferLCY.amttotransfer = Convert.ToString(transMaster.Amount);
                            url = Utils.VaultUrl(transMaster.IsVaultIn, transMaster.CurrCode);
                            Utils.LogNO("CBA request: " + JsonConvert.SerializeObject(request) + " url: " + url);
                            response = VaultAPIService.VaultOutLocal(url, request);
                        }
                        else
                        {
                            url = Utils.VaultUrl(transMaster.IsVaultIn, transMaster.CurrCode);
                            TillTransferRequestForeign tillTransferRequestFCY = new TillTransferRequestForeign();
                            tillTransferRequestFCY.TiiTransferFCY = new TillTransferLCYModel();
                            tillTransferRequestFCY.TiiTransferFCY.amttotransfer = Convert.ToString(transMaster.Amount);
                            if (transMaster.IsVaultIn)
                            {
                                tillTransferRequestFCY.TiiTransferFCY.fromteller = transMaster.TellerId;
                                tillTransferRequestFCY.TiiTransferFCY.toteller = transMaster.ToTellerId;
                            }
                            else
                            {
                                tillTransferRequestFCY.TiiTransferFCY.fromteller = transMaster.ToTellerId;
                                tillTransferRequestFCY.TiiTransferFCY.toteller = transMaster.TellerId;
                            }
                            tillTransferRequestFCY.TiiTransferFCY.Narrative = transMaster.Remark;
                            tillTransferRequestFCY.TiiTransferFCY.TransactionBranch = transMaster.Branch; //"NG0020006";
                            tillTransferRequestFCY.TiiTransferFCY.txncurr = transMaster.CurrCode;
                            tillTransferRequestFCY.TiiTransferFCY.access_token = transMaster.access_token;
                            Utils.LogNO("CBA request: " + JsonConvert.SerializeObject(tillTransferRequestFCY) + " url: " + url);
                            response = VaultAPIService.VaultOutFCY(url, tillTransferRequestFCY, transMaster.access_token);
                        }
                        Utils.LogNO("CBA response: " + JsonConvert.SerializeObject(response));
                        if (response != null)
                        {
                            transMaster.TransRef = response.ResponseId;
                            transMaster.CBAResponse = response.ResponseText;
                            transMaster.CBACode = response.ResponseCode;
                            transMaster.Approved = true;
                            transMaster.Posted = true;
                            result = _tillTransferService.AcceptTillTransfer(transMaster.TillTransferID);
                            try
                            {
                                transMaster.MachineName = DetermineCompName(GetIPAddress());
                                var IPAddress = GetIPAddress();
                                if (transMaster.IsVaultIn)
                                {
                                    General.AuditLog("Performed Vault In Operation of " + transMaster.TotalAmt + " from " + transMaster.ToTellerId + " Teller to " + transMaster.TellerId + " Teller "
                                                                    , transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                                }
                                else
                                {
                                    General.AuditLog("Performed Vault Out Operation of " + transMaster.TotalAmt + " from " + transMaster.TellerId + " Teller to " + transMaster.ToTellerId + " Teller "
                                                                                                    , transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                                }

                            }
                            catch { }
                            _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,3);
                            result = new
                            {
                                success = true,
                                message = "Transaction successful",
                                TransactionRef = response.ResponseId
                            };

                            return Ok(result);
                        }
                        else
                        {

                            result = new
                            {
                                success = false,
                                message = "Transaction not successful",
                                TransactionRef = ""
                            };
                            return Ok(result);
                        }
                    }
                    else
                    {
                        result = _tillTransferService.RejectTillTransfer(transMaster.TillTransferID);
                        result = new
                        {
                            success = true,
                            message = "Transaction rejected successfully",
                            TransactionRef = ""
                        };
                    }
                   
                }
                else
                {
                    transMaster.CBAResponse = "";
                    transMaster.CBACode = "";
                    transMaster.Posted = true;
                    transMaster.Approved = true;
                    transMaster.TransRef = Guid.NewGuid().ToString();
                    try
                    {
                        transMaster.MachineName = DetermineCompName(GetIPAddress());
                        var IPAddress = GetIPAddress();
                        if (transMaster.IsVaultIn)
                        {
                            General.AuditLog("Performed Vault In Operation of " + transMaster.TotalAmt + " from " + transMaster.ToTellerId + " Teller to " + transMaster.TellerId + " Teller "
                                                            , transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                        }
                        else
                        {
                            General.AuditLog("Performed Vault Out Operation of " + transMaster.TotalAmt + " from " + transMaster.TellerId + " Teller to " + transMaster.ToTellerId + " Teller "
                                                                                            , transMaster.TransName, transMaster.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                        }

                    }
                    catch { }
                    result = _transactionBusiness.CreateTransactionDepositWithdrawal(transMaster,3);

                    return Ok(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _transactionBusiness.GetWithdrawalTrans(false, ex.Message, ex);
                return Ok(result);
            }
        }

        [HttpPost, Route("api/Approve/Transaction")]
        public IHttpActionResult TransactionApproval([FromBody]TransactionApprovalModel transMaster)
        {
            Utils.LogNO("api/Approve/Transaction:29:Currently Inside This End Point.");
            object result = null;
            try
            {//InitiatorName Branch TransactionParty CurrencyAbbrev


                string url = null;
                OutputResponse cbaRespnse = null;
                Utils.LogNO("Post Cash Started");
                TransactionModel transactionModel = new TransactionModel();
                var response = _transactionBusiness.GetAllTransById(Convert.ToInt32(transMaster.TranId));
                transactionModel.ChequeNo = response.ToTellerId;
                transactionModel.CurrCode = _transactionBusiness.GetCurrencyAbbrev(response.Currency);
                Utils.LogNO("Serialized data: ..."+ JsonConvert.SerializeObject(response));

                if (isDemo == "false")
                {
                    if (response.TransType == 1 || response.TransType == 13)
                    {
                        Utils.LogNO("Post Cash Withdrawal Started...");
                        transactionModel.CurrCode = _transactionBusiness.GetCurrencyAbbrev(response.Currency);
                        url = System.Configuration.ConfigurationManager.AppSettings["CashWithDrawal"];
                        if (transactionModel.CurrCode != "NGN")
                            url = System.Configuration.ConfigurationManager.AppSettings["CashWithDrawalFCY"];
                       
                        if (transactionModel.ChequeNo != null)
                        {
                            url = transactionModel.ChequeNo.Length == 8 ?
                                response.TransType == 13 ? System.Configuration.ConfigurationManager.AppSettings["CashWithdrawalWithCounterCheque"] :
                                System.Configuration.ConfigurationManager.AppSettings["CashWithdrawalWithCheq"] : url;

                                transactionModel.ChequeNo = transactionModel.ChequeNo.Length == 8 ? transactionModel.ChequeNo : null;
                        }
                        else
                        {
                            transactionModel.ChequeNo = null;
                        }
                        transactionModel.access_token = transMaster.access_token;
                        transactionModel.Amount = response.TotalAmount;
                        transactionModel.InitiatorName = response.TransacterName;
                        transactionModel.CustomerAcctNos = response.AccountNumber;
                        transactionModel.Remark = response.Remarks;
                        transactionModel.TellerId = response.TellerId;
                        transactionModel.Branch = response.BranchCode; 
                       
                        transactionModel.TransactionParty = response.TransactionParty; 
                        cbaRespnse = CreateCBACashWithdrawal(transactionModel, url, response.Narration);
                    }
                    else if (response.TransType == 8)
                    {//InHOUSE CHQ DEPOSIT
                        url = System.Configuration.ConfigurationManager.AppSettings["INHOUSECHEQUES"];
                        var narration = _transactionBusiness.BuildNarration(transactionModel.ChequeNo,
                        transactionModel.Beneficiary, "", transactionModel.Narration, transactionModel.AccountName, 2);
                        Utils.LogNO("Executed narration " + narration);
                       var chequeLodgement = _transactionBusiness.GetChequeLodgement(transMaster.TranId);
                        Utils.LogNO("ChequeLogment Details: " + JsonConvert.SerializeObject(response));
                        transactionModel.access_token = transMaster.access_token;
                        transactionModel.Amount = Math.Round(response.TotalAmount, 2);
                        transactionModel.TotalAmt = Math.Round(response.TotalAmount, 2);
                        transactionModel.InitiatorName = response.TransacterName;
                        transactionModel.AccountNo = response.AccountNumber;
                        transactionModel.CustomerAcctNos = chequeLodgement.AccountNumber;
                        transactionModel.Remark = response.Remarks;
                        transactionModel.TellerId = response.TellerId;
                        transactionModel.Branch = response.BranchCode;
                        transactionModel.ChequeNo = chequeLodgement.ChequeNo;
                        transactionModel.CurrencyAbbrev = _transactionBusiness.GetCurrencyAbbrev(response.Currency);
                        transactionModel.CurrCode = _transactionBusiness.GetCurrencyAbbrev(response.Currency);
                        transactionModel.TransactionParty = response.TransactionParty; 
                        var tresponse = new FTChqResponse();
                        //Creating and posting InHouse Cheques
                        tresponse = CreateCBAInHouseCheques(transactionModel, url, narration);
                        if (tresponse.ResponseCode == "00")
                        {
                            transMaster.TReference = tresponse.ReferenceID;
                            result = _transactionBusiness.ApproveTransaction(transMaster, tresponse.ReferenceID);
                            return Ok(result);
                        }
                        else
                        {
                            if (tresponse.ResponseCode == "x03")
                            {
                                string errorMsg = tresponse.ResponseText;
                                errorMsg = errorMsg.ToLower().Contains("cheque was already cleared") ? "Cheque was already cleared" : errorMsg;
                                result = new
                                {
                                    success = false,
                                    message = errorMsg,
                                    TransactionRef = "",
                                    Result = result
                                };
                                return Ok(result);

                            }
                            result = new
                            {
                                success = false,
                                message = tresponse.ResponseText,
                                TransactionRef = "",
                                Result = result
                            };
                            return Ok(result);
                        }
                    }
                    else
                    {
                        url = ""; // cash withdrawal url
                        Utils.LogNO("Post Cash Deposit Started...");
                        transactionModel.access_token = transMaster.access_token;
                        var narration = _transactionBusiness.BuildNarration(response.CashTransactions.Where(x => x.TranId == transMaster.TranId).Select(y => y.ChequeNo).ToString(),
                            response.CashTransactions.Where(x => x.TranId == transMaster.TranId).Select(y => y.TillId).ToString(), "", response.Narration, response.TransacterName, 3, response.TransType.ToString());

                        transactionModel.Amount = response.TotalAmount;
                        transactionModel.InitiatorName = response.TransacterName;
                        transactionModel.CustomerAcctNos = response.AccountNumber; //response.TransactionMaster.Where(x => x.AccountNo != null).Select(x => x.AccountNo).FirstOrDefault(); ;
                        transactionModel.Remark = response.Remarks;
                        transactionModel.TellerId = response.TellerId;
                        transactionModel.Branch = response.BranchCode;//"NG0020006";
                        transactionModel.CurrCode = _transactionBusiness.GetCurrencyAbbrev(response.Currency);
                        transactionModel.TransactionParty = response.TransactionParty;//"A";
                        cbaRespnse = CreateCBADeposit(transactionModel);
                        Utils.LogNO("Executed Response from CBA " + JsonConvert.SerializeObject(cbaRespnse));

                    }
                    if (cbaRespnse != null)
                    {
                        if (cbaRespnse.ResponseId == null)
                        {
                            result = new
                            {
                                success = false,
                                message = cbaRespnse.ResponseText+ "- No ResponseId returned for this transaction!",
                                TransactionRef = ""
                            };
                            return Ok(result);
                        }
                        if (cbaRespnse.ResponseCode == "-1")
                        {
                            result = new
                            {
                                success = false,
                                message = "Post Service Error. Contact Administrator "+" ("+ cbaRespnse.ResponseCode+").",
                                TransactionRef = ""
                            };
                            return Ok(result);
                        }
                        else
                        {
                            try
                            {
                                transactionModel.MachineName = DetermineCompName(GetIPAddress());
                                var IPAddress = GetIPAddress();
                                if (transactionModel.TransType=="1")
                                {
                                    General.AuditLog("Performed Cash withdrawal (Db) Operation Approvalof " + transactionModel.TotalAmt + " from " + transactionModel.ToTellerId + " Teller to " + transactionModel.TellerId + " Teller "
                                                                    , transactionModel.TransName, transactionModel.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                                }
                                else
                                {
                                    General.AuditLog("Performed Cash Deposit (Db) Operation approval of " + transactionModel.TotalAmt + " from " + transactionModel.TellerId + " Teller to " + transactionModel.ToTellerId + " Teller "
                                                                                                    , transactionModel.TransName, transactionModel.MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                                }

                            }
                            catch { }
                            transMaster.TReference = cbaRespnse.ResponseId;
                            result = _transactionBusiness.ApproveTransaction(transMaster, cbaRespnse.ResponseId);
                            //result = new
                            //{
                            //    success = true,
                            //    message = "Successful",
                            //    TransactionRef = cbaRespnse.ResponseId,
                            //    data = result
                            //};
                            return Ok(result);
                           
                        }
                    }
                    else
                    {
                        result = new
                        {
                            success = false,
                            message = "Service Error: No Response from CBA!",
                            TransactionRef = ""
                        };
                        return Ok(result);
                    }

                }
                else
                {
                    transMaster.TReference = transMaster.TReference;
                    transMaster.TReference = Guid.NewGuid().ToString();
                    result = _transactionBusiness.ApproveTransaction(transMaster, transMaster.TReference);
                    //result = new
                    //{
                    //    success = true,
                    //    message = "Successful",
                    //    TransactionRef = transMaster.TReference,
                    //    data = result
                    //};
                    return Ok(result);
                   
                }
            }
            catch(Exception ex)
            {
                Utils.LogNO("Transaction local exception: " + ex.Message);
                return Ok(result);
            }

            //return Ok(result);
        }

        [HttpPost, Route("api/Disapprove/Transaction")]
        public IHttpActionResult TransactionDisapproval([FromBody]TransactionDisApprovalModel transMaster)
        {
            Utils.LogNO("api/Disapprove/Transaction:30:Currently Inside This End Point.");
            object result = null;
            try
            {
                              
                    var MachineName = DetermineCompName(GetIPAddress());

                    var IPAddress = GetIPAddress();
                   

                result = _transactionBusiness.DisapproveTransaction(transMaster);
                return Ok(result);
            }
            catch
            {
                return Ok(result);
            }

        }

        [HttpPost, Route("api/Transaction/TellerReversal")]
        public IHttpActionResult TellerReversal([FromBody]TellerReversal transMaster)
        {
            Utils.LogNO("api/Transaction/TellerReversal:31:Currently Inside This End Point.");
            try
            {
                if (isDemo == "false")
                {
                    string url = System.Configuration.ConfigurationManager.AppSettings["TellerReversal"];
                    TellerReversalRequest request = new TellerReversalRequest();
                    request.TellerReversal = new TellerReversalModel();
                    request.TellerReversal.access_token = transMaster.access_token;
                    request.TellerReversal.TransactionBranch = string.IsNullOrEmpty(transMaster.TransactionBranch)? _transactionBusiness.GetAllTransById(Convert.ToInt32(transMaster.TranId)).BranchCode: transMaster.TransactionBranch; //"NG0020006";
                    request.TellerReversal.TTReference = transMaster.TReference;
                    OutputResponse response = Transaction.TellerReversal(url, request);
                    if (response != null)
                    {
                        if (response.ResponseCode == "-1")
                        {
                            return Ok(new
                            {
                                success = false,
                                message = "Transaction not found",
                                TransReference = ""
                            });
                        }
                        _transactionBusiness.ReverseTransaction(transMaster);

                        var result = _transactionBusiness.GetAllTrans(true, "");
                        return Ok(result);
                    }
                    else
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "Transaction not reversed successfully",
                            TransReference = transMaster.TReference
                        });
                    }
                }
                else
                {
                    transMaster.TReference = Guid.NewGuid().ToString();
                    try
                    {
                        var MachineName = DetermineCompName(GetIPAddress());
                        var IPAddress = GetIPAddress();
                        
                            General.AuditLog("Performed Reversal Operation on " + transMaster.TranId + " TranId with  " + transMaster.TReference 
                                                            , "", MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                        
                    }
                    catch { }
                    _transactionBusiness.ReverseTransaction(transMaster);
                    var result = _transactionBusiness.GetAllTrans(true, "");
                    return Ok(result);
                }

            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = ex.Message,
                    innerError = ex
                });
            }
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
        public static string GetLocalIPAddress() // we are ok ......  yes  postman testing
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
