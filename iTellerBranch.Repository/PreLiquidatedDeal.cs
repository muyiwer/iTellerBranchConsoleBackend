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
    
    public partial class PreLiquidatedDeal
    {
        public int ID { get; set; }
        public string DealID { get; set; }
        public Nullable<System.DateTime> LiquidationDate { get; set; }
        public Nullable<decimal> PenaltyRate { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<bool> IsPartialLiquidation { get; set; }
        public string CBAReferenceID { get; set; }
    
        public virtual PreLiquidatedDeal PreLiquidatedDeal1 { get; set; }
        public virtual PreLiquidatedDeal PreLiquidatedDeal2 { get; set; }
    }
}
