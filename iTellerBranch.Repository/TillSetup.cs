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
    
    public partial class TillSetup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TillSetup()
        {
            this.TillAssignment = new HashSet<TillAssignment>();
            this.TillAssignmentHistory = new HashSet<TillAssignmentHistory>();
        }
    
        public int ID { get; set; }
        public string TillNo { get; set; }
        public string TillDesc { get; set; }
        public string GLAcctNo { get; set; }
        public Nullable<bool> IsVault { get; set; }
        public string SortCode { get; set; }
        public string GLAcctName { get; set; }
        public Nullable<int> GLAcctCurrency { get; set; }
        public Nullable<decimal> MinTillAmount { get; set; }
        public Nullable<decimal> MaxTillAmount { get; set; }
        public Nullable<int> GLAccountID { get; set; }
        public Nullable<bool> Status { get; set; }
    
        public virtual GLAccountSetup GLAccountSetup { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TillAssignment> TillAssignment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TillAssignmentHistory> TillAssignmentHistory { get; set; }
    }
}