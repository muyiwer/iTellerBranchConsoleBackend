using iTellerBranch.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Repository.Service
{

    public class UserService:BaseService, IUserService
    {

        public UserTransactionPageAccess GetUpdateUserTransactionPageAccess(int? tranId)
        {
            return db.UserTransactionPageAccess.Where(x => x.TranId == tranId)
                                               .FirstOrDefault();
        }

        public void UpdateUserTransactionPageAccess(UserTransactionPageAccess request)
        {
            var userTransactionPageAccess = db.UserTransactionPageAccess.Where(x =>  x.TranId == request.TranId && x.UserId == request.UserId)
                                                .FirstOrDefault();
            if(userTransactionPageAccess == null)
            {
                UserTransactionPageAccess newRequest = new UserTransactionPageAccess();
                newRequest.UserId = request.UserId;
                newRequest.TranId = request.TranId;
                newRequest.DateAccessed = DateTime.Now;
                newRequest.IsActiveOnPage = true;
                db.UserTransactionPageAccess.Add(newRequest);
            }
            else
            {
                userTransactionPageAccess.IsActiveOnPage = true;
                userTransactionPageAccess.DateAccessed = DateTime.Now;
                db.Entry(userTransactionPageAccess).State = EntityState.Modified;

                db.UserTransactionPageAccess
                                    .Where(x => x.TranId == request.TranId && x.UserId != request.UserId).ToList()
                                    .ForEach(a => a.IsActiveOnPage = false);
            }
            db.SaveChanges();
        }

        public bool IsUserActiveOnTransactionPage(UserTransactionPageAccess request)
        {
            var userTransactionPageAccess = db.UserTransactionPageAccess.Where(x => x.UserId == request.UserId && x.TranId == request.TranId)
                                               .FirstOrDefault();
            if (userTransactionPageAccess == null)
            {
                return false;
            }
            else
            {
                return Convert.ToBoolean(userTransactionPageAccess.IsActiveOnPage);
            }
        }

        public void UpdateUserTransactionPageAccess(int? id)
        {
            var userTransactionPageAccess = db.UserTransactionPageAccess.Where(x => x.Id == id)
                                                .FirstOrDefault();
            if (userTransactionPageAccess != null)
            {
                userTransactionPageAccess.IsActiveOnPage = false;
                userTransactionPageAccess.DateAccessed = DateTime.Now;
                db.Entry(userTransactionPageAccess).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public bool CheckIfUseIsStillActiveOnTransactionPage(int? tranId, double minute)
        {
            var userTransactionPageAccess = db.UserTransactionPageAccess.Where(x => x.TranId == tranId).ToList();
            if(!userTransactionPageAccess.Any(x=> x.IsActiveOnPage == true))
            {
                return false;
            }
            else
            {
               var userTrans = userTransactionPageAccess.Where(x => x.IsActiveOnPage == true).FirstOrDefault();
                DateTime dt = DateTime.Now;
                var authorizerDateAccessed = userTrans.DateAccessed.Value.AddMinutes(minute);
                int result = DateTime.Compare(dt, authorizerDateAccessed);
                if(result <= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }



        public object GetAllUsers(bool success, string message, Exception ex = null)
        {
            return new
            {
                success,
                message,
                innerError = ex,
                data = new
                {                   
                    UserDetail = db.Users.Select(x => new
                    {
                        x.Id,
                        x.UserId,
                        x.UserName,
                        x.Email,
                        x.Active,
                        x.BankCode,
                        x.CreationDate,
                        x.DateLastModified,
                        x.Expired,
                        x.ExpiryDate,
                        x.LastLogingTime,
                        x.Locked,
                        x.LowerLimit,
                        x.UpperLimit,
                        x.Password,
                        x.Supervisory,
                        x.UserLevel,
                        x.ValidDays                   
                    }).ToArray()
                }

            };
        }
        public object GetInActiveUsers(bool success, string message, Exception ex = null)
        {
            return new
            {
                success,
                message,
                innerError = ex,
                data = new
                {
                    UserDetail = db.UserLogins.Where(x=>x.Active ==false).Select(y => new
                    {
                        y.Id,
                        y.UserId,
                        y.Active,
                        y.Email,
                        y.DateLastModified,
                        y.Firstname,
                        y.LoginAttempts,
                        y.Staffid,
                        y.Successful
                    }).ToArray()
                }

            };
        }
        
        public LogOnUserRecord GetUserRecordByID(string userId)
        {
            return  db.LogOnUserRecord.Where(x => x.UserId == userId).FirstOrDefault();

        }

            public object GetUserById(string userId)
        {
          
            return
                db.Users.Where(x => x.UserId == userId).Select(x => new
                {
                    x.Id,
                    x.UserId,
                    x.UserName,
                    x.Email,
                    x.Active,
                    x.BankCode,
                    x.CreationDate,
                    x.DateLastModified, // my  mous is d
                    x.Expired,
                    Role = "Admin",
                    x.ExpiryDate,
                    x.LastLogingTime,
                    x.Locked,
                    x.LowerLimit,
                    x.UpperLimit,
                    x.Password,
                    x.Supervisory,
                    x.UserLevel,
                    x.ValidDays
                }).FirstOrDefault();
        }


        public object GetCustomerById(string CustId)
        {
            return


               db.customerMaster.Where(x => x.customerId == CustId).Select(x => new
               {
                   x.Id,
                   x.Currency,
                   x.Availablebalance,
                   x.Bookbalance,
                   x.BranchCode,
                   x.AccountStatus,
                   x.AccountNo,
                   x.AccountName, // my  mous is d
                   x.Abbrev,
                   x.AccountType,
                   x.Accountofficer,
                   x.Email,
                   x.ProductCode,
                   x.PhoneNo,
                   x.OpenDate,
                   x.ProductName,
                   x.ValidForClearingCheque,
                   x.Operatinginstructions
               }).FirstOrDefault();

        }

        public object GetCustomerByAccount(string acctNumber)
        { 
            return


               db.customerMaster.Where(x => x.AccountNo == acctNumber).Select(x => new
               {
                   x.Id,
                   x.Currency,
                   x.Availablebalance,
                   x.Bookbalance,
                   x.BranchCode,
                   x.AccountStatus,
                   x.AccountNo,
                   x.AccountName, // my  mous is d
                   x.Abbrev,
                   x.AccountType,
                   x.Accountofficer,
                   x.Email,
                   x.ProductCode,
                   x.PhoneNo,
                   x.OpenDate,
                   x.ProductName,
                   x.ValidForClearingCheque,
                   x.Operatinginstructions
               }).FirstOrDefault();

        }

        public object GetAccountCustomerById(string customerId)
        {
            return db.CustomerDetails.Where(x => x.CustomerId == customerId).Select(x => new
            {
                x.Id,
                x.CustomerId,
                x.OwnerName,
                x.Bvn,
                x.CustomerStatus,
                x.Address,
                x.PhoneNumber,
                x.Branch,
                NUBAN = "0216789121",
                Accounts = db.customerMaster.Where(c => c.customerId == x.CustomerId).Select(t => new
                {
                    t.Id,
                    AccountNumber = t.AccountNo,
                    t.AccountType,
                     t.AccountStatus,
                     t.Abbrev,
                     t.ProductName,
                    Bvn = "",
                    Branch = t.BranchCode,
                    LockedFunds = 0.00,
                    AccountBalance = new
                    {
                        LedgerBalance = t.Bookbalance,
                        WorkingBalance = t.Availablebalance,
                        ClearedBalance = t.Availablebalance,
                        UnauthorisedBalance = 0.00
                    },
                    t.Accountofficer,
                    CheckSummaryModal = "Available Mandate",
                    AvailableOverdraft = "Available Overdraft",
                    AwaitingAprovals = "Approvals",
                    ArrangementDate = t.OpenDate,
                    Statements = x.AccountStament.Select(z => new
                    {
                        Date = DateTime.Now,
                        z.Narration,
                        z.Id,
                        z.ValDate,
                        z.ExpenseLine,
                        z.Credit,
                        z.Debit,
                        z.Balance,
                        z. Approved
                    }),

                    LCY = "NGN",
                    ER = t.Abbrev == "NGN" ? 1 : 450
                })
            }).FirstOrDefault();

        }

        //public object GetAccountAccountNumber(string accountNumber) 
        //{
        //    return db.CustomerDetails.Where(x => x == accountNumber).Select(x => new
        //    {
        //        x.Id,
        //        x.CustomerId,
        //        x.OwnerName,
        //        x.Bvn,
        //        x.CustomerStatus,
        //        x.Address,
        //        x.PhoneNumber,
        //        x.Branch,
        //        NUBAN = "0216789121",
        //        Accounts = db.customerMaster.Where(c => c.customerId == x.CustomerId).Select(t => new
        //        {
        //            t.Id,
        //            AccountNumber = t.AccountNo,
        //            t.AccountType,
        //            t.AccountStatus,
        //            t.Abbrev,
        //            t.ProductName,
        //            Bvn = "",
        //            Branch = t.BranchCode,
        //            LockedFunds = 0.00,
        //            AccountBalance = new
        //            {
        //                LedgerBalance = t.Bookbalance,
        //                WorkingBalance = t.Availablebalance,
        //                ClearedBalance = t.Availablebalance,
        //                UnauthorisedBalance = 0.00
        //            },
        //            t.Accountofficer,
        //            CheckSummaryModal = "Available Mandate",
        //            AvailableOverdraft = "Available Overdraft",
        //            AwaitingAprovals = "Approvals",
        //            ArrangementDate = t.OpenDate,
        //            Statements = x.AccountStament.Select(z => new
        //            {
        //                Date = DateTime.Now,
        //                z.Narration,
        //                z.Id,
        //                z.ValDate,
        //                z.ExpenseLine,
        //                z.Credit,
        //                z.Debit,
        //                z.Balance,
        //                z.Approved
        //            }),

        //            LCY = "NGN",
        //            ER = t.Abbrev == "NGN" ? 1 : 450
        //        })
        //    }).FirstOrDefault();

        //}




        public object ValidateUser(string email)
        {
            var users = db.Users.ToList();
            Users user = users.Where(x => x.Email == email).FirstOrDefault(); 
            if(user != null)
            {
                return new
                {
                    success = true,
                    message = "",
                    data = new
                    {
                        UserDetail = new
                        {
                            user.Id,
                            user.UserId,
                            user.UserName,
                            user.Email,
                            user.Active,
                            user.BankCode,
                            user.CreationDate,
                            user.DateLastModified,
                            user.Expired,
                            user.ExpiryDate,
                            RoleId=db.USERROLES.Where(x=>x.USERID==user.UserId).Select(x=>x.ROLEID).FirstOrDefault(),
                            RoleName = "",
                            user.LastLogingTime,
                            user.Locked,
                            user.LowerLimit,
                            user.UpperLimit,
                            user.Password,
                            user.Supervisory,
                            user.UserLevel,
                            user.ValidDays
                        },
                        BranchAccount = db.UserBranches.Where(x=> x.UserId == user.UserId).Select(x=> new
                        {
                            Details = db.BranchAccounts.Where(y=> y.SortCode == x.SortCode).Select(y=> new
                            {
                                y.BranchCode,
                                y.VATAccount,
                                y.BranchCRSuspenceAccount,
                                y.BranchDBSuspenceAccount,
                                y.PrincipalAccount,
                                y.WHTAccount,
                                y.InterestAccount
                            }).FirstOrDefault()
                        }).FirstOrDefault()
                    }
                };
            }
            else
            {
                var userloging = new UserLogins()
                {
                    UserId =email
                };
               var result =  UpdateUserLogin(userloging);
                if(result == false)
                {
                    return new
                    {
                        success = false,
                        message = "User is Inactive. Please contact the Admin.",
                        data = new
                        {}
                    };
                }
                else
                {
                    return new
                    {
                        success = false,
                        message = "Invalid username or password.",
                        data = new
                        { }
                    };
                }
                
            }
        }

        public object GetUserById(List<string> userId)
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

                    var userInRequest = new List<Users>();

                    foreach (string uId in userId)
                    {
                        var user = db.Users.Find(uId);
                        if (user != null)
                        {

                            userInRequest.Add(user);

                        }
                    }
                    return userInRequest;
                }
                catch (Exception ex)
                {
                   
                    throw ex;
                }
            }
        }

        public object CreateUser(Users user)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int result = 0;
                    if (!isUserExist(user.UserId))
                    {
                        var param = new SqlParameter[]
                        {
                                            
                           new SqlParameter("@UserId", user.UserId),
                           new SqlParameter("@UserName", user.UserName),
                           new SqlParameter("@Email", user.Email),
                           new SqlParameter("@Active", user.Active),
                           new SqlParameter("@BankCode", user.BankCode),
                           new SqlParameter("@CreationDate", DateTime.Now),
                           new SqlParameter("@DateLastModified", DateTime.Now),
                           new SqlParameter("@Expired", false),
                           new SqlParameter("@ExpiryDate", DateTime.Now),
                           new SqlParameter("@LastLogingTime", DateTime.Now),
                           new SqlParameter("@Locked", false),
                           new SqlParameter("@LowerLimit", DateTime.Now),
                           new SqlParameter("@UpperLimit", DateTime.Now),
                           new SqlParameter("@Password", user.Password),
                           new SqlParameter("@Supervisory", true),
                           new SqlParameter("@UserLevel", user.UserLevel),
                           new SqlParameter("@ValidDays", 365)

                        };
                        result = db.Database.SqlQuery<int>("dbo.UserSetup_CreateUser @UserId,@UserName,@Email,@Active,@BankCode,@CreationDate,@DateLastModified,@Expired,@ExpiryDate,@LastLogingTime,@Locked " +
                                ",@LowerLimit,@UpperLimit,@Password,@Supervisory,@UserLevel,@ValidDays", param)
                            .FirstOrDefault();
                        transaction.Commit();
                        return GetAllUsers(true, "User created successfully");
                    }
                    else
                    {
                        return GetAllUsers(false, "User already exist");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }



        public object UpdateUser(Users user)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int result = 0;
                    if (isUserExist(user.UserId))
                    {
                        var param = new SqlParameter[]
                        {
                           new SqlParameter("@UserId", user.UserId),
                           new SqlParameter("@UserName", user.UserName),
                           new SqlParameter("@Email", user.Email),
                           new SqlParameter("@Active", user.Active),
                           new SqlParameter("@BankCode", user.BankCode),
                          new SqlParameter("@CreationDate", DateTime.Now),
                           new SqlParameter("@DateLastModified", DateTime.Now),
                           new SqlParameter("@Expired", false),
                           new SqlParameter("@ExpiryDate", DateTime.Now),
                           new SqlParameter("@LastLogingTime", DateTime.Now),
                           new SqlParameter("@Locked", false),
                           new SqlParameter("@LowerLimit", DateTime.Now),
                           new SqlParameter("@UpperLimit", DateTime.Now),
                           new SqlParameter("@Password", user.Password),
                           new SqlParameter("@Supervisory", true),
                           new SqlParameter("@UserLevel", user.UserLevel),
                           new SqlParameter("@ValidDays", 365)
                        };
                        try
                        {
                            // hhow did this work? i didnt see d sp how come. I have seen. it is calling the create. the controller or business is ref create
                            result = db.Database.SqlQuery<int>("dbo.UserSetup_UpateUser @UserId,@UserName,@Email,@Active,@BankCode,@CreationDate,@DateLastModified,@Expired,@ExpiryDate,@LastLogingTime,@Locked " +
                                        ",@LowerLimit,@UpperLimit,@Password,@Supervisory,@UserLevel,@ValidDays", param)
                                     .FirstOrDefault();
                            transaction.Commit();
                        }
                        catch (SqlException ex)
                        {
                            throw ex;
                        }
                        return GetAllUsers(true, "User updated successfully");

                    }
                    else
                    {

                        return GetAllUsers(false, "User does not exist");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public object DeleteUser(List<string> ID)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                 
                    //to be completed later
                    //foreach (string uid in ID)
                    //{
                    //    var user = db.Users.Find(uid);
                    //    if (user != null)
                    //    {
                    //        UserHistory userHistory = new UserHistory();
                    //        userHistory.ID = user.ID;
                    //        userHistory.UserName = user.UserName;
                    //        ...
                    //        db.UserHistory.Add(userHistory);
                    //        db.Users.Remove(user);
                    //    }
                    //}
                   // db.SaveChanges();
                   // transaction.Commit();
                    return GetAllUsers(true, "Assigned Till deleted successfully");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }


        public object IsUserLogin(string userId)
        {
            DateTime dt = DateTime.Now;
            var user = db.UserActivity.Where(x => x.UserId == userId && DbFunctions.TruncateTime(x.LastLoginDate) == dt.Date);
            if (!user.Any())
            {
                return false;
            }
            else
            {
                return user.Select(x => new
                {
                    x.Active,
                    x.isLoggedIn,
                    x.ActivityDate,
                    x.LastLoginDate
                }).OrderByDescending(x => x.LastLoginDate).FirstOrDefault();
            }
                                
        }

        public bool? UserLogin(string userId)
        {
            DateTime dt = DateTime.Now;
            var user = db.UserActivity.Where(x => x.UserId == userId && DbFunctions.TruncateTime(x.LastLoginDate) == dt.Date);
            if (!user.Any())
            {
                return false;
            }
            else
            {
                return user.OrderByDescending(x=>x.LastLoginDate).Select(x => x.isLoggedIn).FirstOrDefault();
            }

        }

        public bool? IsUserActive(string userId)
        {
            DateTime dt = DateTime.Now;
            Utils.LogNO("Checking if user is active");
            var user = db.UserLogins.Where(x => x.UserId == userId).FirstOrDefault();
            if (user != null)
            {
                Utils.LogNO("Is user active?" + user.Active);
                return user.Active;
            }
            else
            {
                Utils.LogNO("First time user will login on the system");
                return true;

            }
        }

        public object ActivateUser(string userId)
        {
            DateTime dt = DateTime.Now;
            try
            {
                var userLogin = db.UserLogins.Where(x => x.UserId == userId).FirstOrDefault();
                if (userLogin != null)
                {
                    userLogin.Active = true;
                    userLogin.LoginAttempts = 0;
                    db.Entry(userLogin).State = EntityState.Modified;
                    db.SaveChanges();
                    return GetInActiveUsers(true, "User successfully activated!", null);
                }
                else
                {
                    return GetInActiveUsers(false, "failed to activate user!", null);
                }
            }
            catch(Exception ex)
            {
                return GetInActiveUsers(false, "failed to activate user!", ex);
            }
           
        }
        public void CreateUserLogin(UserLogins userLogin)
        {
            DateTime dt = DateTime.Now;

            var user = db.UserLogins.Where(x => x.UserId == userLogin.UserId).FirstOrDefault();
           
            try
            {
                if (user == null)
                {
                    userLogin.DateLastModified = DateTime.Now;
                    db.UserLogins.Add(userLogin);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
               // Utils.LogNO("User Login Creation Error: " + ex.Message);
                throw (ex);
            }

        }

        public bool UpdateUserLogin(UserLogins userLogin)
        {
            DateTime dt = DateTime.Now;
            var user = db.UserLogins.Where(x => x.UserId == userLogin.UserId).FirstOrDefault();
            
            try
            {
                if (user != null)
                {
                    if (userLogin.Successful==true)
                    {
                        userLogin.DateLastModified = DateTime.Now;
                        userLogin.Active = true;
                        user.LoginAttempts = 0;
                         db.Entry(userLogin).State = EntityState.Modified;
                        db.SaveChanges();
                        return true;
                    }
                                        
                    if(user.LoginAttempts == Convert.ToInt16(db.SystemConfiguration.Where(x => x.KeyName == "LoginAttempts").Select(y => y.KeyValue).FirstOrDefault()))
                    {
                        user.DateLastModified = DateTime.Now;
                        user.Active = false;
                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges();
                        return false;
                    }

                    if (userLogin.Successful == false)
                    {
                        user.DateLastModified = DateTime.Now;
                        user.LoginAttempts = user.LoginAttempts + 1;
                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges();
                        return true;
                    }
               
                }
                else
                {
                    if (userLogin.Successful == true)
                    {
                        userLogin.DateLastModified = DateTime.Now;
                        userLogin.LoginAttempts = 0;
                        userLogin.Active = true;
                        userLogin.Successful = true;
                        db.UserLogins.Add(userLogin);
                        db.SaveChanges();
                        return true;
                    }
                    else
                    {
                        userLogin.DateLastModified = DateTime.Now;
                        userLogin.LoginAttempts = 0;
                        userLogin.Active = true;
                        db.UserLogins.Add(userLogin);
                        db.SaveChanges();
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                //Utils.LogNO("User Login Creation Error: " + ex.Message);
                return false;
            }

        }
        public void CreateUserLoginActivity(string userId)
        {
            DateTime dt = DateTime.Now;

            var userActivity = new UserActivity()
            {
                UserId = userId,
                ActivityDate = DateTime.Now,
                LastLoginDate= DateTime.Now,
                Event = "User Login",
                isLoggedIn = true
            };

            try
            {
                db.UserActivity.Add(userActivity);
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                Utils.LogNO("User Login Activity Error: "+ ex.Message);
                throw (ex);
            }
           
        }
        

        public void LogOutUser(string userId, string Branch)
        {
            DateTime dt = DateTime.Now;
            var userActivity = new UserActivity()
            {
                UserId = userId,
                ActivityDate = DateTime.Now,
                LastLoginDate = DateTime.Now,
                Event = "User LogOut",
                isLoggedIn = false,
                Branch = Branch
            };

            try
            {
                db.UserActivity.Add(userActivity);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Utils.LogNO("User Log Out Activity Error: " + ex.Message);
                throw (ex);
            }

        }

        
        private bool isUserExist(string ID)
        {
            return db.Users.Where(x=> x.UserId == ID).FirstOrDefault()
                 != null ? true : false;
        }

    }
}
