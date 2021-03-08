using iTellerBranch.Business.Setup;
using iTellerBranch.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace iTellerBranch.Controllers
{
    public class CurrencyController : ApiController
    {
        private readonly CashDenominationBusiness _cashDenominationService;

        public CurrencyController()
        {
            _cashDenominationService = new CashDenominationBusiness();
        }

        
        [HttpGet,Route("api/Currency")]
        public IHttpActionResult  GetCurrency()
        {
            var result = _cashDenominationService.GetCashDenomination(true, "");
            return Ok(result);
        }

        [HttpPost, Route("api/Create/Currency")]
        public IHttpActionResult PostCurrency([FromBody] CashDenomination cashDenomination)
        {
            try
            {
                var result = _cashDenominationService.CreateCashDenomination(cashDenomination);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _cashDenominationService.GetCashDenomination(false, ex.Message, ex);
                return Ok(result);
            }
        }

        [HttpPost, Route("api/Update/Currency")]
        public IHttpActionResult PutCurrency([FromBody]CashDenomination cashDenomination)
        {
            try
            {
                var result = _cashDenominationService.UpdateCashDenomination(cashDenomination);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _cashDenominationService.GetCashDenomination(false, ex.Message, ex);
                return Ok(result);
            }
        }

    }
}
