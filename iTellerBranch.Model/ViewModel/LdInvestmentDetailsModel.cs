using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class LdInvestmentDetailsModel
    {
        public class EnquiryResponse
        {
            public string Product { get; set; }
            public string Transaction_Ref { get; set; }
            public string Name { get; set; }
            public string Customer_Id { get; set; }
            public string Ccy { get; set; }
            public string Principal { get; set; }
            public string Interest_Rate { get; set; }
            public string Start_Date { get; set; }
            public string Maturity_Date { get; set; } 
        }

        public class LdInvestmentDetails
        {
            public List<EnquiryResponse> EnquiryResponse { get; set; }
        }

        public class EnquiryCBAResponseDetails 
        {
            public string Product { get; set; }
            public string Transaction_Ref { get; set; }
            public string Name { get; set; }
            public string Customer_Id { get; set; }
            public string Ccy { get; set; }
            public string Principal { get; set; }
            public string Interest_Rate { get; set; }
            public string Start_Date { get; set; }
            public string Maturity_Date { get; set; }
        }

        public class EnquiryCBAResponse 
        {
            public EnquiryCBAResponseDetails Record { get; set; }
        }

        public class EnquiryCBAResponseModel 
        {
            public EnquiryCBAResponse EnquiryResponse { get; set; } 
        }
    }
}
