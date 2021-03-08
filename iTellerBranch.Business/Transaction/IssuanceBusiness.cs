using iTellerBranch.Model;
using iTellerBranch.Model.ViewModel;
using iTellerBranch.Repository;
using iTellerBranch.Repository.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Business.Issuance
{
    
    public class IssuanceBusiness
    {
        private readonly IssuanceService _IssuanceService;
        
        public IssuanceBusiness()
        {
            _IssuanceService = new IssuanceService();
        }

        
        
    }
}
