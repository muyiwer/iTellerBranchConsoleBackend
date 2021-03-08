using iTellerBranch.Repository;
using iTellerBranch.Repository.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Business.Setup
{
    public class UserBusiness
    {
        private readonly UserService _userService;
        public UserBusiness()
        {
            _userService = new UserService();
        }

        public UserTransactionPageAccess GetUpdateUserTransactionPageAccess(int? tranId)
        {
            return _userService.GetUpdateUserTransactionPageAccess(tranId);
        }

        public void UpdateUserTransactionPageAccess(UserTransactionPageAccess request)
        {
             _userService.UpdateUserTransactionPageAccess(request);
        }

        public bool IsUserActiveOnTransactionPage(UserTransactionPageAccess request)
        {
            return _userService.IsUserActiveOnTransactionPage(request);
        }

        public void UpdateUserTransactionPageAccess(int? id)
        {
             _userService.UpdateUserTransactionPageAccess(id);
        }

        public bool CheckIfUseIsStillActiveOnTransactionPage(int? tranId, double minute)
        {
            return _userService.CheckIfUseIsStillActiveOnTransactionPage(tranId, minute);
        }

        public object GetUsers(bool success, string message, Exception ex = null)
        {
            return _userService.GetAllUsers(success, message, ex);
        }

        public object ValidateUser(string uid)
        {
            return _userService.ValidateUser(uid);
        }

        public object GetUserById(string uid)
        {
            return _userService.GetUserById(uid);
        }

        public object GetCustomerById(string CustId)
        {
            return _userService.GetCustomerById(CustId);
        }
        public object GetCustomerByAccount(string acctNumber)
        {
            return _userService.GetCustomerByAccount(acctNumber);
        }


        public object GetCustomerAccountByIdDemo(string CustId)
        {
            return _userService.GetAccountCustomerById(CustId);
        }

        public object GetUsersByID(List<string> uID)
        {
            return _userService.GetUserById(uID);
        }

        public object CreateUser(Users user)
        {
            return _userService.CreateUser(user);
        }

        public object GetInActiveUsers(bool success, string message, Exception ex = null)
        {
            return _userService.GetInActiveUsers(success, message, ex);
        }

        public object UpdateUser(Users user)
        {
            return _userService.UpdateUser(user);
        }

        public object IsUserLogin(string userId)
        {
            return _userService.IsUserLogin(userId);
        }

        public bool? UserLogin(string userId)
        {
            return _userService.UserLogin(userId);
        }
        public bool? IsUserActive(string userId)
        {
            return _userService.IsUserActive(userId);
        }
        
        public object DeleteUser(List<string> ID)
        {
            return _userService.DeleteUser(ID);
        }
        public LogOnUserRecord GetUserRecordByID(string uID)
        {
            return _userService.GetUserRecordByID(uID);
        }

        public bool? UpdateUserLogin(UserLogins uID)
        {
            return _userService.UpdateUserLogin(uID);
        }

        public void LogOutUser(string uID, string branch)
        {
            _userService.LogOutUser(uID, branch);
        }

        public void CreateUserLoginActivity(string uID)
        {
            _userService.CreateUserLoginActivity(uID);
        }
     
        
        public void CreateUserLogin(UserLogins uID)
        {
            _userService.CreateUserLogin(uID);
        }
        
        public object ActivateUser(string userID)
        {
            return _userService.ActivateUser(userID);
        }

    }
}
