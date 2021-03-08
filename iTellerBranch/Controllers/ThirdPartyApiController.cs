using System;
using iTellerBranch.Repository;
using iTellerBranch.Repository.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using iTellerBranch.CustomerDetailsService;
using Newtonsoft.Json;
using System.Xml;
using iTellerBranch.Model;
using iTellerBranch.Business.Transaction;
using iTellerBranch.Model.ViewModel;
using iTellerBranch.BankService;
using static iTellerBranch.Model.ViewModel.IMALRequestModel;
using iTellerBranch.Business.Setup;
using static iTellerBranch.Model.ViewModel.ChequeStatus;
using System.Web;
using System.Net.Sockets;
using static iTellerBranch.Model.ViewModel.VaultDetailsModel;
using static iTellerBranch.Model.ViewModel.ImalAccountMandate;

namespace iTellerBranch
{
    public class ThirdPartyApiController : ApiController
    {
        private readonly TransactionBusiness _transactionBusiness;
        private readonly UserBusiness _userBusiness;
        private readonly TillBusiness _TillBusiness;
        private static string isDemo = System.Configuration.ConfigurationManager.AppSettings["isDemo"];
        public ThirdPartyApiController()
        {
            _transactionBusiness = new TransactionBusiness();
            _userBusiness = new UserBusiness();
            _TillBusiness = new TillBusiness();
        }
        DepotelCustomerDetails wbs = new DepotelCustomerDetails { };

        [HttpGet, Route("api/Token")]
        public IHttpActionResult ValidateToken()
        {
            if (isDemo == "true")
            {
                var token = Guid.NewGuid();
                return Ok(new
                {
                    success = true,
                    details = new
                    {
                        access_token = token
                    }
                });
            }
            else
            {
                string url = System.Configuration.ConfigurationManager.AppSettings["GenerateToken"];
                string client_id = System.Configuration.ConfigurationManager.AppSettings["client_id"];
                string client_secret = System.Configuration.ConfigurationManager.AppSettings["client_secret"];

                TokenRequest tokenRequest = new TokenRequest();
                tokenRequest.client_id = client_id;
                tokenRequest.client_secret = client_secret;
                var result = TokenService.Validate(url, tokenRequest);
                              

                if (result != null)
                {
                    return Ok(new
                    {
                        success = true,
                        details = result
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = false,
                        details = ""
                    });
                }
            }


        }

        [HttpGet, Route("api/LoginUser/InActiveUsers")]
        public IHttpActionResult GetInactiveLoginUsers()
        {
            Utils.LogNO("api/LoginUser/InActiveUsers:1:Currently Inside This End Point.");
            var result = _userBusiness.GetInActiveUsers(true, "");

            return Ok(result);
                          
        }

        [HttpPost, Route("api/LoginUser/Activate")]
        public IHttpActionResult ActivateUser([FromBody] UserLogins userLogins)
        {
            Utils.LogNO("api/LoginUser/Activate:2:Currently Inside This End Point.");
            try
            {
                try
                {
                    var MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();

                    General.AuditLog("Performed User login Activation Operation by " + userLogins.UserId
                                                    , userLogins.UserId, MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);

                }
                catch { }
                var result = _userBusiness.ActivateUser(userLogins.UserId);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    details = ""
                });
            }  

        }

      
        [HttpPost, Route("api/OTP")]
        public IHttpActionResult ValidateOTPToken(OtpValidation otpValidation)
        {
            Utils.LogNO("api / OTP:3:Currently Inside This End Point.");
            var userLogin = new UserLogins()
            {
                UserId = otpValidation.username
            };

            try
            {
                string token = System.Configuration.ConfigurationManager.AppSettings["Token"];


                try
                {
                    var MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();

                    General.AuditLog("Performed OTP login Validation Operation by " + otpValidation.username
                                                    , otpValidation.username, MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);

                }
                catch { }

                if (isDemo == "true")
                {
                    if (otpValidation.otp == token)
                    {
                        var results = _userBusiness.UserLogin(otpValidation.username);
                        //if(Convert.ToBoolean(result.ToString()) != false)
                        //{
                        //    var logres = JsonConvert.DeserializeObject<loginModel>(result.ToString());
                        if (results == true)
                        {
                            return Ok(new
                            {
                                success = false,
                                message = "User already logged in!"
                            });
                        }
                        // }
                        return Ok(new
                        {
                            success = true,
                            message = ""
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "OTP is invalid!"
                        });
                    }

                }
                else
                {

                    otpValidation.username = otpValidation.username.ToUpper();

                    if (_userBusiness.IsUserActive(otpValidation.username)==false)
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "User is inactive. Contact Administrator!"
                        });
                    }
                    if (otpValidation.otp == token)
                    {
                       
                        userLogin.Successful = true;
                        var loginRes = _userBusiness.UpdateUserLogin(userLogin);
                        var results = _userBusiness.UserLogin(otpValidation.username);
                        if (results == true)
                        {
                            return Ok(new
                            {
                                success = false,
                                message = "User already logged in!"
                            });
                        }
                        return Ok(new
                        {
                            success = true,
                            message = ""
                        });
                    }
                    else {
                        string url = System.Configuration.ConfigurationManager.AppSettings["GenerateToken"];
                        string client_id = System.Configuration.ConfigurationManager.AppSettings["client_id"];
                        string client_secret = System.Configuration.ConfigurationManager.AppSettings["client_secret"];
                        TokenRequest tokenRequest = new TokenRequest();
                        tokenRequest.client_id = client_id;
                        tokenRequest.client_secret = client_secret;
                        var result = TokenService.Validate(url, tokenRequest);

                        if (result.access_token != null)
                        {
                            url = System.Configuration.ConfigurationManager.AppSettings["otpvalidation"];
                            string hashkey = System.Configuration.ConfigurationManager.AppSettings["hashkey"];

                            OTPtoken oTPtoken = new OTPtoken();
                            oTPtoken.Body = new TokenBody();
                            oTPtoken.Body.OtpValidation = new OtpValidation();
                            oTPtoken.Body.OtpValidation.hashkey = hashkey;
                            oTPtoken.Body.OtpValidation.username = otpValidation.username;
                            oTPtoken.Body.OtpValidation.otp = otpValidation.otp;
                            var response = TokenService.otpvalidation(url, oTPtoken, result.access_token);
                            if (response.Envelope.Body.OtpValidationResponse.OtpValidationResult == null)
                            {
                                userLogin.Successful = false;
                                var loginRes = _userBusiness.UpdateUserLogin(userLogin);
                              
                                return Ok(new
                                {
                                    success = false,
                                    message = "OTP is invalid!"
                                });
                            }
                            if (response.Envelope.Body.OtpValidationResponse.OtpValidationResult.Contains("02"))
                            {
                                userLogin.Successful = false;
                                
                                var loginRes = _userBusiness.UpdateUserLogin(userLogin);
                                return Ok(new
                                {
                                    success = false,
                                    message = "OTP is invalid!"
                                });
                            }
                            if (response.Envelope.Body.OtpValidationResponse.OtpValidationResult.Contains("00"))
                            {
                                userLogin.Successful = true;
                                userLogin.LoginAttempts = 0;
                                var loginRes = _userBusiness.UpdateUserLogin(userLogin);
                                var results = _userBusiness.UserLogin(otpValidation.username);
                                if (results == true)
                                {
                                    return Ok(new
                                    {
                                        success = false,
                                        message = "User already logged in!"
                                    });
                                }
                                return Ok(new
                                {
                                    success = true,
                                    message = "" 
                                });
                            }
                            else
                            {
                                userLogin.Successful = false;
                                
                                var loginRes = _userBusiness.UpdateUserLogin(userLogin);
                                return Ok(new
                                {
                                    success = false,
                                    message = "OTP is invalid!"
                                });
                            }
                        }
                        else
                        {
                            userLogin.Successful = false;

                            var loginRes = _userBusiness.UpdateUserLogin(userLogin);
                            return Ok(new
                            {
                                success = false,
                                message = "OTP is invalid!"
                            });
                        }
                    }

                }
                    
            }
            catch (Exception ex)
            {
                
                return Ok(new
                {
                    success = false,
                     message = ex.Message
                });
            }

        }

        [HttpPost, Route("api/OTP/Transaction")]
        public IHttpActionResult ValidateTransactionOTPToken(OtpValidation otpValidation)
        {
            Utils.LogNO("api/OTP/Transaction:4:Currently Inside This End Point.");
            var userLogin = new UserLogins()
            {
                UserId = otpValidation.username
            };

            try
            {
                string token = System.Configuration.ConfigurationManager.AppSettings["Token"];


                try
                {
                    var MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();

                    General.AuditLog("Performed OTP login Validation Operation by " + otpValidation.username
                                                    , otpValidation.username, MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);

                }
                catch { }

                if (isDemo == "true")
                {
                    if (otpValidation.otp == token)
                    {
                      
                        return Ok(new
                        {
                            success = true,
                            message = ""
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "OTP is invalid!"
                        });
                    }

                }
                else
                {



                    if (_userBusiness.IsUserActive(otpValidation.username) == false)
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "User is inactive. Contact Administrator!"
                        });
                    }
                    if (otpValidation.otp == token)
                    {

                        userLogin.Successful = true;
                        var loginRes = _userBusiness.UpdateUserLogin(userLogin);
                     
                        return Ok(new
                        {
                            success = true,
                            message = ""
                        });
                    }
                    else
                    {
                        string url = System.Configuration.ConfigurationManager.AppSettings["GenerateToken"];
                        string client_id = System.Configuration.ConfigurationManager.AppSettings["client_id"];
                        string client_secret = System.Configuration.ConfigurationManager.AppSettings["client_secret"];
                        TokenRequest tokenRequest = new TokenRequest();
                        tokenRequest.client_id = client_id;
                        tokenRequest.client_secret = client_secret;
                        var result = TokenService.Validate(url, tokenRequest);

                        if (result.access_token != null)
                        {
                            url = System.Configuration.ConfigurationManager.AppSettings["otpvalidation"];
                            string hashkey = System.Configuration.ConfigurationManager.AppSettings["hashkey"];

                            OTPtoken oTPtoken = new OTPtoken();
                            oTPtoken.Body = new TokenBody();
                            oTPtoken.Body.OtpValidation = new OtpValidation();
                            oTPtoken.Body.OtpValidation.hashkey = hashkey;
                            oTPtoken.Body.OtpValidation.username = otpValidation.username;
                            oTPtoken.Body.OtpValidation.otp = otpValidation.otp;
                            var response = TokenService.otpvalidation(url, oTPtoken, result.access_token);
                            if (response.Envelope.Body.OtpValidationResponse.OtpValidationResult == null)
                            {
                                userLogin.Successful = false;
                                var loginRes = _userBusiness.UpdateUserLogin(userLogin);

                                return Ok(new
                                {
                                    success = false,
                                    message = "OTP is invalid!"
                                });
                            }
                            if (response.Envelope.Body.OtpValidationResponse.OtpValidationResult.Contains("02"))
                            {
                                userLogin.Successful = false;

                                var loginRes = _userBusiness.UpdateUserLogin(userLogin);
                                return Ok(new
                                {
                                    success = false,
                                    message = "OTP is invalid!"
                                });
                            }
                            if (response.Envelope.Body.OtpValidationResponse.OtpValidationResult.Contains("00"))
                            {
                                userLogin.Successful = true;
                                userLogin.LoginAttempts = 0;
                                var loginRes = _userBusiness.UpdateUserLogin(userLogin);
                             
                                return Ok(new
                                {
                                    success = true,
                                    message = ""
                                });
                            }
                            else
                            {
                                userLogin.Successful = false;

                                var loginRes = _userBusiness.UpdateUserLogin(userLogin);
                                return Ok(new
                                {
                                    success = false,
                                    message = "OTP is invalid!"
                                });
                            }
                        }
                        else
                        {
                            userLogin.Successful = false;

                            var loginRes = _userBusiness.UpdateUserLogin(userLogin);
                            return Ok(new
                            {
                                success = false,
                                message = "OTP is invalid!"
                            });
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                userLogin.Successful = false;

                var loginRes = _userBusiness.UpdateUserLogin(userLogin);
                return Ok(new
                {
                    success = false,
                    message = ex.Message
                });
            }

        }



        [HttpPost, Route("api/User/GetDetails")]
        public IHttpActionResult GetDetailsByUsername([FromBody] AD_Credentials aD_Credentials)
        {
            Utils.LogNO("api/User/GetDetails:5:Currently Inside This End Point.");
            try
            {
                object response = null;
                if (isDemo == "true")
                {
                    var result = _userBusiness.ValidateUser(aD_Credentials.AD_Username);
                    return Ok(result);
                }
                if (aD_Credentials.AD_Username == "ADEDEJIIAD2" || isDemo == "false")
                {
                    // string sSortCode = "232000000";
                    var result = _userBusiness.ValidateUser(aD_Credentials.AD_Username);
                    return Ok(result);
                }
                else
                {
                    string url = System.Configuration.ConfigurationManager.AppSettings["GenerateToken"];
                    string client_id = System.Configuration.ConfigurationManager.AppSettings["client_id"];
                    string client_secret = System.Configuration.ConfigurationManager.AppSettings["client_secret"];

                    TokenRequest tokenRequest = new TokenRequest();
                    tokenRequest.client_id = client_id;
                    tokenRequest.client_secret = client_secret;
                    var token = TokenService.Validate(url, tokenRequest);
                    url = System.Configuration.ConfigurationManager.AppSettings["GetDetailsByUsername"];
                    url = url + "/" + aD_Credentials.AD_Username;
                    UserADdetailsModel result = ActiveDirectoryService.GetDetailsByUsername(url, token.access_token);
                    if (result.AD_Details != null)
                    {
                        //try
                        //{
                        //    var MachineName = DetermineCompName(GetIPAddress());
                        //    var IPAddress = GetIPAddress();
                        //    General.AuditLog("User details Retrieval ", aD_Credentials.AD_Username, MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                        //}
                        //catch { }
                        return Ok(new
                        {
                            success = true,
                            message = "",
                            data = new
                            {
                                UserDetail = new
                                {
                                    Id = result.AD_Details.Username,
                                    UserId = result.AD_Details.StaffID,
                                    UserName = result.AD_Details.FullName,
                                    Role = result.AD_Details.SupervisorRole,
                                    RoleId="",
                                    result.AD_Details.Email,
                                    Active = true,
                                    Supervisory = result.AD_Details.SupervisorName
                                },
                            }
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "",
                            data = new
                            {
                                UserDetail = new { }
                            }
                        });
                    }

                }

            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = ex.Message,
                    innerError = ex
                });
            }
        }

        

        [HttpPost, Route("api/LoginUserActivity/Create")]
        public IHttpActionResult CreateUserLoginActivity([FromBody] UserActivity userActivity)
        {
            Utils.LogNO("api/LoginUserActivity/Create:5:Currently Inside This End Point.");
            try
            {
                 _userBusiness.CreateUserLoginActivity(userActivity.UserId);
                try
                {
                    var MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed Login Activity Creation", userActivity.UserId, MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }
                return Ok("Successful");
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }

        //[HttpGet, Route("api/GetSystemConfiguration/{userId}")]
        //public IHttpActionResult GetSystemConfiguration([FromUri] string userId)
        //{

        //    try
        //    {
        //        var result = General.GetSystemConfiguration();
        //        try
        //        {
        //            var MachineName = DetermineCompName(GetIPAddress());
        //            var IPAddress = GetIPAddress();
        //            General.AuditLog("Performed System Configuration retrieval", userId, MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
        //        }
        //        catch { }
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(ex.Message);
        //    }



        //}

        //[HttpPost, Route("api/SystemConfiguration/Update")]
        //public IHttpActionResult UpdateSystemParameter([FromBody] SystemConfigurationModel systemConfiguration)
        //{
        //    try
        //    {
        //        var result= General.UpdateSystemConfiguration(systemConfiguration);
        //        try
        //        {
        //            var MachineName = DetermineCompName(GetIPAddress());
        //            var IPAddress = GetIPAddress();
        //            General.AuditLog("Performed System Configuration Update on Id " + systemConfiguration.Id + " keyname " + systemConfiguration.KeyName+", new keyValue "+ systemConfiguration.KeyValue+", Previous KeyValue "+systemConfiguration.PreviousKeyValue+", description "+ systemConfiguration.Description, systemConfiguration.userId, MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
        //        }
        //        catch { }
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(ex.Message);
        //    }

        //}

        [HttpPost, Route("api/LoginUserActivity/LogOut")]
        public IHttpActionResult LogUserActivity([FromBody] UserActivity userActivity)
        {
            Utils.LogNO("api/LoginUserActivity/LogOut:6:Currently Inside This End Point.");
            try
            {
                _userBusiness.LogOutUser(userActivity.UserId, userActivity.Branch);
                try
                {
                    var MachineName = DetermineCompName(GetIPAddress());
                    var IPAddress = GetIPAddress();
                    General.AuditLog("Performed LogOut from the system ", userActivity.UserId, MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                }
                catch { }
                return Ok("Successful");
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }

        [HttpPost, Route("api/LoginUserActivity/IsUserLoggedIn")]
        public IHttpActionResult IsUserLogin([FromBody] UserActivity userActivity)
        {
            Utils.LogNO("api/LoginUserActivity/IsUserLoggedIn:7:Currently Inside This End Point.");
            try
            {
                var result = _userBusiness.IsUserLogin(userActivity.UserId);
                //try
                //{
                //    var MachineName = DetermineCompName(GetIPAddress());
                //    var IPAddress = GetIPAddress();
                //    General.AuditLog("Performed login Check on the system ", userActivity.UserId, MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                //}
                //catch { }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }


        [HttpPost, Route("api/Login")]
        public IHttpActionResult ValidateUser([FromBody] AD_Credentials aD_Credentials)
        {
            Utils.LogNO("api/Login:8:Currently Inside This End Point.");
            Utils.LogNO("Login starting------");
            var userLogin = new UserLogins()
            {
                UserId = aD_Credentials.AD_Username

            };


            try
            {
               
            
                if (isDemo == "true")
                {
                    var results = _userBusiness.ValidateUser(aD_Credentials.AD_Username);
                    return Ok(results);
                }
                aD_Credentials.AD_Username = aD_Credentials.AD_Username.ToUpper();

                if (aD_Credentials.AD_Password == System.Configuration.ConfigurationManager.AppSettings["Password"] && isDemo == "false")
                {
                    bool result = IsUserValidToOperate(aD_Credentials);
                    Utils.LogNO("successfully validated user role");
                    Utils.LogNO("Is user role valid? " + result);
                    if (result)
                    {
                        userLogin.Successful = true;

                        var loginRes = _userBusiness.UpdateUserLogin(userLogin);
                        try
                        {
                            var MachineName = DetermineCompName(GetIPAddress());
                            var IPAddress = GetIPAddress();
                            General.AuditLog("Performed User Login Validation on the system ", aD_Credentials.AD_Username, MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                        }
                        catch { }
                        return Ok(new
                        {
                            success = true,
                            message = ""
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "You don't have access to this application. Please contact the admin"
                        });
                    }


                }
                else
                {

                    bool result = IsUserValidToOperate(aD_Credentials);
                    Utils.LogNO("successfully validated user role");
                    Utils.LogNO("Is user role valid? " + result);
                    if (result)
                    {
                        if (_userBusiness.IsUserActive(aD_Credentials.AD_Username) == false)
                        {
                            Utils.LogNO("User is inactive");
                            return Ok(new
                            {
                                success = false,
                                message = "User is inactive. Contact Administrator!"
                            });
                        }
                        Encryption encryption = new Encryption();
                        string url = System.Configuration.ConfigurationManager.AppSettings["EncriptionUrl"];
                        ActiveDirectoryModel activeDirectoryModel = new ActiveDirectoryModel();
                        activeDirectoryModel.AD_Credentials = new AD_Credentials();
                        activeDirectoryModel.AD_Credentials.AD_Username = aD_Credentials.AD_Username;
                        activeDirectoryModel.AD_Credentials.AD_Password = aD_Credentials.AD_Password;
                        //var serializedCredentials = JsonConvert.SerializeObject(AD_Credentials);
                        Utils.LogNO("Calling Encryption API");
                        string encriptedCredentials = ActiveDirectoryService.EncryptCredentials(url, activeDirectoryModel);
                        Utils.LogNO("Finished calling Encryption API");
                        url = System.Configuration.ConfigurationManager.AppSettings["ADAuthentication"];
                        //  object response = ActiveDirectoryService.EncryptCredentials(url, activeDirectoryModel); 
                        try
                        {
                            ACtiveDirectoryModel response = ActiveDirectoryService.ValidateUser(url, encriptedCredentials);


                            if (response.AD_Response.Status == "TRUE")
                            {
                                try
                                {
                                    var MachineName = DetermineCompName(GetIPAddress());
                                    var IPAddress = GetIPAddress();
                                    General.AuditLog("Performed User Login Validation Successfully on the system ", aD_Credentials.AD_Username, MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                                }
                                catch { }
                                userLogin.Successful = true;
                                userLogin.LoginAttempts = 0;
                                var loginRes = _userBusiness.UpdateUserLogin(userLogin);
                                return Ok(new
                                {
                                    success = true,
                                    message = ""
                                });
                            }
                            else
                            {
                                try
                                {
                                    var MachineName = DetermineCompName(GetIPAddress());
                                    var IPAddress = GetIPAddress();
                                    General.AuditLog("Performed User Login Validation on the system ", aD_Credentials.AD_Username, MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                                }
                                catch { }
                                userLogin.Successful = false;
                                bool? IsLoginAttempt = _userBusiness.UpdateUserLogin(userLogin);
                                if (IsLoginAttempt == false)
                                {
                                    try
                                    {
                                        var MachineName = DetermineCompName(GetIPAddress());
                                        var IPAddress = GetIPAddress();
                                        General.AuditLog("Performed User Validation on the system ", aD_Credentials.AD_Username, MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                                    }
                                    catch { }
                                    return Ok(new
                                    {
                                        success = false,
                                        message = "User Login attempts Exceeded!"
                                    });
                                }
                                return Ok(new
                                {
                                    success = false,
                                    message = "Invalid Username or Password"
                                });
                            }
                        }
                        catch (Exception ex)
                        {

                            return Ok(new
                            {
                                success = false,
                                message = "AD Service Error: Login fails for user from AD Service!",
                                innerError = ex
                            });

                        }
                    }
                    else
                    {
                        Utils.LogNO("User does not have access to this application");
                        return Ok(new
                        {
                            success = false,
                            message = "You don't have access to this application. Please contact the admin"
                        });
                    }


                 
                }
            }
            catch (Exception ex)
            {
                // var loginRes = _userBusiness.UpdateUserLogin(userLogin);
                Utils.LogNO("Login Error: " + ex.Message + "  " + ex.StackTrace);
                return Ok(new
                {
                    success = false,
                    message = "An error occured. Please come back later when we fixed that problem. Thanks.",
                    innerError = ex
                });
            }

        }

        private static bool IsUserValidToOperate(AD_Credentials aD_Credentials)
        {
            try
            {
                Utils.LogNO("IsUserValid2Operate........? AD_credentials...");
                string url = System.Configuration.ConfigurationManager.AppSettings["GetTillIdByUser"];
                url = url + "/" + aD_Credentials.AD_Username;
                string tokenUrl = System.Configuration.ConfigurationManager.AppSettings["GenerateToken"];
                string client_id = System.Configuration.ConfigurationManager.AppSettings["client_id"];
                string client_secret = System.Configuration.ConfigurationManager.AppSettings["client_secret"];
                TokenRequest tokenRequest = new TokenRequest();
                tokenRequest.client_id = client_id;
                tokenRequest.client_secret = client_secret;
                string access_token = TokenService.Validate(tokenUrl, tokenRequest).access_token;
                UserRecord result = Transaction.GetUserDetails(url, access_token);
                Utils.LogNO("Successfully called GetUserDetails");
                if (result.UserRole != null)
                {
                    Utils.LogNO("user has role");
                    if (result.UserRole.Contains("INP") || result.UserRole.Contains("MGR")
                        || result.UserRole.Contains("ALL") || result.UserRole.Contains("AUDTCTRLC"))
                    {
                        Utils.LogNO("user role is valid");
                        return true;
                    }
                }
                Utils.LogNO("user role is not valid");
                return false;
            }
            catch (Exception ex)
            {
                Utils.LogNO("Error on validating Get user details: " + ex.Message);
                throw;
            }
         
        }

        [HttpGet, Route("api/RMDetails/{accountCode}/Token/{access_token}")]
        public IHttpActionResult RMDetails([FromUri] string accountCode,  string access_token)
        {
            Utils.LogNO("api/RMDetails/{accountCode}/Token/{access_token}:9:Currently Inside This End Point.");
            try
            {
                if (isDemo == "true")
                {
                    return Ok(new
                    {
                        success = true,
                        data = "Sunday ola"
                    });
                }
                else
                {
                    string url = System.Configuration.ConfigurationManager.AppSettings["RMDetails"] + "/" + accountCode;
                    var response = UserAccountStatementService.RMDetails(url,access_token);
                    if(response.AccountofficerName == null)
                    {
                        return Ok(new
                        {
                            success = false,
                            data = ""
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            success = true,
                            data = response.AccountofficerName
                        });
                    }
                    
                }

            }
            catch (Exception ex)
            {
                // var result = _tillService.GetTill(false, ex.Message, ex);
                return Ok(ex.Message);
            }
        }

        [HttpGet, Route("api/NameEnquiry/{accountNo}/Token/{access_token}")]
        public IHttpActionResult GetCustomerDetails([FromUri] string accountNo, string access_token) // can u achieve it- yes ook. When I m done I willc all ub 
        {
            Utils.LogNO("api/NameEnquiry/{accountNo}/Token/{access_token}:10:Currently Inside This End Point.");
            try
            {
                object response = null;
                if (isDemo == "true")
                {
                    // string sSortCode = "232000000";
                    response = _transactionBusiness.GetCustomerDetails(accountNo);
                    return Json(response);
                }
                else
                {
                    // string sSortCode = "232000000";
                    string url = System.Configuration.ConfigurationManager.AppSettings["NameEnquiry"] + "/" + accountNo;
                    response = Transaction.GetAccountFullInfo(url, access_token);  //wbs.getCustomerDetails2(accountNo, sSortCode);
                    return Json(response);
                }

            }
            catch (Exception ex)
            {
                // var result = _tillService.GetTill(false, ex.Message, ex);
                return Ok(ex.Message);
            }
        }


        [HttpGet, Route("api/TellerAccount/{userId}/CurrCode/{currCode}/Token/{access_token}")]
        public IHttpActionResult GetTellerAccount([FromUri] string userId, string currCode, string access_token)
        {
            Utils.LogNO("api/TellerAccount/{userId}/CurrCode/{currCode}/Token/{access_token}:11:Currently Inside This End Point.");
            try
            {
                if (isDemo == "true")
                {

                    return Ok(new
                    {
                        success = true,
                        details = new
                        {
                            BRANCH_CODE = "0061",
                            CIF_NO = "10112",
                            ACC_CIF = "10013",
                            ACC_GL = "101101"
                        }
                    });
                }
                else
                {
                    string url = System.Configuration.ConfigurationManager.AppSettings["ImalTellerAccount"] + 
                                            "?WindowsUsers=" + userId + "&Currency=" + currCode;
                    var response = ImalService.GetTellerAccount(url, access_token); //wbs.getCustomerDetails2(accountNo, sSortCode);
                    if(response == null)
                    {
                        return Ok(new
                        {
                            success = false,
                            details = new {}
                        });
                    }
                    return Ok(new
                    {
                        success = true,
                        details = response
                    });
                }

            }
            catch (Exception ex)
            {
                // var result = _tillService.GetTill(false, ex.Message, ex);
                return Ok(ex.Message);
            }
        }



        [HttpGet, Route("api/CustomerDetails/{CustId}/Token/{access_token}")]
        public IHttpActionResult GetCustomerDetailsById([FromUri] string CustId, string access_token) // can u achieve it- yes ook. When I m done I willc all ub 
        {
            Utils.LogNO("api/CustomerDetails/{CustId}/Token/{access_token}:12:Currently Inside This End Point.");
            try
            {
                object response = null;
                if (isDemo == "true")
                {
                    // string sSortCode = "232000000";
                    response = _userBusiness.GetCustomerById(CustId);
                    return Json(response);
                }
                else
                { 
                    string url = System.Configuration.ConfigurationManager.AppSettings["GetCustomerInfo"] + "/" + CustId;
                     response = Transaction.GetCustomerInfo(url, access_token);
                    return Json(response);
                }

            }
            catch (Exception ex)
            {
                // var result = _tillService.GetTill(false, ex.Message, ex);
                return Ok(ex.Message);
            }
        }


        [HttpGet, Route("api/ImalNameEnquiry/{accountNo}/Token/{access_token}")]
        public IHttpActionResult GetImalCustomerDetails([FromUri] string accountNo, string access_token) // can u achieve it- yes ook. When I m done I willc all ub 
        {
            Utils.LogNO("api/ImalNameEnquiry/{accountNo}/Token/{access_token}:12:Currently Inside This End Point.");
            try
            {
                object response = null;
                if (isDemo == "true")
                {
                    // string sSortCode = "232000000";
                    response = _transactionBusiness.GetCustomerDetails(accountNo);
                    return Json(response);
                }
                else
                {
                    // string sSortCode = "232000000";
                    string url = System.Configuration.ConfigurationManager.AppSettings["ImalNameEnquiry"] + "?Nuban=" + accountNo;
                    response = ImalService.GetAccountDetailsByNuban(url, access_token, accountNo);
                    //AccountDetailsRequestModel requestModel = new AccountDetailsRequestModel();
                    //requestModel.account = accountNo;
                    //requestModel.accountNumber = null;
                    //requestModel.referenceCode = System.Configuration.ConfigurationManager.AppSettings["referenceCode"]; // "#011#11391884638"; 
                    //requestModel.requestCode = System.Configuration.ConfigurationManager.AppSettings["requestCode"]; //"112";
                    //requestModel.principalIdentifier = System.Configuration.ConfigurationManager.AppSettings["principalIdentifier"]; //"011";
                    //response = ImalService.GetAccountFullInfo(url, requestModel, access_token, accountNo);  //wbs.getCustomerDetails2(accountNo, sSortCode);

                    return Json(response);
                }

            }
            catch (Exception ex)
            {
                // var result = _tillService.GetTill(false, ex.Message, ex);
                return Ok(ex.Message);
            }
        }



        [HttpGet, Route("api/ChequeEnquiry/{accountNo}/{chequeNum}/{access_token}")]
        public IHttpActionResult GetChequeDetails([FromUri] string accountNo, string chequeNum, string access_token)
        {
            Utils.LogNO("api/ChequeEnquiry/{accountNo}/{chequeNum}/{access_token}:13:Currently Inside This End Point.");
            string url = System.Configuration.ConfigurationManager.AppSettings["ChequeStatus"] + "/" + accountNo+"/"+ chequeNum;
            try
            {
                if (isDemo == "true")
                {
                    if(chequeNum != "12345678")
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "No matching records",
                            ChequeRecord = new
                            {
                                ACCOUNT_NUMBER = "",
                                IS_CHQ_VALID = "",
                                IS_CHQ_USED = "",
                                IS_CHQ_POSTED = "",
                                CHQ_STATUS = "",
                                CHQ_CCY = "",
                                CHQ_ORIGIN = "",
                                CHQ_ORIGIN_REF = "",
                                CUSTOMER_NUMBER = "",
                                ALTERNATE_ACCOUNT_ID = "",
                                DATE_STOPPED = "",
                                DATE_PRESENTED = ""

                            }
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            success = true,
                            message = "Success",
                            ChequeRecord = new
                            {
                                ACCOUNT_NUMBER = "0000568347",
                                IS_CHQ_VALID = "TRUE",
                                IS_CHQ_USED = "FALSE",
                                IS_CHQ_POSTED = "TRUE",
                                CHQ_STATUS = "CLEARED",
                                CHQ_CCY = "NGN",
                                CHQ_ORIGIN = "TELLER",
                                CHQ_ORIGIN_REF = "TT170886L5F3",
                                CUSTOMER_NUMBER = "2190460",
                                ALTERNATE_ACCOUNT_ID = "21921904600010001000",
                                DATE_STOPPED = "",
                                DATE_PRESENTED = "29 MAR 2017"

                            }
                        });
                    }
                   
                }
                else
                {
                    Utils.LogNO("Get Cheque Status here...");
                    ChequeStatusModel chequeStatusRes = Transaction.GetChequeStatus(url, access_token);
                    Utils.LogNO("Cheque Status Response from API: " + chequeStatusRes.Record.CHQ_STATUS);
                    if (chequeStatusRes != null && chequeStatusRes.Record.CHEQUE_NUMBER == null)
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "Status not available",
                            ChequeRecord = new
                            {
                                ACCOUNT_NUMBER = "",
                                IS_CHQ_VALID = "",
                                IS_CHQ_USED = "",
                                IS_CHQ_POSTED = "",
                                CHQ_STATUS = "",
                                CHQ_CCY = "",
                                CHQ_ORIGIN = "",
                                CHQ_ORIGIN_REF = "",
                                CUSTOMER_NUMBER = "",
                                ALTERNATE_ACCOUNT_ID = "",
                                DATE_STOPPED = "",
                                DATE_PRESENTED = ""

                            }
                        });
                    }
                    else if (chequeStatusRes.Record != null && chequeStatusRes.Record.CHEQUE_NUMBER !=null)
                    {
                        ChequeRecord ChequeRecord = new ChequeRecord();
                                               
                        return Ok(new
                        {
                            success = true,
                            message = "success",
                            ChequeRecord = new
                            {
                                ACCOUNT_NUMBER = chequeStatusRes.Record.ACCOUNT_NUMBER,
                                IS_CHQ_VALID = chequeStatusRes.Record.IS_CHQ_VALID,
                                IS_CHQ_USED = chequeStatusRes.Record.IS_CHQ_USED,
                                IS_CHQ_POSTED = chequeStatusRes.Record.IS_CHQ_POSTED,
                                CHQ_STATUS = chequeStatusRes.Record.CHQ_STATUS,
                                CHQ_CCY = chequeStatusRes.Record.CHQ_CCY,
                                CHQ_ORIGIN = chequeStatusRes.Record.CHQ_ORIGIN,
                                CHQ_ORIGIN_REF = chequeStatusRes.Record.CHQ_ORIGIN_REF,
                                CUSTOMER_NUMBER = chequeStatusRes.Record.CUSTOMER_NUMBER,
                                ALTERNATE_ACCOUNT_ID = chequeStatusRes.Record.ALTERNATE_ACCOUNT_ID,
                                DATE_STOPPED = chequeStatusRes.Record.DATE_STOPPED,
                                DATE_PRESENTED = chequeStatusRes.Record.DATE_PRESENTED
                                // are u done with the sprint 3 API? Yes ld ok build 
                          }
                        });
                    }

                    
                    return Ok(chequeStatusRes);
                    
                }

            }
            catch (Exception ex)
            {
                // var result = _tillService.GetTill(false, ex.Message, ex);
                return Ok(ex.Message);
            }
        }

        [HttpPost, Route("api/PushDetails/")]
        public IHttpActionResult PushCustomerDetails([FromBody]CustomerDetailModel customerDetails)
        {
            Utils.LogNO("api/PushDetails/:13:Currently Inside This End Point.");
            try
            {

                CustomerDetailsService.CustomerDetails customerDetail = new CustomerDetailsService.CustomerDetails();
                customerDetail.accountNumber = customerDetails.accountNumber;
                customerDetail.custId = customerDetails.custId;
                customerDetail.accountTitle = customerDetails.accountTitle;
                customerDetail.emailAddress = customerDetails.emailAddress;
                customerDetail.phoneNumber = customerDetails.phoneNumber;
                customerDetail.bookBalance = customerDetails.bookBalance;
                customerDetail.availableBalance = customerDetails.availableBalance;
                customerDetail.lienAmount = customerDetails.lienAmount;
                customerDetail.accountOfficer = customerDetails.accountOfficer;
                customerDetail.BranchCode = customerDetails.BranchCode;
                customerDetail.accountStatus = customerDetails.accountStatus;
                //customerDetail.Accountofficer_email = customerDetails.Accountofficer_email;
                //customerDetail.Accountofficer_fullname = customerDetails.Accountofficer_fullname;
                customerDetail.remarks = customerDetails.remarks;
                customerDetail.productType = customerDetails.productType;
                customerDetail.account_type = customerDetails.account_type;
                customerDetail.account_Group = customerDetails.account_Group;
                // customerDetail.ProductCode = customerDetails.ProductCode;
                //  customerDetail.ProductName = customerDetails.ProductName;
                customerDetail.branch = customerDetails.branch;
                customerDetail.validForClearingCheque = customerDetails.validForClearingCheque == true ? true : false;
                customerDetail.dateOpened = customerDetails.dateOpened;
                customerDetail.ISOCurrency = new ISOCurrency { connectionString = "", Code = customerDetails.Code, Abbreviation = customerDetails.Abbreviation, Name = customerDetails.Name };

                //XmlNode myXmlNode = JsonConvert.DeserializeXmlNode(customerDetail);
                var response = wbs.PushCustomerDetails(customerDetail);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // var result = _tillTransferService.RequestTill(false, ex.Message, ex);
                return Ok(ex.Message);
            }
        }


        [HttpGet, Route("api/GetAccountMemo/{accountNo}")]
        public IHttpActionResult GetAccountMemo([FromUri]string accountNo)
        {
            Utils.LogNO("api/GetAccountMemo/{accountNo}:14:Currently Inside This End Point.");
            try
            {
                AccountMemoInfo acctMemoInfo = new AccountMemoInfo();
                AccountMemoInfo[] acctMemoInfoArr = new AccountMemoInfo[2];
                var isDemo = System.Configuration.ConfigurationManager.AppSettings["isDemo"];
                if (isDemo == "true")
                {
                    var result = wbs.GetAccountMemoInfo(accountNo);
                    if (result != null)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        return Ok(result);
                    }
                }
                {
                    AccountMemoInfo memoInfo = null;
                    AccountMemoInfo minfo = new AccountMemoInfo();
                    //var mem = wbs.GetAccountMemoInfo(cusid);
                    //accountMemo[] mArray = new accountMemo[mem.accountMemo.Length];

                    //if (mem != null)
                    //{
                    //    for (int i = 0; i < mem.accountMemo.Length; i++)
                    //    {
                    //        accountMemo m = new accountMemo();

                    //        m.memotext = mem.accountMemo[i].memotext;
                    //        m.severity = "";

                    //        mArray[i] = m;
                    //    }


                    //}

                    //minfo.accountMemo = mArray;
                    //minfo.accountNumber = new string[] { "" };

                    //return minfo;


                    //var result = 
                    return Ok(false);
                }

            }
            catch (Exception ex)
            {
                var result = _transactionBusiness.GetAccountMandate(accountNo, false, ex.Message, ex);
                return Ok(result);
            }

        }

        [HttpPost, Route("api/GetMandateImage")]
        public IHttpActionResult GetMandateImage([FromBody]TransactionModel transaction)
        {
            Utils.LogNO("api/GetMandateImage:15:Currently Inside This End Point.");
            string username = System.Configuration.ConfigurationManager.AppSettings["ImageUserName"];
            string password = System.Configuration.ConfigurationManager.AppSettings["ImagePassword"];
            string networkPath = System.Configuration.ConfigurationManager.AppSettings["NetworkPath"];
            // url = url + "/" + accountNo;
            var result = ImageFromRemoteServer.GetBase64String(networkPath, transaction.TellerId, username, password);
            try
            {
                var MachineName = DetermineCompName(GetIPAddress());
                var IPAddress = GetIPAddress();
                General.AuditLog("Performed mandate Validation on the user "+ username, transaction.TellerId, MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
            }
            catch { }
            return Ok(result);

        }
        [HttpGet, Route("api/GetAccountMandate/{accountNo}/Token/{access_token}")]
        public IHttpActionResult GetAccountMandate([FromUri]string accountNo, string access_token)
        {
            Utils.LogNO("api/GetAccountMandate/{accountNo}/Token/{access_token}:16:Currently Inside This End Point.");
            try
            {
                if (isDemo == "false")
                {
                    string url = System.Configuration.ConfigurationManager.AppSettings["GetMandate"];
                    string username = System.Configuration.ConfigurationManager.AppSettings["ImageUserName"];
                    string password = System.Configuration.ConfigurationManager.AppSettings["ImagePassword"];
                    string networkPath = System.Configuration.ConfigurationManager.AppSettings["NetworkPath"];

                    url = url + "/" + accountNo;
                    var result = Transaction.GetCustImage(url, access_token);
                    List<CustomerImageRecord> customerImageRecords = new List<CustomerImageRecord>();
                    foreach(var cusImage in result)
                    {
                        try
                        {
                            var image = ImageFromRemoteServer.GetBase64String(networkPath, cusImage.File_Name, username, password);
                            customerImageRecords.Add(new CustomerImageRecord()
                            {
                                Customer_Bvn = cusImage.Customer_Bvn,
                                Cust_Name = cusImage.Cust_Name,
                                Image_Path = image,
                                Signatory_Class = cusImage.Signatory_Class,
                                Image_Application = null,
                                Image_Instruction = cusImage.Image_Instruction
                            });
                        }
                        catch (Exception ex) {
                            customerImageRecords.Add(new CustomerImageRecord()
                            {
                                Customer_Bvn = cusImage.Customer_Bvn,
                                Cust_Name = cusImage.Cust_Name,
                                Image_Path = "",
                                Signatory_Class = cusImage.Signatory_Class,
                               Image_Application = "No Mandate Image for this customer",
                                Image_Instruction = cusImage.Image_Instruction
                            });
                        }
                       
                    }
                   
                        
                    if (result != null)
                    {
                        return Ok(new
                        {
                            success = true,
                            message = "Customer mandate details",
                            Mandate = customerImageRecords
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "Customer mandate details not available",
                            Mandate = result
                        });
                    }
                }
                else
                {
                    List<CustomerImageRecord> result = new List<CustomerImageRecord>();
                    result.Add(new CustomerImageRecord
                    {
                        Image_Path = "",
                        Cust_Name = "Muyiwa Aro",
                        Signatory_Class = "Test",
                        Image_Application = null,
                        Image_Instruction = "SOLE"
                    });
                    return Ok(new
                    {
                        success = true,
                        message = "Customer mandate details",
                        Mandate = result
                    });
                }



            }
            catch (Exception ex)
            {
                Utils.LogNO("Error executing Image "+ ex.Message);
                List<CustImage> result = new List<CustImage>();
                return Ok(new
                {
                    success = false,
                    message = "Customer mandate details",
                    Mandate = result
                });
            }

        }


        [HttpPost, Route("api/GetImalAccountMandate")]
        public IHttpActionResult GetImalAccountMandate([FromBody]ImalAccountMandateClientRequest accountMandate) 
        {
            Utils.LogNO("api/GetImalAccountMandate:17:Currently Inside This End Point.");
            try
            {
                if (isDemo == "false")
                {
                    string url = System.Configuration.ConfigurationManager.AppSettings["ReturnSignaturesDetails"];
                    accountMandate.Type = System.Configuration.ConfigurationManager.AppSettings["CustomerType"];;
                    var result = ImalService.GetAccountMandate(url, accountMandate);
                    if(result == null)
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "Customer mandate details not available"
                        });
                    }
                    if(result.StatusCode == "0")
                    {
                        return Ok(new
                        {
                            success = true,
                            message = "Customer mandate details",
                            Mandate = result
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            success = false,
                            message = result.StatusDesc
                        });
                    }
                }
                else
                {
                    var result = ImalService.GetAccountMandateDemo("", accountMandate);
                    if (result == null)
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "Customer mandate details not available"
                        });
                    }
                    if (result.StatusCode == "0")
                    {
                        return Ok(new
                        {
                            success = true,
                            message = "Customer mandate details",
                            Mandate = result
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            success = false,
                            message = result.StatusDesc
                        });
                    }
                }



            }
            catch (Exception ex)
            {
                Utils.LogNO("Error executing Image " + ex.Message);
                return Ok(new
                {
                    success = false,
                    message = ex.Message
                });
            }

        }


        [HttpPost, Route("api/PostCashDepoist/")]
        public IHttpActionResult PushCashDeposit([FromBody]TransactionModel transdetail)
        {
            Utils.LogNO("api/PostCashDepoist/:18:Currently Inside This End Point.");
            try
            {

                var tran = new transactionDetails[1];

                var mytran = new transactionDetails
                {
                    branchCode = transdetail.SortCode.Substring(5, 3),
                    beneficiaryName = transdetail.Beneficiary,
                    beneficiaryAccountNo = transdetail.CashierTillGL,//to be figured out
                    accountNo = transdetail.AccountNo,
                    amount = transdetail.Amount == null ? 0m : Convert.ToDecimal(transdetail.Amount),
                    transDate = Convert.ToDateTime(transdetail.CreationDate),
                    userID = transdetail.TellerId,
                    sortCode = transdetail.SortCode,
                    timeApproved = DateTime.Now,
                    transCurrencyCode = Convert.ToInt32(transdetail.Currency),
                    approvalReason = transdetail.Narration,
                    narration = transdetail.Narration,
                    approverID = transdetail.TranId.ToString(),//transdetail.ApproverID,
                    GUID = Guid.NewGuid().ToString()

                };
                tran[0] = mytran;


                var response = wbs.PushCashDeposit(tran);

                return Ok(response);
            }
            catch (Exception ex)
            {
                // var result = _tillTransferService.RequestTill(false, ex.Message, ex);
                return Ok(ex.Message);
            }
        }

        [HttpPost, Route("api/PostCashWithdrawal/")]
        public IHttpActionResult PostCashWithdrawal([FromBody]TransactionModel transdetail)
        {
            Utils.LogNO("api/PostCashWithdrawal/:19:Currently Inside This End Point.");
            try
            {
                var tran = new transactionDetails[1];

                var mytran = new transactionDetails
                {
                    branchCode = transdetail.SortCode.Substring(5, 3),
                    beneficiaryName = transdetail.Beneficiary,
                    beneficiaryAccountNo = transdetail.AccountNo,//to be figured out
                    accountNo = transdetail.CashierTillGL,
                    amount = transdetail.Amount == null ? 0m : Convert.ToDecimal(transdetail.Amount),
                    transDate = Convert.ToDateTime(transdetail.CreationDate),
                    userID = transdetail.TellerId,
                    sortCode = transdetail.SortCode,
                    timeApproved = DateTime.Now,
                    transCurrencyCode = Convert.ToInt32(transdetail.Currency),
                    approvalReason = transdetail.Narration,
                    narration = transdetail.Narration,
                    approverID = transdetail.TranId.ToString(),//transdetail.ApproverID,

                    GUID = Guid.NewGuid().ToString()

                };
                tran[0] = mytran;


                // var response = wbs.PushCashWithdrawal(tran);

                return Ok("Posted succesfully");
            }
            catch (Exception ex)
            {
                // var result = _tillTransferService.RequestTill(false, ex.Message, ex);
                return Ok(ex.Message);
            }
        }


        [HttpPost, Route("api/Till/ApproveCloseTill")]
        public IHttpActionResult ApproveCloseTill([FromBody]TillAssignmentModel tillAssignmentModel)
        {
            Utils.LogNO("api/Till/ApproveCloseTill:20:Currently Inside This End Point.");
            try
            {
                TillManagement tillManagement = _TillBusiness.GetTillManagement(tillAssignmentModel.Id);
                if (isDemo == "false")
                {
                    object result = null;
                    string url = System.Configuration.ConfigurationManager.AppSettings["CloseTill"];
                    TillClosureModel tillClosureRequest = new TillClosureModel();
                    tillClosureRequest.TillClosure = new TillClosure();
                    tillClosureRequest.TillClosure.access_token = tillAssignmentModel.access_token;
                    tillClosureRequest.TillClosure.Comment = tillManagement.Comments;
                    tillClosureRequest.TillClosure.TransactionBranch = tillManagement.TransactionBranch;
                    tillClosureRequest.TillClosure.IsHeadTeller = tillManagement.IsHeadTeller;
                    tillClosureRequest.TillClosure.TellerId = tillManagement.TellerId;
                    tillClosureRequest.TillClosure.Status = tillManagement.Status;
                    Utils.LogNO("Close Till event:  Calling Client Service for posting...");
                    OutputResponse response = TillAPIService.CloseTill(url, tillClosureRequest);
                    if (response.ResponseCode == "-1")
                    {
                        result = new
                        {
                            success = false,
                            message = "Operation not successful",
                        };
                        return Ok(result);
                    }

                    if (response.ResponseCode == "1")
                    {
                        try
                        {
                            tillManagement.CBAResponse = response.ResponseText;

                            var res = _TillBusiness.ApproveTill(tillManagement);

                            try
                            {
                                var MachineName = DetermineCompName(GetIPAddress());
                                var IPAddress = GetIPAddress();
                                General.AuditLog("Performed Till Closure approval for teller "+ tillManagement.TellerId, tillManagement.ApprovedBy, MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                            }
                            catch { }
                        }
                        catch { }

                        result = new
                        {
                            success = true,
                            message = "Till closed successfully",
                            TransactionRef = response.ResponseId
                        };
                        return Ok(result);
                    }
                    else
                    {
                        try
                        {
                            tillManagement.CBAResponse = response.ResponseText;
                            var res = _TillBusiness.ApproveTill(tillManagement);
                        }
                        catch { }

                        result = new
                        {
                            success = false,
                            message = "Operation not successful."
                        };
                        return Ok(result);
                    }
                }
                else
                {
                    var res = _TillBusiness.ApproveTill(tillManagement);
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                Utils.LogNO("Close Till Exception Error:  " + ex.Message);
                object result = new
                {
                    success = false,
                    message = ex.Message
                };
                return Ok(result);
            }
        }


        [HttpPost, Route("api/Till/Open")]
        public IHttpActionResult OpenTill([FromBody]ReopenTill openTill)
        {
            Utils.LogNO("api/Till/Open:21:Currently Inside This End Point.");
            try
            {
                if (isDemo == "false")
                {
                    object result = null;
                    string url = System.Configuration.ConfigurationManager.AppSettings["OpenTill"];
                    ReopenTillModel tillOpenRequest = new ReopenTillModel();
                    tillOpenRequest.reopenTill = new ReopenTill();
                    tillOpenRequest.reopenTill.access_token = openTill.access_token;
                    tillOpenRequest.reopenTill.Comment = openTill.Comment;
                    tillOpenRequest.reopenTill.TransactionBranch = openTill.TransactionBranch;
                    tillOpenRequest.reopenTill.IsHeadTeller = openTill.IsHeadTeller;
                    tillOpenRequest.reopenTill.TellerId = openTill.TellerId;
                    tillOpenRequest.reopenTill.Status = openTill.Status;
                    tillOpenRequest.reopenTill.User = openTill.User;
                    Utils.LogNO("Open Till event:  Calling Client Service for posting...");
                    OutputResponse response = TillAPIService.ReOpenTill(url, tillOpenRequest);
                    if (response.ResponseCode == "-1")
                    {
                        result = new
                        {
                            success = false,
                            message = "Operation not successful",
                        };

                        return Ok(result);
                    }

                    if (response.ResponseCode == "1")
                    {
                        try
                        {
                            var MachineName = DetermineCompName(GetIPAddress());
                            var IPAddress = GetIPAddress();
                            General.AuditLog("Performed Till Open operation for teller " + openTill.TellerId, openTill.User, MachineName, IPAddress, HttpContext.Current.Request.UserHostAddress);
                        }
                        catch { }
                        result = new
                        {
                            success = true,
                            message = "Till re-opened successfully",
                            TransactionRef = response.ResponseId
                        };
                        return Ok(result);
                    }
                    else
                    {
                        result = new
                        {
                            success = false,
                            message = "Till open operation was not successful."
                        };
                        return Ok(result);
                    }
                }
                else
                {
                    object result = new
                    {
                        success = true,
                        message = "Till open successful.",
                        TransactionRef = "1234"
                    };
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                Utils.LogNO("Open Till Exception Error:  " + ex.Message);
                object result = new
                {
                    success = false,
                    message = ex.Message
                };
                return Ok(result);
            }
        }

        [HttpGet, Route("api/Security/Encrypt")]
        public IHttpActionResult Encrypt([FromUri]string plaintText)
        {
            Utils.LogNO("api/Security/Encrypt:22:Currently Inside This End Point.");
            try
            {
                string sharedkeyval = System.Configuration.ConfigurationManager.AppSettings["Sharedkeyval"].ToString();
                string sharedvectorval = System.Configuration.ConfigurationManager.AppSettings["Sharedvectorval"].ToString();


            }
            catch { }


            return Ok();
        }

        [HttpGet, Route("api/Security/Decrypt")]
        public IHttpActionResult Decrypt([FromUri]string plaintText)
        {
            Utils.LogNO("api/Security/Decrypt:23:Currently Inside This End Point.");
            try
            {
                string sharedkeyval = System.Configuration.ConfigurationManager.AppSettings["Sharedkeyval"].ToString();
                string sharedvectorval = System.Configuration.ConfigurationManager.AppSettings["Sharedvectorval"].ToString();
            }
            catch { }


            return Ok();
        }

        [HttpGet, Route("api/User/UserDetails")]
        public IHttpActionResult GetUserDetails([FromUri]string userId, string access_token)
        {
            Utils.LogNO("api/User/UserDetails:24:Currently Inside This End Point.");
            try
            {
                if (isDemo == "false")
                {
                    string url = System.Configuration.ConfigurationManager.AppSettings["GetTillIdByUser"];
                    url = url + "/" + userId;
                    UserRecord result = Transaction.GetUserDetails(url, access_token);
                    if (result.User == null)
                    {
                        return Ok(new
                        {
                            success = false,
                            message = result.Teller_ID,
                            UserDetails = result
                        });
                    }
                    else if (result.User != null)
                    {
                        return Ok(new
                        {
                            success = true,
                            message = result.Teller_ID,
                            UserDetails = result
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "User details not found",
                            UserDetails = result
                        });
                    }
                }
                else
                {
                  
                    var userRecord=_userBusiness.GetUserRecordByID(userId);
                    if(userRecord != null)
                    {
                        return Ok(new
                        {
                            success = true,
                            message = "user details",
                            UserDetails = new UserRecord
                            {
                                CreditAmount = userRecord.CreditAmount,
                                User = userRecord.UserId,
                                TillStatus = userRecord.TillStatus,
                                Teller_ID = userRecord.TellerId,
                                DebitAmount = userRecord.DebitAmount,
                                Time_Opened = userRecord.TimeOpened.ToString(),
                                UserName = userRecord.UserName,
                                UserTillBranch = userRecord.UserTillBranch,
                                UserBranch = "ALL NG00200",
                                LocalCurrencyAmount = userRecord.LocalCurrencyAmount,
                                UserRole = userRecord.UserRole
                            }
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            success = true,
                            message = "user details",
                            UserDetails = new UserRecord
                            {
                                CreditAmount = "1000.00",
                                User = "Muyiwa",
                                TillStatus = "OPEN",
                                Teller_ID = "T102",
                                DebitAmount = "1000.00",
                                Time_Opened = "17:08:53",
                                UserName = "ADEYEMI ADEDEJI",
                                UserBranch = "ALL NG00200",
                                UserTillBranch = "NG0020006",
                                LocalCurrencyAmount = "10000.00",
                                UserRole = "@SBN.FT.ALL"
                            }
                        });
                    }
                   
                }



            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = ex.Message,
                    UserDetails = new UserRecord
                    {
                        CreditAmount = "1000.00",
                        User = "Muyiwa",
                        TillStatus = "OPEN",
                        Teller_ID = "T102",
                        DebitAmount = null,
                        Time_Opened = "17:08:53",
                        UserName = "ADEYEMI ADEDEJI",
                        UserBranch = "ALL NG00200",
                        UserTillBranch = "NG0020006",
                        LocalCurrencyAmount = null,
                        UserRole = "@SBN.FT.INPUT"
                    }
                });
            }

        }

        [HttpGet, Route("api/User/TellerDetails")]
        public IHttpActionResult GetTellerDetails([FromUri]string tellerId, string access_token)
        {
            Utils.LogNO("api/User/TellerDetails:25:Currently Inside This End Point.");
            try
            {
                if (isDemo == "false")
                {
                    string url = System.Configuration.ConfigurationManager.AppSettings["GetTellerDetails"];
                    // tellerId = "ADEDEJIIAD2";
                    url = url + "/" + tellerId;
                    TillDetailsViewModel result = Transaction.GetTellerDetails(url, access_token);
                    if (result.GeTilid == null)
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "Get till is null",
                            UserDetails = result
                        });
                    }
                    else if (result.GeTilid.Record.TELLER_NAME != null)
                    {
                        return Ok(new
                        {
                            success = true,
                            message = "Teller details",
                            TillDetails = new
                            {
                                TellerName = result.GeTilid.Record.TELLER_NAME,
                                TellerBranch = result.GeTilid.Record.TELLER_BRANCH,
                                BranchName = result.GeTilid.Record.BRANCH_NAME
                            }
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            success = true,
                            message = "Teller details",
                            TillDetails = new
                            {
                                TellerName = "",
                                TellerBranch = "",
                                BranchName = ""
                            }
                        });
                    }
                }
                else
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Teller details",
                        TillDetails = new
                        {
                            TellerName = "Muyiwa Aro",
                            TellerBranch = "NG10001",
                            BranchName = "Yaba"
                        }
                    });
                }



            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = ex.Message,
                    UserDetails = new UserRecord
                    {
                        CreditAmount = "1000",
                        User = "Muyiwa",
                        Teller_ID = "T102",
                        DebitAmount = null,
                        Time_Opened = "17:08:53",
                        UserName = "ADEYEMI ADEDEJI",
                        UserTillBranch = "NG0020006",
                        LocalCurrencyAmount = null
                    }
                });
            }

        }


        [HttpGet, Route("api/User/VaultDetails/Vault/{vaultId}/Branch/{branchCode}/Currency/{curAbbrev}/{access_token}")]
        public IHttpActionResult GetVaultDetails(string vaultId,string branchCode, string curAbbrev, string access_token)
        {
            Utils.LogNO("api/User/VaultDetails/Vault/{vaultId}/Branch/{branchCode}/Currency/{curAbbrev}/{access_token}:26:Currently Inside This End Point.");
            try
            {
                if (isDemo == "false")
                {
                    string url = System.Configuration.ConfigurationManager.AppSettings["GetVaultDetails"];
                    // tellerId = "ADEDEJIIAD2";
                    url = url + "/" + branchCode + "/" + vaultId + "/" + curAbbrev;
                    VaultDetailRoot result = Transaction.GetVaultDetails(url, access_token);
                    if (result.GeTilid == null)
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "Get till is null",
                            VaultDetails = result 
                        });
                    }
                    else if (result.GeTilid.Record.TELLERNAME != null)
                    {
                        return Ok(new
                        {
                            success = true,
                            message = "Vault details", 
                            TillDetails = new
                            {
                                TellerName = result.GeTilid.Record.TELLERNAME, 
                                TellerBranch = result.GeTilid.Record.TELLERBRANCH,
                                BranchName = result.GeTilid.Record.BRANCHNAME
                            }
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            success = true,
                            message = "Teller details",
                            TillDetails = new
                            {
                                TellerName = "",
                                TellerBranch = "",
                                BranchName = ""
                            }
                        });
                    }
                }
                else
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Teller details",
                        TillDetails = new
                        {
                            TellerName = "Muyiwa Aro",
                            TellerBranch = "NG10001",
                            BranchName = "Yaba"
                        }
                    });
                }



            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = ex.Message,
                    UserDetails = new UserRecord
                    {
                        CreditAmount = "1000",
                        User = "Muyiwa",
                        Teller_ID = "T102",
                        DebitAmount = null,
                        Time_Opened = "17:08:53",
                        UserName = "ADEYEMI ADEDEJI",
                        UserTillBranch = "NG0020006",
                        LocalCurrencyAmount = null
                    }
                });
            }

        }


        [HttpGet, Route("api/Currency/GetRate/{currAbbrev}/Token/{access_token}")]
        public IHttpActionResult GetRates(string currAbbrev, string access_token)
        {
            Utils.LogNO("api/Currency/GetRate/{currAbbrev}/Token/{access_token}:26:Currently Inside This End Point.");
            try
            {
                if (isDemo == "false")
                {
                    string url = System.Configuration.ConfigurationManager.AppSettings["GetCurrencyRate"];
                    url = url + "/" + currAbbrev;
                    CurrencyRateModel result = Transaction.GetRate(url, access_token);
                    if (result == null)
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "Currency not available",
                            UserDetails = result
                        });
                    }
                    else if (result.GetRates != null)
                    {
                        CurrencyRateModel CurrencyRateModel = new CurrencyRateModel();
                        return Ok(new
                        {
                            success = true,
                            message = "success",
                            CurrencyRateDetails = new
                            {
                                CCY_CODE = result.GetRates.Record.CCY_CODE,
                                NUM_CCY_CODE = result.GetRates.Record.NUM_CCY_CODE,
                                CCY_NAME = result.GetRates.Record.CCY_NAME,
                                CCY_MARKET = result.GetRates.Record.CCY_MARKET,
                                CCY_MID_RATE = result.GetRates.Record.CCY_MID_RATE,
                                CCY_BUY_RATE = result.GetRates.Record.CCY_BUY_RATE,
                                CCY_SELL_RATE = result.GetRates.Record.CCY_SELL_RATE
                            }
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "Rate not found",
                            UserDetails = result
                        });
                    }
                }
                else
                {
                    CurrencyRateModel currencyRateModel = new CurrencyRateModel();
                    currencyRateModel.GetRates = new GetRates();
                    currencyRateModel.GetRates.Record = new Record();
                    currencyRateModel.GetRates.Record.CCY_BUY_RATE = "389.6000";
                    currencyRateModel.GetRates.Record.CCY_MID_RATE = "389.6000";
                    currencyRateModel.GetRates.Record.CCY_SELL_RATE = "389.6000";
                    currencyRateModel.GetRates.Record.CCY_NAME = "Dollar";
                    currencyRateModel.GetRates.Record.CCY_CODE = "USD";
                    return Ok(new
                    {
                        success = true,
                        message = "success",
                        CurrencyRateDetails = new
                        {
                            CCY_CODE = currencyRateModel.GetRates.Record.CCY_CODE,
                            NUM_CCY_CODE = currencyRateModel.GetRates.Record.NUM_CCY_CODE,
                            CCY_NAME = currencyRateModel.GetRates.Record.CCY_NAME,
                            CCY_MARKET = currencyRateModel.GetRates.Record.CCY_MARKET,
                            CCY_MID_RATE = currencyRateModel.GetRates.Record.CCY_MID_RATE,
                            CCY_BUY_RATE = currencyRateModel.GetRates.Record.CCY_BUY_RATE,
                            CCY_SELL_RATE = currencyRateModel.GetRates.Record.CCY_SELL_RATE

                        }
                    });
                }



            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = ex.Message,
                    CurrencyRateDetails = new
                    {
                        CCY_CODE = "",
                        NUM_CCY_CODE = "",
                        CCY_NAME = "",
                        CCY_MARKET = "",
                        CCY_MID_RATE = "",
                        CCY_BUY_RATE = "",
                        CCY_SELL_RATE = ""
                    }
                });
            }

        }

        public static string DetermineCompName(string IP)
        {
            try
            {
                IPAddress myIP = IPAddress.Parse(IP);
                IPHostEntry GetIPHost = Dns.GetHostEntry(myIP);
                List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
                return compName.First();
            }
            catch { return ""; }
        }

        public string GetIPAddress()
        {
            try
            {

                System.Web.HttpContext context = System.Web.HttpContext.Current;
                string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    string[] addresses = ipAddress.Split(',');
                    if (addresses.Length != 0)
                    {
                        return addresses[0];
                    }
                }

                return context.Request.ServerVariables["REMOTE_ADDR"];
            }
            catch
            {
                return HttpContext.Current.Request.Params["HTTP_CLIENT_IP"] ?? HttpContext.Current.Request.UserHostAddress;

            }

        }
        public static string GetLocalIPAddress() // we are ok ......  yes  postman testing
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

    }
}