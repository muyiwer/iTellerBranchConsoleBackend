using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class UserResponseModel
    {
        public UserResponse Response { get; set; }
    } 

    public class UserResponse
    {
        public UserRecord Record { get; set; }  
    }

    public class UserRecord
    {
        public string Teller_ID { get; set; }
        public string User { get; set; }
        public string Time_Opened { get; set; }
        public string UserName { get; set; }
        public string UserTillBranch { get; set; }
        public object LocalCurrencyAmount { get; set; }
        public object DebitAmount { get; set; }
        public object CreditAmount { get; set; }
        public string UserBranch { get; set; }
        public string TillStatus { get; set; }

        public string UserRole { get; set; }
    }
}
