using iTellerBranch.Repository;
using iTellerBranch.Repository.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Business.Setup
{
    public class CashDenominationBusiness
    {
        private readonly CashDenominationService _cashDenominationService;

        public CashDenominationBusiness()
        {
            _cashDenominationService = new CashDenominationService();
        }


        public object GetCashDenomination(bool success, string message, Exception ex = null)
        {
            return _cashDenominationService.GetCashDenomination(true, "");
        }

        public object CreateCashDenomination(CashDenomination cashDenomination)
        {
            return _cashDenominationService.CreateCashDenomination(cashDenomination);
        }

        public object UpdateCashDenomination(CashDenomination cashDenomination)
        {
            return _cashDenominationService.UpdateCashDenomination(cashDenomination); 
        }
    }
}
