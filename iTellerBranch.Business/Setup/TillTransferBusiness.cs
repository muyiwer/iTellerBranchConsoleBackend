using iTellerBranch.Repository;
using iTellerBranch.Repository.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Business.Setup
{

   public class TillTransferBusiness
    {
        private readonly TillTransferService _tillTransferService;
        public TillTransferBusiness()
        {
            _tillTransferService = new TillTransferService();
        }


        public object GetTillRequest(string userId)
        {
            return _tillTransferService.GetTillRequest(userId);
        }

        public object RequestTill(TillTransfer tillTransfer)
        {
            return _tillTransferService.RequestTill(tillTransfer);
        }
    }
}
