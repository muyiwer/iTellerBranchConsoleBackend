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
    public class RoleResourceBusiness
    {
        private readonly ResourceService _resourceService;

        public RoleResourceBusiness()
        {
            _resourceService = new ResourceService();
        }

       
        public object GetRoleResource(bool success, string message, Exception ex = null)
        {
            return _resourceService.GetRoleResource(true, "");
        }

        public object GetRole(bool success, string message, Exception ex = null)
        {
            return _resourceService.GetRole(true, "");
        }

        public object CreateRole(ROLES roles)
        {
            return _resourceService.CreateRole(roles);
        }
        public object GetRoleResource(int id)
        {
            return _resourceService.GetRoleResource(id);
        }
        public object GetRoleResourceByRoleName(string rolename)
        {
            return _resourceService.GetRoleResourceByRoleName(rolename);
        }
        
        public object UpdateRole(ROLES roles)
        {
            return _resourceService.UpdateRole(roles);
        }

        public object AssignRole(RoleResModel roleResModel)
        {
            return _resourceService.AssignRole(roleResModel);
        }

        public object DeleteRole(int Id)
        {
            return _resourceService.DeleteRole(Id);
        }
    }
}
