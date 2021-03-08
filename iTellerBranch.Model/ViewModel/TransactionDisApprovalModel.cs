using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class TransactionDisApprovalModel
    {
        public string TReference { get; set; }
        public string access_token { get; set; }
        public string TranId { get; set; }
        public string DisapprovedBy { get; set; }
        public string DisapprovalReason { get; set; } 
    
    }
}
