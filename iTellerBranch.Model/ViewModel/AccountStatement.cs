using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class StatementViewModel
    {
        public class StatementHeader
        {
            public string Account { get; set; }
            public string Customer_Id { get; set; }
            public string Customer_Name { get; set; }
            public string Currency { get; set; }
            public string Opening_Balance { get; set; }
            public string Closing_Balance { get; set; }
        }

        public class Statement
        {
            public string CUSTOM_HEADER { get; set; }
            public string Booking_Date { get; set; }
            public string Reference { get; set; }
            public string Description { get; set; }
            public string Value_Date { get; set; }
            public string DebitCreditIndicator { get; set; }
            public string Debit { get; set; }
            public string Credit { get; set; }
            public string Closing_Balance { get; set; }
        }

        public class IndividualAccountStatementModel
        {
            public string Status { get; set; }
            public StatementHeader StatementHeader { get; set; }
            public List<Statement> Statement { get; set; }
        }

        public class UserAccountStatement
        {
            public IndividualAccountStatementModel AccountStatement { get; set; }
        }

        public class UserStatementModel
        {
            public string AccountNumber { get; set; }
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }

        public class AccountStatementDetails 
        {
            public bool? Approved { get; set; }
            public double? Balance { get; set; }
            public double? Credit { get; set; }
            public string Date { get; set; }
            public double? Debit { get; set; }
            public string ExpenseLine { get; set; }
            public int? Id { get; set; }
            public string Narration { get; set; }
            public string ValDate { get; set; }
        }
    }
}
