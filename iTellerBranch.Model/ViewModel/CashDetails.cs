using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class CashDetails
    {
        public decimal CashBroughtForward { get; set; }   
        public decimal CashRecievedFromCustomers { get; set; }
        public decimal CashTransferedToVault { get; set; }

        public decimal CashReceivedFromVault { get; set; }

        public decimal CashPaidToCustomers { get; set; }

        public decimal TillTransferIn { get; set; }

        public decimal TillTransferOut { get; set; }

        
    }
}
