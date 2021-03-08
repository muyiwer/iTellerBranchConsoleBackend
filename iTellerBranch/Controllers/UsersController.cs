using iTellerBranch.Business.Setup;
using iTellerBranch.Repository;
using iTellerBranch.Repository.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace iTellerBranch.Controllers
{
    public class UsersController : ApiController
    {
        private readonly UserBusiness _userBusiness;

        public UsersController()
        {
            _userBusiness = new UserBusiness();
        }


        [HttpPost, Route("api/Users/TransactionAccessIsActive")]
        public IHttpActionResult IsUserActiveOnTransactionPage(UserTransactionPageAccess request)  
        {

            var result = _userBusiness.IsUserActiveOnTransactionPage(request);  
            return Ok(result);
        }

        [HttpGet, Route("api/TransactionAccess/IsActive/{tranId}")]
        public IHttpActionResult GetAuthorizerPageStillActive(int tranId)
        {
            var minuteInterval = Convert.ToDouble(ConfigurationManager.AppSettings["TransactionPageAccessIntervalInMinutes"]);
            var result = _userBusiness.CheckIfUseIsStillActiveOnTransactionPage(tranId, minuteInterval); 
            return Ok(result);
        }

        [HttpPost, Route("api/Update/TransactionAccess")]
        public IHttpActionResult UpdateUserTransactionPageAccess([FromBody] UserTransactionPageAccess request)
        {
            try
            {
                _userBusiness.UpdateUserTransactionPageAccess(request);
                return Ok(new
                {
                    success = true,
                    message = "Updated successfully"
                });
            }
            catch (Exception ex)
            {
                Utils.LogNO("UpdateUserTransactionPageAccess error: " + ex.Message);
                return Ok(new
                {
                    success = true,
                    message = "Updated successfully"
                });
            }
           
        }

        [HttpPost, Route("api/Update/TransactionAccess/{Id}")]
        public IHttpActionResult UpdateUserTransactionPageAccess(int Id)
        {
            try
            {
                _userBusiness.UpdateUserTransactionPageAccess(Id);
                return Ok(new
                {
                    success = true,
                    message = "Updated successfully"
                });
            }
            catch (Exception ex)
            {
                Utils.LogNO("UpdateUserTransactionPageAccess error: " + ex.Message);
                return Ok(new
                {
                    success = true,
                    message = "Updated successfully"
                });
            }

        }


        [HttpGet, Route("api/Users")]
        public IHttpActionResult GetUsers()
        {
            var result = _userBusiness.GetUsers(true, "");
            return Ok(result);
        }

        //[HttpPost, Route("api/Login")]
        //public IHttpActionResult Login([FromBody] Users user)
        //{
        //    try
        //    {
        //        var result = _userBusiness.ValidateUser(user.Email);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new
        //        {
        //            success = false,
        //            message = "Email does not exist",
        //            innerError = ex
        //        });
        //    }

        //}
        [HttpGet, Route("api/Configuration/Token")]
        public IHttpActionResult TokenEnable() 
        {
            if(ConfigurationManager.AppSettings["EnableToken"] == "true") 
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }

        [HttpGet, Route("api/Users/{userid}")]
        public IHttpActionResult GetUsers([FromUri] string userid)
        {
            var result = _userBusiness.GetUserById(userid);
            return Ok(result);
        }

        [HttpPost, Route("api/GetUsersByIds/Users")]
        public IHttpActionResult GetUsersByID([FromBody]List<string> uID)
        {
            var result = _userBusiness.GetUsersByID(uID);
            return Ok(result);
        }

        [HttpPost, Route("api/Create/Users")]
        public IHttpActionResult PostUser([FromBody]Users user)
        {
            try
            {
                var result = _userBusiness.CreateUser(user);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _userBusiness.GetUsers(false, ex.Message, ex);
                return Ok(result);
            }
        }

        [HttpPost, Route("api/Update/Users")]
        public IHttpActionResult PutUser([FromBody]Users user)
        {
            try
            {
                var result = _userBusiness.UpdateUser(user);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _userBusiness.GetUsers(false, ex.Message, ex);
                return Ok(result);
            }
        }

        

        [HttpPost, Route("api/Delete/Users")]
        public IHttpActionResult DeleteUser([FromBody]List<string> ID)
        {
            try
            {
                var result = _userBusiness.DeleteUser(ID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = _userBusiness.GetUsers(false, ex.Message, ex);
                return Ok(result);
            }
        }
    }
}
