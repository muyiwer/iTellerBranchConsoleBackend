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
    
    public partial class BranchAccounts
    {
        public int Id { get; set; }
        public string BranchCode { get; set; }
        public string SortCode { get; set; }
        public string BranchCRSuspenceAccount { get; set; }
        public string BranchDBSuspenceAccount { get; set; }
        public string VATAccount { get; set; }
        public string ChargesAccount { get; set; }
        public string PrincipalAccount { get; set; }
        public string WHTAccount { get; set; }
        public string InterestAccount { get; set; }
        public string DrawnAccount { get; set; }
    }
}
