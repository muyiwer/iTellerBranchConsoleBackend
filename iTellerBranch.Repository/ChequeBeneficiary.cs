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
    
    public partial class ChequeBeneficiary
    {
        public int Id { get; set; }
        public Nullable<long> ChequeRef { get; set; }
        public string AcctNo { get; set; }
        public string Beneficiary { get; set; }
        public string Remarks { get; set; }
        public string PresentingbankSortcode { get; set; }
        public Nullable<bool> SMSSent { get; set; }
        public Nullable<bool> EmailSent { get; set; }
        public string Drawer { get; set; }
        public string BVNBeneficiary { get; set; }
        public string BVNPayer { get; set; }
        public string ChannelId { get; set; }
        public Nullable<int> Reviewed { get; set; }
        public string MICRRepair { get; set; }
    }
}