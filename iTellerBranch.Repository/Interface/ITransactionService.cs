using iTellerBranch.Model;
using iTellerBranch.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Repository.Interface
{
    public interface ITransactionService
    {

        int CreateTransactionDeals(TransactionModel transactionModel);
        object GetWithdrawalTrans(bool success, string message, Exception ex = null);
        object GetWithDepositTrans(bool success, string message, Exception ex = null);
        object CreateTransactionDepositWithdrawal(TransactionModel transactionModel,int statusId);
        object CreateTransactionWithdrawal(TransactionModel transactionModel);
      //  object CreateTransactionDeposit(TransMaster transMaster);
        object GetCustomerDetails(string AccountNumber);
        void ReverseTransaction(TellerReversal tellerReversal);
        TreasuryDealsMaster UpdateTreasuryDealTransactionStatus(int? Id);
        TransactionsMaster GetAllTransById(int TransId);
        object ApproveTransaction(TransactionApprovalModel transaction, string transref=null);
        object DisapproveTransaction(TransactionDisApprovalModel transaction);
        int CreateTransaction(TransactionModel transactionModel, int statusId);

        int RemoveAlreadyCreatedTransaction(long TransId);
        ChequeLodgement GetChequeLodgement(long? tranId);
        TransactionFiles GetTransactionFile(TransactionFiles files);
        object ApproveTeasuryDeal(TreasuryDealsModel transaction, string dealID = null);
        object DisapproveTeasuryDeal(TreasuryDealsModel transaction);
        List<TransferDetails> RetrieveBeneficiaries(long tranId);
        int UpdateBeneficiaryForPostStatus(TransferDetails detail);
        int UpdateBeneficiaryForPostStatusHeader(TransferHeader detail);
        bool DeleteTreasuryDeal(int? id);
        bool ValidateAccountNumber(string AccountNumber);
        BranchAccounts FetchGLAccounts(string branchCode);
        int UpdateStatusForMasterTrans(TransactionModel transMaster);
        TransferHeader RetrieveTransferHeader(long tranId);
        object GetAllTreasuryTrans(bool success, string message, Exception ex = null, string transRef = null);
        TreasuryDealsMaster GetAllTreasuryTransById(int TransId);
        int CreateTreasuryDealsTrans(TreasuryDealsModel treasuryDealsModel);
        TreasuryDealsMaster CreateTreasuryDeals(TreasuryDealsModel treasuryDealsModel);

        object CreateTreasuryDealsTransaction(TreasuryDealsModel treasuryDealsModel);

        object GetTreasuryProductDetails(bool success, string message, Exception ex = null);
        object GetTerminationInstruction(bool success, string message, Exception ex = null);
    }
}
