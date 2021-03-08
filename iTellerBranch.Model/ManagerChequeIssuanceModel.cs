using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model
{
    public class ManagerChequeIssuanceModel
    {
        public int ID { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string DraftNumber { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string ChequeNumber { get; set; }
        public Nullable<int> TemplateCode { get; set; }
        public System.DateTime DateCreated { get; set; }
        public Nullable<bool> Printed { get; set; }
        public string PaymentDetails { get; set; }
        public string BeneficiaryName { get; set; }
        public string PaidStatus { get; set; }
        public string BeneficiaryAccount { get; set; }
        public string TransactionReference { get; set; }
        public Nullable<bool> Approved { get; set; }
        public string ApprovedBy { get; set; }
        public Nullable<System.DateTime> WhenApproved { get; set; }
        public string DissaprovedBy { get; set; }
        public Nullable<System.DateTime> WhenDissapproved { get; set; }
        public string CBAResponse { get; set; }
        public string CBAResponseCode { get; set; }
        public string BranchCode { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<int> CurrencyCode { get; set; }
        public Nullable<System.DateTime> ValueDate { get; set; }
        public Nullable<System.DateTime> DebitValueDate { get; set; }
        public string access_token { get; set; }
        public Nullable<decimal> ChargeAmount { get; set; }
    }
}
