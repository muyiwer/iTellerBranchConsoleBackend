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
    
    public partial class TillAssignment
    {
        public int Id { get; set; }
        public string TillNo { get; set; }
        public string UserID { get; set; }
        public Nullable<decimal> MaxAmount { get; set; }
        public Nullable<int> TillID { get; set; }
        public System.DateTime EffectiveDate { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
    
        public virtual TillSetup TillSetup { get; set; }
        public virtual Users Users { get; set; }
    }
}
