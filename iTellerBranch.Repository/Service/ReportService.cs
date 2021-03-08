using iTellerBranch.Model.ViewModel;
using iTellerBranch.Repository.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace iTellerBranch.Repository.Service
{
    public class ReportService : BaseService, IReportService
    {

        public object GetVaultTransaction(DateTime dtFrom, DateTime dtTo)
        {

            int vaultIn = Convert.ToInt16(TransType.TransactionType.vaultIn);
            int vaultOut = Convert.ToInt16(TransType.TransactionType.VaultOut);
            // var transaction = db.TransDepositWithdrawalMaster.ToList();
           // db.Configuration.ProxyCreationEnabled = false;
            var transaction = db.TransactionsMaster.Where(x => DbFunctions.TruncateTime(x.CreationDate).Value >= dtFrom
                                                && DbFunctions.TruncateTime(x.CreationDate).Value <= dtTo
                                                && x.TransType == vaultIn
                                                || DbFunctions.TruncateTime(x.CreationDate).Value >= dtFrom
                                                && DbFunctions.TruncateTime(x.CreationDate).Value <= dtTo
                                                && x.TransType == vaultOut).ToList();
           // return transaction;
            return new
            {
                VaultTransaction = transaction.Select(x => new
                {
                    x.TransRef,
                    x.TranId,
                    IsVaultIn = x.TransType == vaultIn ? true : false,
                     x.TellerId,
                     x.ToTellerId,
                    //ToTellerId = x.CashTransactions.Where(y=> y.TranId == x.TranId).Count() > 0 ? 
                    //x.CashTransactions.Where(y => y.TranId == x.TranId).Select(y => y.TillId).FirstOrDefault(): "",
                    Currency = db.CashDenomination.Where(y => y.ID == x.Currency).Select(y => y.Abbrev).FirstOrDefault(),
                    x.TotalAmount,
                    TellerName = x.TransacterName,
                    x.BranchCode,
                    x.CreationDate
                }).OrderByDescending(x=>x.CreationDate)
            };
        }

        //public object GetVaultTransaction(DateTime dtFrom, DateTime dtTo)
        //{
        //    int vaultIn = Convert.ToInt16(TransType.TransactionType.vaultIn);
        //    int vaultOut = Convert.ToInt16(TransType.TransactionType.VaultOut);
        //    // var transaction = db.TransDepositWithdrawalMaster.ToList();
        //    var transaction = db.TransDepositWithdrawalMaster.Where(x => DbFunctions.TruncateTime(x.CreationDate).Value >= dtFrom
        //                                        && DbFunctions.TruncateTime(x.CreationDate).Value <= dtTo
        //                                        && x.TransType == vaultIn
        //                                        || DbFunctions.TruncateTime(x.CreationDate).Value >= dtFrom
        //                                        && DbFunctions.TruncateTime(x.CreationDate).Value <= dtTo
        //                                        && x.TransType == vaultOut).ToList();
        //    return new
        //    {
        //        VaultTransaction = transaction.Select(x => new
        //        {
        //            x.TransRef,
        //            x.TransactionMaster.FirstOrDefault().TranID,
        //            IsVaultIn = x.TransactionMaster.FirstOrDefault().TransType == vaultIn ? true : false,
        //            TellerId = x.TransactionMaster.FirstOrDefault().TransType
        //                                        == vaultIn ? x.TransactionMaster.Where(y => y.CrDr == "D")
        //                                        .FirstOrDefault().TellerId : x.TransactionMaster.Where(y =>y.CrDr == "C")
        //                                        .FirstOrDefault().TellerId,
        //            ToTellerId = x.TransactionMaster.FirstOrDefault().TransType
        //                                        == vaultIn ? x.TransactionMaster.Where(y =>  y.CrDr == "C")
        //                                        .FirstOrDefault().TellerId : x.TransactionMaster.Where(y => y.CrDr == "D")
        //                                        .FirstOrDefault().TellerId, 
        //            Currency = x.CurrencyAbbrev,
        //            x.TotalAmount,
        //            TellerName = x.InitiatorName,
        //            x.Branch
        //        })
        //    };
        //}

        public object TellerCastReport(DateTime dtFrom, DateTime dtTo)
        {
            int deposit = Convert.ToInt16(TransType.TransactionType.Deposit);
            int withdrawal = Convert.ToInt16(TransType.TransactionType.Withdrawal);
            var transaction = db.TransactionsMaster.Where(x => DbFunctions.TruncateTime(x.CreationDate).Value >= dtFrom
                                               && DbFunctions.TruncateTime(x.CreationDate).Value <= dtTo && x.IsReversed == false).ToList();
            var transDeposit = transaction.Where(x => x.TransType == deposit).Select(x => x.TotalAmount).Sum();
            var transWithdrawal = transaction.Where(x => x.TransType == withdrawal).Select(x => x.TotalAmount).Sum();

            return new
            {
                TransactionDetails  = transaction.Select(x => new
                {
                    x.TransRef,
                    x.TranId,
                    x.TellerId,
                    x.ToTellerId,
                    Currency = db.CashDenomination.Where(y => y.ID == x.Currency).Select(y => y.Abbrev).FirstOrDefault(),
                    x.TotalAmount,
                    TellerName = x.TransacterName,
                    x.BranchCode,
                    x.CreationDate,
                    x.AccountNumber,
                    x.AccountName,
                    x.ApprovedBy,
                    x.DisapprovedBy,
                    x.WhenApproved,
                    x.TransacterName,
                    x.TransactionParty,
                    IsWithdrawal = x.TransType == withdrawal ? true : false,
                    IsDeposit = x.TransType == deposit ? true : false
                }).OrderByDescending(x => x.CreationDate),
                TotalWithdrawal = transWithdrawal,
                TotalDeposit = transDeposit,
                Balance = transDeposit - transWithdrawal
            };
        }

        public object TreasuryDealReport(DateTime dtFrom, DateTime dtTo)
        {
            
            var treasuryDealsMaster = db.TreasuryDealsMaster.Where(x => DbFunctions.TruncateTime(x.CreationDate).Value >= dtFrom
                                               && DbFunctions.TruncateTime(x.CreationDate).Value <= dtTo).ToList();

            return new
            {
                TreasuryDealsMaster = treasuryDealsMaster.Select(x => new
                {
                    x.Id,
                    x.CreationDate,
                    x.CBADealId,
                    ProductName = db.TreasuryProductCode.Where(y => y.ProductCode == x.ProductCode).Select(y => y.ProductName).FirstOrDefault(),
                    Currency = db.CashDenomination.Where(y => y.ID == x.CurrencyCode).Select(y => y.Currency).FirstOrDefault(),
                    x.CustomerName,
                    x.PrincipalAmount,
                    x.ValueDate,
                    x.MaturityDate,
                    x.PaymentDate,
                    x.TransactionStatus,
                    x.ProcessStatus,
                    ApprovalStatus = x.ProcessStatus == 1 ? "Pending" : x.ProcessStatus == 2 ? "Disapproved" :
                                        x.ProcessStatus == 4 ? "Updated" : "Approved",
                    x.ApprovedBy,
                    x.InterestAmount,
                    x.PaymentAmount,
                    x.PaymentAccount,
                    x.Remarks,
                    x.InflowAccount,
                    x.WHTAccount,
                    x.InterestAccount,
                    x.PrincipalAccount,
                    Rate = x.TreasuryInterest.FirstOrDefault() != null ? x.TreasuryInterest.FirstOrDefault().InterestRate : null,
                    x.UserId,
                    x.AccountOfficer,
                    x.CurrencyCode,
                    x.DealersReference,
                    x.DealId,
                    x.Tenure,
                    x.BranchCode,
                    x.CustomerId,
                    x.DisapprovedBy,
                    x.TerminationInstructionCode,
                    TerminationInstruction = db.TerminationInstruction.Where(y=> y.Code == x.TerminationInstructionCode)
                                            .Select(y=> y.Description).FirstOrDefault(),
                    x.ProductCode,
                    PreLiquidation = db.PreLiquidatedDeal.Where(y=> y.DealID == x.DealId).Select(y=> new
                    {
                        y.ID,
                        y.IsPartialLiquidation,
                        y.LiquidationDate
                    }).FirstOrDefault()
                }).OrderByDescending(x => x.CreationDate).AsQueryable()
            };
        }


        public object TerminationReport(DateTime dtFrom, DateTime dtTo)
        {
            
            var terminationReport = db.TreasuryDealsMaster.Where(x => DbFunctions.TruncateTime(x.CreationDate).Value >= dtFrom
                                               && DbFunctions.TruncateTime(x.CreationDate).Value <= dtTo).ToList();
            
            return new
            {
                TerminationReport = terminationReport.Select(x => new
                {
                    x.Id,
                    x.CreationDate,
                    x.CBADealId,
                    TerminationInstruction = db.TerminationInstruction.Where(y => y.CBACode == x.ProductCode).Select(y => y.CBACode).FirstOrDefault(),
                    Currency = db.CashDenomination.Where(y => y.ID == x.CurrencyCode).Select(y => y.Currency).FirstOrDefault(),
                    x.CustomerName,
                    x.PrincipalAmount,
                    x.ValueDate,
                    x.MaturityDate,
                    x.PaymentDate,
                    x.TransactionStatus,
                    x.ProcessStatus,
                    x.ApprovedBy,
                    x.InterestAmount,
                    x.PaymentAmount,
                    x.UserId,
                    DoubleEntries = db.TreasuryDetails.Where(y => y.DealId == x.CBADealId).Select(c => new
                    {
                        c.ID,
                        c.TranId,
                        c.AccountNumber,
                        c.AccountName,
                        c.Naration,
                        c.Amount,
                        c.CrDr,
                        Posted = true
                    }).AsQueryable(),
                }).OrderByDescending(x => x.CreationDate).AsQueryable()
            };
        }



        public object ChequeIssuanceReport(DateTime dtFrom, DateTime dtTo)
        {

            var ChequeIssuanceReport = db.ManagerChequeIssuanceDetails.Where(x => DbFunctions.TruncateTime(x.DateCreated).Value >= dtFrom
                                               && DbFunctions.TruncateTime(x.DateCreated).Value <= dtTo).ToList();

            return new
            {
                ChequeIssuanceReport = ChequeIssuanceReport.OrderByDescending(x => x.DateCreated).AsQueryable()
            };
        }
         
        public object OutwardChequeReport(DateTime dtFrom, DateTime dtTo)
        {

            var OutwardChequeReport = db.OutwardChequeDetails.Where(x => DbFunctions.TruncateTime(x.DateCreated).Value >= dtFrom
                                               && DbFunctions.TruncateTime(x.DateCreated).Value <= dtTo).ToList();

            return new
            {
                OutwardChequeReport = OutwardChequeReport.OrderByDescending(x => x.DateCreated).AsQueryable() 
            };
        }

        public object PostedCallOverReport(DateTime dtFrom, DateTime dtTo)
        {
            int withdrawal = Convert.ToInt16(TransType.TransactionType.Withdrawal);
            int chequeLodgement = Convert.ToInt16(TransType.TransactionType.ChequeLodgement);
            int cheque = Convert.ToInt16(TransType.TransactionType.InHouseChequesDeposit);
            int deposit = Convert.ToInt16(TransType.TransactionType.Deposit);
            int transfer = Convert.ToInt16(TransType.TransactionType.InHouseTransfer);
            int cashWTDCounter= Convert.ToInt16(TransType.TransactionType.CashWithDrawalCounter);
            var transaction = db.TransactionsMaster.Where(x => DbFunctions.TruncateTime(x.CreationDate).Value >= dtFrom
                                               && DbFunctions.TruncateTime(x.CreationDate).Value <= dtTo 
                                               && x.Posted == true).ToList();

            return new
            {
                Transaction = transaction.Where(x=> x.TransType <= 3).Select(x => new
                {
                    x.TransRef,
                    x.TranId,
                    x.TellerId,
                    x.ToTellerId,
                    Currency = db.CashDenomination.Where(y => y.ID == x.Currency).Select(y => y.Abbrev).FirstOrDefault(),
                    x.TotalAmount,
                    TellerName = x.TransacterName,
                    x.CBA,
                    BranchAccounts = db.BranchAccounts.Where(y=> y.BranchCode == x.BranchCode).Select(y=> new
                    {
                        DebitAccount = y.BranchDBSuspenceAccount,
                        CreditAccount = y.BranchCRSuspenceAccount
                    }).FirstOrDefault(),
                    x.BranchCode,
                    x.Posted,
                    x.TransacterEmail,
                    Remarks = x.Narration,
                    Authorizer = x.ApprovedBy,
                    x.CreationDate,
                    x.AccountNumber,
                    x.AccountName,
                    x.ApprovedBy,
                    x.DisapprovedBy,
                    x.WhenApproved,
                    x.TransacterName,
                    x.TransactionParty,
                    //TransTypeName = x.TransType == withdrawal ? "Cash Withdrawal" : x.TransType == deposit ? "Cash Deposit"
                    //                : x.TransType == cheque ? "InHouse Cheque Deposit" : x.TransType == chequeLodgement ? "Cash Withdrwal with Cheque" : "In-House Transfer",
                    TransTypeName = x.TransType == withdrawal ? "Cash Withdrawal" : x.TransType == deposit ? "Cash Deposit"
                                    : x.TransType == cheque ? "InHouse Cheque Deposit" : x.TransType == chequeLodgement?"Cash Withdrawal With Cheque": x.TransType == cashWTDCounter ? "Cash Withdrwal with Counter Cheque" : x.TransType == transfer ? "In-House Transfer" : "Others",
                    //TransTypeName = TransType.getTranTypeName(x.TransType),
                    IsWithdrawal = x.TransType == withdrawal || x.TransType == chequeLodgement ? true : false,
                    IsDeposit = x.TransType == deposit ? true : false
                }).OrderByDescending(x => x.CreationDate)
            };
        }

        public object TransactionReport(DateTime dtFrom, DateTime dtTo) 
        {
            int withdrawal = Convert.ToInt16(TransType.TransactionType.Withdrawal);
            int deposit = Convert.ToInt16(TransType.TransactionType.Deposit);
            int chequeLodgement = Convert.ToInt16(TransType.TransactionType.ChequeLodgement); 
            int cheque = Convert.ToInt16(TransType.TransactionType.InHouseChequesDeposit);
            int transfer = Convert.ToInt16(TransType.TransactionType.InHouseTransfer);
            int cashWTDCounter= Convert.ToInt16(TransType.TransactionType.CashWithDrawalCounter);

            var transaction = db.TransactionsMaster.Where(x => DbFunctions.TruncateTime(x.CreationDate).Value >= dtFrom
                                               && DbFunctions.TruncateTime(x.CreationDate).Value <= dtTo && (x.TransType==withdrawal 
                                               ||x.TransType==deposit || x.TransType==cheque||x.TransType==transfer 
                                               ||x.TransType == chequeLodgement||x.TransType==cashWTDCounter)
                                               ).ToList();

            return new
            {
                Transaction = transaction.Select(x => new
                {
                    x.TransRef,
                    x.TranId,
                    x.TellerId,
                    x.ToTellerId,
                    Currency = db.CashDenomination.Where(y => y.ID == x.Currency).Select(y => y.Abbrev).FirstOrDefault(),
                    x.TotalAmount,
                    TellerName = x.TransacterName,
                    x.BranchCode,
                    x.CreationDate,
                    x.AccountNumber,
                    x.AccountName,
                    x.Status,
                    x.ApprovedBy,
                    x.DisapprovedBy,
                    x.WhenApproved,
                    x.TransacterName,
                    x.TransactionParty,
                    x.Remarks,
                    x.Narration,
                    x.TransType,
                    x.TransacterPhoneNo,
                    x.TransacterEmail,
                    x.Approved,
                    x.CBA,
                    x.ValueDate,
                    IsWithdrawal = x.TransType == withdrawal ? true : false,
                    IsDeposit = x.TransType == deposit ? true : false,
                    IsCheque = x.TransType == cheque ? true : false,
                    //TransTypeName = x.TransType == withdrawal ? "Cash Withdrawal" : x.TransType == deposit ? "Cash Deposit"
                    //                : x.TransType == cheque ? "InHouse Cheque Deposit" : x.TransType == chequeLodgement ? "Cash Withdrwal with Cheque": "In-House Transfer",
                    TransTypeName = x.TransType == withdrawal ? "Cash Withdrawal" : x.TransType == deposit ? "Cash Deposit"
                                    : x.TransType == cheque ? "InHouse Cheque Deposit" : x.TransType == chequeLodgement ? "Cash Withdrawal With Cheque" : x.TransType == cashWTDCounter ? "Cash Withdrwal with Counter Cheque" : x.TransType == transfer ? "In-House Transfer" : "Others",
                    //TransTypeName = TransType.getTranTypeName(x.TransType),
                    Beneficiaries = db.TransferDetails.Where(z => z.TranId == x.TranId).ToArray()
                }).OrderByDescending(x => x.CreationDate)
            };
        }


        public object TillReport(DateTime dtFrom, DateTime dtTo) 
        {
            int till = Convert.ToInt16(TransType.TransactionType.TillTransfer);
            var transaction = db.TransactionsMaster.Where(x => DbFunctions.TruncateTime(x.CreationDate).Value >= dtFrom
                                              && DbFunctions.TruncateTime(x.CreationDate).Value <= dtTo
                                              && x.TransType == till).ToList();
            return new
            {
                TillTransaction = transaction.Select(x => new
                {
                    x.TransRef,
                    x.TranId,
                    x.TellerId,
                    Currency = db.CashDenomination.Where(y => y.ID == x.Currency).Select(y => y.Abbrev).FirstOrDefault(),
                    x.TotalAmount,
                    TellerName = x.TransacterName,
                    x.CBA,
                    x.ToTellerId,
                    x.AccountName,
                    x.BranchCode,
                    x.CreationDate,
                    Remarks= x.Narration
                }).OrderByDescending(x => x.CreationDate)
            };
        }


        public object AuditReport(DateTime dtFrom, DateTime dtTo)
        {
            
            var audit = db.Audit.Where(x => DbFunctions.TruncateTime(x.DateTime).Value >= dtFrom
                                              && DbFunctions.TruncateTime(x.DateTime).Value <= dtTo).ToList();
            return new
            {
                Audit= audit.Select(x => new
                {
                    x.DateTime,
                    x.Event,
                    x.HostName,
                    x.Id,
                    x.UserId,
                    x.MachineName
                   
                }).OrderByDescending(x => x.DateTime)
            };
        }


    }
}
