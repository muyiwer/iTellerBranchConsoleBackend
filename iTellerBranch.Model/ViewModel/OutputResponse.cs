using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class OutputResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseId { get; set; }
        public string ResponseText { get; set; }
    }

    public class DealOutPutResponse 
    {
        public string RespondCode { get; set; }
        public string ResponseId { get; set; }
        public string ResponseText { get; set; }
    }

    public class TellerReversalResponse 
    {
        public OutputResponse OutputResponse { get; set; }
    }

    public class DealResponse
    { 
        public DealOutPutResponse Response { get; set; }
    }


    public class Response
    { 
        public OutputResponse OutputResponse { get; set; }
        public BankAccountFullInfoViewModel BankAccountFullInfo { get; set; }
    }

    public class FTChqResponse
    {
        public string ReferenceID { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseText { get; set; }
        public object Balance { get; set; }
        public string COMMAMT { get; set; }
        public string CHARGEAMT { get; set; }
        public string FTID { get; set; }
    }

    public class FTOutputResponse
    {
        public FTChqResponse FTResponse { get; set; }
    }
    public class CustomerImageRecord
    {
        public string Image_Application { get; set; }
        public string Cust_Name { get; set; }
        public string File_Name { get; set; }
        public string Image_Instruction { get; set; }
        public string Signatory_Class { get; set; }
        public string Media_Type { get; set; }
        public string Image_Reference { get; set; }
        public string Image_Path { get; set; }
        public string Customer_Bvn { get; set; }
    }

    public class GetCustImage
    {
        public List<CustomerImageRecord> Record { get; set; }
    }

    public class CustImage 
    {
        public GetCustImage GetCustImage { get; set; }
    }


    public class CustomerSingleImageRecord
    {
        public string Image_Application { get; set; }
        public string Cust_Name { get; set; }
        public string File_Name { get; set; }
        public string Image_Instruction { get; set; }
        public string Signatory_Class { get; set; }
        public string Media_Type { get; set; }
        public string Image_Reference { get; set; }
        public string Image_Path { get; set; }
        public string Customer_Bvn { get; set; }
    }

    public class GetSingleCustImage 
    {
        public CustomerSingleImageRecord Record { get; set; }
    }

    public class SingleCustImage 
    {
        public GetSingleCustImage GetCustImage { get; set; } 
    }

}
