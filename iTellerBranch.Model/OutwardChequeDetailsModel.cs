using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model
{
    public class OutwardChequeDetailsModel
    {
        public string CreditAccountNumber { get; set; }
        public string CreditAccountName { get; set; }
        public string DebitAccountNumber { get; set; }
        public string DebitAccountName { get; set; }
        public Nullable<decimal> CreditAmount { get; set; }
        public Nullable<int> CurrencyCode { get; set; }
        public string BankAddress { get; set; }
        public string BankCode { get; set; }
        public string DrawalName { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<bool> Approved { get; set; }
        public Nullable<System.DateTime> DebitValueDate { get; set; }
        public int Id { get; set; }
        public string ChequeNumber { get; set; }
        public string TellerName { get; set; }
        public string BranchCode { get; set; }
        public string ApprovedBy { get; set; }
        public string TransactionReference { get; set; }
        public string CBAResponse { get; set; }
        public string CBAResponseCode { get; set; }
        public string DisapprovalReason { get; set; } 
        public Nullable<System.DateTime> WhenApproved { get; set; }
        public Nullable<System.DateTime> DisapprovedBy { get; set; }
        public Nullable<System.DateTime> WhenDissaprroved { get; set; }
        public string access_token { get; set; }
    }
}
