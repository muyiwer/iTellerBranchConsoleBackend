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
    
    public partial class CashTransactions
    {
        public int Id { get; set; }
        public Nullable<long> TranId { get; set; }
        public string TillId { get; set; }
        public string ChequeNo { get; set; }
    
        public virtual TransactionsMaster TransactionsMaster { get; set; }
    }
}
