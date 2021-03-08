using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class PLStatementModel
    {
        public class PLEntriesRequest
        {
            public string Category { get; set; }
            public string access_token { get; set; }
            public string Date { get; set; }
            public string Branch { get; set; }
        }
        public class StatementHeader
        {
            public string Category { get; set; }
            public string Currency { get; set; }
            public string Balance_At_Period_Start { get; set; }
            public string BALANCE_INCLUDING_FORWARDS { get; set; }
        }

        public class StatementDetails
        {
            public string Description { get; set; }
            public string Reference { get; set; }
            public string Booking_date { get; set; }
            public string Amount { get; set; }
            public string Balance { get; set; }
        }

        public class PLStatementDetails
        {
            public StatementHeader Statement_Header { get; set; }
            public List<StatementDetails> Statement { get; set; }
        }

        public class PLStatementResponse
        {
            public PLStatementDetails PL_Statement { get; set; } 
        }
    }
}
