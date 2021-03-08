using iTellerBranch.Model;
using iTellerBranch.Model.ViewModel;
using iTellerBranch.Repository;
using iTellerBranch.Repository.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Service.Business
{
    public class TransactionServiceBusiness
    {
        private readonly TransactionService _transactionService;
        private readonly CBABusiness _cBABusiness;
        private int Id;
        public TransactionServiceBusiness() 
        {
            _transactionService = new TransactionService();
            _cBABusiness = new CBABusiness();
        }
        public  IQueryable<TreasuryDealsMaster> GetMaturedDeals()
        {
            IQueryable<TreasuryDealsMaster> result =  _transactionService.GetMaturedDeals();
            return result;
        }

        public IQueryable<TreasuryDealsMaster> GetTreasuryDealsWithoutSuccessfulDoubleEntries()
        {
            IQueryable<TreasuryDealsMaster> result = _transactionService.GetTreasuryDealsWithoutSuccessfulDoubleEntries();
            return result;
        }

        public void CreateDoubleEntriesForUnsuccesfulDoubleEntries(TreasuryDealsMaster treasuryDealsMaster)
        {
            TreasuryDealsModel treasuryDealsModel = ConvertToViewModel(treasuryDealsMaster);
            string oldCbaDealID = _transactionService.GetCBaDealID(treasuryDealsMaster.ParentDealId);
            _cBABusiness.CreateDoubleEntriesForUnsuccessfulDeals(treasuryDealsModel, oldCbaDealID);
        }

        public void UpdateTreasuryDeal(TreasuryDealsMaster treasuryDealsMaster)
        {
            try
            {
                var id = treasuryDealsMaster.Id;
                TreasuryDealsModel treasuryDealsModel = ConvertToViewModel(treasuryDealsMaster);
                treasuryDealsModel.ParentDealId = Convert.ToString(treasuryDealsMaster.Id);
                treasuryDealsModel = ComputeNewTransactionDeal(treasuryDealsModel, id);
               
            }
            catch (Exception ex)
            {
                Utils.LogNO(string.Format("Update Treasury Deal Exception Message: {0}", ex));
            }
           
        }

        public TreasuryDealsModel ComputeNewTransactionDeal(TreasuryDealsModel treasuryDealsModel, int id) 
        {
            try
            {
                treasuryDealsModel.ProcessStatus = Convert.ToInt16(TransType.ProcessStatus.Approved);
                treasuryDealsModel.TransactionStatus = "A";
                treasuryDealsModel.ValueDate = treasuryDealsModel.PaymentDate;
                treasuryDealsModel.MaturityDate = treasuryDealsModel.ValueDate.Value
                                                        .AddDays(Convert.ToDouble(treasuryDealsModel.Tenure) - 1);
                treasuryDealsModel.PaymentDate = treasuryDealsModel.MaturityDate.Value.AddDays(1);
                var treasuryProductCode = _transactionService.GetTreasuryProductCode(treasuryDealsModel.ProductCode);
                if (treasuryDealsModel.TerminationInstructionCode ==
                  Convert.ToInt16(TransType.TerminationInstructionCode.RollOverPrincipalWithInterest))
                {

                    treasuryDealsModel.PrincipalAmount = treasuryDealsModel.PrincipalAmount + treasuryDealsModel.NetInterestAmount;
                    treasuryDealsModel.WHTAmount = treasuryProductCode.Rate / 100 * treasuryDealsModel.InterestAmount;
                    treasuryDealsModel.NetInterestAmount = treasuryDealsModel.InterestAmount - treasuryDealsModel.WHTAmount;
                    treasuryDealsModel.PaymentAmount = treasuryDealsModel.PrincipalAmount + treasuryDealsModel.NetInterestAmount;
                    treasuryDealsModel.TreasuryInterest = ComputeInterestRate(treasuryDealsModel);
                    Utils.LogNO("Computed deals: " + JsonConvert.SerializeObject(treasuryDealsModel));

                    //int Id = _transactionService.CreateTreasuryDealsTermination(treasuryDealsModel);
                    //_transactionService.UpdateTreasuryDealTransactionStatus(id);
                    //var newTreasuryDeal =  _transactionService.GetNewDeal(Id);

                    //CreateDoubleEntriesForRollOverPrincipalInterest(treasuryDealsModel, treasuryProductCode.WHTPayableAccount, newTreasuryDeal);

                    _cBABusiness.RollOver(treasuryDealsModel,treasuryProductCode, id);
                }
                else if (treasuryDealsModel.TerminationInstructionCode ==
                    Convert.ToInt16(TransType.TerminationInstructionCode.RollOverPrincipal))
                {
                    //treasuryDealsModel.PrincipalAmount = treasuryDealsModel.PrincipalAmount;
                    //treasuryDealsModel.WHTAmount = treasuryProductCode.Rate / 100 * treasuryDealsModel.InterestAmount;
                    //treasuryDealsModel.NetInterestAmount = treasuryDealsModel.InterestAmount - treasuryDealsModel.WHTAmount;
                    //treasuryDealsModel.PaymentAmount = treasuryDealsModel.PrincipalAmount + treasuryDealsModel.NetInterestAmount;
                    //treasuryDealsModel.TreasuryInterest = ComputeInterestRate(treasuryDealsModel);
                    //int Id = _transactionService.CreateTreasuryDealsTermination(treasuryDealsModel);
                    //_transactionService.UpdateTreasuryDealTransactionStatus(id);
                    //var newTreasuryDeal = _transactionService.GetNewDeal(Id);
                    //CreateDoubleEntriesForRollOverPrincipal(treasuryDealsModel, treasuryProductCode.WHTPayableAccount);
                    _cBABusiness.RollOver(treasuryDealsModel, treasuryProductCode, id);
                }
                else if (treasuryDealsModel.TerminationInstructionCode ==
                   Convert.ToInt16(TransType.TerminationInstructionCode.NoRollOver))
                {
                    treasuryDealsModel.PrincipalAmount = treasuryDealsModel.PrincipalAmount;
                    treasuryDealsModel.WHTAmount = treasuryProductCode.Rate / 100 * treasuryDealsModel.InterestAmount;
                    treasuryDealsModel.NetInterestAmount = treasuryDealsModel.InterestAmount - treasuryDealsModel.WHTAmount;
                    treasuryDealsModel.PaymentAmount = treasuryDealsModel.PrincipalAmount + treasuryDealsModel.NetInterestAmount;
                    treasuryDealsModel.TreasuryInterest = ComputeInterestRate(treasuryDealsModel);
                    treasuryDealsModel.TransactionStatus = "P";
                    //int Id = _transactionService.CreateTreasuryDealsTermination(treasuryDealsModel);
                    //_transactionService.UpdateTreasuryDealTransactionStatus(id);
                    //var newTreasuryDeal = _transactionService.GetNewDeal(Id);
                    //CreateDoubleEntriesForRollOverPrincipal(treasuryDealsModel, treasuryProductCode.WHTPayableAccount, newTreasuryDeal);
                    _cBABusiness.RollOver(treasuryDealsModel, treasuryProductCode, id);
                }
                return treasuryDealsModel;
            }
            catch (Exception ex)
            {
                Utils.LogNO("Exception Message for ComputeNewTransactionDeal: " + ex.Message);
                throw ex;
            }
           
        }

        public List<TransactionModel> CreateDoubleEntriesForRollOverPrincipalInterest(TreasuryDealsModel treasuryDealsModel, 
                                          string WHTPayableAccount, string oldDealId)  
        {
            try
            {
                List<TransactionModel> transactionModels = new List<TransactionModel>();
                string transType = "";

                // debit principal account with principal amount and credit settlement 
                treasuryDealsModel.Remarks = "Deal " + oldDealId + " Liquidated";
                transType = Convert.ToString(TransType.TransactionType.TreasuryTransfer);
                transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                    treasuryDealsModel.PrincipalAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount),
                    treasuryDealsModel.PaymentAccount));

                //debit interest account with net interest and credit settlement
                treasuryDealsModel.Remarks = "Net Interest accrued on deal" + oldDealId; 
                transType = Convert.ToString(TransType.TransactionType.TreasuryTransfer);
                transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                    treasuryDealsModel.InterestAccount, Convert.ToDecimal(treasuryDealsModel.NetInterestAmount),
                     treasuryDealsModel.PaymentAccount));

                //debit settlement  account with principal + net interest  and credit principal
                treasuryDealsModel.Remarks = "Deal " + oldDealId + ", rollover to new deal " 
                    + treasuryDealsModel.CBADealId;
                transType = Convert.ToString(TransType.TransactionType.TreasuryTransfer);
                transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                    treasuryDealsModel.PaymentAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount)
                                    + Convert.ToDecimal(treasuryDealsModel.NetInterestAmount),
                    treasuryDealsModel.PrincipalAccount));

                //debit wht account with wht amount and credit witholding tax payable
                treasuryDealsModel.Remarks = "Narration witholding tax of deal " + oldDealId;
                transType = Convert.ToString(TransType.TransactionType.TreasuryTransfer);
                transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                    treasuryDealsModel.WHTAccount, Convert.ToDecimal(treasuryDealsModel.WHTAmount),
                    WHTPayableAccount));
                Utils.LogNO("Double Entries: " + transactionModels.Count());
                return transactionModels;
                //foreach (var transaction in transactionModels)
                //{
                //    _transactionService.CreateTransactionDeals(transaction);

                //}
                //  _transactionService.CreateRangeTransaction(transactionModels);


            }
            catch (Exception ex)
            {
                Utils.LogNO("Exception Message for CreateDoubleEntriesForRollOverPrincipalInterest: " + ex);
                return null;
            }
           
        }

        public List<TransactionModel> CreateDoubleEntriesForRollOverPrincipal(TreasuryDealsModel treasuryDealsModel,
                                            string WHTPayableAccount, string oldDealId) 
        {
            try
            {
               // var CustomerAccountDetails = _transactionService.GetCustomerAccountDetailsByCustId(treasuryDealsModel.CustomerId);
                List<TransactionModel> transactionModels = new List<TransactionModel>();
                string transType = "";

                // debit principal account with principal amount / credit settlement account
                treasuryDealsModel.Remarks = "Deal " + oldDealId + " Liquidated";
                transType = Convert.ToString(TransType.TransactionType.TreasuryTransfer);
                transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                    treasuryDealsModel.PrincipalAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount),
                     treasuryDealsModel.PaymentAccount));

                // debit interest account with net interest / credit settlement account 
                treasuryDealsModel.Remarks = "Net Interest accrued on deal" + oldDealId;
                transType = Convert.ToString(TransType.TransactionType.TreasuryTransfer);
                transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                    treasuryDealsModel.InterestAccount, Convert.ToDecimal(treasuryDealsModel.NetInterestAmount),
                    treasuryDealsModel.PaymentAccount));

                //debit settlement account with princpal amt / credit principal account 
                treasuryDealsModel.Remarks = "Deal " + oldDealId + ", rollover to new deal " 
                    + treasuryDealsModel.CBADealId;
                transType = Convert.ToString(TransType.TransactionType.TreasuryTransfer);
                transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                    treasuryDealsModel.InterestAccount, Convert.ToDecimal(treasuryDealsModel.NetInterestAmount),
                    treasuryDealsModel.PrincipalAccount));

                //debit wht account with wht amount / credit wht tax payable 
                treasuryDealsModel.Remarks = "Narration witholding tax of deal " + oldDealId;
                transType = Convert.ToString(TransType.TransactionType.TreasuryTransfer);
                transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                    treasuryDealsModel.WHTAccount, Convert.ToDecimal(treasuryDealsModel.WHTAmount),
                    WHTPayableAccount));

                Utils.LogNO("Range transaction begins");
                return transactionModels;
                //foreach (var transactionModel in transactionModels)
                //{
                //    _transactionService.CreateTransactionDeals(transactionModel);
                //}
                //_transactionService.CreateRangeTransaction(transactionModels);
                //Utils.LogNO("Range transaction Ends");
                //foreach (var transaction in transactionModels)
                //{
                //    _transactionService.CreateTransactionDeals(transaction);

                //}
            }
            catch (Exception ex)
            {
                Utils.LogNO("Exception Message for CreateDoubleEntriesForRollOverPrincipal: " + ex);
                return null;
            }
           
        }

        public List<TransactionModel> CreateDoubleEntriesLiquidation(TreasuryDealsModel treasuryDealsModel, 
                                          string WHTPayableAccount, string oldDealId)
        {
            try
            {
                List<TransactionModel> transactionModels = new List<TransactionModel>();
                string transType = "";

                // debit principal account with principal amount / credit settlement account
                treasuryDealsModel.Remarks = "Deal " + oldDealId + " Liquidated";
                transType = Convert.ToString(TransType.TransactionType.TreasuryTransfer);
                transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                    treasuryDealsModel.PrincipalAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount),
                     treasuryDealsModel.PaymentAccount));

                // debit interest account with net interest / credit settlement account 
                treasuryDealsModel.Remarks = "Net Interest accrued on deal " + oldDealId;
                transType = Convert.ToString(TransType.TransactionType.TreasuryTransfer);
                transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                    treasuryDealsModel.InterestAccount, Convert.ToDecimal(treasuryDealsModel.NetInterestAmount),
                    treasuryDealsModel.PaymentAccount));

                //debit wht account with wht amount / credit wht tax payable 
                treasuryDealsModel.Remarks = "Narration witholding tax of deal " + oldDealId;
                transType = Convert.ToString(TransType.TransactionType.TreasuryTransfer);
                transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                    treasuryDealsModel.WHTAccount, Convert.ToDecimal(treasuryDealsModel.WHTAmount),
                    WHTPayableAccount));

                Utils.LogNO("Range transaction begins");
                return transactionModels;
                //foreach (var transactionModel in transactionModels)
                //{
                //    _transactionService.CreateTransactionDeals(transactionModel);
                //}
                //_transactionService.CreateRangeTransaction(transactionModels);
                //Utils.LogNO("Range transaction Ends");
                //foreach (var transaction in transactionModels)
                //{
                //    _transactionService.CreateTransactionDeals(transaction);

                //}
            }
            catch (Exception ex)
            {
                Utils.LogNO("Exception Message for CreateDoubleEntriesForRollOverPrincipal: " + ex);
                return null;
            }

        }


        public TransactionModel UpdateTransactionModel(TreasuryDealsModel treasuryDealsModel, string transType, 
                                            string debitAccountNumber, decimal amount, string creditAccountNumber)   
        {
            try
            {
              //  var DebitAccountDetails = _transactionService.GetCustomerAccountDetails(debitAccountNumber);
               // var CreditAccountDetails = _transactionService.GetCustomerAccountDetails(creditAccountNumber);
                TransactionModel transactionModel = new TransactionModel 
                {
                    TotalAmt = amount,
                    TellerId = debitAccountNumber,
                    ToTellerId = creditAccountNumber,
                    TransName = treasuryDealsModel.UserId,
                    AccountName ="",
                    TransacterEmail = "",
                    PhoneNo = "",
                    TransactionParty = "",
                    Status = 3,
                    AccountNo = debitAccountNumber,
                    SortCode = "",
                    CustomerAcctNos = creditAccountNumber,
                    Currency = treasuryDealsModel.CurrencyCode,
                    TransType = transType,
                    ValueDate = treasuryDealsModel.ValueDate,
                    Narration = treasuryDealsModel.Remarks,
                    MachineName = "",
                    TransRef = Guid.NewGuid().ToString(),
                    hasMemo = true,
                    hasMandate = true,
                    Remark = treasuryDealsModel.Remarks,
                    NeededApproval = false,
                    ApprovedBy = treasuryDealsModel.UserId,
                    Approved = true,
                    IsReversed = false,
                    DisapprovalReason = "",
                    DisapprovedBy = "",
                    WhenDisapproved = "",
                    WhenApproved = DateTime.Now,
                    BranchCode = treasuryDealsModel.BranchCode,
                    Posted = true,
                    CBAResponse = "",
                    CreationDate = DateTime.Now,
                    CBACode = "",
                    CBA = "T24"
                };

               //transactionModel.TransactionBeneficiary = new List<TransactionBeneficiaries>();
               // transactionModel.TransactionBeneficiary.Add(new TransactionBeneficiaries
               // {
               //     AccountNumber = CreditAccountDetails.AccountNo, //increase d identity to 1000 ok
               //     AccountName = CreditAccountDetails.AccountName,
               //     Amount = amount,
               //     Narration = treasuryDealsModel.Remarks,
               //     ChargeAmt = 0,
               //     TransRef = "",
               // });
                Utils.LogNO("transactionModel " + JsonConvert.SerializeObject(transactionModel));
                return transactionModel;
            }
            catch (Exception ex)
            {
                Utils.LogNO("Exception Message for UpdateTransactionModel: " + ex.Message);
                throw ex;
            }
        }
        public TreasuryDealsModel ConvertToViewModel(TreasuryDealsMaster treasuryDealsMaster)
        {
            try
            {
                TreasuryDealsModel treasuryDealsModel = new TreasuryDealsModel();
                treasuryDealsModel.DealId = treasuryDealsMaster.DealId;
                treasuryDealsModel.CBADealId = treasuryDealsMaster.CBADealId;
                treasuryDealsModel.DealersReference = treasuryDealsMaster.DealersReference;
                treasuryDealsModel.ProductCode = treasuryDealsMaster.ProductCode;
                treasuryDealsModel.CurrencyCode = treasuryDealsMaster.CurrencyCode;
                treasuryDealsModel.CustomerId = treasuryDealsMaster.CustomerId;
                treasuryDealsModel.CustomerName = treasuryDealsMaster.CustomerName;
                treasuryDealsModel.PrincipalAmount = treasuryDealsMaster.PrincipalAmount;
                treasuryDealsModel.ValueDate = treasuryDealsMaster.ValueDate;
                treasuryDealsModel.Tenure = treasuryDealsMaster.Tenure;
                treasuryDealsModel.MaturityDate = treasuryDealsMaster.MaturityDate;
                treasuryDealsModel.PaymentDate = treasuryDealsMaster.PaymentDate;
                treasuryDealsModel.InterestAmount = treasuryDealsMaster.InterestAmount;
                treasuryDealsModel.WHTAmount = treasuryDealsMaster.WHTAmount;
                treasuryDealsModel.NetInterestAmount = treasuryDealsMaster.NetInterestAmount;
                treasuryDealsModel.PaymentAmount = treasuryDealsMaster.PaymentAmount;
                treasuryDealsModel.InflowAccount = treasuryDealsMaster.InflowAccount;
                treasuryDealsModel.PaymentAccount = treasuryDealsMaster.PaymentAccount;
                treasuryDealsModel.AccountOfficer = treasuryDealsMaster.AccountOfficer;
                treasuryDealsModel.TerminationInstructionCode = treasuryDealsMaster.TerminationInstructionCode;
                treasuryDealsModel.Remarks = treasuryDealsMaster.Remarks;
                treasuryDealsModel.PrincipalAccount = treasuryDealsMaster.PrincipalAccount;
                treasuryDealsModel.InterestAccount = treasuryDealsMaster.InterestAccount;
                treasuryDealsModel.WHTAccount = treasuryDealsMaster.WHTAccount;
                treasuryDealsModel.TransactionStatus = treasuryDealsMaster.TransactionStatus;
                treasuryDealsModel.ParentDealId = treasuryDealsMaster.DealId;
                treasuryDealsModel.ProcessStatus = treasuryDealsMaster.ProcessStatus;
                treasuryDealsModel.BranchCode = treasuryDealsMaster.BranchCode;
                treasuryDealsModel.UserId = treasuryDealsMaster.UserId;
                treasuryDealsModel.TreasuryInterest = new List<TreasuryInterestModel>();
                foreach(var treasuryInterest in treasuryDealsMaster.TreasuryInterest)
                {
                    treasuryDealsModel.TreasuryInterest.Add(new TreasuryInterestModel
                    {
                        InterestAmount = treasuryInterest.InterestAmount,
                        InterestRate = treasuryInterest.InterestRate,
                        StartDate = treasuryInterest.StartDate,
                        EndDate = treasuryInterest.EndDate,
                        NoOfDaysInYear =  treasuryInterest.NoOfDaysInYear,
                    });
                }
                return treasuryDealsModel;
            }
            catch (Exception ex)
            {
                Utils.LogNO("Exception Message for ConvertToViewModel: " + ex.Message);
                throw ex;
            }
           
        }


        public List<TreasuryInterestModel> ComputeInterestRate(TreasuryDealsModel treasuryDealsMaster)
        {
            try
            {
                
                List<TreasuryInterestModel> interestRates = new List<TreasuryInterestModel>();
                int count = 0;
                DateTime endDt = DateTime.Now;
              
                foreach (var interestRate in treasuryDealsMaster.TreasuryInterest)
                {
                    int dayDifference = Convert.ToDateTime(interestRate.EndDate).Subtract(Convert.ToDateTime(interestRate.StartDate)).Days;
                    Utils.LogNO("dayDifference: " + dayDifference);
                    var noOfDays = count == 0 ? interestRate.NoOfDaysInYear : interestRates[count - 1].NoOfDaysInYear;

                    var interestAmount = (treasuryDealsMaster.PrincipalAmount * interestRate.InterestRate * dayDifference)
                                          / (100 * noOfDays);
                    Utils.LogNO("dayDifference: " + interestRate.InterestRate);
                    Utils.LogNO("dayDifference: " + noOfDays);
                    Utils.LogNO("dayDifference: " + treasuryDealsMaster.PrincipalAmount);
                    interestRates.Add(new TreasuryInterestModel
                    {
                        InterestRate = interestRate.InterestRate,
                        StartDate = count == 0 ? treasuryDealsMaster.ValueDate : interestRates[count - 1].EndDate.Value.AddDays(1),
                        EndDate = count == 0 ? treasuryDealsMaster.ValueDate.Value.AddDays(dayDifference) 
                                             : interestRates[count].EndDate.Value.AddDays(dayDifference),
                        NoOfDaysInYear = count == 0 ? DateTime.IsLeapYear(treasuryDealsMaster.ValueDate.Value.Year) == true ? 366 : 365 
                                                    : DateTime.IsLeapYear(interestRates[count - 1].EndDate.Value.AddDays(1).Year) == true ? 366 : 365,
                                                    
                        InterestAmount = interestAmount
                    });
                  
                    count = count + 1;
                }
                Utils.LogNO(JsonConvert.SerializeObject(treasuryDealsMaster));
                
                return interestRates;
            }
            catch (Exception ex)
            {
                Utils.LogNO("Exception Message for ComputeInterestRate: " + ex.Message);
                throw ex;
            }
          
        }


    }
}
