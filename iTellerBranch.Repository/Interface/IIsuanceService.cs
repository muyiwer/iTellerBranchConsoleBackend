using iTellerBranch.Model;
using iTellerBranch.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Repository.Interface
{
    public interface IIssuanceService
    {

        List<MCAccount> GetMCAccounts();
        List<ManagerChequeIssuanceDetails> GetTransactionMaster();
        List<MCCharge> GetMCCharge();
        List<ManagerChequeIssuanceDetails> GetApprovedDraft();
        //TransactionsMaster CreateManagerChequeIssuance(ManagerChequeIssuanceDetailsModel managerChequeIssuanceDetailsModel);
    }

}
