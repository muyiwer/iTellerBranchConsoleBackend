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
    
    public partial class DraftIssuedCharges
    {
        public int ID { get; set; }
        public Nullable<int> ChargeID { get; set; }
        public Nullable<decimal> ChargeAmount { get; set; }
        public Nullable<bool> Approved { get; set; }
        public Nullable<int> TranId { get; set; }
    
        public virtual MCCharge MCCharge { get; set; }
        public virtual ManagerChequeIssuanceDetails ManagerChequeIssuanceDetails { get; set; }
    }
}
