//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iTellerBranch.Repository
{
    using System;
    using System.Collections.Generic;
    
    public partial class TreasuryDealsMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TreasuryDealsMaster()
        {
            this.TreasuryInterest = new HashSet<TreasuryInterest>();
            this.UserTransactionPageAccess = new HashSet<UserTransactionPageAccess>();
        }
    
        public int Id { get; set; }
        public string DealId { get; set; }
        public string CBADealId { get; set; }
        public string DealersReference { get; set; }
        public string ProductCode { get; set; }
        public Nullable<int> CurrencyCode { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Nullable<decimal> PrincipalAmount { get; set; }
        public Nullable<System.DateTime> ValueDate { get; set; }
        public Nullable<int> Tenure { get; set; }
        public Nullable<System.DateTime> MaturityDate { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public Nullable<decimal> InterestAmount { get; set; }
        public Nullable<decimal> WHTAmount { get; set; }
        public Nullable<decimal> NetInterestAmount { get; set; }
        public Nullable<decimal> PaymentAmount { get; set; }
        public string InflowAccount { get; set; }
        public string PaymentAccount { get; set; }
        public string AccountOfficer { get; set; }
        public Nullable<int> TerminationInstructionCode { get; set; }
        public string Remarks { get; set; }
        public string PrincipalAccount { get; set; }
        public string InterestAccount { get; set; }
        public string WHTAccount { get; set; }
        public string TransactionStatus { get; set; }
        public Nullable<int> ParentDealId { get; set; }
        public Nullable<int> ProcessStatus { get; set; }
        public string ApprovedBy { get; set; }
        public Nullable<System.DateTime> WhenApproved { get; set; }
        public string DisapprovedBy { get; set; }
        public Nullable<System.DateTime> WhenDisapproved { get; set; }
        public string DisapprovalReason { get; set; }
        public Nullable<bool> Posted { get; set; }
        public string CBA { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public string MachineName { get; set; }
        public string BranchCode { get; set; }
        public string UserId { get; set; }
        public Nullable<bool> DoubleEntrySuccessful { get; set; }
        public Nullable<bool> IsReversed { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TreasuryInterest> TreasuryInterest { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserTransactionPageAccess> UserTransactionPageAccess { get; set; }
    }
}