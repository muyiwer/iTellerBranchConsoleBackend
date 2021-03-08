using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model
{
    public class StatementHeaderModel
    {
        public string Account { get; set; }
        public string Customer_Id { get; set; }
        public string Customer_Name { get; set; }
        public string Currency { get; set; }
        public string Opening_Balance { get; set; }
        public string Closing_Balance { get; set; }
    }

    public class StatementModel
    {
        public object CUSTOM_HEADER { get; set; }
        public string Booking_Date { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public string Value_Date { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public string Closing_Balance { get; set; }
    }

    public class AccountStatementModel
    {
        public StatementHeaderModel Statement_Header { get; set; }
        public StatementModel Statement { get; set; }
    }

    public class AccountRecordModel
    {
        public AccountStatementModel Account_Statement { get; set; }
    }

    public class HVTStatementModel
    {
        public string AccountNo { get; set; }
        public string BranchCode { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string access_token { get; set; }
    }
}
