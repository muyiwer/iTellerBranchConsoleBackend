using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class UserADdetailsModel
    {
        public ADDetails AD_Details { get; set; }
    }

    public class ADDetails
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StaffID { get; set; }
        public string DeptName { get; set; }
        public string Unit { get; set; }
        public string Group { get; set; }
        public string Division { get; set; }
        public string JobTitle { get; set; }
        public string Grade { get; set; }
        public string Region { get; set; }
        public string CardNumber { get; set; }
        public string SupervisorName { get; set; }
        public string SupervisorDept { get; set; }
        public string SupervisorEmail { get; set; }
        public string SupervisorUsername { get; set; }
        public string SupervisorRole { get; set; }
        public string Mobile { get; set; }
        public string SkypeNo { get; set; }
        public string Title { get; set; }
        public string Gender { get; set; }
    }

    public class loginModel
    {
        public  bool Active { get; set; }
        public bool isLoggedIn { get; set; }
        public DateTime ActivityDate { get; set; }
        public DateTime LastLoginDate { get; set; }
    }
   
}
