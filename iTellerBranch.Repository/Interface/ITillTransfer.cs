using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Repository.Interface
{
    interface ITillTransfer
    {
        object RequestTill(TillTransfer tillTransfer);
        //object TransferTill();
        object GetTillRequest(string userId);

        object GetTillTransfer(string userId, bool success, string message = null, Exception innererror = null);
        object GetFufilledTillTransfer(string userId, bool success, string message = null, Exception innererror = null);
        object AcceptTillTransfer(int id, int? transID=null);
        object TransferTill(int id, decimal amount);
    }
}
