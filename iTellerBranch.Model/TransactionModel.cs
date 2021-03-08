using iTellerBranch.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model
{
    public class TransactionModel
    {
        public string AccountNo { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string TransType { get; set; }
        public string TillId { get; set; }
        public string TillNo { get; set; }
        public string TellerId { get; set; }
        public string CustomerAcctNos { get; set; }
        public string CustomerAcctName { get; set; } 
        public decimal TotalAmt { get; set; }
        public string TransName { get; set; }
        public string PhoneNo { get; set; }
        public Nullable<byte> Status { get; set; }
        public string CashierID { get; set; }
        public string CashierTillNos { get; set; }
        public string CashierTillGL { get; set; }
        public decimal TotalAmount { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> WhenApproved { get; set; }
        public string SortCode { get; set; }
        public Nullable<int> Currency { get; set; }
        public Nullable<System.DateTime> ValueDate { get; set; }
        public Nullable<System.DateTime> DebitValueDate { get; set; }
        public string SupervisoryUser { get; set; }
        public string Beneficiary { get; set; }
        public string ChequeNo { get; set; }
        public Nullable<System.DateTime> DateOnCheque { get; set; }
        public string Narration { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public long TranId { get; set; }
        public string MachineName { get; set; }
        public int TillTransferID { get; set; }
        public bool IsTillTransfer { get; set; }
        public string Remark { get; set; }
        public bool hasMemo { get; set; }
        public bool hasMandate { get; set; }
        public bool NeededApproval { get; set; }
        public string BankAddress { get; set; }
        public string BankCode { get; set; }
        public string DrawalName { get; set; }
        
        public string InitiatorName { get; set; }
        public bool IsT24 { get; set; }

        public string Branch { get; set; }
        public string CurrCode { get; set; }
        public string TransRef { get; set; }
        public string ToTellerId { get; set; }
        public ICollection<TransactionDetailsModel> TransactionDetailsModels { get; set; }
        public string access_token { get; set; }
        public string TransactionParty { get; set; }

        public string CBAResponse { get; set; }
        public bool Approved { get; set; }

        public bool Posted { get; set; }

        public bool IsVaultIn { get; set; }

        public string GLAccountNo { get; set; }

        public string CBACode { get; set; }

        public string CBA { get; set; }
        public string DisapprovalReason { get; set; }
        public string DisapprovedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string WhenDisapproved { get; set; }
        public string CurrencyAbbrev { get; set; }

        public string NIBCashSettlement { get; set; }
        public string VaultAccount { get; set; }
        public string SMCIFNumber { get; set; }
        public string BranchCode { get; set; }
        public string TransacterEmail { get; set; }
        public bool IsReversed { get; set; }
        public string CrDr { get; set; }
        public long ReversedTranId { get; set; }

        public string TransationLocation { get; set; }

        public string AccountName { get; set; }

        public Nullable<decimal> CHARGEAMT { get; set; }

        public string FileName { get; set; }
        public string TransactionCount { get; set; }

        public string ChargeType { get; set; }

        public bool IsBulkTran { get; set; }

        public string reversalUrl { get; set; }

        public string url { get; set; }

        public virtual ManagerChequeIssuanceDetailsModel ManagerChequeIssuanceDetailsModel { get; set; } 

        public virtual ICollection<TransactionBeneficiaries> TransactionBeneficiary { get; set; }
        public virtual ICollection<TreasuryDetailsModel> TreasuryDetails { get; set; }
        public virtual ICollection<DraftIssuedChargesModel> DraftIssuedChargesModel { get; set; } 
    }

    public class TreasuryDetailsModel
    {
        public int ID { get; set; }
        public Nullable<long> TranId { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public virtual TransactionModel TransactionModel { get; set; }  
    }
}
