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
    
    public partial class InwardMessages
    {
        public long msgID { get; set; }
        public string currency { get; set; }
        public Nullable<int> ItemCount { get; set; }
        public Nullable<decimal> TotalValue { get; set; }
        public Nullable<int> SettlementWindow { get; set; }
        public Nullable<System.DateTime> SettlementTime { get; set; }
        public Nullable<int> id { get; set; }
        public Nullable<System.DateTime> SystemDate { get; set; }
    }
}
