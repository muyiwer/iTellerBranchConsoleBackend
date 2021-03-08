using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class AccountFullInfoViewModel
    {
        public class CustomerBankAccountFullInfoModel
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
            public object ISO_ACCT_TYPE { get; set; }
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
            public object CourtesyTitle { get; set; }
            public string AccountTitle { get; set; }
            public object ManualRiskRating { get; set; }
            public object CalculatedRiskRating { get; set; }
        }

        public class RestrictFlag
        {
            public string ResCode { get; set; }
            public string ResDescp { get; set; }
        }

        public class GetAccountFullInfoModel
        {
            public List<CustomerBankAccountFullInfoModel> BankAccountFullInfo { get; set; }
            public List<RestrictFlag> RestrictFlag { get; set; }
        }

        public class AccountFullInformationModel
        {
            public GetAccountFullInfoModel GetAccountFullInfo { get; set; }
        }
    }
}
