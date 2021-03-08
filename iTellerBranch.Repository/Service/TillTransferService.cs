using iTellerBranch.Model;
using iTellerBranch.Repository.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Repository.Service
{
    public class TillTransferService : BaseService, ITillTransfer
    {
        public object GetTillRequest(string userId)
        {
            return db.TillTransfer.Where(x => x.GiverUser == userId && x.IsFufilled == false).Select(x => new
            {
                RequesterUserDetails = db.Users.Where(y => y.UserId == x.ReceiverUser).Select(y => new
                {
                    y.UserId,
                    y.UserName,
                    y.Id,
                    y.Email
                }),
                RequesterTillDetails = db.TillAssignment.Where(y => y.TillNo == x.ReceiverTillNo).Select(y => new
                {
                    y.TillID,
                    y.TillNo,
                    y.MaxAmount,
                    y.TillSetup.TillDesc,
                    y.TillSetup.GLAccountSetup.CashDenomination.Currency,
                    y.TillSetup.GLAccountSetup.GLAcctNo,
                    y.TillSetup.GLAccountSetup.GLAcctName, 
                }),
                x.TranId,
                x.Id,
                x.CashDenomination.Currency,
                x.CurrencyCode,
                x.Amount
            }).ToArray();
        }

        public object GetTillRequestForImal(string userId)
        { 
            return db.TillTransferIMAL.Where(x => x.CashierID == userId && x.IsFufilled == false).Select(x => new
            {
                RequesterUserDetails = db.Users.Where(y => y.UserId == x.ReceiverUser).Select(y => new
                {
                    y.UserId,
                    y.UserName,
                    y.Id,
                    y.Email
                }),
                RequesterTillDetails = db.TillAssignment.Where(y => y.TillNo == x.ReceiverTillNo).Select(y => new
                {
                    y.TillID,
                    y.TillNo,
                    y.MaxAmount,
                    y.TillSetup.TillDesc,
                    y.TillSetup.GLAccountSetup.CashDenomination.Currency,
                    y.TillSetup.GLAccountSetup.GLAcctNo,
                    y.TillSetup.GLAccountSetup.GLAcctName,
                }),
                x.TranId,
                x.Id,
                x.CashDenomination.Currency,
                x.CurrencyCode,
                x.Amount
            }).ToArray();
        }


        public object GetTillTransfer(string TillNo, bool success, string message = null, Exception innererror = null)
        {
            return new
            {
                success,
                message,
                innererror,
                TillRequest = db.TillTransfer.Where(x => x.ReceiverTillNo == TillNo).Select(x => new {
                    x.GiverTillNo,
                    x.GiverUser,
                    x.ReceiverTillNo,
                    x.Amount,
                    x.AmountReleased,
                    x.TransferDate,
                    x.Accepted,
                    x.IsFufilled,
                    x.Id,
                    x.ReceiverUser,
                    x.TransferMode,
                    x.Branch,
                    x.CashDenomination.Currency,
                    x.CashDenomination.Abbrev,
                    x.CashDenomination.ID,
                    x.CBA
                })

            };
        }


        public object GetTillTransferImal(string TillNo, bool success, string message = null, Exception innererror = null)
        {
            return db.TillTransferIMAL.Where(x => x.ReceiverUser == TillNo && x.Accepted == null).Select(x => new
            {
                x.GiverTillNo,
                x.GiverUser,
                x.ReceiverTillNo,
                x.Amount,
                x.AmountReleased,
                x.TransferDate,
                x.Accepted,
                x.IsFufilled,
                x.Id,
                x.ReceiverUser,
                x.TransferMode,
                x.Branch,
                x.CashDenomination.Currency,
                x.CashDenomination.Abbrev,
                x.CashDenomination.ID,
                x.CBA
            });
        }
        public object GetFufilledTillTransfer(string userId, bool success, string message = null, Exception innererror = null)
        {
            return new
            { 
                success,
                message,
                innererror,
                TillRequest = db.TillTransfer.Where(x => x.ReceiverUser == userId && x.Accepted == false).ToArray()
            };
        }


        public object AcceptTillTransfer(int id, int? transID=null)
        {
            try
            {
                var tillTransfer = db.TillTransfer.Find(id);
                if (tillTransfer == null)
                {
                    return new
                    {
                        success = false,
                        message = "till request does not exist"
                    };
                }
                tillTransfer.Accepted = true;
                tillTransfer.IsFufilled = true;
                tillTransfer.TranId = transID;
                tillTransfer.TransferDate = DateTime.Now;
                db.Entry(tillTransfer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var tillRequest = GetTillRequest(tillTransfer.ReceiverUser);
                return GetFufilledTillTransfer(tillTransfer.ReceiverUser, true);
            }
            catch (Exception ex)
            {
                return new
                {
                    success = false,
                    message = ex.Message,
                    innerError = ex
                };
            }

        }

        public bool RejectImalTillTransfer(int id, int? transID = null)
        {
            try
            {
                var tillTransfer = db.TillTransferIMAL.Find(id);
                if (tillTransfer == null)
                {
                    Utils.LogNO("Error on Rejecting: Null tillTransfer");
                    return false;
                }
                tillTransfer.Accepted = false;
                tillTransfer.IsFufilled = true;
                tillTransfer.TranId = transID;
                tillTransfer.TransferDate = DateTime.Now;
                db.Entry(tillTransfer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Utils.LogNO("Error on Rejecting: " + ex.Message);
                return false;
            }

        }


        public bool AcceptImalTillTransfer(int id, int? transID = null)
        {
            try
            {
                var tillTransfer = db.TillTransferIMAL.Find(id);
                if (tillTransfer == null)
                {
                    Utils.LogNO("tillTransfer details is null");
                    return false;
                }
                tillTransfer.Accepted = true;
                tillTransfer.IsFufilled = true;
                tillTransfer.TranId = transID;
                tillTransfer.TransferDate = DateTime.Now;
                db.Entry(tillTransfer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Utils.LogNO("Error on till transfer: " + ex.Message);
                return false;
            }

        }


        public object RejectTillTransfer(int id, int? transID = null) 
        {
            try
            {
                var tillTransfer = db.TillTransfer.Find(id);
                if (tillTransfer == null)
                {
                    return new
                    {
                        success = false,
                        message = "till request does not exist"
                    };
                }
                tillTransfer.Accepted = false;
                tillTransfer.IsFufilled = true;
                tillTransfer.TranId = transID;
                tillTransfer.TransferDate = DateTime.Now;
                db.Entry(tillTransfer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var tillRequest = GetTillRequest(tillTransfer.ReceiverUser);
                return GetFufilledTillTransfer(tillTransfer.ReceiverUser, true);
            }
            catch (Exception ex)
            {
                return new
                {
                    success = false,
                    message = ex.Message,
                    innerError = ex
                };
            }

        }


        public object RequestTill(TillTransfer tillTransfer)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int result = 0;
                    Utils.LogNO(JsonConvert.SerializeObject(tillTransfer));
                        var param = new SqlParameter[]
                        {

                           new SqlParameter("@ReceiverTillNo", tillTransfer.ReceiverTillNo), 
                           new SqlParameter("@ReceiverUser", tillTransfer.ReceiverUser),
                           new SqlParameter("@GiverTillNo", tillTransfer.GiverTillNo),
                           new SqlParameter("@GiverUser", tillTransfer.GiverUser),
                           new SqlParameter("@CurrencyCode", tillTransfer.CurrencyCode),
                           new SqlParameter("@Amount", tillTransfer.Amount),
                           new SqlParameter("@CashierID", tillTransfer.CashierID),
                           new SqlParameter("@TransferMode", tillTransfer.TransferMode),
                           new SqlParameter("@Branch", tillTransfer.Branch),
                           new SqlParameter("@CBA", tillTransfer.CBA),

                        };
                    if(tillTransfer.CBA == "T24")
                    {
                        result = db.Database.SqlQuery<int>("dbo.TillManagement_RequestTill @ReceiverTillNo,@ReceiverUser,@GiverTillNo,@GiverUser,@CurrencyCode,@Amount,@CashierID,@TransferMode,@Branch,@CBA", param)
                          .FirstOrDefault();
                    }
                    else
                    {
                        result = db.Database.SqlQuery<int>("dbo.TillManagement_RequestTillImal @ReceiverTillNo,@ReceiverUser,@GiverTillNo,@GiverUser,@CurrencyCode,@Amount,@CashierID,@TransferMode,@Branch,@CBA", param)
                         .FirstOrDefault();
                    }
                      
                        transaction.Commit();
                    return new { success= true, message="Till Transfer requested successfully"};
                }
                catch (Exception ex)
                {
                    Utils.LogNO("Error on till transfer: " + ex.Message);
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
     

        public object TransferTill(int id, decimal amount)
        {
            try
            {
                var tillTransfer = db.TillTransfer.Find(id);
                if(tillTransfer == null)
                {
                    return new
                    {
                        success = false,
                        message = "till request does not exist"
                    };
                }
                tillTransfer.AmountReleased = amount;
                tillTransfer.IsFufilled = true;
                tillTransfer.TransferDate = DateTime.Now;
                db.Entry(tillTransfer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var tillRequest = GetTillRequest(tillTransfer.ReceiverUser);
                return GetTillTransfer(tillTransfer.GiverUser,true);
            }
            catch (Exception ex)
            {
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
