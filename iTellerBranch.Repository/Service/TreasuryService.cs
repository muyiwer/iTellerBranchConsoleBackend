using iTellerBranch.Model;
using iTellerBranch.Model.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Repository.Service
{
    public class TreasuryService : BaseService
    {
        private readonly TransactionService _transactionService;
        public TreasuryService()
        {
            _transactionService = new TransactionService(); 
        }

        public int GetDiscountDepositType(string dealId) 
        {
            var preLiquidatedDeal =  db.PreLiquidatedDeal.Where(x => x.DealID == dealId).FirstOrDefault();
            if(preLiquidatedDeal == null)
            {
                return 0;
            }else if(preLiquidatedDeal.IsPartialLiquidation == false)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        public IQueryable<DiscountedTreasuryProduct> GetDiscountedTreasuryProduct()
        {
            return db.DiscountedTreasuryProduct.AsQueryable();
        }

        public TreasuryProductCode GetDiscountTreasuryProduct() 
        {
            return db.TreasuryProductCode.Where(x=> x.Discounted == true).FirstOrDefault();
        }
        public object GetMatureDeals() 
        {
            DateTime dt = DateTime.Now;
            int processStatus = Convert.ToInt16(TransType.ProcessStatus.Approved);
            int terminationCode = Convert.ToInt16(TransType.TerminationInstructionCode.NoRollOver);
            int productCode = Convert.ToInt16(TransType.ProductCode.CallDeposit);
            var pCode = productCode.ToString();
            //return db.TreasuryDealsMaster.Where(x=> x.TerminationInstructionCode == terminationCode && x.ProcessStatus == processStatus
            //                         && x.TransactionStatus == "A"   && DbFunctions.TruncateTime(x.MaturityDate).Value <= dt).AsQueryable();
            return new
            {
                data = new
                {
                    TreasuryTransaction = db.TreasuryDealsMaster.Where(x => x.ProductCode != pCode && x.IsReversed != true &&
                                        x.TerminationInstructionCode == terminationCode && x.ProcessStatus == processStatus
                                     && x.TransactionStatus == "A" && DbFunctions.TruncateTime(x.MaturityDate).Value <= dt).Select(x => new
                    {
                        x.Id,
                        x.DealId,
                        x.CurrencyCode,
                        CurrencyName = db.CashDenomination.Where(y => y.ID == x.CurrencyCode).FirstOrDefault().Currency,
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

                    }).AsQueryable(),

                }

            };
        }


        public object GetDiscountedDeals() 
        {
            DateTime dt = DateTime.Now;
            int processStatus = Convert.ToInt16(TransType.ProcessStatus.Approved);
            int productCode = Convert.ToInt16(TransType.ProductCode.BankersAcceptances);
            var pCode = productCode.ToString();
            //return db.TreasuryDealsMaster.Where(x=> x.TerminationInstructionCode == terminationCode && x.ProcessStatus == processStatus
            //                         && x.TransactionStatus == "A"   && DbFunctions.TruncateTime(x.MaturityDate).Value <= dt).AsQueryable();
            return new
            {
                data = new
                {
                    TreasuryTransaction = db.TreasuryDealsMaster.Where(x => x.ProductCode == pCode  && x.ProcessStatus == processStatus
                                     && x.TransactionStatus == "A" && x.IsReversed != true ).Select(x => new
                                     {
                                         x.Id,
                                         x.DealId,
                                         x.CurrencyCode,
                                         CurrencyName = db.CashDenomination.Where(y => y.ID == x.CurrencyCode).FirstOrDefault().Currency,
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

                                     }).AsQueryable(),

                }

            };
        }

        public string GetCurrencyAbbrev(int? Id)
        {
            try
            {
                return db.CashDenomination.Where(x => x.ID == Id).FirstOrDefault().Abbrev;
            }
            catch (Exception ex)
            {

                Utils.LogNO("GetCurrencyAbbrev: " + Id + ex.Message);
                return "";
            }
            
        }

        public string GetproductCode(string productCode)
        {
            return db.TreasuryProductCode.Where(x => x.ProductCode == productCode).FirstOrDefault().CBAProductCode;
        }

        public bool ProductTypeIsTenured(string productCode)
        {
            try
            {
                return Convert.ToBoolean(db.TreasuryProductCode
                                .Where(x => x.ProductCode == productCode).FirstOrDefault().Tenured);
            }
            catch (Exception ex)
            {
                Utils.LogNO("ProductTypeIsTenured: " + productCode + ex.Message);
                return false;
            }
            
        }

        public void TerminateDeal(TreasuryDealsModel transMaster)
        {
            var deal = db.TreasuryDealsMaster.Where(x => x.DealId == transMaster.DealId).FirstOrDefault();
            deal.TransactionStatus = "D";
            deal.ApprovedBy = transMaster.ApprovedBy;
            deal.WhenApproved = transMaster.WhenApproved;
            deal.CreationDate = DateTime.Now;
            deal.ProcessStatus = 10;
            db.Entry(deal).State = EntityState.Modified;

            var preliquidate = db.PreLiquidatedDeal.Where(x => x.DealID == transMaster.DealId).FirstOrDefault();
            preliquidate.CBAReferenceID = transMaster.TransRef;
            db.Entry(deal).State = EntityState.Modified;

            db.SaveChanges();
        }

        public void ReverseDiscountedDeal(string dealId) 
        {
            var deal = db.TreasuryDealsMaster.Where(x => x.DealId == dealId).FirstOrDefault();
            deal.IsReversed = true;
            deal.TransactionStatus = "R";
            db.Entry(deal).State = EntityState.Modified;
            db.SaveChanges();
        }

        public bool PreLiquidate(TreasuryDealsModel treasuryDealsModel)
        {
            TreasuryDealsMaster treasuryDealsMaster =  db.TreasuryDealsMaster.Find(treasuryDealsModel.Id);
            if(treasuryDealsMaster != null)
            {
                treasuryDealsMaster.TransactionStatus = "D";
                treasuryDealsMaster.ProcessStatus = 1;
                db.Entry(treasuryDealsMaster).State = EntityState.Modified;
                // save to PreLiquidatedDeal table
                PreLiquidatedDeal preLiquidatedDeal = new PreLiquidatedDeal();
                preLiquidatedDeal.DealID = treasuryDealsModel.DealId;
                preLiquidatedDeal.LiquidationDate = treasuryDealsModel.LiquidatedDate;
                preLiquidatedDeal.PenaltyRate = treasuryDealsModel.PenaltyRate;
                preLiquidatedDeal.IsPartialLiquidation = false;
                preLiquidatedDeal.DateCreated = DateTime.Now;
                db.PreLiquidatedDeal.Add(preLiquidatedDeal);
                db.SaveChanges();
                return true;
                //  CreatDoubleEntries(treasuryDealsModel, treasuryProductCode);
            }
            else
            {
                return false;
            }

        }

        public bool PreLiquidatePartially(TreasuryDealsModel treasuryDealsModel) 
        {
            TreasuryDealsMaster treasuryDealsMaster = db.TreasuryDealsMaster.Find(treasuryDealsModel.Id);
            if (treasuryDealsMaster != null)
            {
                

                //save to treasury master table
                treasuryDealsModel.PrincipalAmount = treasuryDealsModel.PrincipalAmount - treasuryDealsModel.PreLiquidatedAmount;
                treasuryDealsModel.ParentDealId = Convert.ToString(treasuryDealsModel.Id);
                treasuryDealsModel.ProcessStatus = 1;
                int newId = treasuryDealsModel.Id + 1;
                treasuryDealsModel.DealId = treasuryDealsModel.CustomerId + "/" + newId;
                int Id = _transactionService.CreateTreasuryDealsTrans(treasuryDealsModel);

                // save to PreLiquidatedDeal table
                PreLiquidatedDeal preLiquidatedDeal = new PreLiquidatedDeal();
                preLiquidatedDeal.DealID = treasuryDealsModel.CustomerId + "/" + Id + 1;
                preLiquidatedDeal.LiquidationDate = treasuryDealsModel.LiquidatedDate;
                preLiquidatedDeal.PenaltyRate = treasuryDealsModel.PenaltyRate;
                preLiquidatedDeal.IsPartialLiquidation = true;
                preLiquidatedDeal.DateCreated = DateTime.Now;
                db.PreLiquidatedDeal.Add(preLiquidatedDeal);
                db.SaveChanges();
                return true;
                //  CreatDoubleEntries(treasuryDealsModel, treasuryProductCode);
            }
            else
            {
                return false;
            }

        }


        public bool LiquidateCallDeal(TreasuryDealsModel treasuryDealsModel) 
        {
            TreasuryDealsMaster treasuryDealsMaster = db.TreasuryDealsMaster.Find(treasuryDealsModel.Id);
            if (treasuryDealsMaster != null)
            {
                treasuryDealsMaster.TransactionStatus = "P";
                db.Entry(treasuryDealsMaster).State = System.Data.Entity.EntityState.Modified;
                // save to PreLiquidatedDeal table
                PreLiquidatedDeal preLiquidatedDeal = new PreLiquidatedDeal();
                preLiquidatedDeal.DealID = treasuryDealsModel.DealId;
                preLiquidatedDeal.LiquidationDate = treasuryDealsModel.LiquidatedDate;
                preLiquidatedDeal.PenaltyRate = treasuryDealsModel.PenaltyRate;
                preLiquidatedDeal.DateCreated = DateTime.Now;
                db.PreLiquidatedDeal.Add(preLiquidatedDeal);
                db.SaveChanges();
                return true;
                // CreatDoubleEntriesForCallDeposit(treasuryDealsModel);  
            }
            else
            {
                return false;
            }

        }

        public List<TransactionModel> CreatDoubleEntries(TreasuryDealsModel treasuryDealsModel)
        {
            TreasuryProductCode treasuryProductCode = db.TreasuryProductCode
                                          .Where(x => x.ProductCode == treasuryDealsModel.ProductCode)
                                                                .FirstOrDefault();
            List<TransactionModel> transactionModels = new List<TransactionModel>();
            string transType = "";

            // debit principal account with principal amount and credit settlement 
            treasuryDealsModel.Remarks = "Deal " + treasuryDealsModel.DealId + " Liquidated";
            transType = Convert.ToString(TransType.TransactionType.Liquidation);
            transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                treasuryDealsModel.PrincipalAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount),
                treasuryDealsModel.PaymentAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount)));

            //debit interest account with net interest and credit settlement
            treasuryDealsModel.Remarks = "Net Interest accrued on deal" + treasuryDealsModel.DealId;
            var interest = treasuryDealsModel.InterestAmount - treasuryDealsModel.Penalty - treasuryDealsModel.WHTAmount;
            transType = Convert.ToString(TransType.TransactionType.Liquidation);
            transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                treasuryDealsModel.InterestAccount, Convert.ToDecimal(interest),
                 treasuryDealsModel.PaymentAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount)));

            //debit setllement account with penalty amount and credit product interest account 
            //TODO add interestExpense to product table
            treasuryDealsModel.Remarks = "Interest with penalty on Deal " + treasuryDealsModel.DealId 
                + treasuryDealsModel.DealId;
            transType = Convert.ToString(TransType.TransactionType.Liquidation);
            transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                treasuryDealsModel.PaymentAccount, Convert.ToDecimal(treasuryDealsModel.Penalty),
                treasuryProductCode.InterestPenaltyAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount)));

            //debit wht account with wht amount and credit witholding tax payable
            treasuryDealsModel.Remarks = "Penalty on deal by " + treasuryDealsModel.DealId;
            transType = Convert.ToString(TransType.TransactionType.Liquidation);
            transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                treasuryDealsModel.WHTAccount, Convert.ToDecimal(treasuryDealsModel.WHTAmount),
                treasuryProductCode.WHTPayableAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount)));
            Utils.LogNO("Double Entries count: " + transactionModels.Count());
            return transactionModels;
            //foreach (var transaction in transactionModels)
            //{
            //    _transactionService.CreateTransactionDeals(transaction);

            //}
        }


        public List<TransactionModel> CreatDoubleEntriesForPreLiquidateDiscount(TreasuryDealsModel treasuryDealsModel) 
        {
            TreasuryProductCode treasuryProductCode = db.TreasuryProductCode
                                          .Where(x => x.ProductCode == treasuryDealsModel.ProductCode)
                                                                .FirstOrDefault();
            var OldInterestAmount = db.TreasuryDealsMaster.Where(x => x.Id == treasuryDealsModel.Id).FirstOrDefault().InterestAmount;
            List<TransactionModel> transactionModels = new List<TransactionModel>();
            string transType = "";

            // debit settlement  credit interest account with unend insterest (old interest - new insterest) 
            var unEndInterest = Convert.ToDecimal(OldInterestAmount) - Convert.ToDecimal(treasuryDealsModel.InterestAmount);
            treasuryDealsModel.Remarks = "Deal " + treasuryDealsModel.DealId + " Liquidated";
            transType = Convert.ToString(TransType.TransactionType.Liquidation);
            transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                treasuryDealsModel.PrincipalAccount, unEndInterest ,
                treasuryDealsModel.PaymentAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount)));

            //debit settlement account with penalty credit interest expense with penalty
            treasuryDealsModel.Remarks = "Settlement with penalty on Deal " + treasuryDealsModel.DealId;
           // var interest = treasuryDealsModel.InterestAmount - treasuryDealsModel.Penalty - treasuryDealsModel.WHTAmount;
            transType = Convert.ToString(TransType.TransactionType.Liquidation);
            transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                treasuryDealsModel.PaymentAccount, Convert.ToDecimal(treasuryDealsModel.Penalty),
                 treasuryProductCode.InterestExpenseAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount)));

            //debit setllement account with penalty amount and credit product interest account 
            //TODO add interestExpense to product table
            treasuryDealsModel.Remarks = "Interest with penalty on Deal " + treasuryDealsModel.DealId
                + treasuryDealsModel.DealId;
            transType = Convert.ToString(TransType.TransactionType.Liquidation);
            transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                treasuryDealsModel.PaymentAccount, Convert.ToDecimal(treasuryDealsModel.Penalty),
                treasuryProductCode.InterestPenaltyAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount)));

            //debit witholding tax account credit wht tax payable with (new insterest)
            treasuryDealsModel.Remarks = "Penalty on deal by " + treasuryDealsModel.DealId;
            transType = Convert.ToString(TransType.TransactionType.Liquidation);
            transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                treasuryDealsModel.WHTAccount, Convert.ToDecimal(treasuryDealsModel.NetInterestAmount),
                treasuryProductCode.WHTPayableAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount)));

            //debit the witholding tax account and credit d settlement account (unend interest)
            treasuryDealsModel.Remarks = "Penalty on deal by " + treasuryDealsModel.DealId;
            transType = Convert.ToString(TransType.TransactionType.Liquidation);
            transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                treasuryDealsModel.WHTAccount, unEndInterest,
                treasuryDealsModel.PaymentAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount)));

            Utils.LogNO("Double Entries count: " + transactionModels.Count());
            return transactionModels;
            //foreach (var transaction in transactionModels)
            //{
            //    _transactionService.CreateTransactionDeals(transaction);

            //}
        }

        public List<TransactionModel> CreatDoubleEntriesForPartialLiquidation(TreasuryDealsModel treasuryDealsModel) 
        {
            TreasuryProductCode treasuryProductCode = db.TreasuryProductCode
                                          .Where(x => x.ProductCode == treasuryDealsModel.ProductCode)
                                                                .FirstOrDefault();
            List<TransactionModel> transactionModels = new List<TransactionModel>();
            string transType = "";
            //Debit settlement account with principal - preliquidated Amount credit principal account
            treasuryDealsModel.Remarks = "Partial liquidation of " + treasuryProductCode.ProductName
                + ". Deal with deal number " + treasuryDealsModel.CBADealId;
            var amount = Convert.ToDecimal(treasuryDealsModel.PrincipalAmount)
                                            - Convert.ToDecimal(treasuryDealsModel.PreLiquidatedAmount);
            transType = Convert.ToString(TransType.TransactionType.TreasuryTransfer);
            transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                treasuryDealsModel.PaymentAccount, amount,
                treasuryDealsModel.PaymentAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount)));

            Utils.LogNO("Double Entries count: " + transactionModels.Count());
            return transactionModels;
        }

        public List<TransactionModel> CreatDoubleEntriesForDiscountLiquidation(TreasuryDealsModel treasuryDealsModel)
        {
            TreasuryProductCode treasuryProductCode = db.TreasuryProductCode
                                          .Where(x => x.ProductCode == treasuryDealsModel.ProductCode)
                                                                .FirstOrDefault();
            List<TransactionModel> transactionModels = new List<TransactionModel>();
            string transType = "";
            treasuryDealsModel.Remarks = "liquidation of " + treasuryProductCode.ProductName
                + ". Deal with deal number " + treasuryDealsModel.CBADealId;
            transType = Convert.ToString(TransType.TransactionType.TreasuryTransfer);
            transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                treasuryDealsModel.PrincipalAccount, Convert.ToDecimal(treasuryDealsModel.InterestAmount),
                treasuryDealsModel.PaymentAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount)));

            Utils.LogNO("Double Entries count: " + transactionModels.Count());
            return transactionModels;
        }


        public List<TransactionModel> CreatDoubleEntriesForDeals(TreasuryDealsModel treasuryDealsModel) 
        {
            TreasuryProductCode treasuryProductCode = db.TreasuryProductCode
                                          .Where(x => x.ProductCode == treasuryDealsModel.ProductCode)
                                                                .FirstOrDefault();
            List<TransactionModel> transactionModels = new List<TransactionModel>();
            string transType = "";

            //debit inflow account with principal amount -- credit principal account with thesame amount
            treasuryDealsModel.Remarks = "Money market deposit with deal " + treasuryDealsModel.CBADealId;
            transType = Convert.ToString(TransType.TransactionType.TreasuryTransfer);
            transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                treasuryDealsModel.InflowAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount),
                treasuryDealsModel.PrincipalAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount)));
            
            Utils.LogNO("Double Entries count: " + transactionModels.Count());
            return transactionModels;
        }

        public List<TransactionModel> CreatDoubleEntriesForDiscountedDeals(TreasuryDealsModel treasuryDealsModel) 
        {
            TreasuryProductCode treasuryProductCode = db.TreasuryProductCode
                                         .Where(x => x.ProductCode == treasuryDealsModel.ProductCode)
                                                               .FirstOrDefault();
            List<TransactionModel> transactionModels = new List<TransactionModel>();
            string transType = "";
            //debit inflow account with the phase value, credit principal account with the same amount
            treasuryDealsModel.Remarks = treasuryProductCode.ProductName + " investment with deal  " + treasuryDealsModel.DealId;
            transType = Convert.ToString(TransType.TransactionType.TreasuryTransfer);
            transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                treasuryDealsModel.InflowAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount),
                treasuryDealsModel.PrincipalAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount)));

            //Debit interest account with net interest credit inflow account with the same amount
            //upfront interest with
            treasuryDealsModel.Remarks = "Upward interest with deal " + treasuryDealsModel.DealId;
            transType = Convert.ToString(TransType.TransactionType.TreasuryTransfer);
            transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                treasuryDealsModel.InterestAccount, Convert.ToDecimal(treasuryDealsModel.NetInterestAmount),
                treasuryDealsModel.InflowAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount)));

            //Debit the inflow account with d witholding tax amount tax and credit the witholding tax account account with d same.
            treasuryDealsModel.Remarks = "Witholding Tax with  deal " + treasuryDealsModel.DealId;
            transType = Convert.ToString(TransType.TransactionType.TreasuryTransfer);
            transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                treasuryDealsModel.InflowAccount, Convert.ToDecimal(treasuryDealsModel.WHTAmount),
                treasuryDealsModel.WHTAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount)));

            Utils.LogNO("Double Entries count: " + transactionModels.Count());
            return transactionModels;
        }


        public void CreateTransactionDeals(List<TransactionModel> transactionModels)
        {
            foreach (var transaction in transactionModels)
            {
                _transactionService.CreateTransactionDeals(transaction);

            }
        }

        public List<TransactionModel> CreatDoubleEntriesForCallDeposit(TreasuryDealsModel treasuryDealsModel)
        {
            TreasuryProductCode treasuryProductCode = db.TreasuryProductCode
                                          .Where(x => x.ProductCode == treasuryDealsModel.ProductCode)
                                                                .FirstOrDefault();
            List<TransactionModel> transactionModels = new List<TransactionModel>();
            string transType = ""; 

            // debit principal account with principal amount and credit settlement 
            treasuryDealsModel.Remarks = "Deal " + treasuryDealsModel.DealId + " Liquidated";
            transType = Convert.ToString(TransType.TransactionType.Liquidation);
            transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                treasuryDealsModel.PrincipalAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount),
                treasuryDealsModel.PaymentAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount)));

            //debit interest account with net interest and credit settlement
            treasuryDealsModel.Remarks = "Net Interest accrued on deal" + treasuryDealsModel.DealId;
            var interest = treasuryDealsModel.InterestAmount - treasuryDealsModel.Penalty - treasuryDealsModel.WHTAmount;
            transType = Convert.ToString(TransType.TransactionType.Liquidation);
            transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                treasuryDealsModel.InterestAccount, Convert.ToDecimal(treasuryDealsModel.InterestAccount),
                 treasuryDealsModel.PaymentAccount, Convert.ToDecimal(treasuryDealsModel.PrincipalAmount)));

            //debit wht account with wht amount and credit witholding tax payable
            treasuryDealsModel.Remarks = "Penalty on deal by " + treasuryDealsModel.DealId;
            transType = Convert.ToString(TransType.TransactionType.Liquidation);
            transactionModels.Add(UpdateTransactionModel(treasuryDealsModel, transType,
                treasuryDealsModel.WHTAccount, Convert.ToDecimal(treasuryDealsModel.WHTAmount),
                treasuryProductCode.WHTPayableAccount,Convert.ToDecimal(treasuryDealsModel.PrincipalAmount)));
            Utils.LogNO("Double Entries count: " + transactionModels.Count());
            return transactionModels;
            //foreach (var transaction in transactionModels)
            //{
            //    _transactionService.CreateTransactionDeals(transaction);

            //}
        }

        public TransactionModel UpdateTransactionModel(TreasuryDealsModel treasuryDealsModel, string transType,
                                           string debitAccountNumber, decimal amount, string creditAccountNumber,
                                           decimal principalAmount)
        {
            try
            {
                //  var DebitAccountDetails = _transactionService.GetCustomerAccountDetails(debitAccountNumber);
                // var CreditAccountDetails = _transactionService.GetCustomerAccountDetails(creditAccountNumber);
                TransactionModel transactionModel = new TransactionModel
                {
                    TotalAmt = principalAmount,
                    Amount = amount,
                    TellerId = debitAccountNumber,
                    ToTellerId = treasuryDealsModel.DealId,
                    TransName = treasuryDealsModel.UserId,
                    AccountName = "",
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
                    TransRef = treasuryDealsModel.TransRef,
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
    }
}
