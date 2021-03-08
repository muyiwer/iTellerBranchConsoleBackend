using iTellerBranch.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Repository.Service
{
    public  class BaseService
    {
        public BranchConsoleEntities db;
        
        public BaseService()
        {
            db = new BranchConsoleEntities();
        }
    }
}
