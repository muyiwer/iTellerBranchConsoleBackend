using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class TransactionApprovalModel
    {
    public string TReference { get; set; } 
    public string access_token { get; set; }
    public long? TranId { get; set; }
    public string ApprovedBy { get; set; }
    }
}
