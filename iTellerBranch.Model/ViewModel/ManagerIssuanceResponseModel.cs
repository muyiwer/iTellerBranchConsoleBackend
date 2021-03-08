using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class ManagerIssuanceResponseModel
    {
        public class ManagerIssuanceResponseDetails
        {
            public string ReferenceID { get; set; }
            public string ResponseCode { get; set; }
            public string ResponseText { get; set; }
            public object RequestID { get; set; }
            public object Balance { get; set; }
            public string COMMAMT { get; set; }
            public string CHARGEAMT { get; set; }
            public string FTID { get; set; }
        }

        public class ManagerIssuanceResponse
        {
            public ManagerIssuanceResponseDetails FTResponse { get; set; } 
        }

        public class MCRepurchaseResponseDetails
        {
            public string Transaction_Reference { get; set; }
            public string Draft_No { get; set; }
            public string Draft_Ccy { get; set; }
            public string Draft_Amt { get; set; }
            public string Payee_Name { get; set; }
            public string Issue_Date { get; set; }
            public string Origin { get; set; }
            public string Origin_Reference { get; set; }
            public string Status { get; set; }
        }

        public class McRepurchase
        {
            public MCRepurchaseResponseDetails Record { get; set; }
        }

        public class MCRepurchaseResponse 
        {
            public McRepurchase McRepurchase { get; set; }
        }

        public class OutwardChequeResponseDetails
        {
            public string ReferenceID { get; set; }
            public string ResponseCode { get; set; }
            public string ResponseText { get; set; }
        }

        public class OutwardChequeResponse
        {
            public OutwardChequeResponseDetails FTResponse { get; set; }
        }

    }
}
