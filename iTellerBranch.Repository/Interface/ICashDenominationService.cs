using iTellerBranch.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Repository.Interface
{
    interface ICashDenominationService
    {
        object GetCashDenomination(bool success, string message, Exception ex = null);
        object CreateCashDenomination(CashDenomination cashDenomination); 
        object UpdateCashDenomination(CashDenomination cashDenomination); 
    }
}
