using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class BankAccountFullInfoViewModel
    {
        public string NUBAN { get; set; }
        public string BRA_CODE { get; set; }
        public string DES_ENG { get; set; }
        public string CUS_NUM { get; set; }
        public string CUR_CODE { get; set; }
        public string LED_CODE { get; set; }
        public string CUS_SHO_NAME { get; set; }
        public string AccountGroup { get; set; }
        public string CustomerStatus { get; set; }
        public string ADD_LINE1 { get; set; }
        public string ADD_LINE2 { get; set; }
        public string MOB_NUM { get; set; }
        public string email { get; set; }
        public string ACCT_NO { get; set; }
        public string MAP_ACC_NO { get; set; }
        public string ACCT_TYPE { get; set; }
        public string ISO_ACCT_TYPE { get; set; }
        public string TEL_NUM { get; set; }
        public string DATE_OPEN { get; set; }
        public string STA_CODE { get; set; }
        public string CLE_BAL { get; set; }
        public string CRNT_BAL { get; set; }
        public string TOT_BLO_FUND { get; set; }
        public string INTRODUCER { get; set; }
        public string DATE_BAL_CHA { get; set; }
        public string NAME_LINE1 { get; set; }
        public string NAME_LINE2 { get; set; }
        public string BVN { get; set; }
        public string REST_FLAG { get; set; }
        public List<RESTRICTION> RESTRICTION { get; set; }
        public string IsSMSSubscriber { get; set; }
        public string Alt_Currency { get; set; }
        public string Currency_Code { get; set; }
        public string T24_BRA_CODE { get; set; }
        public string T24_CUS_NUM { get; set; }
        public string T24_CUR_CODE { get; set; }
        public string T24_LED_CODE { get; set; }
        public string OnlineActualBalance { get; set; }
        public string OnlineClearedBalance { get; set; }
        public string OpenActualBalance { get; set; }
        public string OpenClearedBalance { get; set; }
        public string WorkingBalance { get; set; }
        public string CustomerStatusCode { get; set; }
        public string CustomerStatusDeecp { get; set; }
        public object LimitID { get; set; }
        public string LimitAmt { get; set; }
        public string MinimumBal { get; set; }
        public string UsableBal { get; set; }
        public string AccountDescp { get; set; }
        public string CourtesyTitle { get; set; }
        public string AccountTitle { get; set; }
    }
    public class RESTRICTION
    {
        public object RestrictionCode { get; set; }
        public object RestrictionDescription { get; set; }
    }


    public class AccountBalanceViewModel
    {
        public decimal? LedgerBalance { get; set; }
        public decimal? WorkingBalance { get; set; }
        public decimal? ClearedBalance { get; set; }
        public double UnauthorisedBalance { get; set; }
    }

    public class StatementsViewModel
    {
        public string Date { get; set; }
        public string Narration { get; set; }
        public string Id { get; set; }
        public string ValDate { get; set; }
        public string ExpenseLine { get; set; }
        public string Credit { get; set; }
        public string Debit { get; set; }
        public string Balance { get; set; }
        public string Approved { get; set; }
    }

    public class Account
    {
        public string Id { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public string AccountGroup { get; set; }
        public string AccountStatus { get; set; }
        public string Abbrev { get; set; }
        public string ProductName { get; set; }
        public string Bvn { get; set; }
        public string Branch { get; set; }
        public string LockedFunds { get; set; }
        public string LCY { get; set; }
        public string ER { get; set; }
        public AccountBalanceViewModel AccountBalance { get; set; }
        public string Accountofficer { get; set; }
        public string CheckSummaryModal { get; set; }
        public string AvailableOverdraft { get; set; }
        public string AwaitingAprovals { get; set; }
        public string ArrangementDate { get; set; }
        public StatementsViewModel Statements { get; set; }
    }

    public class CustomerDetailsViewModel
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string OwnerName { get; set; }
        public string Bvn { get; set; }
        public string CustomerStatus { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Branch { get; set; }
        public string NUBAN { get; set; }
        public List<Account> Accounts { get; set; }
    }

    public class Root
    {
        public CustomerDetailsViewModel CustomerDetails { get; set; }
    }

}
