using iTellerBranch.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Repository.Service
{
    public static class General
    {
       

       
        public static void AuditLog(string activity, string userId, string MachineName, string IPAddress, string HostName)
        {
            BranchConsoleEntities db = new BranchConsoleEntities();

            Audit audit = new Audit();
            audit.DateTime = DateTime.Now;
            audit.Event = activity;
            audit.HostName = HostName;
            audit.UserId = userId;
            audit.MACAddress = "";
            audit.MachineName = MachineName;
            audit.IPAddress = IPAddress;
            db.Audit.Add(audit);
            db.SaveChanges();
        }

        public static object GetSystemConfiguration()
        {
            BranchConsoleEntities db = new BranchConsoleEntities();

            return new
            {
                SystemConfiguration = db.SystemConfiguration.Where(x => x.KeyName != null).ToArray()
            };

            
        }

        public static object UpdateSystemConfiguration(SystemConfigurationModel systemConfiguration)
        {
            BranchConsoleEntities db = new BranchConsoleEntities();

            SystemConfiguration sysConfig = new SystemConfiguration();
            sysConfig.KeyValue = systemConfiguration.KeyValue;
            sysConfig.Description = systemConfiguration.Description;
            db.Entry(sysConfig).State= EntityState.Modified;
            db.SaveChanges();

            return GetSystemConfiguration();
        }

        public static void UserActivity(string activity, string userId, string branch, bool? isLogIn = null, int? loginAttempt = null)
        {
            BranchConsoleEntities db = new BranchConsoleEntities();
            UserActivity userActivity = new UserActivity();
            userActivity.ActivityDate = DateTime.Now;
            userActivity.Event = activity;
            userActivity.isLoggedIn = isLogIn;
            userActivity.UserId = userId;
            if (isLogIn == true) userActivity.LastLoginDate = DateTime.Now;
            userActivity.Branch = branch;
            db.UserActivity.Add(userActivity);
            db.SaveChanges();
        }

        public static object GetUserActivity(string branch, DateTime? dt)
        {
            BranchConsoleEntities db = new BranchConsoleEntities();
            return new
            {
                UserActivity = db.UserActivity.Where(x => x.Branch == branch &&
                DbFunctions.TruncateTime(x.ActivityDate).Value == dt).ToArray()
            };
        }

        public static object ActivateUser(int id, bool IsActive)
        {
            BranchConsoleEntities db = new BranchConsoleEntities();
            UserActivity userActivity =  db.UserActivity.Find(id);
            userActivity.Active = IsActive;
            db.Entry(userActivity).State = EntityState.Modified;
            db.SaveChanges();
            return GetUserActivity(userActivity.Branch, userActivity.ActivityDate);
        }
    }
}
