using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using iTellerBranch.Model.ViewModel;

namespace iTellerBranch.BankService
{
    public static class APIService
    {
        public static string GET(string url, string access_token)
        {
            try
            {
                string response;
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add(HttpRequestHeader.Accept, "application/json");
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    client.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + access_token);
                    // client.Headers.Add("AppId", AppID);
                    // client.Headers.Add("AppKey", AppKey);

                    // var reqbody = new JavaScriptSerializer().Serialize();

                    Utils.Log("=========================POSTING========================================");

                    Utils.Log("CALLING API with  " + " **** Endpoint Url:" + url + "..." + "access_token:  " + access_token);

                    response = client.DownloadString(url);
                    //response = "{\"Record\":{\"ACCOUNT.NUMBER\":\"0000568347\",\"CHEQUE_NUMBER\":\"14019199\",\"IS.CHQ.VALID\":\"TRUE\",\"IS.CHQ.USED\":\"TRUE\",\"IS.CHQ.POSTED\":\"TRUE\",\"CHQ.STATUS\":\"CLEARED\",\"CHQ.CCY\":\"NGN\",\"CHQ.AMOUNT\":\"19800\",\"CHQ.ORIGIN\":\"TELLER\",\"CHQ.ORIGIN.REF\":\"TT170886L5F3\",\"CUSTOMER.NUMBER\":\"2190460\",\"ALTERNATE.ACCOUNT.ID\":\"21921904600010001000\",\"DATE.STOPPED\":null,\"DATE.PRESENTED\":\"29 MAR 2017\"}}";

                    Utils.Log("END CALLING API: DETAILS - " + " **** Endpoint Url:" + response + "...");
                    return response;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public static string GETChequeStatus(string url, string access_token)
        {
            try
            {
                string response;
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add(HttpRequestHeader.Accept, "application/json");
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    client.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + access_token);
                    // client.Headers.Add("AppId", AppID);
                    // client.Headers.Add("AppKey", AppKey);

                    // var reqbody = new JavaScriptSerializer().Serialize();

                    Utils.Log("=========================POSTING========================================");

                    Utils.Log("CALLING API with  " + " **** Endpoint Url:" + url + "..." + "access_token:  " + access_token);

                    response = client.DownloadString(url);

                    Utils.Log("END CALLING NAME ENQUIRY (SINGLE): DETAILS - " + " **** Endpoint Url:" + response + "...");
                    return response;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }



        public static string POST(string url, object request, string access_token)
        {
            try 
            {
                string response;
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add(HttpRequestHeader.Accept, "application/json");
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    client.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + access_token);
                    // client.Headers.Add("AppId", AppID);
                    // client.Headers.Add("AppKey", AppKey);

                    var reqbody = new JavaScriptSerializer().Serialize(request);

                    Utils.Log("=========================POSTING DETAILS REQUEST========================================");

                    Utils.Log("CALLING POSTING SERVICE: DETAILS - " + reqbody.ToString() + " **** Endpoint Url:" + url + "...");

                    response = client.UploadString(url, "POST", reqbody);
                    return response;
                }
            }
            catch (Exception ex)
            {
                Utils.Log("API Error: " + ex.Message);
                throw;
            }
        }

        public static string POSTDeal(string url, string reqbody, string access_token) 
        {
            try
            {
                string response;
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add(HttpRequestHeader.Accept, "application/json");
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    client.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + access_token);
                    // client.Headers.Add("AppId", AppID);
                    // client.Headers.Add("AppKey", AppKey);

                    //var reqbody = new JavaScriptSerializer().Serialize(request);

                    Utils.Log("=========================POSTING DETAILS REQUEST========================================");

                    Utils.Log("CALLING POSTING SERVICE: DETAILS - " + reqbody.ToString() + " **** Endpoint Url:" + url + "...");

                    response = client.UploadString(url, "POST", reqbody);
                    return response;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static string TestDummiAPI(string url, object request, string access_token)
        {
            return "{\"branchCode\":\"1\",\"subAccountCode\":\"0\",\"currencyCode\":\"566\",\"glCode\":\"210801\",\"customerNumber\":\"146054\",\"name\":\"OLUWAFEMI OLADAPO ADELEKE\",\"BVN\":\"22141736578\",\"responseCode\":\"0\",\"skipProcessing\":\"false\",\"skipLog\":\"false\",\"status\":\"A\",\"availableBalance\":\"-34950\",\"ledgerBalance\":\"-34950\",\"lastTransactionDate\":\"8/31/2020 12:00:00 AM\",\"LedgerName\":\"Saving Account\",\"CurrencyName\":\"NGN\",\"Mobile\":\"07039837434\",\"email\":\"femmyadeleke@gmail.com\"}";

        }


        public static string POSTAD(string url, object request)
        {
            try
            {
                string response;
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add(HttpRequestHeader.Accept, "application/json");
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    // client.Headers.Add("AppId", AppID);
                    // client.Headers.Add("AppKey", AppKey);

                    var reqbody = new JavaScriptSerializer().Serialize(request);

                    Utils.Log("=========================NEFT OUTWARD POSTING========================================");

                    Utils.Log("CALLING POSTING SERVICE 4 OUTWARD NEFT (SINGLE): DETAILS - " + "(***Not Viewable***)" + " **** Endpoint Url:" + url + "...");

                    response = client.UploadString(url, "POST", reqbody);
                    return response;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

            public static string POSTEncryption(string url, string request)
            {
                try
                {
                
                    string response;
                    using (WebClient client = new WebClient())
                    {                        
                        client.Headers.Add(HttpRequestHeader.ContentType, "text/plain");
                        // client.Headers.Add("AppId", AppID);
                        // client.Headers.Add("AppKey", AppKey);

                       // var reqbody = new JavaScriptSerializer().Serialize(request);

                        Utils.Log("=========================NEFT OUTWARD POSTING========================================");

                        Utils.Log("CALLING POSTING SERVICE 4 OUTWARD NEFT (SINGLE): DETAILS - " + request + " **** Endpoint Url:" + url + "...");

                        response = client.UploadString(url, "POST", request);
                        return response;
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }

        public static string POSTtoken(string url, TokenRequest token)
        {
            try
            {
                string response;
                string parameters = "client_id=" + token.client_id + "&client_secret=" + token.client_secret;
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                    client.Headers.Add("grant_type", "client_credentials");
                    // client.Headers.Add("AppId", AppID);
                    // client.Headers.Add("AppKey", AppKey);

                    var reqbody = new JavaScriptSerializer().Serialize(token);

                    Utils.Log("=========================Token POSTING========================================");

                    Utils.Log("CALLING GENERATE TOKEN (SINGLE): DETAILS - " + reqbody.ToString() + " **** Endpoint Url:" + url + "...");

                    response = client.UploadString(url, "POST", parameters);
                    return response;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
