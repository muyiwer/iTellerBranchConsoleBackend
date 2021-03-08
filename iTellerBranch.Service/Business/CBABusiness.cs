using iTellerBranch.BankService;
using iTellerBranch.Model;
using iTellerBranch.Model.ViewModel;
using iTellerBranch.Repository;
using iTellerBranch.Repository.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iTellerBranch.Model.ViewModel.TreasuryRequestModel;

namespace iTellerBranch.Service.Business
{
    public class CBABusiness
    {
        private readonly TransactionServiceBusiness _transactionServiceBusiness;
        private readonly TransactionService _transactionService;
        public CBABusiness()
        {
            _transactionServiceBusiness = new TransactionServiceBusiness();
            _transactionService = new TransactionService();
        }
        public void RollOver(TreasuryDealsModel transMaster, TreasuryProductCode treasuryProductCode, int oldDealID)
        {

            string productId = _transactionService.GetTerminationCode(Convert.ToInt16(transMaster.TerminationInstructionCode));
            CollaterizedDepositModel collaterizedDepositModel = new CollaterizedDepositModel();
            collaterizedDepositModel.Request = new CollaterizedDepositDetail();
            collaterizedDepositModel.Request.AcctName = transMaster.CustomerName;
            collaterizedDepositModel.Request.Amt = Math.Round(Convert.ToDouble(transMaster.PaymentAmount), 2) + "";
            collaterizedDepositModel.Request.CompCode = transMaster.BranchCode;
            collaterizedDepositModel.Request.Currency = transMaster.CurrencyAbbrev;
            collaterizedDepositModel.Request.CustId = transMaster.CustomerId;
            collaterizedDepositModel.Request.dao = Config.GetConfigValue("dao");
            collaterizedDepositModel.Request.EffectiveDate = transMaster.MaturityDate.Value.Year
                                                            + transMaster.MaturityDate.Value.Month
                                                            + transMaster.MaturityDate.Value.Day + "";
            collaterizedDepositModel.Request.PayinAcct = transMaster.PaymentAccount;
            collaterizedDepositModel.Request.PayoutAcct = transMaster.PrincipalAccount;
            collaterizedDepositModel.Request.ProductId = productId;
            collaterizedDepositModel.Request.Rate = transMaster.TreasuryInterest.FirstOrDefault().InterestRate + "";
            if (transMaster.TerminationInstructionCode ==
                 Convert.ToInt16(TransType.TerminationInstructionCode.NoRollOver))
            {
                collaterizedDepositModel.Request.Term = transMaster.Tenure + "D";
            }
            else
            {
                collaterizedDepositModel.Request.ChangePeriod = transMaster.Tenure + "D"; 
            }
               
            string url = Config.GetConfigValue("BookAADeposit");
            TreasuryResponse treasury = TreasuryCbaService.CollaterizedDeposit
                                                            (url, collaterizedDepositModel, transMaster.access_token);
            if (treasury.response.RespondCode == "1")
            {
                transMaster.CBADealId = treasury.response.ArrangementId;
                transMaster.DealId = treasury.response.ArrangementId;
                transMaster.CBA = "T24";
                transMaster.DealersReference = treasury.response.ResponseId;
                _transactionService.CreateTreasuryDealsTermination(transMaster);
                var oldTreasuryDeal = _transactionService.UpdateTreasuryDealTransactionStatus(oldDealID);

                //List<TransactionModel> entries = new List<TransactionModel>();
                //if (transMaster.TerminationInstructionCode ==
                //  Convert.ToInt16(TransType.TerminationInstructionCode.RollOverPrincipalWithInterest))
                //{
                //    entries = _transactionServiceBusiness.CreateDoubleEntriesForRollOverPrincipalInterest
                //                         (transMaster, treasuryProductCode.WHTPayableAccount, oldTreasuryDeal.CBADealId);
                //}
                //else if (transMaster.TerminationInstructionCode ==
                //    Convert.ToInt16(TransType.TerminationInstructionCode.RollOverPrincipal))
                //{
                //    entries = _transactionServiceBusiness.CreateDoubleEntriesForRollOverPrincipal
                //                          (transMaster, treasuryProductCode.WHTPayableAccount, oldTreasuryDeal.CBADealId);
                //}
                //else 
                //{
                //    entries = _transactionServiceBusiness.CreateDoubleEntriesLiquidation
                //                         (transMaster, treasuryProductCode.WHTPayableAccount, oldTreasuryDeal.CBADealId);
                //}

                  
                //bool isSuccessful = DoubleEntriesTransfer.TransferEntries(entries);
                //if (isSuccessful)
                //{
                //    foreach (var entry in entries)
                //    {
                //        _transactionService.CreateTransactionDeals(entry);
                //    }
                //    transMaster.DoubleEntrySuccessful = true;
                //    _transactionService.ApproveTeasuryDeal(transMaster);
                //    _transactionService.CreateRangeTransaction(entries);
                //}
                //else
                //{
                //    transMaster.DoubleEntrySuccessful = false;
                //    _transactionService.ApproveTeasuryDeal(transMaster);
                //}
            }
            
        }

        public void CreateDoubleEntriesForUnsuccessfulDeals(TreasuryDealsModel transMaster, string oldCbaDealId)
        {
            var treasuryProductCode = _transactionService.GetTreasuryProductCode(transMaster.ProductCode);
            List<TransactionModel> entries = new List<TransactionModel>();
            if (transMaster.TerminationInstructionCode ==
              Convert.ToInt16(TransType.TerminationInstructionCode.RollOverPrincipalWithInterest))
            {
                entries = _transactionServiceBusiness.CreateDoubleEntriesForRollOverPrincipalInterest
                                     (transMaster, treasuryProductCode.WHTPayableAccount, oldCbaDealId);
            }
            else if (transMaster.TerminationInstructionCode ==
                Convert.ToInt16(TransType.TerminationInstructionCode.RollOverPrincipal))
            {
                entries = _transactionServiceBusiness.CreateDoubleEntriesForRollOverPrincipal
                                      (transMaster, treasuryProductCode.WHTPayableAccount, oldCbaDealId);
            }
            else
            {
                entries = _transactionServiceBusiness.CreateDoubleEntriesLiquidation
                                     (transMaster, treasuryProductCode.WHTPayableAccount, oldCbaDealId);
            }


            bool isSuccessful = DoubleEntriesTransfer.TransferEntries(entries);
            if (isSuccessful)
            {
                foreach (var entry in entries)
                {
                    _transactionService.CreateTransactionDeals(entry);
                }
                transMaster.DoubleEntrySuccessful = true;
                _transactionService.ApproveTeasuryDeal(transMaster);
                _transactionService.CreateRangeTransaction(entries);
            }
            else
            {
                transMaster.DoubleEntrySuccessful = false;
                _transactionService.ApproveTeasuryDeal(transMaster);
            }
        }

    }
}
