using iTellerBranch.Model;
using iTellerBranch.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Repository.Interface
{
    interface ITillService
    {
        object GetTillApproval(bool success, string message, Exception ex = null);
        object ApproveTill(TillManagement tillManagement);
        object DisapproveTill(TillManagement tillManagement);
        object OpenTill(TillManagement tillManagement);
        object CloseTill(TillAssignmentModel tillManagement);
        object ConfirmTillBalance(TillManagement tillManagement);
        object GetTill(bool success, string message, Exception ex = null);
        object CreateTill(TillSetup tillSetup);
        object UpdateTill(TillSetup tillSetup);

        object GetUserTill(bool success, string message, Exception ex = null);
        object AssignTill(TillAssignment tillAssignment);
        object DeleteTillAssignment(List<int> ID);
        object GetGiverTill();
        TillManagement GetTillManagement(int id);
        object GetTillTransactions(string tellerId, int currencyId);
        object GetIMALTillTransactions(TillBalanceModel tillModel);
        bool? GetCheckedTillBalance(string tellerId);
    }
}
