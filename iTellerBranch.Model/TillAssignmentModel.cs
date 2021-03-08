using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model
{
    public class TillAssignmentModel
    {
        public int Id { get; set; }
        public string access_token { get; set; }
        public string TransactionBranch { get; set; }
        public string TellerId { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        public string IsHeadTeller { get; set; }
        public string User { get; set; }
        public Nullable<bool> Approve { get; set; }
        public string ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<bool> IsClosed { get; set; }
        public string Event { get; set; }
        public string CBAResponse { get; set; }
        public Nullable<decimal> ClosingBalance { get; set; }
        public Nullable<decimal> ShortageOverageAmount { get; set; }
    }
}
