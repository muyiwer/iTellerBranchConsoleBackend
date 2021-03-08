using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class BalanceCategoryViewModel
    {
        public class BalanceRecordModel
        {
            public string Account_Number { get; set; }
            public string Currency { get; set; }
            public object Customer_No { get; set; }
            public string Account_Name { get; set; }
            public string Previous_Day_Bal { get; set; }
            public string Today_Bal { get; set; }
            public string Variance { get; set; }
            public string Opening_Date { get; set; }
            public string Date_Last_Updated { get; set; }
            public string Company_Code { get; set; }
            public string Company_Name { get; set; }
        }

        public class EnquiryResponseModel
        {
            public List<BalanceRecordModel> Record { get; set; } 
        }

        public class BalanceByCategoryModel
        {
            public EnquiryResponseModel EquiryResponse { get; set; }
        }

    }
}
