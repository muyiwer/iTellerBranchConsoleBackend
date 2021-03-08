using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Repository.Interface
{
    interface IResourceService
    {
        object GetRoleResource(bool success, string message, Exception ex = null);

        object CreateRole(ROLES Role);

        object UpdateRole(ROLES Role);
        object DeleteRole(int Id);
        object AssignRole(int Id, List<string> ResourceId);

        object GetRoleResource(int id);



    }
}
