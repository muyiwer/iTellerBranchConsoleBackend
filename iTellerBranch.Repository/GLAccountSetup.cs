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
    
    public partial class GLAccountSetup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GLAccountSetup()
        {
            this.TillSetup = new HashSet<TillSetup>();
        }
    
        public int ID { get; set; }
        public string GLAcctName { get; set; }
        public string GLAcctNo { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<int> CurrencyID { get; set; }
    
        public virtual CashDenomination CashDenomination { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TillSetup> TillSetup { get; set; }
    }
}
