using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class ImalTellerAccount
    {
        public List<ImalTellerAccountDetails> ImalTellerAccountDetails { get; set; }  
    }

    public class ImalTellerAccountDetails 
    {
        public int BRANCH_CODE { get; set; }
        public string BRANCH_NAME { get; set; }
        public string USER_ID { get; set; }
        public string WINDOWS_USER { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public DateTime USER_VALID_DT { get; set; }
        public string EMAIL_ID { get; set; }
        public int CIF_NO { get; set; }
        public int CY_CODE { get; set; }
        public int ACC_BR { get; set; }
        public int ACC_CY { get; set; }
        public int ACC_GL { get; set; }
        public int ACC_CIF { get; set; }
        public int ACC_SL { get; set; }
    }
}
