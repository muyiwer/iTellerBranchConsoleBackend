
using iTellerBranch.Model;
using iTellerBranch.Model.ViewModel;
using iTellerBranch.Repository.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Repository.Service
{
    public class TransactionService : BaseService, ITransactionService
    {
        private readonly TillTransferService _tillService;

        public TransactionService()
        {
            _tillService = new TillTransferService();
        }
        
        public TreasuryProductCode GetTreasuryProductCode(string productCode) 
        {
            return db.TreasuryProductCode.Where(x => x.ProductCode == productCode).FirstOrDefault();
        }

        public BranchAccounts BranchAccounts(string branchCode)
        {
            return db.BranchAccounts.Where(x=> x.BranchCode == branchCode).FirstOrDefault();
        }

        public IQueryable<TreasuryDealsMaster> GetTreasuryDealsWithoutSuccessfulDoubleEntries()
        {
            return db.TreasuryDealsMaster.Where(x => x.DoubleEntrySuccessful == false).AsQueryable();
        } 

        public string GetCBaDealID(int? id)
        {
            return db.TreasuryDealsMaster.Where(x => x.Id == id).FirstOrDefault().CBADealId;
        }

        public string GetTerminationCode(int code)  
        {
            try
            {
                return db.TerminationInstruction.Where(x => x.Code == code).FirstOrDefault().CBACode;
            }
            catch (Exception ex)
            {
                Utils.LogNO("GetTerminationCode: " + code + ex.Message);
                return "";
            }
           
        }

        public customerMaster GetCustomerAccountDetails(string accountNumber)
        {
            return db.customerMaster.Where(x => x.AccountNo == accountNumber).FirstOrDefault();
        }

        public customerMaster GetCustomerAccountDetailsByCustId(string custId) 
        {
            return db.customerMaster.Where(x => x.customerId == custId).FirstOrDefault();
        }

        //ENSURE the two trans enter the same table----
        public object GetWithdrawalTrans(bool success, string message, Exception ex = null)
        {
            return new
            {
                success,
                message,
                innerError = ex,
                data = new
                {
                    Transaction = db.TransMaster.Select(x => new
                    {
                        x.TranId,
                        x.TotalAmt,
                        x.CashierID,
                        x.CashierTillGL,
                        x.CashierTillNos,
                        x.Currency,
                        x.CustomerAcctNos,
                        x.Status,
                        x.DepositorName,
                        x.DepositorPhoneNo,
                        x.Creation_Date,
                        x.Address,
                        x.MachineName,
                        x.Narration,
                        x.SupervisoryUser,
                        x.SortCode,
                        x.ValueDate,
                        x.WhenApproved,
                    }).ToArray(),
                    TransDetails = db.TransDetails.Select(y => new
                    {
                        y.Id,
                        y.Amount,
                        y.Counter,
                        y.TranId
                    }).ToArray()
                    
                }

            };
        }

        public IQueryable<TreasuryDealsMaster> GetMaturedDeals()
        {
            Utils.LogNO("GET");
            DateTime dt = DateTime.Now;
            int processStatus = Convert.ToInt16(TransType.ProcessStatus.Approved);
            return db.TreasuryDealsMaster.Where(x =>  x.ProcessStatus == processStatus && x.TransactionStatus == "A" 
                                                && DbFunctions.TruncateTime(x.MaturityDate).Value <= dt).AsQueryable();
        }

        public TreasuryDealsMaster UpdateTreasuryDealTransactionStatus(int? Id)
        {
            try
            {
                using (var context = new BranchConsoleEntities())
                {
                    TreasuryDealsMaster treasuryDealsMaster = context.TreasuryDealsMaster.Find(Id);
                    treasuryDealsMaster.TransactionStatus = "R";
                    context.Entry(treasuryDealsMaster).State = EntityState.Modified;
                    context.SaveChanges();
                    return treasuryDealsMaster;
                }
            }
            catch (Exception ex)
            {
                Utils.LogNO("UpdateTreasuryDealTransactionStatus Exception" + ex.Message);
                return null;
            }
          
                
        }
        public object GetTerminationInstruction(bool success, string message, Exception ex = null)
        {
            return new
            {
                success,
                message,
                innerError = ex,
                data = new
                {
                    Transaction = db.TerminationInstruction.Select(x => new
                    {
                        x.Id,
                        x.Code,
                        x.Description
                    }).ToArray(),
                    

                }

            };
        }

        public object GetTreasuryProductDetails(bool success, string message, Exception ex = null)
        {
            return new
            {
                success,
                message,
                innerError = ex,
                data = new
                {
                    Transaction = db.TreasuryProductCode.Select(x => new
                    {
                        x.Id,
                        x.ProductCode,
                        x.ProductName,
                        x.AccountType,
                        x.Tenured,
                        x.Discounted,
                        x.MinTaxablePrincipal,
                        x.WHTMinAmt,
                        x.CBAProductCode,
                        x.Rate,
                        x.ChargeLiquidationAccount,
                        x.DrawnDownAccount
                    }).ToArray(),


                }

            };
        }
        //object GetAllTreasuryTrans(bool success, string message, Exception ex = null, string transRef = null) 
        //TreasuryDealsMaster GetAllTreasuryTransById(int TransId)   int CreateTreasuryDealsTrans(TreasuryDealsModel treasuryDealsModel) TreasuryDealsMaster CreateTreasuryDeals(TreasuryDealsModel treasuryDealsModel)
        // ApproveTransaction DisapproveTransaction
        public object GetCustomerDetails(string AccountNumber)
        {
            return db.customerMaster.Where(x => x.AccountNo == AccountNumber).FirstOrDefault();
        }

        public TransactionsMaster GetAllTransById(int TranID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            TransactionsMaster Transaction = db.TransactionsMaster.Find(TranID);
            if(Transaction == null)
            {
                Utils.LogNO("Transaction is null");
            }
            else
            {
                Utils.LogNO("Transaction is not null");
            }
           
            return Transaction;
        }

        public TransactionFiles GetTransactionFile(TransactionFiles files)
        {
            try
            {
                return db.TransactionFiles.ToList().Where(x => x.FileName == files.FileName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Utils.LogNO("Error fetching transaction files: " + ex.Message);
                return null;
            }
            

        }

        public BranchAccounts FetchGLAccounts(string branchCode)
        {
            BranchAccounts branchAccounts = new BranchAccounts();
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                branchAccounts = db.BranchAccounts.Where(x => x.BranchCode == branchCode).FirstOrDefault();
            }
            catch { }
 
            return branchAccounts;
        }

        public object ApproveTransaction(TransactionApprovalModel transaction, string transRef = null)
        {
            var Transaction = GetAllTransById(Convert.ToInt32(transaction.TranId));
            Utils.LogNO("TransactionApproval Details: " + JsonConvert.SerializeObject(Transaction));
            try
            {
            if (Transaction != null)
            {
                Transaction.Status = 2;
                Transaction.Approved = true;
                Transaction.Posted = true;
                Transaction.ApprovedBy = transaction.ApprovedBy;
                Transaction.TransRef = transaction.TReference;
                Transaction.WhenApproved = DateTime.Now;

                db.Entry(Transaction).State = EntityState.Modified;
                db.SaveChanges();
                    Utils.LogNO("executed Transaction approval on local");
                    return GetUnApprovedTrans(true, "Transaction successfully Approved!",null,transRef);
            }
            else
            {
                    Utils.LogNO("Transaction does not exist");
                    return GetUnApprovedTrans(false, "Transaction failed to approve!");
            }
            }
            catch (Exception ex)
            {
                Utils.LogNO("Transaction local exception: " + ex.Message);
                return GetUnApprovedTrans(false, "Transaction failed to approve!", ex);
            }

        }


        public object ApproveImalTransaction(TransactionApprovalModel transaction, string transRef = null)
        {
            var Transaction = GetAllTransById(Convert.ToInt32(transaction.TranId));
            Utils.LogNO("TransactionApproval Details: " + JsonConvert.SerializeObject(Transaction));
            try
            {
                if (Transaction != null)
                {
                    Transaction.Status = 2;
                    Transaction.Approved = true;
                    Transaction.Posted = true;
                    Transaction.ApprovedBy = transaction.ApprovedBy;
                    Transaction.TransRef = transaction.TReference;
                    Transaction.WhenApproved = DateTime.Now;

                    db.Entry(Transaction).State = EntityState.Modified;
                    db.SaveChanges();
                    Utils.LogNO("executed Transaction approval on local");
                    return GetAllTrans(true, "Transaction successfully Approved!", null, transRef);
                }
                else
                {
                    Utils.LogNO("Transaction does not exist");
                    return GetUnApprovedTrans(false, "Transaction failed to approve!");
                }
            }
            catch (Exception ex)
            {
                Utils.LogNO("Transaction local exception: " + ex.Message);
                return GetUnApprovedTrans(false, "Transaction failed to approve!", ex);
            }

        }

        public object ApproveInHouseTransactionTransaction(TransactionApprovalModel transaction, string transRef = null) 
        {
            var Transaction = GetAllTransById(Convert.ToInt32(transaction.TranId));
            Utils.LogNO("TransactionApproval Details: " + JsonConvert.SerializeObject(Transaction));
            try
            {
                if (Transaction != null)
                {
                    Transaction.Status = 2;
                    Transaction.Approved = true;
                    Transaction.Posted = true;
                    Transaction.ApprovedBy = transaction.ApprovedBy;
                    Transaction.TransRef = transaction.TReference;
                    Transaction.WhenApproved = DateTime.Now;

                    db.Entry(Transaction).State = EntityState.Modified;
                    db.SaveChanges();
                    Utils.LogNO("executed Transaction approval on local");
                    return GetInHouseTransferTransaction(true, "Transaction successfully Approved!", null, transRef);
                }
                else
                {
                    Utils.LogNO("Transaction does not exist");
                    return GetInHouseTransferTransaction(false, "Transaction failed to approve!");
                }
            }
            catch (Exception ex)
            {
                Utils.LogNO("Transaction local exception: " + ex.Message);
                return GetInHouseTransferTransaction(false, "Transaction failed to approve!", ex); 
            }

        }

        public object ApproveTeasuryDeal(TreasuryDealsModel transaction, string dealID = null) 
        {
            db.Configuration.ProxyCreationEnabled = false;
            var Transaction = db.TreasuryDealsMaster.Find(transaction.Id);

            try
            {
                if (Transaction != null)
                {
                    Transaction.ApprovedBy = transaction.ApprovedBy;
                    Transaction.Posted = true;
                    Transaction.ProcessStatus = 10;
                    Transaction.DoubleEntrySuccessful = transaction.DoubleEntrySuccessful;
                    Transaction.TransactionStatus = "A";
                    if(transaction.CBADealId != null)
                    {
                        Transaction.DealId = transaction.CBADealId;
                    }
                    Transaction.CBADealId = transaction.CBADealId;
                    Transaction.DealersReference = transaction.DealersReference;
                    Transaction.WhenApproved = DateTime.Now;
                    db.Entry(Transaction).State = EntityState.Modified;
                    db.SaveChanges();
                    dealID = dealID == null ? Transaction.DealId : dealID;
                    return new { success = true, message = "Transaction successfully Approved!", TransactionRef= dealID };
                }
                else
                {
                    return GetAllTreasuryTrans(false, "Transaction failed to approve!");
                }
            }
            catch (Exception ex)
            {
                return GetAllTreasuryTrans(false, "Transaction failed to approve!", ex);
            }
        }

        public object DisapproveTeasuryDeal(TreasuryDealsModel transaction)
        {
            db.Configuration.ProxyCreationEnabled = false;
            Utils.LogNO("transaction" + JsonConvert.SerializeObject(transaction));
            var Transaction = db.TreasuryDealsMaster.Find(transaction.Id);
            Utils.LogNO("Transaction" + JsonConvert.SerializeObject(Transaction));
            try
            {
                if (Transaction != null)
                {
                    Transaction.DisapprovedBy = transaction.DisapprovedBy; 
                    Transaction.Posted = false;
                    Transaction.ProcessStatus = 2;
                    Transaction.TransactionStatus = "D";
                    Transaction.DisapprovalReason = transaction.DisapprovalReason;
                    Transaction.WhenDisapproved = DateTime.Now;
                    db.Entry(Transaction).State = EntityState.Modified;

                    db.UserTransactionPageAccess
                                        .Where(x => x.TranId == transaction.Id).ToList()
                                        .ForEach(a => a.IsActiveOnPage = false);

                    db.SaveChanges();
                    return new {  success = true, message = "Dissapproved successfully"  };
                }
                else
                {
                    Utils.LogNO("Error: Treasury deal is null with primary key Id: " + transaction.Id);
                    return new { success = false, message = "Transaction failed on disapproval" };
                }
            }
            catch (Exception ex)
            {
                Utils.LogNO("Error: " + ex.Message);
                return new { success = false, message = ex.Message };
            }
        }

        public TreasuryDealsMaster GetNewDeal(int id)  
        {
            return db.TreasuryDealsMaster.Where(x => x.Id == id).FirstOrDefault();
        }
        public bool DeleteTreasuryDeal(int? id)
        {
            try
            {
                TreasuryDealsMaster treasuryDealsMaster = db.TreasuryDealsMaster.Find(id);
                if (treasuryDealsMaster == null)
                {
                    return false;
                }
                else
                {
                    Utils.LogNO("Id" + id);
                    var treasuryInterest = db.TreasuryInterest.Where(x => x.DealId == id).ToList();
                    Utils.LogNO("treasuryInterest count: " + treasuryInterest.Count());
                    if (treasuryInterest.Count() > 0)
                    {
                        db.TreasuryInterest.RemoveRange(treasuryInterest);
                    }
                    db.UserTransactionPageAccess.RemoveRange(db.UserTransactionPageAccess.Where(x => x.TranId == id).ToList());
                    db.TreasuryDealsMaster.Remove(treasuryDealsMaster);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Utils.LogNO("DeleteTreasuryDeal: " + ex);
                return false;
            }
           
          
        }

        public string GenerateDealID(TreasuryDealsModel transaction)
        {
            return transaction.CustomerId + "/" + transaction.Id;
        }

        public object DisapproveTransaction(TransactionDisApprovalModel transaction)
        {
            try
            {                
                var Transaction = db.TransactionsMaster.Find(Convert.ToInt64(transaction.TranId));
                if (Transaction != null)
                {
                    Transaction.Status = 3;
                    Transaction.Approved = false;
                    Transaction.DisapprovalReason = transaction.DisapprovalReason;
                    Transaction.DisapprovedBy = transaction.DisapprovedBy;
                    Transaction.WhenDisapproved = DateTime.Now;

                    db.Entry(Transaction).State = EntityState.Modified;
                    db.SaveChanges();

                    return GetUnApprovedTrans(true, "Transaction successfully disapproved!");
                }
                else
                {
                    return GetUnApprovedTrans(false, "Transaction failed to disapprove!");
                }
            }
            catch(Exception ex)
            {
                return GetUnApprovedTrans(false, "Transaction failed to disapprove!", ex);
            }
           
        }



        public object GetAccountMandate(string AccountNumber, bool success, string message, Exception ex = null)
        {
            return new
            {
                success,
                message,
                innerError = ex,
                Mandate = db.accountmandates.Where(x => x.AccountNo == AccountNumber).ToArray()
            };
                      
        }
        public object CreateTransactionWithdrawal(TransactionModel transactionModel)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int result = 0; //oya
                    string transRef = GenerateTransactionReference();
                    string narration =  BuildNarration(transactionModel.ChequeNo, transactionModel.Beneficiary,transRef,
                        transactionModel.Narration,transactionModel.TransName, 3);
                    
                   // string MachineName = GeneralService.DetermineCompName(transactionModel.MachineName);
                   //no transref. Should we add?  should we add colun for remark, has mandate, has memo?Answer. yes sir.ok
                    var param = new SqlParameter[]
                       {
                           new SqlParameter("@CustomerAcctNos", transactionModel.CustomerAcctNos),
                           new SqlParameter("@TotalAmt", transactionModel.TotalAmt),
                           new SqlParameter("@TransName", transactionModel.TransName),
                           new SqlParameter("@PhoneNo", transactionModel.PhoneNo),
                           new SqlParameter("@SortCode", transactionModel.SortCode),
                           new SqlParameter("@TransType", transactionModel.TransType),
                           new SqlParameter("@CashierID", transactionModel.CashierID),
                           new SqlParameter("@CashierTillNos", transactionModel.CashierTillNos),
                            new SqlParameter("@CashierTillGL", transactionModel.CashierTillGL),
                            new SqlParameter("@Currency", transactionModel.Currency),
                            new SqlParameter("@ValueDate", transactionModel.ValueDate),
                           new SqlParameter("@SupervisoryUser", transactionModel.SupervisoryUser),
                            new SqlParameter("@Beneficiary", transactionModel.Beneficiary),
                            new SqlParameter("@Chequeno", transactionModel.ChequeNo),
                            new SqlParameter("@DateonCheque", transactionModel.DateOnCheque),
                            new SqlParameter("@Narration", narration),
                            new SqlParameter("@Machine_Name", transactionModel.MachineName),
                            new SqlParameter("@TillTransferID", transactionModel.TillTransferID),
                            new SqlParameter("@IsTillTransfer", transactionModel.IsTillTransfer),
                            new SqlParameter("@TransRef", transRef),
                            new SqlParameter("@Remark", transactionModel.Remark),
                            new SqlParameter("@hasMemo", transactionModel.hasMemo),
                            new SqlParameter("@hasMandate", transactionModel.hasMandate),
                            new SqlParameter("@NeededApproval", transactionModel.NeededApproval),
                             new SqlParameter("@CBAcode", transactionModel.IsT24 == true ? "T24" : "IMAL"),
                       };
                    result = db.Database.SqlQuery<int>("dbo.TransactionWithdrawal_Create @CustomerAcctNos," +
                        "@TotalAmt," + "@TransName,@PhoneNo,@SortCode,@TransType,@CashierID,@CashierTillNos," +
                        "@CashierTillGL,@Currency,@ValueDate," + "@SupervisoryUser,@Beneficiary,@Chequeno," +
                        "@DateonCheque,@Narration, @Machine_Name,@TillTransferID,@IsTillTransfer, " +
                        "@TransRef,@Remark,@hasMemo,@hasMandate,@NeededApproval,@CBAcode", param)
                         .FirstOrDefault();
                    if(transactionModel.TransactionDetailsModels.Count() > 0 && result > 0)
                    {
                        List<TransMasterDetails> transMasterDetails = new List<TransMasterDetails>();
                        foreach(var transDetails in transactionModel.TransactionDetailsModels)
                        {
                            transMasterDetails.Add(new TransMasterDetails
                            {
                                Counter = transDetails.Counter,
                                TranId = result, //increase d identity to 1000 ok
                                Amount = transDetails.Amount
                            });
                        }
                        db.TransMasterDetails.AddRange(transMasterDetails);
                    }
                    transaction.Commit(); // if there is error here it should not save changes
                    db.SaveChanges();
                    return new
                    {
                        success = true,
                        message = "transaction saved successfully",
                        TransactionRef = result
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new
                    {
                        success = false,
                        message = ex.Message,
                        innerError = ex
                    };
                  
                }
            }
        }

        public ChequeLodgement GetChequeLodgement(long? tranId)
        {
            return db.ChequeLodgement.Where(x => x.TranId == tranId).FirstOrDefault();
        }
        /// <summary>
        /// /--TransType 
        //--1 CashWithdrawal --Credit Teller --Debit customer--2 Deposit --Debit Teller --Credit Customer--3 ChequeLodgement --Debit Teller
        //--Credit Customer --4 ClearingCheque  --Credit Teller --Debit Customer --5 Vault Out --Debit FroTeller --Credit ToTeller(Vault)
        //--6 Vault In          --Credit FroTeller         --Debit ToTeller(Vault)         --7 Till Transfer         --Credit FroTeller         --Debit ToTeller
        /// </summary>
        /// <param name="transactionModel"></param>
        /// <param name="result"></param>
        public void CreateTransactionDetails(TransactionModel transactionModel, int result)
        {
            if (Convert.ToInt16(transactionModel.TransType) == 8)
            {
                ChequeLodgement chequeLodgements = new ChequeLodgement
                {
                    TranId = result,
                    TillId = transactionModel.TillId,
                    ChequeNo = transactionModel.ChequeNo,
                    AccountNumber= transactionModel.CustomerAcctNos,
                    AccountName = transactionModel.Beneficiary,
                    ChequeDate =transactionModel.DateOnCheque,
                    TransType =Convert.ToInt16(transactionModel.TransType),
                    ChargeAmt = transactionModel.CHARGEAMT
                };

                db.ChequeLodgement.Add(chequeLodgements);
            }
            else if (Convert.ToInt16(transactionModel.TransType) == 9)
            {
                TransferHeader transfer = new TransferHeader
                {
                    TranId = result,
                    InstrumentNumber = transactionModel.ChequeNo,
                    ApplicableCharge = transactionModel.CHARGEAMT,
                    ChargeType = transactionModel.ChargeType,
                    IsBulkTran = transactionModel.IsBulkTran
                };

                db.TransferHeader.Add(transfer);
                //TODO 1: insert into Transferdetails table done
                    //TODO 2: Insert into TransationFiles table if there is any file

                    var FileDetail = new TransactionFiles()
                    {
                        TranId = result,
                        FileName = transactionModel.FileName,
                        UserId = transactionModel.TransName,
                        Branch = transactionModel.Branch,
                        TotalCount = Convert.ToInt16(transactionModel.TransactionCount),
                        TotalAmount = transactionModel.TotalAmt,
                        DateUploaded = DateTime.Now

                    };

                    db.TransactionFiles.Add(FileDetail);

                    List<TransferDetails> transferDetails = new List<TransferDetails>();
                    foreach (var transferDetail in transactionModel.TransactionBeneficiary)
                    {
                        transferDetails.Add(new TransferDetails
                        {
                            TranId = result,
                            BenAccountNumber = transferDetail.AccountNumber, //increase d identity to 1000 ok
                            BenAccountName = transferDetail.AccountName,
                            Amount = transferDetail.Amount,
                            Narration = transferDetail.Narration,
                            ApplicableCharge = transferDetail.ChargeAmt,
                            TransRef = transferDetail.TransRef
                        });
                    }

                    db.TransferDetails.AddRange(transferDetails);
                

            }

            else
            {
                CashTransactions tranMasterModel = new CashTransactions
                {
                    TranId = result,
                    TillId = transactionModel.TillId,
                    ChequeNo = transactionModel.ChequeNo

                };

                db.CashTransactions.Add(tranMasterModel);
            }
       
                       
        }

        public bool ValidateAccountNumber(string AccountNumber)
        {
            return  db.customerMaster.Where(x => x.AccountNo == AccountNumber).FirstOrDefault()==null ? false : true;
        }

        public object GetWithDepositTrans(bool success, string message, Exception ex = null)
        {
            throw new NotImplementedException();
        }

        public object CreateTransactionDepositWithdrawal(TransactionModel transactionModel, int statusId)
        {//After Restructuring 18Sept2020
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int result = 0; //oya
                                    //string transRef = GenerateTransactionReference();
                   // Utils.LogNO("TransactionModel Request: " + JsonConvert.SerializeObject(transactionModel));
                    string narration = BuildNarration(transactionModel.ChequeNo, transactionModel.Beneficiary,
                        transactionModel.TransRef, transactionModel.Narration, transactionModel.TransName, statusId, transactionModel.TransType);
                    //if (transactionModel.TransactionDetailsModels.Count() > 0)
                    //{
                    //    transactionModel.TransType=((int)TransType.TransactionType.CashWithDrawalCounter).ToString();
                    //}
                    if(transactionModel.CBA == "IMAL")
                    {
                        narration = transactionModel.Remarks;
                    }
                    Utils.LogNO("Transtype " + transactionModel.TransType+ ",Trans Name:"+ transactionModel.TransName+ ", status:"+ transactionModel.TransName);
                    // string MachineName = GeneralService.DetermineCompName(transactionModel.MachineName);
                    //no transref. Should we add?  should we add colun for remark, has mandate, has memo?Answer. yes sir.ok
                    var param = new SqlParameter[]
                       {
                           new SqlParameter("@TotalAmount", transactionModel.TotalAmt),
                           new SqlParameter("@TellerId", transactionModel.TellerId),
                           new SqlParameter("@ToTellerId", transactionModel.ToTellerId),
                           new SqlParameter("@TransacterName", transactionModel.TransName),
                           new SqlParameter("@AccountName", transactionModel.AccountName),
                           new SqlParameter("@TransacterEmail", transactionModel.TransacterEmail),
                           new SqlParameter("@TransacterPhoneNo", transactionModel.PhoneNo),
                           new SqlParameter("@TransactionParty", transactionModel.TransactionParty),
                           new SqlParameter("@Status", transactionModel.Status),
                           new SqlParameter("@AccountNumber", transactionModel.AccountNo),
                           new SqlParameter("@SortCode", transactionModel.SortCode),
                              
                           new SqlParameter("@TransType", Convert.ToInt16(transactionModel.TransType)),
                           new SqlParameter("@Currency", transactionModel.Currency),
                           new SqlParameter("@ValueDate", transactionModel.ValueDate),
                           new SqlParameter("@Narration", narration),
                           new SqlParameter("@CreationDate", DateTime.Now),
                           new SqlParameter("@MachineName", transactionModel.MachineName),
                           new SqlParameter("@TransRef", transactionModel.TransRef),

                           new SqlParameter("@hasMemo", transactionModel.hasMemo),
                           new SqlParameter("@hasMandate", transactionModel.hasMandate),
                           new SqlParameter("@Remarks", transactionModel.Remark),
                           new SqlParameter("@NeededApproval", transactionModel.NeededApproval),
                           new SqlParameter("@ApprovedBy", transactionModel.ApprovedBy),
                           new SqlParameter("@WhenApproved", DateTime.Now),
                           new SqlParameter("@Approved",transactionModel.Approved),
                           new SqlParameter("@IsReversed",transactionModel.IsReversed),
                           new SqlParameter("@DisapprovalReason",transactionModel.DisapprovalReason),
                           new SqlParameter("@DisapprovedBy",transactionModel.DisapprovedBy),
                           new SqlParameter("@WhenDisapproved",transactionModel.WhenDisapproved),

                           new SqlParameter("@BranchCode",transactionModel.BranchCode),
                           new SqlParameter("@Posted",transactionModel.Posted),
                           new SqlParameter("@CBAResponse",transactionModel.CBAResponse),
                           new SqlParameter("@CBACode", transactionModel.CBACode),
                           new SqlParameter("@CBA", transactionModel.CBA),
                           new SqlParameter("@ReversedTranId", transactionModel.IsReversed==true?transactionModel.TranId:transactionModel.ReversedTranId),


                       };

                    result = db.Database.SqlQuery<int>("dbo.TransactionsMaster_Create @TotalAmount,@TellerId,@ToTellerId,@TransacterName,@AccountName, @TransacterEmail," +
                              "@TransacterPhoneNo,@TransactionParty,@Status,@AccountNumber,@SortCode,@TransType,@Currency,@ValueDate,@Narration,"+
                              "@CreationDate,@MachineName,@TransRef,@hasMemo,@hasMandate,@Remarks,@NeededApproval,@ApprovedBy,@WhenApproved,"+
                              "@Approved,@IsReversed,@DisapprovalReason,@DisapprovedBy,@WhenDisapproved,@BranchCode,@Posted,@CBAResponse,@CBACode,@CBA,"+
                              "@ReversedTranId", param)
                               .FirstOrDefault();



                    Utils.LogNO("Executed SP " + result);
                   
                    CreateTransactionDetails(transactionModel, result);
                    // transactionModel.TransRef = result.ToString();
                    if (transactionModel.TransactionDetailsModels.Count() > 0 && result > 0)
                    {
                        List<TransMasterDetails> transMasterDetails = new List<TransMasterDetails>();
                        foreach (var transDetails in transactionModel.TransactionDetailsModels)
                        {
                            transMasterDetails.Add(new TransMasterDetails
                            {
                                Counter = transDetails.Counter,
                                TranId = result, //increase d identity to 1000 ok
                                Amount = transDetails.Amount
                            });
                        }
                        db.TransMasterDetails.AddRange(transMasterDetails);
                    }
                    transaction.Commit(); // if there is error here it should not save changes
                    db.SaveChanges();

                    if (transactionModel.IsTillTransfer)
                    {
                        _tillService.AcceptTillTransfer(transactionModel.TillTransferID, result);
                    }

                    Utils.LogNO("Executed transaction " + result);
                    string transactionRefernceView = null;

                    if (transactionModel.TransRef == null || transactionModel.TransRef == string.Empty)
                    {
                        transactionRefernceView = Convert.ToString(result);
                    }
                    else
                    {
                        transactionRefernceView = transactionModel.TransRef;
                    }
                    return new
                    {
                        success = true,
                        message = "transaction saved successfully",
                        TransactionRef = transactionRefernceView
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new
                    {
                        success = false,
                        message = ex.Message,
                        innerError = ex
                    };

                }
            }
        }

        public int RemoveAlreadyCreatedTransaction(long TranId)
        {
            int result = 0;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var tran = db.TransactionsMaster.Find(TranId);

                    if (tran != null)
                    {
                       result =  Convert.ToInt16(db.TransactionsMaster.Remove(tran));
                        if(result > 0)
                        {
                            var ben = db.TransferHeader.Find(TranId);
                            if(ben != null)
                            {
                                result = 0;
                                result = Convert.ToInt16(db.TransferHeader.Remove(ben));
                            }
                            if (result > 0)
                            {
                                result = 0;
                                var beneficiaries = db.TransferDetails.Where(x=>x.TranId==TranId).ToList();
                                if(beneficiaries !=null)
                                   result = Convert.ToInt16(db.TransferDetails.RemoveRange(beneficiaries));

                                if (result > 0)
                                {
                                    var fileDetail = db.TransactionFiles.Find(TranId);

                                    if (fileDetail != null)
                                    {
                                        result = Convert.ToInt16(db.TransactionFiles.Remove(fileDetail));
                                    }

                                }
                            }
                               

                        }
                    }

                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Utils.LogNO("Error occured while removing from local db: " + ex.Message);
                }
            }
                return result;

        }

        public List<TransferDetails> RetrieveBeneficiaries(long tranId)
        {
            return db.TransferDetails.Where(x => x.TranId == tranId).ToList();
        }

        public TransferHeader RetrieveTransferHeader(long tranId)
        {
            return db.TransferHeader.Where(x => x.TranId == tranId).FirstOrDefault();
        }
        public int CreateTransaction(TransactionModel transactionModel, int statusId)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int result = 0; //oya
                                    //string transRef = GenerateTransactionReference();
                   // Utils.LogNO("TransactionModel Request: " + JsonConvert.SerializeObject(transactionModel));
                    string narration = BuildNarration(transactionModel.ChequeNo, transactionModel.Beneficiary,
                        transactionModel.TransRef, transactionModel.Narration, transactionModel.TransName, statusId);
                    Utils.LogNO("Transtype " + transactionModel.TransType);
                    // string MachineName = GeneralService.DetermineCompName(transactionModel.MachineName);
                    //no transref. Should we add?  should we add colun for remark, has mandate, has memo?Answer. yes sir.ok
                    var param = new SqlParameter[]
                      {
                           new SqlParameter("@TotalAmount", transactionModel.TotalAmt),
                           new SqlParameter("@TellerId", transactionModel.TellerId),
                           new SqlParameter("@ToTellerId", transactionModel.ToTellerId),
                           new SqlParameter("@TransacterName", transactionModel.TransName),
                           new SqlParameter("@AccountName", transactionModel.AccountName),
                           new SqlParameter("@TransacterEmail", transactionModel.TransacterEmail),
                           new SqlParameter("@TransacterPhoneNo", transactionModel.PhoneNo),
                           new SqlParameter("@TransactionParty", transactionModel.TransactionParty),
                           new SqlParameter("@Status", transactionModel.Status),
                           new SqlParameter("@AccountNumber", transactionModel.AccountNo),
                           new SqlParameter("@SortCode", transactionModel.SortCode),

                           new SqlParameter("@TransType", Convert.ToInt16(transactionModel.TransType)),
                           new SqlParameter("@Currency", transactionModel.Currency),
                           new SqlParameter("@ValueDate", transactionModel.ValueDate),
                           new SqlParameter("@Narration", narration),
                           new SqlParameter("@CreationDate", DateTime.Now),
                           new SqlParameter("@MachineName", transactionModel.MachineName),
                           new SqlParameter("@TransRef", transactionModel.TransRef),

                           new SqlParameter("@hasMemo", transactionModel.hasMemo),
                           new SqlParameter("@hasMandate", transactionModel.hasMandate),
                           new SqlParameter("@Remarks", transactionModel.Remark),
                           new SqlParameter("@NeededApproval", transactionModel.NeededApproval),
                           new SqlParameter("@ApprovedBy", transactionModel.ApprovedBy),
                           new SqlParameter("@WhenApproved", DateTime.Now),
                           new SqlParameter("@Approved",transactionModel.Approved),
                           new SqlParameter("@IsReversed",transactionModel.IsReversed),
                           new SqlParameter("@DisapprovalReason",transactionModel.DisapprovalReason),
                           new SqlParameter("@DisapprovedBy",transactionModel.DisapprovedBy),
                           new SqlParameter("@WhenDisapproved",transactionModel.WhenDisapproved),

                           new SqlParameter("@BranchCode",transactionModel.BranchCode),
                           new SqlParameter("@Posted",transactionModel.Posted),
                           new SqlParameter("@CBAResponse",transactionModel.CBAResponse),
                           new SqlParameter("@CBACode", transactionModel.CBACode),
                           new SqlParameter("@CBA", transactionModel.CBA),
                           new SqlParameter("@ReversedTranId", transactionModel.IsReversed==true?transactionModel.TranId:transactionModel.ReversedTranId),


                      };

                    result = db.Database.SqlQuery<int>("dbo.TransactionsMaster_Create @TotalAmount,@TellerId,@ToTellerId,@TransacterName,@AccountName, @TransacterEmail," +
                              "@TransacterPhoneNo,@TransactionParty,@Status,@AccountNumber,@SortCode,@TransType,@Currency,@ValueDate,@Narration," +
                              "@CreationDate,@MachineName,@TransRef,@hasMemo,@hasMandate,@Remarks,@NeededApproval,@ApprovedBy,@WhenApproved," +
                              "@Approved,@IsReversed,@DisapprovalReason,@DisapprovedBy,@WhenDisapproved,@BranchCode,@Posted,@CBAResponse,@CBACode,@CBA," +
                              "@ReversedTranId", param)
                               .FirstOrDefault();

                    Utils.LogNO("Executed SP " + result);
                    CreateTransactionDetails(transactionModel, result);
                    // transactionModel.TransRef = result.ToString();
                    if (transactionModel.TransactionDetailsModels.Count() > 0 && result > 0)
                    {
                        List<TransMasterDetails> transMasterDetails = new List<TransMasterDetails>();
                        foreach (var transDetails in transactionModel.TransactionDetailsModels)
                        {
                            transMasterDetails.Add(new TransMasterDetails
                            {
                                Counter = transDetails.Counter,
                                TranId = result, //increase d identity to 1000 ok
                                Amount = transDetails.Amount
                            });
                        }
                        db.TransMasterDetails.AddRange(transMasterDetails);
                    }

                    
                    transaction.Commit(); // if there is error here it should not save changes
                    db.SaveChanges();

                    if (transactionModel.IsTillTransfer)
                    {
                        _tillService.AcceptTillTransfer(transactionModel.TillTransferID, result);
                    }

                    Utils.LogNO("Executed transaction " + result);
                    string transactionRefernceView = null;

                    if (transactionModel.TransRef == null || transactionModel.TransRef == string.Empty)
                    {
                        transactionRefernceView = Convert.ToString(result);
                    }
                    else
                    {
                        transactionRefernceView = transactionModel.TransRef;
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Utils.LogNO("Error occured while saving to local db: " + ex.Message);
                }
            }
            return 0;
        }


        public int CreateTransactionForImal(TransactionModel transactionModel) 
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int result = 0; //oya
                                    //string transRef = GenerateTransactionReference();
                                    // Utils.LogNO("TransactionModel Request: " + JsonConvert.SerializeObject(transactionModel));
                    string narration = BuildNarration(transactionModel.ChequeNo, transactionModel.Beneficiary,
                        transactionModel.TransRef, transactionModel.Narration, transactionModel.TransName, 3);
                    Utils.LogNO("Transtype " + transactionModel.TransType);
                    // string MachineName = GeneralService.DetermineCompName(transactionModel.MachineName);
                    //no transref. Should we add?  should we add colun for remark, has mandate, has memo?Answer. yes sir.ok
                    var param = new SqlParameter[]
                      {
                           new SqlParameter("@TotalAmount", transactionModel.TotalAmt),
                           new SqlParameter("@TellerId", transactionModel.TellerId),
                           new SqlParameter("@ToTellerId", transactionModel.ToTellerId),
                           new SqlParameter("@TransacterName", transactionModel.TransName),
                           new SqlParameter("@AccountName", transactionModel.AccountName),
                           new SqlParameter("@TransacterEmail", transactionModel.TransacterEmail),
                           new SqlParameter("@TransacterPhoneNo", transactionModel.PhoneNo),
                           new SqlParameter("@TransactionParty", transactionModel.TransactionParty),
                           new SqlParameter("@Status", transactionModel.Status),
                           new SqlParameter("@AccountNumber", transactionModel.AccountNo),
                           new SqlParameter("@SortCode", transactionModel.SortCode),

                           new SqlParameter("@TransType", Convert.ToInt16(transactionModel.TransType)),
                           new SqlParameter("@Currency", transactionModel.Currency),
                           new SqlParameter("@ValueDate", transactionModel.ValueDate),
                           new SqlParameter("@Narration", narration),
                           new SqlParameter("@CreationDate", DateTime.Now),
                           new SqlParameter("@MachineName", transactionModel.MachineName),
                           new SqlParameter("@TransRef", transactionModel.TransRef),

                           new SqlParameter("@hasMemo", transactionModel.hasMemo),
                           new SqlParameter("@hasMandate", transactionModel.hasMandate),
                           new SqlParameter("@Remarks", transactionModel.Remark),
                           new SqlParameter("@NeededApproval", transactionModel.NeededApproval),
                           new SqlParameter("@ApprovedBy", transactionModel.ApprovedBy),
                           new SqlParameter("@WhenApproved", DateTime.Now),
                           new SqlParameter("@Approved",transactionModel.Approved),
                           new SqlParameter("@IsReversed",transactionModel.IsReversed),
                           new SqlParameter("@DisapprovalReason",transactionModel.DisapprovalReason),
                           new SqlParameter("@DisapprovedBy",transactionModel.DisapprovedBy),
                           new SqlParameter("@WhenDisapproved",transactionModel.WhenDisapproved),

                           new SqlParameter("@BranchCode",transactionModel.Branch),
                           new SqlParameter("@Posted",transactionModel.Posted),
                           new SqlParameter("@CBAResponse",transactionModel.CBAResponse),
                           new SqlParameter("@CBACode", transactionModel.CBACode),
                           new SqlParameter("@CBA", transactionModel.CBA),
                           new SqlParameter("@ReversedTranId", transactionModel.IsReversed==true?transactionModel.TranId:transactionModel.ReversedTranId),


                      };

                    result = db.Database.SqlQuery<int>("dbo.TransactionsMaster_Create @TotalAmount,@TellerId,@ToTellerId,@TransacterName,@AccountName, @TransacterEmail," +
                              "@TransacterPhoneNo,@TransactionParty,@Status,@AccountNumber,@SortCode,@TransType,@Currency,@ValueDate,@Narration," +
                              "@CreationDate,@MachineName,@TransRef,@hasMemo,@hasMandate,@Remarks,@NeededApproval,@ApprovedBy,@WhenApproved," +
                              "@Approved,@IsReversed,@DisapprovalReason,@DisapprovedBy,@WhenDisapproved,@BranchCode,@Posted,@CBAResponse,@CBACode,@CBA," +
                              "@ReversedTranId", param)
                               .FirstOrDefault();

                    Utils.LogNO("Executed SP " + result);
                    CreateTransactionDetails(transactionModel, result);
                    // transactionModel.TransRef = result.ToString();
                    if (transactionModel.TransactionDetailsModels.Count() > 0 && result > 0)
                    {
                        List<TransMasterDetails> transMasterDetails = new List<TransMasterDetails>();
                        foreach (var transDetails in transactionModel.TransactionDetailsModels)
                        {
                            transMasterDetails.Add(new TransMasterDetails
                            {
                                Counter = transDetails.Counter,
                                TranId = result, //increase d identity to 1000 ok
                                Amount = transDetails.Amount
                            });
                        }
                        db.TransMasterDetails.AddRange(transMasterDetails);
                    }


                    transaction.Commit(); // if there is error here it should not save changes
                    db.SaveChanges();

                    if (transactionModel.IsTillTransfer)
                    {
                        _tillService.AcceptTillTransfer(transactionModel.TillTransferID, result);
                    }

                    Utils.LogNO("Executed transaction " + result);
                    string transactionRefernceView = null;

                    if (transactionModel.TransRef == null || transactionModel.TransRef == string.Empty)
                    {
                        transactionRefernceView = Convert.ToString(result);
                    }
                    else
                    {
                        transactionRefernceView = transactionModel.TransRef;
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Utils.LogNO("Error occured while saving to local db: " + ex.Message);
                }
            }
            return 0;
        }



        public int CreateTransactionDeals(TransactionModel transactionModel)
        {
            using (var context = new BranchConsoleEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        int result = 0;
                        string narration = BuildNarration(transactionModel.ChequeNo, transactionModel.Beneficiary,
                            transactionModel.TransRef, transactionModel.Narration, transactionModel.TransName, 3);
                        Utils.LogNO("Transtype " + transactionModel.TransType);
                        var transType =  Convert.ToInt16(TransType.TransactionType.TreasuryTransfer);
                        var param = new SqlParameter[]
                          {
                           new SqlParameter("@TotalAmount", transactionModel.TotalAmt),
                           new SqlParameter("@TellerId", transactionModel.TellerId),
                           new SqlParameter("@ToTellerId", transactionModel.ToTellerId),
                           new SqlParameter("@TransacterName", transactionModel.TransName),
                           new SqlParameter("@AccountName", transactionModel.AccountName),
                           new SqlParameter("@TransacterEmail", transactionModel.TransacterEmail),
                           new SqlParameter("@TransacterPhoneNo", transactionModel.PhoneNo),
                           new SqlParameter("@TransactionParty", transactionModel.TransactionParty),
                           new SqlParameter("@Status", transactionModel.Status),
                           new SqlParameter("@AccountNumber", transactionModel.AccountNo),
                           new SqlParameter("@SortCode", transactionModel.SortCode),

                           new SqlParameter("@TransType", transType),
                           new SqlParameter("@Currency", transactionModel.Currency),
                           new SqlParameter("@ValueDate", transactionModel.ValueDate),
                           new SqlParameter("@Narration", narration),
                           new SqlParameter("@CreationDate", DateTime.Now),
                           new SqlParameter("@MachineName", transactionModel.MachineName),
                           new SqlParameter("@TransRef", transactionModel.TransRef),

                           new SqlParameter("@hasMemo", transactionModel.hasMemo),
                           new SqlParameter("@hasMandate", transactionModel.hasMandate),
                           new SqlParameter("@Remarks", transactionModel.Remark),
                           new SqlParameter("@NeededApproval", transactionModel.NeededApproval),
                           new SqlParameter("@ApprovedBy", transactionModel.ApprovedBy),
                           new SqlParameter("@WhenApproved", DateTime.Now),
                           new SqlParameter("@Approved",transactionModel.Approved),
                           new SqlParameter("@IsReversed",transactionModel.IsReversed),
                           new SqlParameter("@DisapprovalReason",transactionModel.DisapprovalReason),
                           new SqlParameter("@DisapprovedBy",transactionModel.DisapprovedBy),
                           new SqlParameter("@WhenDisapproved",transactionModel.WhenDisapproved),

                           new SqlParameter("@BranchCode",transactionModel.BranchCode),
                           new SqlParameter("@Posted",true),
                           new SqlParameter("@CBAResponse",transactionModel.CBAResponse),
                           new SqlParameter("@CBACode", transactionModel.CBACode),
                           new SqlParameter("@CBA", transactionModel.CBA),
                           new SqlParameter("@ReversedTranId", transactionModel.IsReversed==true?transactionModel.TranId:transactionModel.ReversedTranId),


                          };

                        result = context.Database.SqlQuery<int>("dbo.TransactionsMaster_Create @TotalAmount,@TellerId,@ToTellerId,@TransacterName,@AccountName, @TransacterEmail," +
                                  "@TransacterPhoneNo,@TransactionParty,@Status,@AccountNumber,@SortCode,@TransType,@Currency,@ValueDate,@Narration," +
                                  "@CreationDate,@MachineName,@TransRef,@hasMemo,@hasMandate,@Remarks,@NeededApproval,@ApprovedBy,@WhenApproved," +
                                  "@Approved,@IsReversed,@DisapprovalReason,@DisapprovedBy,@WhenDisapproved,@BranchCode,@Posted,@CBAResponse,@CBACode,@CBA," +
                                  "@ReversedTranId", param)
                                   .FirstOrDefault();

                        Utils.LogNO("Executed SP " + result);
                        //List<TransferDetails> transferDetails = new List<TransferDetails>();
                        //Utils.LogNO("TransactionBeneficiary: " + transactionModel.TransactionBeneficiary);
                        //foreach (var transferDetail in transactionModel.TransactionBeneficiary)
                        //{
                        //    transferDetails.Add(new TransferDetails
                        //    {
                        //        TranId = result,
                        //        BenAccountNumber = transferDetail.AccountNumber, //increase d identity to 1000 ok
                        //        BenAccountName = transferDetail.AccountName,
                        //        Amount = transferDetail.Amount,
                        //        Narration = transferDetail.Narration,
                        //        ApplicableCharge = transferDetail.ChargeAmt,
                        //        TransRef = transferDetail.TransRef,
                        //        Posted = true
                        //    });
                        //}

                        //context.TransferDetails.AddRange(transferDetails);

                        transaction.Commit(); // if there is error here it should not save changes
                        context.SaveChanges();
                        return result;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Utils.LogNO("Error occured while saving to local db: " + ex.Message);
                    }
                }
            }
           
            return 0;
        }


        public void CreateRangeTransaction(List<TransactionModel> transactionModels) 
        {
            Utils.LogNO("Transmaster begin 1");
            using (var context = new BranchConsoleEntities())
            {
                Utils.LogNO("Transmaster begin 2");
                try
                {
                    int transtype = Convert.ToInt16(TransType.TransactionType.TreasuryTransfer);
                    List<TransactionsMaster> transactionsMasters = new List<TransactionsMaster>();
                    int count = 0;
                    List<TreasuryDetails> treasuryDetails = new List<TreasuryDetails>();
                    foreach (var transactionModel in transactionModels)
                    {
                        treasuryDetails.Add(new TreasuryDetails
                        {
                            AccountName = "",
                            AccountNumber = transactionModel.AccountNo,
                           // DealId = transactionModel.ToTellerId,
                            CBAResponse = transactionModel.CBAResponse,
                            CBAResponseCode = transactionModel.CBACode,
                            Posted = true,
                            IsReversed = false,
                            Amount = transactionModel.Amount,
                            CrDr = "D",
                            Naration = transactionModel.Remark,
                            TransRef = transactionModel.TransRef,
                        });
                        treasuryDetails.Add(new TreasuryDetails
                        {
                            AccountName = "",
                            AccountNumber = transactionModel.CustomerAcctNos,
                          //  DealId = transactionModel.ToTellerId,
                            CBAResponse = transactionModel.CBAResponse,
                            CBAResponseCode = transactionModel.CBACode,
                            Posted = true,
                            IsReversed = false,
                            Amount = transactionModel.Amount,
                            CrDr = "C",
                            Naration = transactionModel.Remark,
                            TransRef = transactionModel.TransRef
                        });
                        count = count + 1;
                       // Utils.LogNO("transactionsMasters: " + count + JsonConvert.SerializeObject(transactionModel));
                      //  Utils.LogNO("Transtype " + transactionModel.TransType);
                    }
                    
                    var transaction = transactionModels[0];
                    TransactionsMaster transactionsMaster =   new TransactionsMaster
                    {
                        TotalAmount = transaction.TotalAmt,
                        TellerId = transaction.TellerId,
                        ToTellerId = "",
                        TransacterName = transaction.TransName,
                        AccountName = "",
                        Status = 3,
                        AccountNumber = "",
                        Currency = Convert.ToInt32(transaction.Currency),
                        TransType = transtype,
                        ValueDate = DateTime.Now,
                        Narration = "Treasury deal fund transfer on deal " + transaction.ToTellerId,
                        TransRef = "",
                        hasMemo = true,
                        hasMandate = true,
                        Remarks = "Treasury deal fund transfer on deal " + transaction.ToTellerId,
                        NeededApproval = false,
                        ApprovedBy = transaction.TellerId,
                        Approved = true,
                        IsReversed = false,
                        DisapprovalReason = "",
                        DisapprovedBy = "",
                        BranchCode = transaction.BranchCode,
                        Posted = true,
                        CBAResponse = "",
                        CBACode = "",
                        CBA = "T24",
                        TreasuryDetails = treasuryDetails
                    };

                    context.TransactionsMaster.Add(transactionsMaster);
                    //transaction.Commit();
                    context.SaveChanges();
                    Utils.LogNO("Transmaster Saved");
                }
                catch (Exception ex)
                {
                   // transaction.Rollback();
                    Utils.LogNO("Error occured while saving to local db: " + ex.Message);
                }
            }
        }



        public int UpdateBeneficiaryForPostStatusHeader(TransferHeader detail)
        {
            int result = 0;
            try
            {
                if (detail != null)
                {
                    db.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    result = 1;

                }

            }
            catch (Exception ex)
            {
                Utils.LogNO("Error updating beneficiary Header: ID - " + detail.Id + " because:  " + ex.Message);
            }

            return result;
        }


        public int UpdateBeneficiaryForPostStatus(TransferDetails detail)
        {
            int result = 0;
            try
            {
                if (detail != null)
                {
                    db.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    result = 1;

                }
            
            }
            catch (Exception ex)
            {
                Utils.LogNO("Error updating beneficiary detail: ID - " +  detail.Id +" because:  "+ ex.Message);
            }

            return result;
        }

        public int UpdateStatusForMasterTrans(TransactionModel transMaster)
        {
            int result = 0;
            try
            {
                var detail = db.TransactionsMaster.Find(transMaster.TranId);
                if (detail != null)
                {
                    detail.Posted = transMaster.Posted;
                    detail.CBACode = transMaster.CBACode;
                    detail.CBAResponse = transMaster.CBAResponse;
                    detail.TransRef = transMaster.TransRef;
                    
                    if (transMaster.Status == 2)
                    {
                        detail.Approved = true;
                        detail.ApprovedBy = transMaster.ApprovedBy;
                        detail.WhenApproved = DateTime.Now;
                        detail.Status = transMaster.Status;
                    }

                    db.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    result = 1;

                }

            }
            catch (Exception ex)
            {
                Utils.LogNO("Error updating Transaction detail: ID - " + transMaster.TranId + " because:  " + ex.Message);
            }

            return result;
        }
        public void ReverseTransaction(TellerReversal tellerReversal)
        {
            try
            {
                var transmaster = db.TransactionsMaster
                    .Where(x => x.TranId == tellerReversal.TranId).FirstOrDefault();
                if(transmaster == null)
                {
                    throw new Exception("Transaction with reference ID "
                        + tellerReversal.TReference + "does not exist");
                }
                else
                {
                    transmaster.IsReversed = true;
                    db.Entry(transmaster).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string BuildNarration( string SerialNo, string Beneficiary, string transRef, string remarks,string depositor, int status, string transType="") //ok lets factor what MD said as per narration here
        {
            Utils.LogNO("Building Narration inside Transaction. TransRef:" + transRef + ", serialNo:" + SerialNo + ", status:" + status + ", transtype:" + transType);
            string narration = string.Empty;
            string word = "";
            if (!string.IsNullOrEmpty(SerialNo) && SerialNo.Length == 8)
                word = "CHEQUE";
            else
                word = "CASH";

            //if(status==1)//cheque withdrawal
            //narration = @" " + transRef +  @" CASH WTD B/O " + @" " + Beneficiary + @" CHQ" + @" " +
            //            SerialNo + @" " + remarks;
            //if (status == 2)//cheque deposit
            //    narration = @" " + transRef + @" CASH DEPOSIT B/O " + @" " + depositor + @" CHQ" + @" " +
            //            SerialNo + @" " + remarks;
            //if (status == 3)//Pure cash withdrawal
            //    narration = @" " + transRef + @" CASH WTD B/O " + @" " + depositor + @" " + remarks;
            //if (status == 4)//Pure cash deposit
            //    narration = @" " + transRef + @" CASH DEPOSIT B/O " + @" " + depositor + @" " + remarks;

            //if (status == 1)//cheque withdrawal
            //    narration = @" " + transRef + @"" + word + " WTD B/O " + @" " + Beneficiary + @" CHQ" + @" " +
            //                SerialNo + @" " + remarks;
            //if (status == 2)//cheque deposit
            //    narration = @" " + transRef + @"" + word + " DEPOSIT B/O " + @" " + depositor + @" CHQ" + @" " +
            //            SerialNo + @" " + remarks;
            //if (status == 3)//Pure cash withdrawal
            //    narration = @" " + transRef + @"" + word + "  B/O " + @" " + depositor + @" " + remarks;
            //if (status == 4)//Pure cash deposit
            //    narration = @" " + transRef + @"" + word + " DEPOSIT B/O " + @" " + depositor + @" " + remarks;

            //if (status == 1)//cheque withdrawal
            //    narration = @" " + transRef + @" CASH WTD B/O " + @" " + Beneficiary  + @" " +
            //                SerialNo + @" " + remarks;
            //if (status == 2)//cheque deposit
            //    narration = transRef + @"CHEQUE DEPOSIT B/O " + depositor + @" CHQ" + @" " +
            //            SerialNo + @" " + remarks;
            //if (status == 3)//Pure cash withdrawal
            //    narration = @" " + transRef + @" CASH WTD B/O CHQ" + @" " + depositor + @" " + remarks;
            //if (status == 4)//Pure cash deposit
            //    narration = @" " + transRef + @" CASH DEPOSIT B/O " + @" " + depositor + @" " + remarks;
            //if (status == 5)//Pure cash deposit
            //    narration = @" " + transRef + @" CASH DEPOSIT B/O " + @" " + depositor + @" " + remarks;

            //if (status == 6)//Pure cash deposit
            //    narration = @" " + transRef + @" FCY DEPOSIT B/O " + @" " + depositor + @" " + remarks;


           
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

            if (transType == "13" || transType=="3")
            {
                narration = @" " + transRef + @" CASH WTD CHQ B/O" + @" " + depositor + @" " + remarks;
            }



            Utils.LogNO("naration " + narration);
            return narration;
        }

        public string GenerateTransactionReference()
        {
            string transRef = "STERLNB";
            Random rnd = new Random();
            string[] alphabets = {"A", "B" , "D" , "E" , "F" , "G" , "H" , "I" , "J" , "K" , "L" , "M" , "N" , "O" , "P"
                                    ,"Q","R","S","T", "U", "V", "W", "X", "Y", "Z"};
            int alphabetRandom = rnd.Next(alphabets.Length);
            int alphabetRandom2 = rnd.Next(alphabets.Length);
            transRef = transRef + "-" + rnd.Next(100, 1000) + alphabets[alphabetRandom] + alphabets[alphabetRandom2] + "-" + rnd.Next(1000, 10000);
            return transRef;
        }

        public object GetDepositTrans(bool success, string message, Exception ex = null)
        {
            return new
            {
                success,
                message,
                innerError = ex,
                data = new
                {
                    Transaction = db.TransMaster.Select(x => new
                    {
                     
                        x.TranId,
                        x.TotalAmt,
                        x.CashierID,
                        x.CashierTillGL,
                        x.CashierTillNos,
                        x.Currency,
                        x.CustomerAcctNos,
                        x.Status,
                        x.DepositorName,
                        x.DepositorPhoneNo,
                        x.Creation_Date,
                        x.Address,
                        x.MachineName,
                        x.Narration,
                        x.SupervisoryUser,
                        x.SortCode,
                        x.ValueDate,
                        x.WhenApproved,
                        x.Occupation,
                        x.IDNo,
                        x.IDType,
                        x.MotherMaidenNAme
                                              
                    }).ToArray(),
                    TransDetails = db.TransDetails.Select(y => new
                    {
                        y.Id,
                        y.Amount,
                        y.Counter,
                        y.TranId
                    }).ToArray()

                }

            };
        }

        //public object CreateTransactionDeposit(TransMaster transMaster)
        //{

        //    using (var transaction = db.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            int result = 0;
        //            string transRef = GenerateTransactionReference();
        //            string narration = BuildNarration(transMaster.Chequeno, transMaster.Beneficiary, transRef,
        //                transMaster.Narration, transMaster.DepositorName, 4);

        //           // string MachineName = GeneralService.DetermineCompName(transMaster.MachineName);
        //            //no transref. Should we add?  should we add colun for remark, has mandate, has memo?Answer. yes sir.ok
        //            var param = new SqlParameter[]
        //               {
        //                    new SqlParameter("@CustomerAcctNos", transMaster.CustomerAcctNos),
        //                    new SqlParameter("@TotalAmt", transMaster.TotalAmt),
        //                    new SqlParameter("@CashierID", transMaster.CashierID),
        //                    new SqlParameter("@CashierTillNos", transMaster.CashierTillNos),
        //                    new SqlParameter("@CashierTillGL", transMaster.CashierTillGL),
        //                    new SqlParameter("@DepositorName", transMaster.DepositorName),
        //                    new SqlParameter("@DepositorPhoneNo", transMaster.DepositorPhoneNo),
        //                    new SqlParameter("@Status", transMaster.Status),
        //                    new SqlParameter("@WhenApproved", DateTime.Now),
        //                    new SqlParameter("@SortCode", transMaster.SortCode),
        //                    new SqlParameter("@Currency", transMaster.Currency),
        //                    new SqlParameter("@ValueDate", transMaster.ValueDate),
        //                    new SqlParameter("@SupervisoryUser", transMaster.SupervisoryUser),
        //                    new SqlParameter("@Narration", narration),
        //                    new SqlParameter("@Creation_Date", DateTime.Now),
        //                    new SqlParameter("@Occupation", transMaster.Occupation),
        //                    new SqlParameter("@DOB", transMaster.DOB),
        //                    new SqlParameter("@MotherMaideNAme", transMaster.MotherMaidenNAme),
        //                    new SqlParameter("@Address", transMaster.Address),
        //                    new SqlParameter("@IDType", transMaster.IDType),
        //                    new SqlParameter("@IDNo", transMaster.IDNo),
        //                    new SqlParameter("@Machine_Name", transMaster.MachineName),
        //                    new SqlParameter("@NeededApproval", transMaster.NeededApproval),
        //                    new SqlParameter("@Remark", transMaster.Narration),
        //                    new SqlParameter("@TransType", 2),
        //                    new SqlParameter("@Chequeno", transMaster.Chequeno),
        //                    new SqlParameter("@DateonCheque", transMaster.DateonCheque),
        //                    new SqlParameter("@Beneficiary", transMaster.Beneficiary)
                 
        //               };
        //            result = db.Database.SqlQuery<int>("dbo.TransactionDeposit_Create @CustomerAcctNos,@TotalAmt, @CashierID, @CashierTillNos," +
        //                "@CashierTillGL,@DepositorName,@DepositorPhoneNo,@Status,@WhenApproved,@SortCode,@Currency,@ValueDate,@SupervisoryUser,@Narration, " +
        //                "@Creation_Date,@Occupation,@DOB,@MotherMaideNAme,@Address,@IDType,@IDNo,@Machine_Name,@NeededApproval,@Remark,@TransType,@Chequeno,@DateonCheque,@Beneficiary", param)
        //                 .FirstOrDefault();



        //            if (transMaster.transMasterDetails.Count() > 0 && result > 0)
        //            {
        //                List<TransMasterDetails> transMasterDetails = new List<TransMasterDetails>();
        //                foreach (var transDetails in transMaster.transMasterDetails)
        //                {
        //                    transMasterDetails.Add(new TransMasterDetails
        //                    {
        //                        Counter = transDetails.Counter,
        //                        TranId = result,
        //                        Amount = transDetails.Amount
        //                    });
        //                }
        //                db.TransMasterDetails.AddRange(transMasterDetails);
        //                transaction.Commit(); // if there is error here it should not save changes
        //                db.SaveChanges();
        //            }
        //            return new
        //            {
        //                success = true,
        //                message = "transaction saved successfully!",
        //                TransactionRef = result
        //            };
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            return new
        //            {
        //                success = false,
        //                message = ex.Message,
        //                innerError = ex
        //            };

        //        }
        //    }
        //}

        public object GetAllTrans(bool success, string message, Exception ex = null, string transRef = null)
        {
            int withdrawal = Convert.ToInt16(TransType.TransactionType.Withdrawal);
            int deposit = Convert.ToInt16(TransType.TransactionType.Deposit);
            int cheque = Convert.ToInt16(TransType.TransactionType.InHouseChequesDeposit);
            int transfer = Convert.ToInt16(TransType.TransactionType.InHouseTransfer);
            int counterWTD = Convert.ToInt16(TransType.TransactionType.CashWithDrawalCounter);
            int WTDCHQ = Convert.ToInt16(TransType.TransactionType.ChequeLodgement);
            return new
            {
                success,
                message,
                TransactionRef = transRef,
                innerError = ex,
                data = new
                {
                    Transaction = db.TransactionsMaster.Select(x => new
                    {
                        x.TranId,
                        x.TotalAmount,
                        x.Currency,
                        x.Status,
                        x.TransacterPhoneNo,
                        x.TransacterEmail,
                        x.TransactionParty,
                        x.TransacterName,
                        x.AccountName,
                        x.CreationDate,
                        Beneficiary=x.CashTransactions.Where(y => y.TranId != x.TranId).Select(y => y.TillId).FirstOrDefault(),
                        x.MachineName,
                        x.Narration,
                        x.ApprovedBy,
                        x.SortCode,
                        x.ValueDate,
                        x.WhenApproved,
                        x.WhenDisapproved,
                        x.CBAResponse,
                        x.Posted,
                        x.hasMandate,
                        x.hasMemo,
                        x.Remarks,
                        x.TransRef,
                        x.TransType,
                        x.IsReversed,
                        x.BranchCode,
                        x.NeededApproval,
                        x.CBA,
                        x.Approved,
                        x.TellerId,
                        AccountNo =x.AccountNumber,
                        CurrncyCode = db.CashDenomination.Where(y=> y.ID == x.Currency).Select(y=> y.Abbrev).FirstOrDefault(),
                        //TransTypeName = TransType.getTranTypeName(x.TransType),
                        TransTypeName = x.TransType == withdrawal ? "Cash Withdrawal" : x.TransType == deposit ? "Cash Deposit"
                                    : x.TransType == cheque ? "InHouse Cheque Deposit" : x.TransType==transfer? "In-House Transfer":x.TransType==WTDCHQ?"Cash Withdrawal with Cheque": x.TransType==counterWTD? "Cash Withdrawal with Cheque":"Other",
                        TransactionBeneficiary = db.TransferDetails.ToList().Where(y=>y.TranId==x.TranId),
                        //TransactionBeneficiary = db.TransferDetails.Where(y => y.TranId == x.TranId)
                        //.Select(y => new
                        //{
                        //    y.Id,
                        //    y.BenAccountNumber,
                        //    y.BenAccountName,
                        //    y.Amount,
                        //    y.Narration,
                        //    y.Posted,
                        //    y.CBAResponse,
                        //    y.CBAResponseCode,
                        //    y.TransRef,
                        //    y.ApplicableCharge

                        //}).ToArray(),
                        TransactionBeneficiaryHeader = db.TransferHeader.ToList().Where(y=>y.TranId==x.TranId),
                        //TransactionBeneficiaryHeader = db.TransferHeader.Where(y => y.TranId == x.TranId).
                        //Select(y => new
                        //{
                        //    y.Id,
                        //    y.TranId,
                        //    y.InstrumentNumber,
                        //    y.ApplicableCharge,
                        //    y.ChargeType,
                        //    y.IsBulkTran,

                        //}).ToArray(),
                        ChequeDetails = db.ChequeLodgement.Where(y=>y.TranId==x.TranId)
                        .Select(y=>new {
                            y.Id,
                            y.ChequeNo,
                            y.AccountNumber,
                            y.AccountName,
                            y.ChargeAmt,
                            y.ChequeDate,
                            y.TillId,
                            y.TransType
                        }).FirstOrDefault(),
                        FileDetails = db.TransactionFiles.Where(y => y.TranId == x.TranId)
                        .Select(y => new {
                            y.Id,
                            y.TotalAmount,
                            y.TotalCount,
                            y.FileName,
                            y.DateUploaded,
                            y.Branch,
                            y.UserId,
                            
                        }).FirstOrDefault(),
                    }).OrderByDescending(x=> x.CreationDate).ToArray(),
                        TransDetails = db.TransDetails.Select(y => new
                        {
                        y.Id,
                        y.Amount,
                        y.Counter,
                        y.TranId
                    }).ToArray(),
                                       
                }

            };
        }


        public object GetUnApprovedTrans(bool success, string message, Exception ex = null, string transRef = null) 
        {
            //Utils.LogNO("Getting unapproved trans here...");
            int withdrawal = Convert.ToInt16(TransType.TransactionType.Withdrawal);
            int deposit = Convert.ToInt16(TransType.TransactionType.Deposit);
            int wtdCheque = Convert.ToInt16(TransType.TransactionType.ChequeLodgement);
            int ClearingCHQ = Convert.ToInt16(TransType.TransactionType.ClearingCheque);
            int vaultOut = Convert.ToInt16(TransType.TransactionType.VaultOut);
            int vaultIn = Convert.ToInt16(TransType.TransactionType.vaultIn);
            int tillTrans = Convert.ToInt16(TransType.TransactionType.TillTransfer);
            int cheque = Convert.ToInt16(TransType.TransactionType.InHouseChequesDeposit);
            int transfer = Convert.ToInt16(TransType.TransactionType.InHouseTransfer);
            int chqWTDCounter= Convert.ToInt16(TransType.TransactionType.CashWithDrawalCounter);

            return new
            {
                success,
                message,
                TransactionRef = transRef,
                innerError = ex,
                data = new
                {
                    Transaction = db.TransactionsMaster.Where(x => x.NeededApproval == true && x.Approved == false
                    && x.Status != 3 && x.TransType != transfer).Select(x => new
                    {
                        x.TranId,
                        x.TotalAmount,
                        x.Currency,
                        x.Status,
                        x.TransacterPhoneNo,
                        x.TransacterEmail,
                        x.TransactionParty,
                        x.TransacterName,
                        x.AccountName,
                        x.CreationDate,
                        Beneficiary = x.CashTransactions.Where(y => y.TranId != x.TranId).Select(y => y.TillId).FirstOrDefault(),
                        x.MachineName,
                        x.Narration,
                        x.ApprovedBy,
                        x.SortCode,
                        x.ValueDate,
                        x.WhenApproved,
                        x.WhenDisapproved,
                        x.CBAResponse,
                        x.Posted,
                        x.hasMandate,
                        x.hasMemo,
                        x.Remarks,
                        x.TransRef,
                        x.TransType,
                        x.IsReversed,
                        x.BranchCode,
                        x.NeededApproval,
                        x.CBA,
                        x.Approved,
                        x.TellerId,
                        x.ToTellerId,
                        AccountNo = x.AccountNumber,
                        CurrncyCode = db.CashDenomination.Where(y => y.ID == x.Currency).Select(y => y.Abbrev).FirstOrDefault(),

                        TransTypeName = x.TransType == withdrawal ? "Cash Withdrawal" : x.TransType == deposit ? "Cash Deposit"
                                    : x.TransType == wtdCheque ? "Cash Withdrawal with Cheque" : x.TransType == ClearingCHQ ? "InHouse Cheque Deposit" : x.TransType == vaultOut?"Vault Out": x.TransType == vaultIn ? "Vault In": x.TransType == tillTrans ? "Till Transfer": 
                                    x.TransType == cheque?"InHouse Cheque Deposit" : x.TransType == transfer? "In-House Transfer" : x.TransType == chqWTDCounter ? "Cash Withdrawal with Counter Cheque":"Treasury Transfer",
                        //TransTypeName = TransType.getTranTypeName(x.TransType),
                        TransactionBeneficiary = db.TransferDetails.ToList().Where(y => y.TranId == x.TranId),
                        //TransactionBeneficiary = db.TransferDetails.Where(y => y.TranId == x.TranId)
                        //.Select(y => new
                        //{
                        //    y.Id,
                        //    y.BenAccountNumber,
                        //    y.BenAccountName,
                        //    y.Amount,
                        //    y.Narration,
                        //    y.Posted,
                        //    y.CBAResponse,
                        //    y.CBAResponseCode,
                        //    y.TransRef,
                        //    y.ApplicableCharge

                        //}).ToArray(),
                        TransactionBeneficiaryHeader = db.TransferHeader.ToList().Where(y => y.TranId == x.TranId),
                        //TransactionBeneficiaryHeader = db.TransferHeader.Where(y => y.TranId == x.TranId).
                        //Select(y => new
                        //{
                        //    y.Id,
                        //    y.TranId,
                        //    y.InstrumentNumber,
                        //    y.ApplicableCharge,
                        //    y.ChargeType,
                        //    y.IsBulkTran,

                        //}).ToArray(),
                        ChequeDetails = db.ChequeLodgement.Where(y => y.TranId == x.TranId)
                        .Select(y => new {
                            y.Id,
                            y.ChequeNo,
                            y.AccountNumber,
                            y.AccountName,
                            y.ChargeAmt,
                            y.ChequeDate,
                            y.TillId,
                            y.TransType
                        }).FirstOrDefault(),
                        FileDetails = db.TransactionFiles.Where(y => y.TranId == x.TranId)
                        .Select(y => new {
                            y.Id,
                            y.TotalAmount,
                            y.TotalCount,
                            y.FileName,
                            y.DateUploaded,
                            y.Branch,
                            y.UserId,

                        }).FirstOrDefault(),
                    }).OrderByDescending(x => x.CreationDate).ToArray(),
                    TransDetails = db.TransDetails.Select(y => new
                    {
                        y.Id,
                        y.Amount,
                        y.Counter,
                        y.TranId
                    }).ToArray(),

                }

            };

        }


        public object GetInHouseTransferTransaction(bool success, string message, Exception ex = null, string transRef = null)
        {
            int withdrawal = Convert.ToInt16(TransType.TransactionType.Withdrawal);
            int deposit = Convert.ToInt16(TransType.TransactionType.Deposit);
            int cheque = Convert.ToInt16(TransType.TransactionType.InHouseChequesDeposit);
            int transfer = Convert.ToInt16(TransType.TransactionType.InHouseTransfer);
            return new
            {
                success,
                message, 
                TransactionRef = transRef,
                innerError = ex,
                data = new
                {
                    Transaction = db.TransactionsMaster.Where(x => x.NeededApproval == true && x.Approved == false
                    && x.Status != 3 && x.TransType == transfer).Select(x => new
                    {
                        x.TranId,
                        x.TotalAmount,
                        x.Currency,
                        x.Status,
                        x.TransacterPhoneNo,
                        x.TransacterEmail,
                        x.TransactionParty,
                        x.TransacterName,
                        x.AccountName,
                        x.CreationDate,
                        Beneficiary = x.CashTransactions.Where(y => y.TranId != x.TranId).Select(y => y.TillId).FirstOrDefault(),
                        x.MachineName,
                        x.Narration,
                        x.ApprovedBy,
                        x.SortCode,
                        x.ValueDate,
                        x.WhenApproved,
                        x.WhenDisapproved,
                        x.CBAResponse,
                        x.Posted,
                        x.hasMandate,
                        x.hasMemo,
                        x.Remarks,
                        x.TransRef,
                        x.TransType,
                        x.IsReversed,
                        x.BranchCode,
                        x.NeededApproval,
                        x.CBA,
                        x.Approved,
                        x.TellerId,
                        AccountNo = x.AccountNumber,
                        CurrncyCode = db.CashDenomination.Where(y => y.ID == x.Currency).Select(y => y.Abbrev).FirstOrDefault(),
                        //TransTypeName = TransType.getTranTypeName(x.TransType),
                        TransTypeName = x.TransType == withdrawal ? "Cash Withdrawal" : x.TransType == deposit ? "Cash Deposit"
                                    : x.TransType == cheque ? "InHouse Cheque Deposit" : "In-House Transfer",
                        TransactionBeneficiary = db.TransferDetails.ToList().Where(y => y.TranId == x.TranId),
                        //TransactionBeneficiary = db.TransferDetails.Where(y => y.TranId == x.TranId)
                        //.Select(y => new
                        //{
                        //    y.Id,
                        //    y.BenAccountNumber,
                        //    y.BenAccountName,
                        //    y.Amount,
                        //    y.Narration,
                        //    y.Posted,
                        //    y.CBAResponse,
                        //    y.CBAResponseCode,
                        //    y.TransRef,
                        //    y.ApplicableCharge

                        //}).ToArray(),
                        TransactionBeneficiaryHeader = db.TransferHeader.ToList().Where(y => y.TranId == x.TranId),
                        //TransactionBeneficiaryHeader = db.TransferHeader.Where(y => y.TranId == x.TranId).
                        //Select(y => new
                        //{
                        //    y.Id,
                        //    y.TranId,
                        //    y.InstrumentNumber,
                        //    y.ApplicableCharge,
                        //    y.ChargeType,
                        //    y.IsBulkTran,

                        //}).ToArray(),
                        ChequeDetails = db.ChequeLodgement.Where(y => y.TranId == x.TranId)
                        .Select(y => new {
                            y.Id,
                            y.ChequeNo,
                            y.AccountNumber,
                            y.AccountName,
                            y.ChargeAmt,
                            y.ChequeDate,
                            y.TillId,
                            y.TransType
                        }).FirstOrDefault(),
                        FileDetails = db.TransactionFiles.Where(y => y.TranId == x.TranId)
                        .Select(y => new {
                            y.Id,
                            y.TotalAmount,
                            y.TotalCount,
                            y.FileName,
                            y.DateUploaded,
                            y.Branch,
                            y.UserId,

                        }).FirstOrDefault(),
                    }).OrderByDescending(x => x.CreationDate).ToArray(),
                    TransDetails = db.TransDetails.Select(y => new
                    {
                        y.Id,
                        y.Amount,
                        y.Counter,
                        y.TranId
                    }).ToArray(),

                }

            };
        }


        public string GetCurAbbrev(int currencyId)
        {
            return db.CashDenomination.Where(x => x.ID == currencyId).FirstOrDefault().Abbrev;
        }

        public object GetAllTreasuryTrans(bool success, string message, Exception ex = null, string transRef = null)
        {            
            return new
            {
                success,
                message,
                TransactionRef = transRef,
                innerError = ex,
                data = new
                {
                    TreasuryTransaction = db.TreasuryDealsMaster.Select(x => new
                    {
                        x.Id,
                        x.DealId,
                        x.CurrencyCode,
                        CurrencyName = db.CashDenomination.Where(y=> y.ID == x.CurrencyCode).FirstOrDefault().Currency,
                        x.CustomerId,
                        x.ProcessStatus,
                        x.PrincipalAccount,
                        x.PrincipalAmount,
                        x.AccountOfficer,
                        x.UserId,
                        x.ApprovedBy,
                        x.CBA,
                        x.CBADealId,
                        x.CreationDate,
                        x.MachineName,
                        x.DealersReference,
                        x.ValueDate,
                        x.WhenApproved,
                        x.WhenDisapproved,
                        x.Posted,
                        x.Remarks,
                        x.ProductCode,
                        x.ParentDealId,
                        x.BranchCode,
                        x.InflowAccount,
                        x.InterestAccount,
                        x.CustomerName,
                        x.InterestAmount,
                        x.PaymentAccount,
                        x.PaymentDate,
                        x.PaymentAmount,
                        x.Tenure,
                        x.TransactionStatus,
                        x.WHTAccount,
                        x.WHTAmount,
                        x.TerminationInstructionCode,
                        
                        x.MaturityDate,
                        x.DisapprovedBy,
                        x.DisapprovalReason,
                        x.NetInterestAmount,
                        TreasuryInterest = x.TreasuryInterest.AsQueryable(),
                        TerminationInstruction = db.TerminationInstruction.Where(y => y.Code == x.TerminationInstructionCode).AsQueryable(),
                        TreasuryProduct = db.TreasuryProductCode.Where(y => y.ProductCode == x.ProductCode).AsQueryable(),
                        PreLiquidation = db.PreLiquidatedDeal.Where(y=> y.DealID == x.DealId).AsQueryable(),
                        DateAccessed = x.UserTransactionPageAccess.Where(y=> y.IsActiveOnPage == true).Select(y=> y.DateAccessed).FirstOrDefault()
                    }).ToArray().OrderByDescending(x=> x.Id),
                    
                }

            };
        }

        public TreasuryDealsMaster GetAllTreasuryTransById(int TransId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            TreasuryDealsMaster Transaction = db.TreasuryDealsMaster.Find(TransId);

            return Transaction;

        }

        public int CreateTreasuryDealsTrans(TreasuryDealsModel treasuryDealsModel)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int result = 0; //oya
                                    //string transRef = GenerateTransactionReference();
                                    //Utils.LogNO("TransactionModel Request: " + JsonConvert.SerializeObject(transactionModel));

                    // string MachineName = GeneralService.DetermineCompName(transactionModel.MachineName);
                    //no transref. Should we add?  should we add colun for remark, has mandate, has memo?Answer. yes sir.ok


                    var TParam = new SqlParameter[]
                       {
                           new SqlParameter("@DealId", treasuryDealsModel.DealId),
                           new SqlParameter("@CBADealId", treasuryDealsModel.CBADealId),
                           new SqlParameter("@DealersReference", treasuryDealsModel.DealersReference),
                           new SqlParameter("@ProductCode", treasuryDealsModel.ProductCode),
                           new SqlParameter("@CurrencyCode", treasuryDealsModel.CurrencyCode),
                           new SqlParameter("@CustomerId", treasuryDealsModel.CustomerId),
                           new SqlParameter("@CustomerName", treasuryDealsModel.CustomerName),
                           new SqlParameter("@PrincipalAmount", treasuryDealsModel.PrincipalAmount),
                           new SqlParameter("@ValueDate", treasuryDealsModel.ValueDate),
                           new SqlParameter("@Tenure", treasuryDealsModel.Tenure),
                           new SqlParameter("@MaturityDate", treasuryDealsModel.MaturityDate),
                           new SqlParameter("@PaymentDate", treasuryDealsModel.PaymentDate),

                           new SqlParameter("@InterestAmount",treasuryDealsModel.InterestAmount),
                           new SqlParameter("@WHTAmount", treasuryDealsModel.WHTAmount),
                           new SqlParameter("@NetInterestAmount", treasuryDealsModel.NetInterestAmount),
                           new SqlParameter("@PaymentAmount", treasuryDealsModel.PaymentAmount),
                           new SqlParameter("@InflowAccount", treasuryDealsModel.InflowAccount),
                           new SqlParameter("@PaymentAccount", treasuryDealsModel.PaymentAccount),
                           new SqlParameter("@AccountOfficer", treasuryDealsModel.AccountOfficer),

                           new SqlParameter("@TerminationInstructionCode", treasuryDealsModel.TerminationInstructionCode),
                           new SqlParameter("@Remarks", treasuryDealsModel.Remarks),
                           new SqlParameter("@PrincipalAccount", treasuryDealsModel.PrincipalAccount),
                           new SqlParameter("@InterestAccount", treasuryDealsModel.InterestAccount),
                           new SqlParameter("@WHTAccount", treasuryDealsModel.WHTAccount),
                           new SqlParameter("@TransactionStatus", treasuryDealsModel.TransactionStatus),
                           new SqlParameter("@ParentDealId",treasuryDealsModel.ParentDealId),
                           new SqlParameter("@ProcessStatus",treasuryDealsModel.ProcessStatus),
                           new SqlParameter("@BranchCode",treasuryDealsModel.BranchCode),
                           new SqlParameter("@UserId",treasuryDealsModel.UserId)


                       };

                    var test = TParam;

                    result = db.Database.SqlQuery<int>("dbo.TreasuryDealsMaster_Create @DealId,@CBADealId,@DealersReference,@ProductCode,@CurrencyCode, @CustomerId,@CustomerName," +
                              "@PrincipalAmount,@ValueDate,@Tenure,@MaturityDate,@PaymentDate,@InterestAmount,@WHTAmount,@NetInterestAmount,@PaymentAmount," +
                              "@InflowAccount,@PaymentAccount,@AccountOfficer,@TerminationInstructionCode,@Remarks,@PrincipalAccount,@InterestAccount,@WHTAccount,@TransactionStatus," +
                              "@ParentDealId,@ProcessStatus,@BranchCode,@UserId", TParam)
                               .FirstOrDefault();
                    Utils.LogNO("Executed SP " + result);


                    if (treasuryDealsModel.TreasuryInterest.Count() > 0 && result > 0)
                    {
                        List<TreasuryInterest> transMasterDetails = new List<TreasuryInterest>();
                        foreach (var transDetails in treasuryDealsModel.TreasuryInterest)
                        {
                            transMasterDetails.Add(new TreasuryInterest
                            {
                                DealId = result,

                                StartDate = transDetails.StartDate,

                                EndDate = transDetails.EndDate,
                                NoOfDaysInYear = transDetails.NoOfDaysInYear,
                                InterestRate = transDetails.InterestRate,
                                InterestAmount = transDetails.InterestAmount

                            });
                        }
                        db.TreasuryInterest.AddRange(transMasterDetails);// that table doesnt have fk. the table needs to be re-imported.ok
                    }

                    transaction.Commit(); // if there is error here it should not save changes
                    db.SaveChanges();


                    Utils.LogNO("Executed transaction " + result);

                    return result;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return 0;

                }
            }
        }

        public object CreateTreasuryDealsTransaction(TreasuryDealsModel treasuryDealsModel)
        {
            Utils.LogNO("Executing....." + JsonConvert.SerializeObject(treasuryDealsModel));
            using (var context = new BranchConsoleEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        int result = 0; //oya
                                        //string transRef = GenerateTransactionReference();
                                        //Utils.LogNO("TransactionModel Request: " + JsonConvert.SerializeObject(transactionModel));

                        // string MachineName = GeneralService.DetermineCompName(transactionModel.MachineName);
                        //no transref. Should we add?  should we add colun for remark, has mandate, has memo?Answer. yes sir.ok
                        object maturityDate = "";
                        object paymentDate = "";
                        int parentDealId = 0;
                        if(string.IsNullOrEmpty(treasuryDealsModel.ParentDealId))
                        {
                            parentDealId = 0;
                        }
                        else
                        {
                            parentDealId = Convert.ToInt32(treasuryDealsModel.ParentDealId);
                        }
                        if (treasuryDealsModel.MaturityDate == null || treasuryDealsModel.PaymentDate == null)
                        {
                            maturityDate = "";
                            paymentDate = "";
                        }
                        else
                        {
                            maturityDate = treasuryDealsModel.MaturityDate;
                            paymentDate = treasuryDealsModel.PaymentDate;
                        }
                        var TParam = new SqlParameter[]
                          {
                           new SqlParameter("@DealId", treasuryDealsModel.DealId),
                           new SqlParameter("@CBADealId", treasuryDealsModel.CBADealId),
                           new SqlParameter("@DealersReference", treasuryDealsModel.DealersReference),
                           new SqlParameter("@ProductCode", treasuryDealsModel.ProductCode),
                           new SqlParameter("@CurrencyCode", treasuryDealsModel.CurrencyCode),
                           new SqlParameter("@CustomerId", treasuryDealsModel.CustomerId),
                            new SqlParameter("@CustomerName", treasuryDealsModel.CustomerName),
                           new SqlParameter("@PrincipalAmount", treasuryDealsModel.PrincipalAmount),
                           new SqlParameter("@ValueDate", treasuryDealsModel.ValueDate),
                           new SqlParameter("@Tenure", treasuryDealsModel.Tenure),
                           new SqlParameter("@MaturityDate", maturityDate),
                           new SqlParameter("@PaymentDate", paymentDate),

                           new SqlParameter("@InterestAmount", treasuryDealsModel.InterestAmount),
                           new SqlParameter("@WHTAmount", treasuryDealsModel.WHTAmount),
                           new SqlParameter("@NetInterestAmount", treasuryDealsModel.NetInterestAmount),
                           new SqlParameter("@PaymentAmount", treasuryDealsModel.PaymentAmount),
                           new SqlParameter("@InflowAccount", treasuryDealsModel.InflowAccount),
                           new SqlParameter("@PaymentAccount", treasuryDealsModel.PaymentAccount),
                           new SqlParameter("@AccountOfficer", treasuryDealsModel.AccountOfficer),

                           new SqlParameter("@TerminationInstructionCode", treasuryDealsModel.TerminationInstructionCode),
                           new SqlParameter("@Remarks", treasuryDealsModel.Remarks),
                           new SqlParameter("@PrincipalAccount", treasuryDealsModel.PrincipalAccount),
                           new SqlParameter("@InterestAccount", treasuryDealsModel.InterestAccount),
                           new SqlParameter("@WHTAccount", treasuryDealsModel.WHTAccount),
                           new SqlParameter("@TransactionStatus", treasuryDealsModel.TransactionStatus),
                           new SqlParameter("@ParentDealId",parentDealId),
                           new SqlParameter("@ProcessStatus",treasuryDealsModel.ProcessStatus),
                           new SqlParameter("@BranchCode",treasuryDealsModel.BranchCode),
                           new SqlParameter("@UserId",treasuryDealsModel.UserId)


                          };

                        var test = TParam;

                        result = context.Database.SqlQuery<int>("dbo.TreasuryDealsMaster_Create @DealId,@CBADealId,@DealersReference,@ProductCode,@CurrencyCode, @CustomerId,@CustomerName," +
                                  "@PrincipalAmount,@ValueDate,@Tenure,@MaturityDate,@PaymentDate,@InterestAmount,@WHTAmount,@NetInterestAmount,@PaymentAmount," +
                                  "@InflowAccount,@PaymentAccount,@AccountOfficer,@TerminationInstructionCode,@Remarks,@PrincipalAccount,@InterestAccount,@WHTAccount,@TransactionStatus," +
                                  "@ParentDealId,@ProcessStatus,@BranchCode,@UserId", TParam)
                                   .FirstOrDefault();
                        Utils.LogNO("Executed SP " + result);


                        if (treasuryDealsModel.TreasuryInterest.Count() > 0 && result > 0)
                        {
                            List<TreasuryInterest> transMasterDetails = new List<TreasuryInterest>();
                            foreach (var transDetails in treasuryDealsModel.TreasuryInterest)
                            {
                                transMasterDetails.Add(new TreasuryInterest
                                {
                                    DealId = result,

                                    StartDate = transDetails.StartDate,

                                    EndDate = transDetails.EndDate,
                                    NoOfDaysInYear = transDetails.NoOfDaysInYear,
                                    InterestRate = transDetails.InterestRate,
                                    InterestAmount = transDetails.InterestAmount

                                });
                            }
                            context.TreasuryInterest.AddRange(transMasterDetails);// that table doesnt have fk. the table needs to be re-imported.ok
                        }

                        UserTransactionPageAccess newRequest = new UserTransactionPageAccess();
                        newRequest.UserId = treasuryDealsModel.UserId;
                        newRequest.TranId = result;
                        newRequest.DateAccessed = DateTime.Now;
                        newRequest.IsActiveOnPage = true;
                        context.UserTransactionPageAccess.Add(newRequest);


                        transaction.Commit(); // if there is error here it should not save changes
                        context.SaveChanges();


                        Utils.LogNO("Executed transaction " + result);

                        return new
                        {
                            success = true,
                            message = "Treasury Deal(s) saved successfully",
                            TransactionRef = result
                        };
                    }
                    catch (SqlException ex)
                    {
                        transaction.Rollback();
                        Utils.LogNO("CreateTreasuryDealsTransaction Exception" + ex);
                        return new
                        {
                            success = false,
                            message = ex.Message,
                            innerError = ex
                        };

                    }
                }
            }
            
        }


        public int CreateTreasuryDealsTermination(TreasuryDealsModel treasuryDealsModel) 
        {
            Utils.LogNO("Executing....." + JsonConvert.SerializeObject(treasuryDealsModel));
            using (var context = new BranchConsoleEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        int result = 0; //oya
                                        //string transRef = GenerateTransactionReference();
                                        //Utils.LogNO("TransactionModel Request: " + JsonConvert.SerializeObject(transactionModel));

                        // string MachineName = GeneralService.DetermineCompName(transactionModel.MachineName);
                        //no transref. Should we add?  should we add colun for remark, has mandate, has memo?Answer. yes sir.ok
                        object maturityDate = "";
                        object paymentDate = "";
                        if (treasuryDealsModel.MaturityDate == null || treasuryDealsModel.PaymentDate == null)
                        {
                            maturityDate = "";
                            paymentDate = "";
                        }
                        else
                        {
                            maturityDate = treasuryDealsModel.MaturityDate;
                            paymentDate = treasuryDealsModel.PaymentDate;
                        }
                        var TParam = new SqlParameter[]
                          {
                           new SqlParameter("@DealId", treasuryDealsModel.DealId),
                           new SqlParameter("@CBADealId", treasuryDealsModel.CBADealId),
                           new SqlParameter("@DealersReference", treasuryDealsModel.DealersReference),
                           new SqlParameter("@ProductCode", treasuryDealsModel.ProductCode),
                           new SqlParameter("@CurrencyCode", treasuryDealsModel.CurrencyCode),
                           new SqlParameter("@CustomerId", treasuryDealsModel.CustomerId),
                            new SqlParameter("@CustomerName", treasuryDealsModel.CustomerName),
                           new SqlParameter("@PrincipalAmount", treasuryDealsModel.PrincipalAmount),
                           new SqlParameter("@ValueDate", treasuryDealsModel.ValueDate),
                           new SqlParameter("@Tenure", treasuryDealsModel.Tenure),
                           new SqlParameter("@MaturityDate", maturityDate),
                           new SqlParameter("@PaymentDate", paymentDate),

                           new SqlParameter("@InterestAmount", treasuryDealsModel.InterestAmount),
                           new SqlParameter("@WHTAmount", treasuryDealsModel.WHTAmount),
                           new SqlParameter("@NetInterestAmount", treasuryDealsModel.NetInterestAmount),
                           new SqlParameter("@PaymentAmount", treasuryDealsModel.PaymentAmount),
                           new SqlParameter("@InflowAccount", treasuryDealsModel.InflowAccount),
                           new SqlParameter("@PaymentAccount", treasuryDealsModel.PaymentAccount),
                           new SqlParameter("@AccountOfficer", treasuryDealsModel.AccountOfficer),

                           new SqlParameter("@TerminationInstructionCode", treasuryDealsModel.TerminationInstructionCode),
                           new SqlParameter("@Remarks", treasuryDealsModel.Remarks),
                           new SqlParameter("@PrincipalAccount", treasuryDealsModel.PrincipalAccount),
                           new SqlParameter("@InterestAccount", treasuryDealsModel.InterestAccount),
                           new SqlParameter("@WHTAccount", treasuryDealsModel.WHTAccount),
                           new SqlParameter("@TransactionStatus", treasuryDealsModel.TransactionStatus),
                           new SqlParameter("@ParentDealId",Convert.ToInt32(treasuryDealsModel.ParentDealId)),
                           new SqlParameter("@ProcessStatus",treasuryDealsModel.ProcessStatus),
                           new SqlParameter("@BranchCode",treasuryDealsModel.BranchCode),
                           new SqlParameter("@UserId",treasuryDealsModel.UserId)


                          };

                        var test = TParam;

                        result = context.Database.SqlQuery<int>("dbo.TreasuryDealsMaster_Create @DealId,@CBADealId,@DealersReference,@ProductCode,@CurrencyCode, @CustomerId,@CustomerName," +
                                  "@PrincipalAmount,@ValueDate,@Tenure,@MaturityDate,@PaymentDate,@InterestAmount,@WHTAmount,@NetInterestAmount,@PaymentAmount," +
                                  "@InflowAccount,@PaymentAccount,@AccountOfficer,@TerminationInstructionCode,@Remarks,@PrincipalAccount,@InterestAccount,@WHTAccount,@TransactionStatus," +
                                  "@ParentDealId,@ProcessStatus,@BranchCode,@UserId", TParam)
                                   .FirstOrDefault();
                        Utils.LogNO("Executed SP " + result);


                        if (treasuryDealsModel.TreasuryInterest.Count() > 0 && result > 0)
                        {
                            List<TreasuryInterest> transMasterDetails = new List<TreasuryInterest>();
                            foreach (var transDetails in treasuryDealsModel.TreasuryInterest)
                            {
                                transMasterDetails.Add(new TreasuryInterest
                                {
                                    DealId = result,

                                    StartDate = transDetails.StartDate,

                                    EndDate = transDetails.EndDate,
                                    NoOfDaysInYear = transDetails.NoOfDaysInYear,
                                    InterestRate = transDetails.InterestRate,
                                    InterestAmount = transDetails.InterestAmount

                                });
                            }
                            context.TreasuryInterest.AddRange(transMasterDetails);// that table doesnt have fk. the table needs to be re-imported.ok
                        }


                        transaction.Commit(); // if there is error here it should not save changes
                        context.SaveChanges();


                        Utils.LogNO("Executed transaction " + result);

                        return result;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Utils.LogNO("CreateTreasuryDealsTransaction Exception" + ex);
                        return 0;

                    }
                }
            }

        }

        public TreasuryDealsMaster CreateTreasuryDeals(TreasuryDealsModel treasuryDealsModel)
        {
            throw new NotImplementedException();
        }
    }
}
