using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class TransactionResponseMessages
    {
	    public string msgId {get; set; }
		public string responseMessage { get; set; }
		public string responseCode { get; set; }
		public string AccountNumber { get; set; }

		public bool isSuccessful { get; set; }
        public string RefId { get; set; }

		public int rowNumber { get; set; }

        public decimal ChargeAmt { get; set; }
    }
}
