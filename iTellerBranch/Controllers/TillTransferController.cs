using iTellerBranch.BankService;
using iTellerBranch.Business.Transaction;
using iTellerBranch.Model;
using iTellerBranch.Model.ViewModel;
using iTellerBranch.Repository;
using iTellerBranch.Repository.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Web;
using System.Web.Http;

namespace iTellerBranch.Controllers
{
    public class TillTransferController : ApiController
    {
        private readonly TillTransferService _tillTransferService;
        private readonly TransactionBusiness _transactionBusiness;
        private readonly TreasuryService _treasuryService;
        private static string isDemo = System.Configuration.ConfigurationManager.AppSettings["isDemo"];
        // GET: TillTransfer
        public TillTransferController()
        {
            _tillTransferService = new TillTransferService();
            _treasuryService = new TreasuryService();
        }

        //[HttpGet, Route("api/TillTransfer")]
        //public IHttpActionResult GetTillTransfer()
        //{
        //    var result = _tillTransferService.GetTillTransfer(true, "");
        //    return Ok(result);
        //}
        [HttpGet, Route("api/GetRequest/TillTransfer/{userId}")]
        public IHttpActionResult PostRequest([FromUri] string userId)
        {
            try
            {
              
                    var result = _tillTransferService.GetTillRequest(userId);
                    return Ok(result);
            }
            catch (Exception ex)
            {
                // var result = _tillService.GetTill(false, ex.Message, ex);
                return Ok(ex.Message);
            }
        }

        //[HttpGet, Route("api/ImalGetRequest/TillTransfer/{userId}")]
        //public IHttpActionResult ImalPostRequest([FromUri] string userId)
        //{
        //    try
        //    {

        //        var result = _tillTransferService.GetTillTransferImal(userId); 
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        // var result = _tillService.GetTill(false, ex.Message, ex);
        //        return Ok(ex.Message);
        //    }
        //}

        [HttpGet, Route("api/GetFulfilled/TillTransfer/{userId}")]
        public IHttpActionResult GetFufilledTillTransfer([FromUri] string userId)
        {
            try
            {
                var result = _tillTransferService.GetFufilledTillTransfer(userId, true, "");
                return Ok(result);
            }
            catch (Exception ex)
            {
                // var result = _tillService.GetTill(false, ex.Message, ex);
                return Ok(ex.Message);
            }
        }
    
        
        [HttpPost, Route("api/Accept/TillTransfer")]
        public IHttpActionResult AcceptTillTransfer([FromBody]TillTransfer tillTransfer)
        {
            try
            {
                var result = _tillTransferService.AcceptTillTransfer(tillTransfer.Id);
            return Ok(result);
            }
            catch (Exception ex)
            {
                // var result = _tillService.GetTill(false, ex.Message, ex);
                return Ok(ex.Message);
            }
        }

        [HttpPost, Route("api/Reject/TillTransferImal")]
        public IHttpActionResult RejectImalTillTransfer([FromBody]TillTransfer tillTransfer)
        {
            try
            {
                var result = _tillTransferService.RejectImalTillTransfer(tillTransfer.Id);
                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Rejaected successfully"
                    });

                }
                else
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Not Rejected successfully"
                    });
                }
            }
            catch (Exception ex)
            {
                // var result = _tillService.GetTill(false, ex.Message, ex);
                return Ok(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        [HttpGet, Route("api/GetTill/TillTransfer/{TillNo}")]
        public IHttpActionResult GetTillTransfer([FromUri] string TillNo)
        {
            try
            {
                var result = _tillTransferService.GetTillTransfer(TillNo, true, "");
                return Ok(result);
            }
            catch (Exception ex)
            {
                // var result = _tillService.GetTill(false, ex.Message, ex);
                return Ok(ex.Message);
            }
        }

        [HttpGet, Route("api/ImalGetRequest/TillTransfer/{TillNo}")]
        public IHttpActionResult GetImalTillTransfer([FromUri] string TillNo)
        {
            try
            {
                var result = _tillTransferService.GetTillTransferImal(TillNo, true, "");
                return Ok(result);
            }
            catch (Exception ex)
            {
                // var result = _tillService.GetTill(false, ex.Message, ex);
                return Ok(ex.Message);
            }
        }

        [HttpPost, Route("api/Transfer/TillTransfer")]
        public IHttpActionResult TransferTill([FromBody]TillTransfer tillTransfer)
        {
            try
            {
                var result = _tillTransferService.TransferTill(tillTransfer.Id, tillTransfer.Amount);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }

        [HttpPost, Route("api/Request/TillTransfer")]
        public IHttpActionResult PostRequestTillTransfer([FromBody]TillTransfer tillTransfer)
        {
            try
            {
                Utils.LogNO("Till transfer details: " + JsonConvert.SerializeObject(tillTransfer));
                if (isDemo == "false")
                {
                    object result = null;
                    if (tillTransfer.CBA == "T24")
                    {
                        result = _tillTransferService.RequestTill(tillTransfer);
                        
                    }
                    else
                    {
                        var currencyCode = System.Configuration.ConfigurationManager.AppSettings["CurrencyCode"];
                        tillTransfer.GiverTillNo = tillTransfer.CashierID + currencyCode +
                        System.Configuration.ConfigurationManager.AppSettings["SMLedgerCode"] + tillTransfer.GiverTillNo +
                        "000";
                        tillTransfer.ReceiverTillNo = tillTransfer.CashierID + currencyCode +
                            System.Configuration.ConfigurationManager.AppSettings["SMLedgerCode"] + tillTransfer.ReceiverTillNo +
                            "000";
                        Utils.LogNO("Till transfer started");
                        result = _tillTransferService.RequestTill(tillTransfer);
                    }
                    Utils.LogNO("Till sent for approval to till" + tillTransfer.ReceiverTillNo + " by " + tillTransfer.GiverTillNo + "successfully");
                    return Ok(result);
                }
                else
                {

                    var result = _tillTransferService.RequestTill(tillTransfer);
                    return Ok(result);
                }

            }
            catch (Exception ex)
            {
                // var result = _tillTransferService.RequestTill(false, ex.Message, ex);
                return Ok(ex.Message);
            }

        }

        public static string DetermineCompName(string IP)
        {
            try
            {
                IPAddress myIP = IPAddress.Parse(IP);
                IPHostEntry GetIPHost = Dns.GetHostEntry(myIP);
                List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
                return compName.First();
            }
            catch { return ""; }
        }

        public string GetIPAddress()
        {
            try
            {

                System.Web.HttpContext context = System.Web.HttpContext.Current;
                string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    string[] addresses = ipAddress.Split(',');
                    if (addresses.Length != 0)
                    {
                        return addresses[0];
                    }
                }

                return context.Request.ServerVariables["REMOTE_ADDR"];
            }
            catch
            {
                return HttpContext.Current.Request.Params["HTTP_CLIENT_IP"] ?? HttpContext.Current.Request.UserHostAddress;

            }

        }
        public static string GetLocalIPAddress() // we are ok ......  yes  postman testing
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}