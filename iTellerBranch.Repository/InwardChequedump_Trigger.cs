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
    
    public partial class InwardChequedump_Trigger
    {
        public int ID { get; set; }
        public long ItemID { get; set; }
        public string PreBankSeqNumber { get; set; }
        public string PayBankRoutNumber { get; set; }
        public string AccountNumber { get; set; }
        public string TransactionCode { get; set; }
        public string SerialNumber { get; set; }
        public Nullable<decimal> ItemAmount { get; set; }
        public string BankOfFirstDepoSortCode { get; set; }
        public string MICRRepairFlags { get; set; }
        public string Beneficiary { get; set; }
        public Nullable<bool> Marked { get; set; }
        public string Status { get; set; }
        public Nullable<double> Charges { get; set; }
        public Nullable<System.DateTime> Payment_Date { get; set; }
        public bool Entry_Generated { get; set; }
        public string Cancel_Reason { get; set; }
        public string payer { get; set; }
        public string payee { get; set; }
        public string trans_narration { get; set; }
        public string collectionType { get; set; }
        public Nullable<long> downloadBatchID { get; set; }
        public string presentingBankSortCode { get; set; }
        public Nullable<System.DateTime> presentmentDate { get; set; }
        public string cycleNo { get; set; }
        public string currencyInd { get; set; }
        public string DepositorAcct { get; set; }
        public Nullable<System.DateTime> BOFDBusDate { get; set; }
        public string instrumentType { get; set; }
        public Nullable<bool> confirmedAccountMandate { get; set; }
        public Nullable<bool> confirmedFundsBalance { get; set; }
        public Nullable<bool> confirmedCheque { get; set; }
        public string confirmedAccountMandateUser { get; set; }
        public string confirmedFundsBalanceUser { get; set; }
        public string confirmedChequeUser { get; set; }
        public string udf1 { get; set; }
        public string udf2 { get; set; }
        public string udf3 { get; set; }
        public string udf4 { get; set; }
        public string udf5 { get; set; }
        public string udf6 { get; set; }
        public string udf7 { get; set; }
        public string udf8 { get; set; }
        public string udf9 { get; set; }
        public string udf10 { get; set; }
        public Nullable<bool> SentFundMailAO { get; set; }
        public Nullable<bool> SentFundMailBranch { get; set; }
        public Nullable<bool> SentChqConfrmMailAO { get; set; }
        public Nullable<bool> SentChqConfrmMailBranch { get; set; }
        public string ErrMsg { get; set; }
        public Nullable<bool> Validated { get; set; }
        public string udf11 { get; set; }
        public string udf12 { get; set; }
        public string udf13 { get; set; }
        public string udf14 { get; set; }
        public string udf15 { get; set; }
        public string udf16 { get; set; }
        public string udf17 { get; set; }
        public string udf18 { get; set; }
        public string udf19 { get; set; }
        public string udf20 { get; set; }
        public string beneficiaryBVN { get; set; }
        public string payerBVN { get; set; }
        public string beneficiaryAccountNo { get; set; }
        public Nullable<bool> processed { get; set; }
        public Nullable<bool> accepted { get; set; }
        public Nullable<System.DateTime> SettlementDateTime { get; set; }
        public string CustomerId { get; set; }
        public Nullable<int> PostBatch { get; set; }
        public Nullable<int> PostStatus { get; set; }
        public string CBAResponse { get; set; }
        public Nullable<int> PostedAttempted { get; set; }
        public string CBABeneficiary { get; set; }
        public Nullable<int> Match { get; set; }
        public string CheckDigit { get; set; }
        public string ExpiryDate { get; set; }
        public Nullable<System.DateTime> xdate { get; set; }
    }
}
