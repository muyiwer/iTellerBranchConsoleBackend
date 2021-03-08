using iTellerBranch.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTellerBranch.Model.ViewModel;
using System.Data.Entity.Core.Objects;
using iTellerBranch.Model;
using Newtonsoft.Json;

namespace iTellerBranch.Repository.Service
{
    public class TillService:BaseService, ITillService
    {
        public object GetTill(bool success, string message, Exception ex = null)
        {
            return new
            {
                success,
                message,
                innerError = ex,
                data = new
                {
                    TillSetup = db.TillSetup.Select(x => new
                    {
                        x.ID,
                        x.TillNo,
                        x.TillDesc,
                        x.GLAccountSetup.GLAcctNo,
                        x.GLAccountSetup.GLAcctName,
                        x.GLAccountSetup.CashDenomination.Currency,
                        x.GLAccountID,
                        x.MinTillAmount,
                        x.MaxTillAmount,
                        x.Status
                    }).ToArray(),
                    GLAccount = db.GLAccountSetup.Select(x=> new
                    {
                        x.ID,
                        x.GLAcctName,
                        x.GLAcctNo,
                        x.CashDenomination.Currency
                    }),
                    User = db.Users.Select(y=> new
                    {
                        y.Id,
                        y.UserId,
                        y.UserName,
                        y.Email
                    }).ToArray()
                }

            };
        }
                

        public object CreateTill(TillSetup Till)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int result = 0;
                    var validateTillMessage = ValidateTill(Till.TillNo, Till.GLAccountID);
                    if (validateTillMessage == null)
                    {
                        var param = new SqlParameter[]
                        {
                         
                           new SqlParameter("@TillNo", Till.TillNo),
                           new SqlParameter("@TillDesc", Till.TillDesc),
                           new SqlParameter("@GLAcctountID", Till.GLAccountID),
                           new SqlParameter("@IsVault", Till.IsVault),
                           new SqlParameter("@Sortcode", Till.SortCode),
                           new SqlParameter("@MinTillAmount", Till.MinTillAmount),
                           new SqlParameter("@MaxTillAmount", Till.MaxTillAmount),
                           new SqlParameter("@Status", Till.Status)
                        };
                        result = db.Database.SqlQuery<int>("dbo.TillSetup_CreateTill @TillNo,@TillDesc,@GLAcctountID,@IsVault,@Sortcode" +
                                ",@MinTillAmount,@MaxTillAmount,@Status", param)
                            .FirstOrDefault();
                        transaction.Commit();
                        return GetTill(true, "Till created successfully");
                    }
                    else
                    {
                        return GetTill(false, validateTillMessage);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }


        public object UpdateTill(TillSetup Till)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int result = 0;
                    if(IsGLAccountAssigned(Till.GLAccountID, Till.ID))
                    {
                        return GetTill(false, "GL Account already assigned to another till");
                    }
                    if (IsTillExist(Till.ID))
                    {
                        if (string.IsNullOrEmpty(Till.TillNo))
                        {
                            return GetTill(false, "Till Number is expected for this operation!");
                        }
                        if (Till.Status==null)
                        {
                            return GetTill(false, "Till Status is expected for this operation!");
                        }
                        var param = new SqlParameter[]
                        {
                           new SqlParameter("@ID", Till.ID),
                           new SqlParameter("@TillNo", Till.TillNo),
                           new SqlParameter("@TillDesc", Till.TillDesc),
                           new SqlParameter("@GLAccountID", Till.GLAccountID),
                           new SqlParameter("@IsVault", Till.IsVault),
                           new SqlParameter("@Sortcode", Till.SortCode),
                           new SqlParameter("@MinTillAmount", Till.MinTillAmount),
                           new SqlParameter("@MaxTillAmount", Till.MaxTillAmount),
                           new SqlParameter("@Status", Till.Status)
                        };
                        try
                        {
                            // hhow did this work? i didnt see d sp how come. I have seen. it is calling the create. the controller or business is ref create
                            result = db.Database.SqlQuery<int>("dbo.TillSetup_UpdateTill @ID, @TillNo,@TillDesc,@GLAccountID,@IsVault,@Sortcode,@MinTillAmount,@MaxTillAmount,@Status", param)
                          .FirstOrDefault();
                            transaction.Commit();
                        }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                        catch (SqlException ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                        {
                            throw new Exception("Something is wrong with user input. check the input and try again!");
                        }
                        return GetTill(true, "Till updated successfully");

                    }
                    else
                    { 
                   
                        return GetTill(false, "TILL does not exist");
                    }
                }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                {
                    transaction.Rollback();
                    throw new Exception("Something is wrong with user input. check the input and try again!");
                }
            }
        }

        public object GetUserTill(bool success, string message, Exception ex = null)
        {
            return new
            {
                success,
                message,
                innerError = ex,
                data = db.TillAssignment.Select(x=> new
                {
                    x.Id,
                    x.Users.UserId,
                    x.Users.UserName,
                    x.Users.Email,
                    x.TillID,
                    x.TillSetup.TillNo,
                    x.TillSetup.TillDesc,
                    x.MaxAmount,
                    x.EffectiveDate,
                    x.TillSetup.GLAccountSetup.GLAcctName,
                    x.TillSetup.GLAccountSetup.GLAcctNo,
                    x.TillSetup.GLAccountSetup.CashDenomination.Currency
                }).ToArray()

            };
        }

        public object GetGiverTill()
        {
            var dt = DateTime.Now.Date;
            return
                db.TillAssignment.Where(x=>x.EffectiveDate == dt).Select(x => new
                {
                    x.Id,
                    x.Users.UserId,
                    x.Users.UserName,
                    x.Users.Email,
                    x.TillID,
                    x.TillSetup.TillNo,
                    x.TillSetup.TillDesc,
                    x.MaxAmount,
                    x.EffectiveDate,
                    x.TillSetup.GLAccountSetup.GLAcctName,
                    x.TillSetup.GLAccountSetup.GLAcctNo,
                    x.TillSetup.GLAccountSetup.CashDenomination.Currency
                }).ToArray();
        }

        public object AssignTill(TillAssignment tillAssignment)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int result = 0;
                    var validateTillAssignmentErrorMessage = ValidateTillAsignment(tillAssignment.TillID, tillAssignment.UserID);
                    if(tillAssignment.Id == 0)
                    {
                        if (validateTillAssignmentErrorMessage == null)
                        {
                            var param = new SqlParameter[]
                            {

                           new SqlParameter("@TillID", tillAssignment.TillID),
                           new SqlParameter("@TillNo", tillAssignment.TillNo), // frontend did not supply d value ok
                           new SqlParameter("@UserID", tillAssignment.UserID),
                           new SqlParameter("@MaxAmount", tillAssignment.MaxAmount)
                            };
                            result = db.Database.SqlQuery<int>("dbo.TillAssignment_Create @TillID, @TillNo,@UserID,@MaxAmount", param)
                                .FirstOrDefault();
                            transaction.Commit();
                            return GetUserTill(true, "Till assigned successfully");

                        }
                        else
                        {
                            return GetUserTill(false, validateTillAssignmentErrorMessage);
                        }
                    }
                    else
                    {
                        TillAssignment tillAssignmentDb = TillAssignment(tillAssignment.Id);
                        if (tillAssignment.UserID == tillAssignmentDb.UserID || 
                            tillAssignment.TillID == tillAssignmentDb.TillID)
                        {
                            result = SaveTillAssignmentToDB(tillAssignment);
                            transaction.Commit();
                            return GetUserTill(true, "Till assignd successfully");
                        }
                        else
                        {
                           
                            if (validateTillAssignmentErrorMessage == null)
                            {
                                result = SaveTillAssignmentToDB(tillAssignment);
                                transaction.Commit();
                                return GetUserTill(true, "Till assignd successfully");
                            }
                            else
                            {
                                return GetUserTill(false, validateTillAssignmentErrorMessage);
                            }
                        }
                       
                    }

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public object DeleteTillAssignment(List<int> ID)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    //TODO: To be done later when theres less pressure
                    //string Ids = string.Join(",", ID);
                    //var param = new SqlParameter[]
                    //{
                    //    new SqlParameter("@Ids", Ids)
                    //};
                    //result = db.Database.SqlQuery<int>("dbo.TillAssignment_Delete @Ids", param)
                    //    .FirstOrDefault();
                    
                    foreach(int id in ID)
                    {
                       var tillAssignment = db.TillAssignment.Find(id);
                        if(tillAssignment != null)
                        {
                            TillAssignmentHistory tillAssignmentHistory = new TillAssignmentHistory();
                            tillAssignmentHistory.TillID = tillAssignment.TillID;
                            tillAssignmentHistory.UserId = tillAssignment.UserID;
                            db.TillAssignmentHistory.Add(tillAssignmentHistory);
                            db.TillAssignment.Remove(tillAssignment);
                        }
                    }
                    db.SaveChanges();
                    transaction.Commit();
                    return GetUserTill(true, "Assigned Till deleted successfully");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public object DeleteTill(List<int> ID)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    //TODO: To be done later when theres less pressure
                    //string Ids = string.Join(",", ID);
                    //var param = new SqlParameter[]
                    //{
                    //    new SqlParameter("@Ids", Ids)
                    //};
                    //var result = db.Database.SqlQuery<int>("dbo.TillSetup_Delete @Ids", param)
                    //    .FirstOrDefault();

                    foreach (int id in ID)
                    {
                        var till = db.TillSetup.Find(id);
                        if (till != null)
                        {
                            var tillAssignment = db.TillAssignment.Where(t=>t.TillID==till.ID).FirstOrDefault();

                            if(tillAssignment == null)
                            {
                                TillSetupDeleted tillSetupDel = new TillSetupDeleted();
                                tillSetupDel.ID = till.ID;
                                tillSetupDel.TillNos = till.TillNo;
                                tillSetupDel.TillDesc = till.TillDesc;
                                tillSetupDel.GLAcctName = till.GLAcctName;
                                tillSetupDel.GLAcctNo = till.GLAcctNo;
                                tillSetupDel.IsVault = till.IsVault == true ? true : false;
                                tillSetupDel.GLAcctCurrency = till.GLAcctCurrency;
                                //tillSetupDel.WhoDeleted = till ;//We must find a way of getting user who logs in and performs this op

                                db.TillSetupDeleted.Add(tillSetupDel);
                                db.TillSetup.Remove(till);

                                db.SaveChanges();
                                transaction.Commit();
                                return GetUserTill(true, "Till deleted successfully!");
                            }
                            else
                            {
                                return GetUserTill(false, "Till: "+ tillAssignment.TillNo+" is already assigned for operation and cannot be deleted!");
                            }
                        
                        }
                    }
                    return GetUserTill(false, "Till is already assigned for operation and cannot be deleted!");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }


        public decimal GetCashAtHand(string tellerId, int currencyId)
        {
            decimal CashAtHand = 0m;

            var cashTrans = GetTillTransactions(tellerId, currencyId).ToString();
            var dcashTrans = JsonConvert.DeserializeObject<CashDetails>(cashTrans);
            // cashTrans.CashBroughForward + cashReceivedFromVault + CashReceivedFromCcustomer + TillTransferIn - (cashToVault + CashToCustomer + TillTransferOut);
                                    //
            CashAtHand = dcashTrans.CashBroughtForward + dcashTrans.CashReceivedFromVault + dcashTrans.CashRecievedFromCustomers +
                dcashTrans.TillTransferIn - (dcashTrans.CashTransferedToVault + dcashTrans.CashPaidToCustomers + dcashTrans.TillTransferOut);

            return CashAtHand;
        }

        public object GetIMALTillTransactions(TillBalanceModel tillModel)
        {
            DateTime dt = DateTime.Now.Date;
            try
            {
                Utils.LogNO("Till Search Details" + JsonConvert.SerializeObject(tillModel));
                var cashTransactionIn = db.TransactionsMaster.Where(x => x.TellerId == tillModel.TellerId
                && DbFunctions.TruncateTime(x.CreationDate).Value == dt
                                        && x.Currency == tillModel.CurrencyId && x.IsReversed == false && x.ReversedTranId == 0
                                        && x.Approved == true && x.CBA == "IMAL" 
                                        || x.TellerId == tillModel.FromTeller
                                        && DbFunctions.TruncateTime(x.CreationDate).Value == dt
                                        && x.Currency == tillModel.CurrencyId && x.IsReversed == false && x.ReversedTranId == 0
                                        && x.Approved == true && x.CBA == "IMAL" 
                                        || x.TellerId == tillModel.ToTeller 
                                        && DbFunctions.TruncateTime(x.CreationDate).Value == dt
                                        && x.Currency == tillModel.CurrencyId && x.IsReversed == false && x.ReversedTranId == 0
                                        && x.Approved == true && x.CBA == "IMAL"
                                        ).ToList();
                var cashTransactionOut = db.TransactionsMaster.Where(x => x.ToTellerId == tillModel.TellerId
                                         && DbFunctions.TruncateTime(x.CreationDate).Value == dt
                                        && x.Currency == tillModel.CurrencyId && x.IsReversed == false && x.ReversedTranId == 0
                                        && x.Approved == true && x.CBA == "IMAL"
                                        || x.ToTellerId == tillModel.FromTeller
                                         && DbFunctions.TruncateTime(x.CreationDate).Value == dt
                                        && x.Currency == tillModel.CurrencyId && x.IsReversed == false && x.ReversedTranId == 0
                                        && x.Approved == true && x.CBA == "IMAL"
                                        || x.ToTellerId == tillModel.ToTeller
                                        && DbFunctions.TruncateTime(x.CreationDate).Value == dt
                                        && x.Currency == tillModel.CurrencyId && x.IsReversed == false && x.ReversedTranId == 0
                                        && x.Approved == true && x.CBA == "IMAL").ToList();
                var cashierBalancing = db.CashierBalancing.Where(x => x.TellerId == tillModel.TellerId
                                        || x.TellerId == tillModel.FromTeller || x.TellerId == tillModel.ToTeller
                                         && DbFunctions.TruncateTime(x.CheckingTime).Value < dt && x.Finalize == true)
                                        .OrderByDescending(x => x.CheckingTime).FirstOrDefault();

                return new
                {//receive from vault is vault out/ out of d four which category will cheque clearing be?. 
                 //Vault out-- cash recieved from vault?
                 //CashRecievedFromVault = cashTransaction.Where(x => x.TransType == Convert.ToInt16(TransType.TransactionType.VaultOut)
                 //                                && x.TellerId == tellerId && x.CrDr == "D")
                 //                                .Select(x => x.Amount).Sum(),

                    //THIS PART WILL BE WORKED UPON WITH ED CashBroughtForward CashRecievedFromCustomers CashTransferedToVault CashReceivedFromVault CashPaidToCustomersTillTransferInTillTransferOut
                    CashBroughtForward = cashierBalancing == null ? 0 : cashierBalancing.CashAtHand,

                    CashRecievedFromCustomers = cashTransactionIn.Where(x => x.TransType ==
                                        Convert.ToInt16(TransType.TransactionType.Deposit))
                                            .Select(x => x.TotalAmount).Sum(),


                    CashTransferedToVault = cashTransactionIn.Where(x => x.TransType == Convert.ToInt16(TransType.TransactionType.vaultIn))
                                                    .Select(x => x.TotalAmount).Sum(),

                    CashReceivedFromVault = cashTransactionIn.Where(x => x.TransType == Convert.ToInt16(TransType.TransactionType.VaultOut))
                                                    .Select(x => x.TotalAmount).Sum(),


                    CashPaidToCustomers = cashTransactionIn.Where(x => x.TransType ==
                                        Convert.ToInt16(TransType.TransactionType.Withdrawal)
                                      || x.TransType == Convert.ToInt16(TransType.TransactionType.ChequeLodgement))
                                            .Select(x => x.TotalAmount).Sum(),

                    TillTransferIn = cashTransactionOut.Where(x => x.TransType ==
                                    Convert.ToInt16(TransType.TransactionType.TillTransfer))
                                            .Select(x => x.TotalAmount).Sum(),

                    TillTransferOut = cashTransactionIn.Where(x => x.TransType ==
                                                  Convert.ToInt16(TransType.TransactionType.TillTransfer))
                                            .Select(x => x.TotalAmount).Sum(), // no need to check for null fok



                    // ExpectedBalance = Totalfromvault + depositfromCustomers + TilltransferIn - (VaulIn + CashPaidToCustomers + TillTransferOut)
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public object GetTillTransactions(string tellerId, int currencyId)
        {
            DateTime dt = DateTime.Now.Date;
            try
            { 
                var cashTransactionIn = db.TransactionsMaster.Where(x => x.TellerId==tellerId 
                                        && DbFunctions.TruncateTime(x.CreationDate).Value == dt 
                                        && x.Currency == currencyId && x.IsReversed ==false && x.ReversedTranId ==0 
                                        && x.Approved==true).ToList();

                var cashTransactionOut = db.TransactionsMaster.Where(x => x.ToTellerId == tellerId
                                        && DbFunctions.TruncateTime(x.CreationDate).Value == dt
                                        && x.Currency == currencyId && x.IsReversed == false && x.ReversedTranId == 0 
                                        && x.Approved == true).ToList();
                var cashierBalancing = db.CashierBalancing.Where(x => x.TellerId == tellerId &&
                                        DbFunctions.TruncateTime(x.CheckingTime).Value < dt && x.Finalize == true)
                                        .OrderByDescending(x => x.CheckingTime).FirstOrDefault();

                return new
                {//receive from vault is vault out/ out of d four which category will cheque clearing be?. 
                 //Vault out-- cash recieved from vault?
                 //CashRecievedFromVault = cashTransaction.Where(x => x.TransType == Convert.ToInt16(TransType.TransactionType.VaultOut)
                 //                                && x.TellerId == tellerId && x.CrDr == "D")
                 //                                .Select(x => x.Amount).Sum(),

                    //THIS PART WILL BE WORKED UPON WITH ED CashBroughtForward CashRecievedFromCustomers CashTransferedToVault CashReceivedFromVault CashPaidToCustomersTillTransferInTillTransferOut
                    CashBroughtForward = cashierBalancing == null ? 0 : cashierBalancing.CashAtHand,

                    CashRecievedFromCustomers = cashTransactionIn.Where(x => x.TransType ==
                                        Convert.ToInt16(TransType.TransactionType.Deposit))
                                            .Select(x => x.TotalAmount).Sum(),


                    CashTransferedToVault = cashTransactionIn.Where(x => x.TransType == Convert.ToInt16(TransType.TransactionType.vaultIn))
                                                    .Select(x => x.TotalAmount).Sum(),

                    CashReceivedFromVault = cashTransactionIn.Where(x => x.TransType == Convert.ToInt16(TransType.TransactionType.VaultOut))
                                                    .Select(x => x.TotalAmount).Sum(),


                    CashPaidToCustomers = cashTransactionIn.Where(x => x.TransType ==
                                        Convert.ToInt16(TransType.TransactionType.Withdrawal) 
                                      || x.TransType == Convert.ToInt16(TransType.TransactionType.ChequeLodgement))
                                            .Select(x => x.TotalAmount).Sum(),

                    TillTransferIn = cashTransactionOut.Where(x => x.TransType ==
                                    Convert.ToInt16(TransType.TransactionType.TillTransfer))
                                            .Select(x => x.TotalAmount).Sum(),

                    TillTransferOut = cashTransactionIn.Where(x => x.TransType ==
                                                  Convert.ToInt16(TransType.TransactionType.TillTransfer))
                                            .Select(x => x.TotalAmount).Sum(), // no need to check for null fok

                  

                    // ExpectedBalance = Totalfromvault + depositfromCustomers + TilltransferIn - (VaulIn + CashPaidToCustomers + TillTransferOut)
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public decimal? ShortageOverage(string tellerId, decimal? outstandingBalance)
        {
            DateTime dt = DateTime.Now.Date;
            var cashTransaction = db.TransactionsMaster.Where(x => x.TellerId == tellerId &&
            DbFunctions.TruncateTime(x.CreationDate).Value == dt).ToList();// test

            decimal overageShortage = 0m;
           // var CashRecievedFromVault = cashTransaction.Where(x => x.TransType == Convert.ToInt16(TransType.TransactionType.VaultOut)
           //                                  )
           //                                   .Select(x => x.Amount).Sum();



            // var CashRecievedFromCustomers = cashTransaction.Where(x => x.TransType ==
            //                      Convert.ToInt16(TransType.TransactionType.Deposit) && x.TellerId == tellerId && x.CrDr == "D")
            //                          .Select(x => x.Amount).Sum();


            // var CashTransferedToVault = cashTransaction.Where(x => x.TransType == Convert.ToInt16(TransType.TransactionType.vaultIn)
            //                                     && x.TellerId == tellerId && x.CrDr == "C")
            //                                     .Select(x => x.Amount).Sum();


            // var CashPaidToCustomers = cashTransaction.Where(x => x.TransType ==
            //                        Convert.ToInt16(TransType.TransactionType.Withdrawal) && x.TellerId == tellerId && x.CrDr == "C")
            //                            .Select(x => x.Amount).Sum();

            // var TillTransferIn = cashTransaction.Where(x => x.TransType ==
            //                    Convert.ToInt16(TransType.TransactionType.TillTransfer) && x.TellerId == tellerId && x.CrDr == "D")
            //                            .Select(x => x.Amount).Sum();

            //var TillTransferOut = cashTransaction.Where(x => x.TransType ==
            //                                   Convert.ToInt16(TransType.TransactionType.TillTransfer) && x.TellerId == tellerId && x.CrDr == "C")
            //                             .Select(x => x.Amount).Sum();

            //var credit =CashRecievedFromVault - (CashRecievedFromCustomers + TillTransferIn);
            //var debit = outstandingBalance - (CashPaidToCustomers + TillTransferOut);
            //var overageShortage = credit - debit;

            return overageShortage;
            //return outstandingBalance - (CashRecievedFromVault + CashRecievedFromCustomers) - (CashTransferedToVault + CashPaidToCustomers);
        }


        private int SaveTillAssignmentToDB(TillAssignment tillAssignment)
        {
            int result;
            var param = new SqlParameter[]
              {
                    new SqlParameter("@Id", tillAssignment.Id),
                    new SqlParameter("@TillID", tillAssignment.TillID),
                    new SqlParameter("@UserID", tillAssignment.UserID),
                    new SqlParameter("@MaxAmount", tillAssignment.MaxAmount)
              };
                    result = db.Database.SqlQuery<int>("dbo.TillAssignment_Update @Id,@TillID,@UserID,@MaxAmount", param)
                        .FirstOrDefault();
            return result;
        }

      
        private string ValidateTill(string TillNo, int? glAccountID)
        {
            var till = db.TillSetup.ToList();
            string result = till.Where(x => x.TillNo == TillNo)
                 .ToList().Count() > 0 ? "Till already exist" : null;
            if(result == null)
            {
                result = till.Where(x => x.GLAccountID == glAccountID).ToList().Any() ? "GL Account already assigned to a till" : null;
            }
            return result;
        }

        private bool IsTillExist(int ID)
        {
            return db.TillSetup.Find(ID)
                 != null ? true : false;
        }

        private string ValidateTillAsignment(int? tillID, string userID)  
        {
            var today = DateTime.Now.Date;
            var currencyId = db.TillSetup.Find(tillID).GLAccountSetup.CurrencyID;
            var tillAssignment = db.TillAssignment.ToList();
            //var isUserAssignedTillCurrency = tillAssignment.Where(x => x.UserID == userID && x.DateCreated.Date == today
            //&& x.TillSetup.GLAccountSetup.CurrencyID == currencyId).Any() ? true : false;
            //if (!isUserAssignedTillCurrency) { return null; };
             var result = tillAssignment.Where(x => x.TillID == tillID && x.UserID == userID && x.EffectiveDate.Date == today
                                                && x.TillSetup.GLAccountSetup.CurrencyID == currencyId)
                 .ToList().Any() ? "Till already assigned to a user" : null;
            if(result == null)
            {
                result = tillAssignment.Where(x=> x.UserID== userID &&  x.EffectiveDate.Date == today &&
                                         x.TillSetup.GLAccountSetup.CurrencyID == currencyId)
                 .ToList().Any() ? "User already assigned a till" : null;
            }
            return result;
        }

        private TillAssignment TillAssignment(int ID) 
        {
            return db.TillAssignment.Find(ID); 
        }

        private bool IsGLAccountAssigned(int? glAccountID, int? tillID)
        {
            bool result = false;
            var tills = db.TillSetup.ToList();
            var till = tills.Where(x=> x.ID == tillID).FirstOrDefault();
            if(till.GLAccountID != glAccountID)
            {
               result = tills.Where(x => x.GLAccountID == glAccountID).Count() > 0 ? true : false;
               return result;
            }
            else
            {
                return false;
            }
        }
        public bool? GetCheckedTillBalance(string tellerId)
        {
            DateTime dt = DateTime.Now.Date;
            
            return db.TillManagement.Where(x => x.TellerId == tellerId && DbFunctions.TruncateTime(x.DateCreated).Value == dt)
                .Select(x => x.ShortageOverageAmount).FirstOrDefault()>0 ? false:true;

        }
        public object GetTillApproval(bool success, string message, Exception ex = null)
        {
            
            return new
            {
                success,
                message,
                innerError = ex,
                TillApprovalDetails = db.TillManagement.Where(x => x.Approve == null).ToList()
            };
        }

        public object ApproveTill(TillManagement tillManagement)
        {
            try
            {
                var tillClosure = db.TillManagement.Find(tillManagement.Id);
                if (tillClosure != null)
                {
                    tillClosure.Approve = true;
                    db.Entry(tillClosure).State = EntityState.Modified;
                    db.SaveChanges();
                    return GetTillApproval(true, "Approved successfully");
                }
                else
                {
                    return GetTillApproval(false, "Till does not exist");
                }
            }
            catch (Exception ex)
            {
                return GetTillApproval(false, "Server Error", ex); throw;
            }
          
        }

        public object DisapproveTill(TillManagement tillManagement)
        {
            try
            {
                var tillClosure = db.TillManagement.Find(tillManagement.Id);
                if (tillClosure != null)
                {
                    tillClosure.Approve = false;
                    db.Entry(tillClosure).State = EntityState.Modified;
                    db.SaveChanges();
                    return GetTillApproval(true, "Approved successfully");
                }
                else
                {
                    return GetTillApproval(false, "Till does not exist");
                }
            }
            catch (Exception ex)
            {
                return GetTillApproval(false, "Server Error", ex); throw;
            }
        }

        public object OpenTill(TillManagement tillManagement)
        {
            try
            {
                db.TillManagement.Add(tillManagement);
                db.SaveChanges();
               return GetTillApproval(true, "Till sent for approval successfully");
            }
            catch (Exception ex)
            {
               return GetTillApproval(false, "Server Error", ex);
            }
          
        }

        public object CloseTill(TillAssignmentModel tillManagementModel)
        {
            try
            {
                tillManagementModel.DateCreated = DateTime.Now;
                tillManagementModel.IsClosed = true;
                tillManagementModel.Approve = true;
                TillManagement tillManagement = new TillManagement();
                tillManagement.Approve = tillManagementModel.Approve;
                tillManagement.ApprovedBy = tillManagementModel.User;
                tillManagement.CBAResponse = tillManagementModel.CBAResponse;
                tillManagement.Comments = tillManagementModel.Comments;
                tillManagement.IsClosed = tillManagementModel.IsClosed;
                tillManagement.IsHeadTeller = tillManagementModel.IsHeadTeller;
                tillManagement.Event = tillManagementModel.Event;
                tillManagement.Status = tillManagementModel.Status;
                tillManagement.TellerId = tillManagementModel.TellerId;
                tillManagement.TransactionBranch = tillManagementModel.TransactionBranch;
                tillManagement.User = tillManagementModel.User;

                // tillManagement.ShortageOverageAmount = ShortageOverage(tillManagement.TellerId, tillManagement.ClosingBalance);
                db.TillManagement.Add(tillManagement);
                db.SaveChanges();
                return GetTillApproval(true, "Till closure sent for approval successfully");
                
            }
            catch (Exception ex)
            {
                return GetTillApproval(false, "Server Error", ex);
            }
        }

        public object ConfirmTillBalance(TillManagement tillManagement) 
        {
            try
            {
                tillManagement.DateCreated = DateTime.Now;
                tillManagement.IsClosed = null;
                tillManagement.Approve = true;
                tillManagement.ApprovedDate = DateTime.Now;
                tillManagement.TransactionBranch = tillManagement.TransactionBranch;
                tillManagement.ShortageOverageAmount = tillManagement.ShortageOverageAmount;

                CashierBalancing cashierBalancing = new CashierBalancing();
                cashierBalancing.CashAtHand = tillManagement.ClosingBalance;
                cashierBalancing.ExpectedCashAtHand = tillManagement.ShortageOverageAmount;
                cashierBalancing.CheckingTime = DateTime.Now;
                cashierBalancing.Finalize = true;
                cashierBalancing.TellerId = tillManagement.TellerId;
                cashierBalancing.TillId = tillManagement.TellerId;
               
                if (!ValidateTillBalanceExist(tillManagement.TellerId))
                {
                    db.CashierBalancing.Add(cashierBalancing);
                    db.TillManagement.Add(tillManagement);
                    db.SaveChanges();
                    return GetTillApproval(true, "Till balance confirmed successfully");
                }
                else
                {
                    return GetTillApproval(false, "Till balance already confirmed");
                }

            }
            catch (Exception ex)
            {
                return GetTillApproval(false, "Server Error", ex);
            }
        }

        public TillManagement GetTillManagement(int id)
        {
           return  db.TillManagement.Find(id);
        }
        public bool IsTillClosureExist(int id)
        {
            return db.TillManagement.Find(id) != null ? true : false;
        }

        private bool ValidateTillClosure(string tellerId)
        {
           return db.TillManagement.Where(x => x.TellerId == tellerId && x.Approve == null && x.IsClosed == true).Any() ? true : false;
        }

        private bool ValidateTillBalanceExist(string tellerId) 
        {
            DateTime dt = DateTime.Now.Date;
            return db.CashierBalancing.Where(x => x.TellerId == tellerId && DbFunctions.TruncateTime(x.CheckingTime).Value == dt).Count() > 0 ? true : false;
        }
    }
}
