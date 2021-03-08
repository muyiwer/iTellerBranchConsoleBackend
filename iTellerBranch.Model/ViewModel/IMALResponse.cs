using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace iTellerBranch.Model.ViewModel
{
    public class IMALResponse
    {
        public class AccountDetailsModel
        {
            [JsonProperty("branchCode", NullValueHandling = NullValueHandling.Ignore)]
            public string branchCode { get; set; }
            [JsonProperty("subAccountCode", NullValueHandling = NullValueHandling.Ignore)]
            public string subAccountCode { get; set; }
            [JsonProperty("currencyCode", NullValueHandling = NullValueHandling.Ignore)]
            public string currencyCode { get; set; }
            [JsonProperty("glCode", NullValueHandling = NullValueHandling.Ignore)]
            public string glCode { get; set; }
            [JsonProperty("customerNumber", NullValueHandling = NullValueHandling.Ignore)]
            public string customerNumber { get; set; }
            [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
            public string name { get; set; }
            [JsonProperty("BVN", NullValueHandling = NullValueHandling.Ignore)]
            public string BVN { get; set; }
            [JsonProperty("responseCode", NullValueHandling = NullValueHandling.Ignore)]
            public string responseCode { get; set; }
            [JsonProperty("skipProcessing", NullValueHandling = NullValueHandling.Ignore)]
            public string skipProcessing { get; set; }
            [JsonProperty("skipLog", NullValueHandling = NullValueHandling.Ignore)]
            public string skipLog { get; set; }
            [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
            public string status { get; set; }
            [JsonProperty("availableBalance", NullValueHandling = NullValueHandling.Ignore)]
            public string availableBalance { get; set; }
            [JsonProperty("ledgerBalance", NullValueHandling = NullValueHandling.Ignore)]
            public string ledgerBalance { get; set; }
            [JsonProperty("lastTransactionDate", NullValueHandling = NullValueHandling.Ignore)]
            public string lastTransactionDate { get; set; }
            [JsonProperty("LedgerName", NullValueHandling = NullValueHandling.Ignore)]
            public string LedgerName { get; set; }
            [JsonProperty("CurrencyName", NullValueHandling = NullValueHandling.Ignore)]
            public string CurrencyName { get; set; }
            [JsonProperty("Mobile", NullValueHandling = NullValueHandling.Ignore)]
            public string Mobile { get; set; }
            [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
            public string email { get; set; }
        }

        public class IMALLocalFTResponse
        {
            public string availabeBalanceAfterOperation { get; set; }
            public string responseCode { get; set; }
            public string responseMessage { get; set; }
            public string errorCode { get; set; }
            public string errorMessage { get; set; }
            public string iMALTransactionCode { get; set; }
            public string skipProcessing { get; set; }
            public string originalResponseCode { get; set; }
            public string skipLog { get; set; }
            public object transactionID { get; set; }
        }

        public class GetAccounts
        {
            public string BRANCH_NAME { get; set; }
            public string BRANCH_CODE { get; set; }
            public string ACCOUNT_NO { get; set; }
            public string ACC_TYPE { get; set; }
            public string CURRENCY { get; set; }
            public int CURRENCY_CODE { get; set; }
            public int GL_CODE { get; set; }
            public int CIF_SUB_NO { get; set; }
            public int SL_NO { get; set; }
            public string BVN { get; set; }
            public string ACC_NAME { get; set; }
            public string STATUS { get; set; }
            public string CIF_STATUS { get; set; }
            public string PHONE_NUMBER { get; set; }
            public string EMAIL { get; set; }
            public string HAS_PND { get; set; }
            public string CUST_TYPE { get; set; }
            public string DATE_OPENED { get; set; }
            public string FIRST_NAME { get; set; }
            public string LAST_NAME { get; set; }
            public Nullable<decimal> Aval_Balance { get; set; }
        }

        public class IMALAccountModel 
        {
            public string Message { get; set; }
            public GetAccounts GetAccounts { get; set; }
        }
    }
}
