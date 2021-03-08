using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model
{
    public class MCApprovalModel
    {
        public int ID { get; set; }  
        public string ReasonForDisapproval { get; set; }
        public string UserId { get; set; }
    }
}
