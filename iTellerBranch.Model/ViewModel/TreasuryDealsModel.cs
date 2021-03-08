using iTellerBranch.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model
{
    public class TreasuryDealsModel
    {
        public int Id { get; set; }
        public string DealId { get; set; }
        public string CBADealId { get; set; }
        public string DealersReference { get; set; }
        public string ProductCode { get; set; }
        public string UserId { get; set; }
        public Nullable<int> CurrencyCode { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Nullable<decimal> PrincipalAmount { get; set; }
        public Nullable<System.DateTime> ValueDate { get; set; }
        public Nullable<int> Tenure { get; set; }
        public Nullable<System.DateTime> MaturityDate { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public Nullable<decimal> InterestAmount { get; set; }
        public Nullable<decimal> WHTAmount { get; set; }
        public Nullable<decimal> NetInterestAmount { get; set; }
        public Nullable<decimal> PaymentAmount { get; set; }
        public Nullable<decimal> Penalty { get; set; }
        public string PenaltyAccount { get; set; } 
        public Nullable<decimal> PenaltyRate { get; set; }
        public Nullable<System.DateTime> LiquidatedDate { get; set; } 
        public string InflowAccount { get; set; }
        public string PaymentAccount { get; set; }
        public string AccountOfficer { get; set; }
        public Nullable<int> TerminationInstructionCode { get; set; }
        public string Remarks { get; set; }
        public string PrincipalAccount { get; set; }
        public string InterestAccount { get; set; }
        public string WHTAccount { get; set; }
        public string TransactionStatus { get; set; }
        public string ParentDealId { get; set; }
        public Nullable<int> ProcessStatus { get; set; }
        public string ApprovedBy { get; set; }
        public Nullable<System.DateTime> WhenApproved { get; set; }
        public string DisapprovedBy { get; set; }
        public string DisapprovalReason { get; set; }
        
        public decimal PreLiquidatedAmount { get; set; }
        public Nullable<System.DateTime> WhenDisapproved { get; set; }
        public Nullable<bool> Posted { get; set; }
        public string CBA { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public string MachineName { get; set; }

        public string BranchCode { get; set; }

        public string CBAProductCode { get; set; } 
        public string TransRef { get; set; }
        public string CurrencyAbbrev { get; set; }

        public string access_token { get; set; } 
        public bool DoubleEntrySuccessful { get; set; }
        public virtual ICollection<TreasuryInterestModel> TreasuryInterest { get; set; }
        public virtual PreLiquidationModel PreLiquidation { get; set; }  
    }

    public class PreLiquidationModel
    {
        public int ID { get; set; }
        public string DealID { get; set; }
        public Nullable<System.DateTime> LiquidationDate { get; set; }
        public Nullable<decimal> PenaltyRate { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<bool> IsPartialLiquidation { get; set; }

    }
}
