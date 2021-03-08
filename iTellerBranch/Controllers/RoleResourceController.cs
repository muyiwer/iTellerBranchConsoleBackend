using iTellerBranch.Business.Setup;
using iTellerBranch.Repository.Service;
using iTellerBranch.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using iTellerBranch.Model.ViewModel;

namespace iTellerBranch.Controllers
{
    public class RoleResourceController : ApiController
    {

     private readonly RoleResourceBusiness _resourceBusiness;

    public RoleResourceController()
    {
        _resourceBusiness = new RoleResourceBusiness();
    }

        // GET: RoleResource
    [HttpGet, Route("api/RoleResource")]
    public IHttpActionResult GetRoleResource()
    {
        var result = _resourceBusiness.GetRoleResource(true, "");
        return Ok(result);
    }

    [HttpGet, Route("api/RoleResource/Role/{id}")]
    public IHttpActionResult GetResourcesByRoleID(int id)
    {
        var result = _resourceBusiness.GetRoleResource(id);
        return Ok(result);
    }

        [HttpGet, Route("api/RoleResource/RoleByName/{roleName}")]
        public IHttpActionResult GetResourcesByRoleByRoleName(string rolename)
        {
            var result = _resourceBusiness.GetRoleResourceByRoleName(rolename);
            return Ok(result);
        }

        // GET: Role
        [HttpGet, Route("api/Role")]
    public IHttpActionResult GetRole()
    {
        var result = _resourceBusiness.GetRole(true, "");
        return Ok(result);
    }



    // POST: RoleResource/Create
    [HttpPost, Route("api/Create/Role/")]
    public IHttpActionResult CreateRole([FromBody]ROLES Roles)
    {
        try
        {
                var result = _resourceBusiness.CreateRole(Roles);
                return Ok(result);
            }
        catch (Exception ex)
        {
            var result = _resourceBusiness.GetRole(false, ex.Message, ex);
            return Ok(result);
        }
    }

     // PUT: RoleResource/Create
     [HttpPost, Route("api/Update/Role")]
    public IHttpActionResult PutRole([FromBody]ROLES Roles)
    {
        try
        {
                var result = _resourceBusiness.UpdateRole(Roles);
                return Ok(result);
        }
        catch (Exception ex)
        {
            var result = _resourceBusiness.GetRole(false, ex.Message, ex);
            return Ok(result);
        }
    }

        // POST: RoleResource/Create
        [HttpPost, Route("api/Assign/Role")]
        public IHttpActionResult AssignRole([FromBody]RoleResModel roleResModel)
        {
            try
            {
                var result = _resourceBusiness.AssignRole(roleResModel);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _resourceBusiness.GetRole(false, ex.Message, ex);
                return Ok(result);
            }
        }

        [HttpPost, Route("api/Delete/Role")]
        public IHttpActionResult DeleteRole([FromBody]RoleResModel roleResModel)
        {
            try
            {
                var result = _resourceBusiness.DeleteRole(roleResModel.RoleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _resourceBusiness.GetRole(false, ex.Message, ex);
                return Ok(result);
            }
        }
    }
}
