using iTellerBranch.Model;
using iTellerBranch.Model.ViewModel;
using iTellerBranch.Repository;
using iTellerBranch.Repository.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Business.Setup
{
   public class TillBusiness
   {
        private readonly TillService _tillService;

        public TillBusiness()
        {
            _tillService = new TillService();
        }

        public object GetIMALTillTransactions(TillBalanceModel tillModel)
        {
            return _tillService.GetIMALTillTransactions(tillModel);
        }


        public object GetTillApproval(bool success, string message, Exception ex = null)
        {
            return _tillService.GetTillApproval(true, "");
        }
        public object ApproveTill(TillManagement tillManagement)
        {
            return _tillService.ApproveTill(tillManagement);
        }
        public object DisapproveTill(TillManagement tillManagement)
        {

            return _tillService.DisapproveTill(tillManagement);
        }
        public object OpenTill(TillManagement tillManagement)
        {
            return _tillService.OpenTill(tillManagement);
        }
        public object CloseTill(TillAssignmentModel tillManagement)
        {
            return _tillService.CloseTill(tillManagement);
        }

        public object GetTill(bool success, string message, Exception ex = null)
        {
            return _tillService.GetTill(true, "");
        }

        public object CreateTill(TillSetup tillSetup)
        {
            return _tillService.CreateTill(tillSetup);
        }

        public object UpdateTill(TillSetup tillSetup)
        {
            return _tillService.UpdateTill(tillSetup);
        }

        public object AssignTill(TillAssignment tillAssignment)
        {
            return _tillService.AssignTill(tillAssignment);
        }

        public object GetUserTill(bool success, string message, Exception ex = null)
        {
            return _tillService.GetUserTill(true, "");
        }

        public object DeleteTillAssignment(List<int> ID)
        {
            return _tillService.DeleteTillAssignment(ID);
        }

        public object GetTillTransactions(string tellerId, int currencyId)
        {
            return _tillService.GetTillTransactions(tellerId, currencyId);
        }

        public TillManagement GetTillManagement(int id)
        {
            return _tillService.GetTillManagement(id);
        }
    }
}
