using iTellerBranch.Model;
using iTellerBranch.Model.ViewModel;
using iTellerBranch.Repository;
using iTellerBranch.Repository.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Business.Transaction
{
    
    public class TransactionBusiness
    {
        private readonly TransactionService _transactionService;
        
        public TransactionBusiness()
        {
            _transactionService = new TransactionService();
        }

        //public object GetWithdrawalTrans(bool success, string message, Exception ex = null)
        //{
        //    return _transactionService.GetWithdrawalTrans(true, "");
        //}
        public object GetWithdrawalTrans(bool success, string message, Exception ex = null)
        {
            return _transactionService.GetWithdrawalTrans(success, message, ex);
        }
        public object GetAllTrans(bool success, string message, Exception ex = null, string transRef = null)
        {
            return _transactionService.GetAllTrans(success, message, ex, transRef);
        }

        public object ApproveTeasuryDeal(TreasuryDealsModel transaction, string dealID = null)
        {
            return _transactionService.ApproveTeasuryDeal(transaction, dealID);
        }

        public bool DeleteTreasuryDeal(int? id)
        {
            return _transactionService.DeleteTreasuryDeal(id);
        }
        public object DisapproveTeasuryDeal(TreasuryDealsModel transaction)
        {
            return _transactionService.DisapproveTeasuryDeal(transaction);
        }

        public TransactionsMaster GetAllTransById(int TransId)
        {
            return  _transactionService.GetAllTransById(TransId);
        }
        
        public object GetAllTreasuryTrans(bool success, string message, Exception ex = null, string transRef = null)
        {
            return _transactionService.GetAllTreasuryTrans(success, message, ex, transRef);
        }
        public TreasuryDealsMaster GetAllTreasuryTransById(int TransId)
        {
            return _transactionService.GetAllTreasuryTransById(TransId);
        }

        public int CreateTreasuryDealsTrans(TreasuryDealsModel treasuryDealsModel)
        {
            return _transactionService.CreateTreasuryDealsTrans(treasuryDealsModel);
        }

        public TreasuryDealsMaster CreateTreasuryDeals(TreasuryDealsModel treasuryDealsModels)
        {
            return _transactionService.CreateTreasuryDeals(treasuryDealsModels);
        }

        public object CreateTreasuryDealsTransaction(TreasuryDealsModel treasuryDealsModel)
        {
            return _transactionService.CreateTreasuryDealsTransaction(treasuryDealsModel);
        }

        public int RemoveAlreadyCreatedTransaction(long TransId)
        {
            return _transactionService.RemoveAlreadyCreatedTransaction(TransId);
        }
        
        public TransferHeader RetrieveTransferHeader(long TransId)
        {
            return _transactionService.RetrieveTransferHeader(TransId);
        }

        public TransactionFiles GetTransactionFile(TransactionFiles files)
        {
            return _transactionService.GetTransactionFile(files);
        }

        public object ApproveTransaction(TransactionApprovalModel transaction, string transref = null)
        {
            return _transactionService.ApproveTransaction(transaction, transref);
        }

        public object ApproveImalTransaction(TransactionApprovalModel transaction, string transref = null)
        {
            return _transactionService.ApproveImalTransaction(transaction, transref);
        }

        public object DisapproveTransaction(TransactionDisApprovalModel transaction)
        {
            return _transactionService.DisapproveTransaction(transaction);
        }

        public object GetAccountMandate(string AccountNumber, bool success, string message, Exception ex = null)
        {
            return _transactionService.GetAccountMandate(AccountNumber, success, message, ex = null);
        }
        public object CreateTransactionWithdrawal(TransactionModel transaction)
        {
            return _transactionService.CreateTransactionWithdrawal(transaction);
        }

        //public object CreateTransactionDeposit(TransMaster transaction)
        //{
        //    return _transactionService.CreateTransactionDeposit(transaction);
        //}
        public object CreateTransactionDepositWithdrawal(TransactionModel transaction, int statusId)
        {
            return _transactionService.CreateTransactionDepositWithdrawal(transaction, statusId);
        }

        public ChequeLodgement GetChequeLodgement(long? tranId)
        {
            return _transactionService.GetChequeLodgement(tranId);
        }

        public int CreateTransaction(TransactionModel transaction, int statusId) 
        {
            return _transactionService.CreateTransaction(transaction, statusId);
        }

        public int CreateTransactionForImal(TransactionModel transaction)
        {
            return _transactionService.CreateTransactionForImal(transaction);
        }

        public int CreateTransactionDeals(TransactionModel transactionModel)
        {
            return _transactionService.CreateTransactionDeals(transactionModel);
        }

        public List<TransferDetails> RetrieveBeneficiaries(long TranId)
        {
            return _transactionService.RetrieveBeneficiaries(TranId);
        }
        
        public int UpdateBeneficiaryForPostStatus(TransferDetails detail)
        {
            return _transactionService.UpdateBeneficiaryForPostStatus(detail);
        }

        public int UpdateBeneficiaryForPostStatusHeader(TransferHeader detail)
        {
            return _transactionService.UpdateBeneficiaryForPostStatusHeader(detail);
        }

        public int UpdateStatusForMasterTrans(TransactionModel transMaster)
        {
            return _transactionService.UpdateStatusForMasterTrans(transMaster);
        }
        public object GetTransactionDeposit(bool success, string message, Exception ex = null)
        {
            return _transactionService.GetDepositTrans(success, message, ex);
        }

        public object GetCustomerDetails(string AccountNumber)
        {
            return _transactionService.GetCustomerDetails(AccountNumber);
        }

        public string GetCurrencyAbbrev(int currency)
        {
            return _transactionService.GetCurAbbrev(currency);
        }

        public BranchAccounts FetchGLAccounts(string BranchCode)
        {
            return _transactionService.FetchGLAccounts(BranchCode);
        }
        public string BuildNarration(string SerialNo, string Beneficiary, string transRef, string remarks, string depositor, int status, string transType="") //ok lets factor what MD said as per narration here
        {
            Utils.LogNO("Building Narration inside Transaction Biz. TransRef:" + transRef + ", serialNo:" + SerialNo + ", status:" + status + ", transtype:" + transType);
            string narration = string.Empty;
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

            if ((status == 3 && transType == "13") |(status==3 && transType=="3")|| transType=="13"|| transType == "3")
            {
               narration =  @" " + transRef + @" CASH WTD CHQ B/O" + @" " + depositor + @" " + remarks;
            }

            return narration;
        }

        public void ReverseTransaction(TellerReversal tellerReversal)
        {
              _transactionService.ReverseTransaction(tellerReversal);
        }

        public bool ValidateAccountNumber(string AccountNumber)
        {
            return _transactionService.ValidateAccountNumber(AccountNumber);
        }
        public FundTransferModel ConvertToFundTransferModel(TransactionModel transMaster)
        {
            FundTransferModel fundTransferModel = new FundTransferModel();
            fundTransferModel.FT_Request = new FTRequest();
            fundTransferModel.FT_Request.TransactionBranch = transMaster.Branch;//ook
            fundTransferModel.FT_Request.TransactionType = transMaster.TransType; //"AVCE";
            fundTransferModel.FT_Request.DebitAcctNo = transMaster.NIBCashSettlement;
            fundTransferModel.FT_Request.CreditAccountNo = transMaster.VaultAccount;
            fundTransferModel.FT_Request.DebitCurrency = transMaster.CurrencyAbbrev;
            fundTransferModel.FT_Request.CreditCurrency = transMaster.CurrencyAbbrev;
            fundTransferModel.FT_Request.DebitAmount = "" + transMaster.Amount;
            fundTransferModel.FT_Request.CommissionCode = "";
            fundTransferModel.FT_Request.narrations = transMaster.Remark;
            fundTransferModel.FT_Request.SessionId = transMaster.access_token;
            fundTransferModel.FT_Request.TrxnLocation = "1";
            return fundTransferModel;
        } 
        public FundTransferModel ConvertToFundTransferModelForVaultIn(TransactionModel transMaster)
        {
            FundTransferModel fundTransferModel = new FundTransferModel();
            fundTransferModel.FT_Request = new FTRequest();
            fundTransferModel.FT_Request.TransactionBranch = transMaster.Branch;//ook
            fundTransferModel.FT_Request.TransactionType = transMaster.TransType;//"AVCE";
            fundTransferModel.FT_Request.DebitAcctNo = transMaster.VaultAccount;
            fundTransferModel.FT_Request.CreditAccountNo = transMaster.NIBCashSettlement;
            fundTransferModel.FT_Request.DebitCurrency = transMaster.CurrencyAbbrev;
            fundTransferModel.FT_Request.CreditCurrency = transMaster.CurrencyAbbrev;
            fundTransferModel.FT_Request.DebitAmount = "" + transMaster.Amount;
            fundTransferModel.FT_Request.CommissionCode = "";
            fundTransferModel.FT_Request.narrations = transMaster.Remark;
            fundTransferModel.FT_Request.SessionId = transMaster.access_token;
            fundTransferModel.FT_Request.TrxnLocation = "1";
            return fundTransferModel;
        }
        
    }
}
