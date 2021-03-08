using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class TransactionBeneficiaries
    {
        public int SN { get; set; }

        public int RowNumber { get; set; }

        public string AccountNumber { get; set; }

        public string CurrencyAbbrev { get; set; }

        public string AccountName { get; set; }
        public string Narration { get; set; }
        public decimal Amount { get; set; }

        public string TransRef { get; set; }

        public decimal ChargeAmt { get; set; }


    }
}
