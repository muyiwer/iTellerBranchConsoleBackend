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
    
    public partial class TransactionsMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TransactionsMaster()
        {
            this.CashTransactions = new HashSet<CashTransactions>();
            this.TillTransfer = new HashSet<TillTransfer>();
            this.TransactionFiles = new HashSet<TransactionFiles>();
            this.TreasuryDetails = new HashSet<TreasuryDetails>();
            this.TillTransferIMAL = new HashSet<TillTransferIMAL>();
        }
    
        public long TranId { get; set; }
        public decimal TotalAmount { get; set; }
        public string TellerId { get; set; }
        public string ToTellerId { get; set; }
        public string TransacterName { get; set; }
        public string AccountName { get; set; }
        public string AccountOfficer { get; set; }
        public string TransacterEmail { get; set; }
        public string TransacterPhoneNo { get; set; }
        public string TransactionParty { get; set; }
        public Nullable<byte> Status { get; set; }
        public string AccountNumber { get; set; }
        public string SortCode { get; set; }
        public int TransType { get; set; }
        public int Currency { get; set; }
        public Nullable<System.DateTime> ValueDate { get; set; }
        public string Narration { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public string MachineName { get; set; }
        public string TransRef { get; set; }
        public Nullable<bool> hasMemo { get; set; }
        public Nullable<bool> hasMandate { get; set; }
        public string Remarks { get; set; }
        public Nullable<bool> NeededApproval { get; set; }
        public string ApprovedBy { get; set; }
        public Nullable<System.DateTime> WhenApproved { get; set; }
        public Nullable<bool> Approved { get; set; }
        public Nullable<bool> IsReversed { get; set; }
        public string DisapprovalReason { get; set; }
        public string DisapprovedBy { get; set; }
        public Nullable<System.DateTime> WhenDisapproved { get; set; }
        public string BranchCode { get; set; }
        public Nullable<bool> Posted { get; set; }
        public string CBAResponse { get; set; }
        public string CBACode { get; set; }
        public string CBA { get; set; }
        public Nullable<long> ReversedTranId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CashTransactions> CashTransactions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TillTransfer> TillTransfer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransactionFiles> TransactionFiles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TreasuryDetails> TreasuryDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TillTransferIMAL> TillTransferIMAL { get; set; }
    }
}
