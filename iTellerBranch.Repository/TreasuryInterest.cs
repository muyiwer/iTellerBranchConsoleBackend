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
    
    public partial class TreasuryInterest
    {
        public int Id { get; set; }
        public Nullable<int> DealId { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> NoOfDaysInYear { get; set; }
        public Nullable<decimal> InterestRate { get; set; }
        public Nullable<decimal> InterestAmount { get; set; }
    
        public virtual TreasuryDealsMaster TreasuryDealsMaster { get; set; }
    }
}
