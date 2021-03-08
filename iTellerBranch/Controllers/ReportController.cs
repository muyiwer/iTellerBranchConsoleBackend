using iTellerBranch.Business.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace iTellerBranch.Controllers
{
    public class ReportController : ApiController
    {
        private readonly ReportBusiness _reportBusinessBusiness;
        public ReportController()
        {
            _reportBusinessBusiness = new ReportBusiness();
        }
        [HttpGet, Route("api/Report/VaultTransaction/From/{From}/To/{To}")]
        public IHttpActionResult GetChequeDetails([FromUri] DateTime From, DateTime To)
        {
            try
            {
                var result = _reportBusinessBusiness.GetVaultTransaction(From, To);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpGet, Route("api/Report/GetTransactionReport/From/{From}/To/{To}")]
        public IHttpActionResult GetTransactionReport([FromUri] DateTime From, DateTime To)
        {
            try
            {
                var result = _reportBusinessBusiness.TransactionReport(From, To);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpGet, Route("api/Report/GetPostedCallOverReport/From/{From}/To/{To}")]
        public IHttpActionResult GetPostedCallOverReport([FromUri] DateTime From, DateTime To)
        {
            try
            {
                var result = _reportBusinessBusiness.PostedCallOverReport(From, To);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }


        [HttpGet, Route("api/Report/GetTellerCastReport/From/{From}/To/{To}")]
        public IHttpActionResult GetTellerCastReport([FromUri] DateTime From, DateTime To)
        {
            try
            {
                var result = _reportBusinessBusiness.TellerCastReport(From, To);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpGet, Route("api/Report/GetTillReport/From/{From}/To/{To}")]
        public IHttpActionResult GetTillReport([FromUri] DateTime From, DateTime To)
        {
            try
            {
                var result = _reportBusinessBusiness.TillReport(From, To);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpGet, Route("api/Report/GetAuditReport/From/{From}/To/{To}")]
        public IHttpActionResult GetAuditReport([FromUri] DateTime From, DateTime To)
        {
            try
            {
                var result = _reportBusinessBusiness.AuditReport(From, To);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        
        [HttpGet, Route("api/Report/TreasuryDealReport/From/{From}/To/{To}")]
        public IHttpActionResult TreasuryDealReport([FromUri] DateTime From, DateTime To)
        {
            try
            {
                var result = _reportBusinessBusiness.TreasuryDealReport(From, To);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        
        [HttpGet, Route("api/Report/TerminationReport/From/{From}/To/{To}")]
        public IHttpActionResult TerminationReport([FromUri] DateTime From, DateTime To)
        {
            try
            {
                var result = _reportBusinessBusiness.TerminationReport(From, To);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpGet, Route("api/Report/ChequeIssuanceReport/From/{From}/To/{To}")]
        public IHttpActionResult ChequeIssuanceReport([FromUri] DateTime From, DateTime To)
        {
            try 
            {
                var result = _reportBusinessBusiness.ChequeIssuanceReport(From, To);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpGet, Route("api/Report/OutwardChequeReport/From/{From}/To/{To}")]
        public IHttpActionResult OutwardChequeReport([FromUri] DateTime From, DateTime To)
        {
            try
            {
                var result = _reportBusinessBusiness.OutwardChequeReport(From, To);
                return Ok(result); 
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

    }
}
