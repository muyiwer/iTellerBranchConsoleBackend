
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
using static iTellerBranch.Model.ViewModel.TransType;

namespace iTellerBranch.Repository.Service
{
    public class IssuanceService : BaseService, IIssuanceService
    {

        public IssuanceService()
        {
            
        }


        public object GetChequeTemplates()
        {

          return  db.Cheque_Template.Select(x => new
            {
                x.Template_Code,
                x.Template_Name
            }).ToList();
        }

        public void UpdateTransactionChequeTemplate(int tranId, int templateCode)
        {
            var transaction = db.ManagerChequeIssuanceDetails.Where(x => x.ID == tranId).FirstOrDefault();
            transaction.TemplateCode = templateCode;
            db.Entry(transaction).State = EntityState.Modified;
            db.SaveChanges();
        }
        
        public List<MCAccount> GetMCAccounts()
        {
            try
            {
                var mcaccount = db.MCAccount.ToList();
                return mcaccount;
            }
            catch (Exception ex)
            {
                Utils.LogNO(ex.Message);
                return null;
            }
 
            
        }
        

        public IQueryable<ManagerChequeIssuanceDetails> GetMCAccountsByAccountNumber(string accountNumber)
        {
            try
            {
                return db.ManagerChequeIssuanceDetails.Where(x=> x.Approved == true && x.AccountNumber == accountNumber 
                                            && x.PaidStatus == null).AsQueryable();
            }
            catch (Exception ex)
            {
                Utils.LogNO("GetMCAccountsByAccountNumber: " + ex.Message);
                return null;
            }


        }

        public List<MCCharge> GetMCCharge()
        {
            try
            {
                var mccharge = db.MCCharge.ToList();
                return mccharge;
            }
            catch (Exception ex)
            {
                Utils.LogNO(ex.Message);
                return null;
            }
 
            
        }

        public List<ManagerChequeIssuanceDetails> GetTransactionMaster()
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var Issuance = db.ManagerChequeIssuanceDetails.Where(x=>x.Approved == null && x.PaidStatus == null).ToList();

                return Issuance;
            }
            catch (Exception ex)
            {
                Utils.LogNO(ex.Message);
                throw;
            }

        }

        public List<ManagerChequeIssuanceDetails> GetApprovedDraft() 
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var Issuance = db.ManagerChequeIssuanceDetails.Where(x => x.Approved == true && x.Printed == false).ToList();

                return Issuance;
            }
            catch (Exception ex)
            {
                Utils.LogNO(ex.Message);
                throw;
            }

        }

        public List<ManagerChequeIssuanceDetails> GetDraftForRepurchase() 
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var Issuance = db.ManagerChequeIssuanceDetails.
                                Where(x => x.Approved == false  && x.PaidStatus == "R").ToList();

                return Issuance;
            }
            catch (Exception ex)
            {
                Utils.LogNO(ex.Message);
                throw;
            }

        }

        public List<OutwardChequeDetails> GetOutwardCheque()
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var Issuance = db.OutwardChequeDetails.
                                Where(x => x.Approved == false ).ToList();

                return Issuance;
            }
            catch (Exception ex)
            {
                Utils.LogNO(ex.Message);
                throw;
            }

        }

        public void ApproveManagerIssuance(ManagerChequeIssuanceModel transMaster)
        {
            Utils.LogNO("Records: " + transMaster);
            var transmaster = db.ManagerChequeIssuanceDetails.Where(x => x.ID == transMaster.ID).FirstOrDefault();
            transmaster.Approved = true;
            transmaster.ApprovedBy = transMaster.ApprovedBy;
            transmaster.TransactionReference = transMaster.TransactionReference;
            transmaster.WhenApproved = DateTime.Now;
            db.Entry(transmaster).State = EntityState.Modified;
            db.SaveChanges();
        }
         
        public void ApproveOutwardCheque(OutwardChequeDetailsModel transMaster)
        {
            try
            {
                Utils.LogNO("Approving.......");
                var transmaster = db.OutwardChequeDetails.Where(x => x.Id == transMaster.Id).FirstOrDefault();
                Utils.LogNO("Records: " + transmaster);
                transmaster.Approved = true;
                transmaster.WhenApproved = DateTime.Now;
                transmaster.ApprovedBy = transMaster.ApprovedBy;
                transmaster.CBAResponse = transMaster.CBAResponse;
                transmaster.CBAResponseCode = transMaster.CBAResponseCode;
                transmaster.TransactionReference = transMaster.TransactionReference;
                db.Entry(transmaster).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Utils.LogNO("Error while approving: " + ex.Message);
            }
           
        }

        public void DisapproveOutwardCheque(OutwardChequeDetailsModel transMaster)
        {
            try
            { 
                Utils.LogNO("Dispproving.......");
                var transmaster = db.OutwardChequeDetails.Where(x => x.Id == transMaster.Id).FirstOrDefault();
                Utils.LogNO("Records: " + transmaster);
                transmaster.Approved = false;
                transmaster.WhenDissaprroved = DateTime.Now;
                transmaster.DisapprovedBy = transMaster.DisapprovedBy;
                transmaster.DisapprovalReason = transMaster.DisapprovalReason;
                db.Entry(transmaster).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Utils.LogNO("Error while approving: " + ex.Message);
            }

        }


        public void UpdateManagerIssuanceForRepurchase(int tranId) 
        {
            var transmaster = db.ManagerChequeIssuanceDetails.Where(x => x.ID == tranId).FirstOrDefault();
            transmaster.Approved = false;
            transmaster.PaidStatus = "R";
            db.Entry(transmaster).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void ApproveManagerIssuanceForRepurchase(ManagerChequeIssuanceModel transMaster) 
        {
            var transmaster = db.ManagerChequeIssuanceDetails.Where(x => x.ID == transMaster.ID).FirstOrDefault();
            transmaster.Approved = true;
            transmaster.PaidStatus = "R";
            transmaster.WhenApproved = DateTime.Now;
            transmaster.ApprovedBy = transMaster.ApprovedBy;
            transmaster.RepurchaseRefId = transMaster.TransactionReference;
            transmaster.CBAResponse = transMaster.CBAResponse;
            transMaster.CBAResponseCode = transMaster.CBAResponseCode;
            db.Entry(transmaster).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void DissaproveManagerIssuance(MCApprovalModel transMaster) 
        {
            var transmaster = db.ManagerChequeIssuanceDetails.Where(x => x.ID == transMaster.ID).FirstOrDefault();
            transmaster.Approved = false;
            transmaster.WhenDissapproved = DateTime.Now;
            transmaster.PaidStatus = "";
            transmaster.DissaprovedBy = transMaster.UserId; 
            db.Entry(transmaster).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void DissaproveOutwardCheque(int tranId) 
        {
            var transmaster = db.OutwardChequeDetails.Where(x => x.Id == tranId).FirstOrDefault();
            transmaster.Approved = false;
            transmaster.WhenDissaprroved = DateTime.Now;
            db.Entry(transmaster).State = EntityState.Modified;
            db.SaveChanges();
        }

        public object GetManagerIssuance(int tranId) 
        {
            try
            {

                return new
                {
                    ManagerChequeIssuanceDetails = db.ManagerChequeIssuanceDetails.Where(x => x.ID == tranId).FirstOrDefault()
                };
            }
            catch (Exception ex)
            {
                Utils.LogNO(ex.Message);
                throw;
            }

        }

        public void CreateManagerChequeIssuance(TransactionModel transactionModel)
        {
            try
            {
               
                ManagerChequeIssuanceDetails managerChequeIssuanceDetails = new ManagerChequeIssuanceDetails();
                managerChequeIssuanceDetails.AccountName = transactionModel.ManagerChequeIssuanceDetailsModel.AccountName;
                managerChequeIssuanceDetails.AccountNumber = transactionModel.ManagerChequeIssuanceDetailsModel.AccountNumber;
                managerChequeIssuanceDetails.Amount = transactionModel.TotalAmt;
                managerChequeIssuanceDetails.DraftNumber = transactionModel.ManagerChequeIssuanceDetailsModel.DraftNumber;
                managerChequeIssuanceDetails.PaidStatus = null;
                managerChequeIssuanceDetails.PaymentDetails = transactionModel.Remark;
                managerChequeIssuanceDetails.Printed = false;
                managerChequeIssuanceDetails.ChequeNumber = transactionModel.ChequeNo;
                managerChequeIssuanceDetails.BeneficiaryAccount = transactionModel.AccountNo;
                managerChequeIssuanceDetails.BeneficiaryName = transactionModel.AccountName;
                managerChequeIssuanceDetails.CreatedBy = transactionModel.InitiatorName;
                managerChequeIssuanceDetails.Approved = null;
                managerChequeIssuanceDetails.DateCreated = DateTime.Now;
                managerChequeIssuanceDetails.TransactionReference = transactionModel.TransRef;
                managerChequeIssuanceDetails.ValueDate = transactionModel.ValueDate;
                managerChequeIssuanceDetails.DebitValueDate = transactionModel.DebitValueDate;
                managerChequeIssuanceDetails.BranchCode = transactionModel.Branch;
                managerChequeIssuanceDetails.CurrencyCode = Convert.ToInt16(transactionModel.Currency);
                managerChequeIssuanceDetails.ChargeAmount = transactionModel.CHARGEAMT;
                db.ManagerChequeIssuanceDetails.Add(managerChequeIssuanceDetails);
                db.SaveChanges();
                var tranId = db.ManagerChequeIssuanceDetails.Where(x => x.TransactionReference == transactionModel.TransRef)
                    .FirstOrDefault().ID;

                List<DraftIssuedCharges> draftIssuedCharges = new List<DraftIssuedCharges>();
                foreach (var draftissuedCharge in transactionModel.DraftIssuedChargesModel)
                {
                    //DraftIssuedCharges draftIssuedCharges = new DraftIssuedCharges();
                    //draftIssuedCharges.Approved = false;
                    //draftIssuedCharges.ChargeAmount = draftissuedCharge.ChargeAmount;
                    //draftIssuedCharges.ChargeID = draftissuedCharge.ChargeID;
                    draftIssuedCharges.Add(new DraftIssuedCharges
                    {
                        Approved = false,
                        ChargeAmount = draftissuedCharge.ChargeAmount,
                        ChargeID = draftissuedCharge.ChargeID,
                        TranId = tranId
                    });
                }
                db.DraftIssuedCharges.AddRange(draftIssuedCharges);
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                Utils.LogNO("CreateManagerChequeIssuance error: " + ex);
            }
          

        }


        public void CreateAndApproveManagerChequeIssuance(TransactionModel transactionModel) 
        {
            try
            {

                ManagerChequeIssuanceDetails managerChequeIssuanceDetails = new ManagerChequeIssuanceDetails();
                managerChequeIssuanceDetails.AccountName = transactionModel.ManagerChequeIssuanceDetailsModel.AccountName;
                managerChequeIssuanceDetails.AccountNumber = transactionModel.ManagerChequeIssuanceDetailsModel.AccountNumber;
                managerChequeIssuanceDetails.Amount = transactionModel.TotalAmt;
                managerChequeIssuanceDetails.DraftNumber = transactionModel.ManagerChequeIssuanceDetailsModel.DraftNumber;
                managerChequeIssuanceDetails.PaidStatus =null;
                managerChequeIssuanceDetails.PaymentDetails = transactionModel.Remark;
                managerChequeIssuanceDetails.Printed = false;
                managerChequeIssuanceDetails.ValueDate = transactionModel.ValueDate;
                managerChequeIssuanceDetails.DebitValueDate = transactionModel.DebitValueDate;
                managerChequeIssuanceDetails.ChequeNumber = transactionModel.ChequeNo;
                managerChequeIssuanceDetails.BeneficiaryAccount = transactionModel.AccountNo;
                managerChequeIssuanceDetails.BeneficiaryName = transactionModel.AccountName;
                managerChequeIssuanceDetails.CreatedBy = transactionModel.InitiatorName;
                managerChequeIssuanceDetails.Approved = true;
                managerChequeIssuanceDetails.ApprovedBy = transactionModel.ApprovedBy;
                managerChequeIssuanceDetails.WhenApproved = DateTime.Now;
                managerChequeIssuanceDetails.DateCreated = DateTime.Now;
                managerChequeIssuanceDetails.TransactionReference = transactionModel.TransRef;
                managerChequeIssuanceDetails.BranchCode = transactionModel.Branch;
                managerChequeIssuanceDetails.CBAResponse = transactionModel.CBAResponse;
                managerChequeIssuanceDetails.CBAResponseCode = transactionModel.CBACode;
                managerChequeIssuanceDetails.TransactionReference = transactionModel.TransRef;
                managerChequeIssuanceDetails.CreatedBy = transactionModel.ApprovedBy;
                managerChequeIssuanceDetails.CurrencyCode = Convert.ToInt16(transactionModel.Currency);
                managerChequeIssuanceDetails.ChargeAmount = transactionModel.CHARGEAMT;
                db.ManagerChequeIssuanceDetails.Add(managerChequeIssuanceDetails);
                db.SaveChanges();
                var tranId = db.ManagerChequeIssuanceDetails.Where(x => x.TransactionReference == transactionModel.TransRef)
                    .FirstOrDefault().ID;

                List<DraftIssuedCharges> draftIssuedCharges = new List<DraftIssuedCharges>();
                foreach (var draftissuedCharge in transactionModel.DraftIssuedChargesModel)
                {
                    //DraftIssuedCharges draftIssuedCharges = new DraftIssuedCharges();
                    //draftIssuedCharges.Approved = false;
                    //draftIssuedCharges.ChargeAmount = draftissuedCharge.ChargeAmount;
                    //draftIssuedCharges.ChargeID = draftissuedCharge.ChargeID;
                    draftIssuedCharges.Add(new DraftIssuedCharges
                    {
                        Approved = false,
                        ChargeAmount = draftissuedCharge.ChargeAmount,
                        ChargeID = draftissuedCharge.ChargeID,
                        TranId = tranId
                    });
                }
                db.DraftIssuedCharges.AddRange(draftIssuedCharges);
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                Utils.LogNO("CreateManagerChequeIssuance error: " + ex);
            }


        }

        public void CreateOutwardChequeIssuance(TransactionModel transactionModel)
        {
            try
            {

                OutwardChequeDetails outwardChequeDetails  = new OutwardChequeDetails();
                outwardChequeDetails.DebitAccountName = transactionModel.ManagerChequeIssuanceDetailsModel.AccountName;
                outwardChequeDetails.DebitAccountNumber = transactionModel.ManagerChequeIssuanceDetailsModel.AccountNumber;
                outwardChequeDetails.CreditAmount = transactionModel.TotalAmt;
                outwardChequeDetails.BankAddress = transactionModel.BankAddress;
                outwardChequeDetails.ChequeNumber = transactionModel.ChequeNo;
                outwardChequeDetails.CreditAccountNumber = transactionModel.AccountNo;
                outwardChequeDetails.CreditAccountName = transactionModel.AccountName;
                outwardChequeDetails.TellerName = transactionModel.TellerId;
                outwardChequeDetails.DrawalName = transactionModel.DrawalName;
                outwardChequeDetails.DebitValueDate = transactionModel.DebitValueDate;
                outwardChequeDetails.DateCreated = DateTime.Now;
                outwardChequeDetails.BankCode = transactionModel.BankCode;
                outwardChequeDetails.BranchCode = transactionModel.BranchCode;
                outwardChequeDetails.Approved = false;
               // outwardChequeDetails = transactionModel.TransRef;
               // outwardChequeDetails.BranchCode = transactionModel.Branch;
                outwardChequeDetails.CurrencyCode = Convert.ToInt16(transactionModel.Currency);
                db.OutwardChequeDetails.Add(outwardChequeDetails);
                db.SaveChanges();
               
            }
            catch (Exception ex)
            {
                Utils.LogNO("CreateManagerChequeIssuance error: " + ex);
            }


        }

        public void CreateAndApproveOutwardChequeIssuance(TransactionModel transactionModel) 
        {
            try
            {

                OutwardChequeDetails outwardChequeDetails = new OutwardChequeDetails();
                outwardChequeDetails.DebitAccountName = transactionModel.ManagerChequeIssuanceDetailsModel.AccountName;
                outwardChequeDetails.DebitAccountNumber = transactionModel.ManagerChequeIssuanceDetailsModel.AccountNumber;
                outwardChequeDetails.CreditAmount = transactionModel.TotalAmt;
                outwardChequeDetails.BankAddress = transactionModel.BankAddress;
                outwardChequeDetails.ChequeNumber = transactionModel.ChequeNo;
                outwardChequeDetails.CreditAccountNumber = transactionModel.AccountNo;
                outwardChequeDetails.CreditAccountName = transactionModel.AccountName;
                outwardChequeDetails.TellerName = transactionModel.TellerId;
                outwardChequeDetails.DrawalName = transactionModel.DrawalName;
                outwardChequeDetails.DebitValueDate = transactionModel.DebitValueDate;
                outwardChequeDetails.DateCreated = DateTime.Now;
                outwardChequeDetails.BankCode = transactionModel.BankCode;
                outwardChequeDetails.BranchCode = transactionModel.BranchCode;
                outwardChequeDetails.Approved = true;
                outwardChequeDetails.ApprovedBy = transactionModel.ApprovedBy;
                outwardChequeDetails.WhenApproved = DateTime.Now;
                outwardChequeDetails.TransactionReference = transactionModel.TransRef;
                outwardChequeDetails.CBAResponse = transactionModel.CBAResponse;
                outwardChequeDetails.CBAResponseCode = transactionModel.CBACode;
                 outwardChequeDetails.BranchCode = transactionModel.Branch;
                outwardChequeDetails.CurrencyCode = Convert.ToInt16(transactionModel.Currency);
                db.OutwardChequeDetails.Add(outwardChequeDetails);
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                Utils.LogNO("CreateAndApproveManagerChequeIssuance error: " + ex);
            }


        }


        public string BuildNarration(string SerialNo, string Beneficiary, string transRef, string remarks) //ok lets factor what MD said as per narration here
        {
            string narration = string.Empty;
            narration = @" " + transRef + @" CASH Deposit B/O " + @" " + Beneficiary  + @" " + remarks;
            return narration;
        }


    }
}
