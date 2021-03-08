using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Repository.Interface
{
    interface IUserService
    {
        object GetAllUsers(bool success, string message, Exception ex = null);

        object GetUserById(List<string> userId);
        object GetCustomerById(string CustId);
        object ValidateUser(string userId);
    }
}
