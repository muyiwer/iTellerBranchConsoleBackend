using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class CBACustomerDetailsModel
    {
        public class AccountBalanceModel
        {
            public int LedgerBalance { get; set; }
            public int WorkingBalance { get; set; }
            public int ClearedBalance { get; set; }
            public string UnauthorisedBalance { get; set; }
            public string UnAuthorisedBalance { get; set; }
        }

        public class AccountMandateModel
        {
            public string CustName { get; set; }
            public string ImagePath { get; set; }
            public string SignatoryClass { get; set; }
            public string ImageApplication { get; set; }
        }

        public class AccountModel
        {
            public int Id { get; set; }
            public string AccountNumber { get; set; }
            public string AccountStatus { get; set; }
            public string Bvn { get; set; }
            public string Branch { get; set; }
            public string CustomerStatus { get; set; }
            public AccountBalanceModel AccountBalance { get; set; }
            public int LockedFunds { get; set; }
            public List<AccountMandateModel> AccountMandate { get; set; }
            public string Accountofficer { get; set; }
            public string AvailableMandate { get; set; }
            public string AvailableOverdraft { get; set; }
            public string AwaitingAprovals { get; set; }
            public DateTime ArrangementDate { get; set; }
        }

        public class AccountRecordViewModel
        {
            public AccountModel Record { get; set; }
        }

        public class AccountDetailModel
        {
            public string Id { get; set; }
            public string CustomerId { get; set; }
            public string OwnerName { get; set; }
            public List<AccountModel> Accounts { get; set; }
        }
    }
}
