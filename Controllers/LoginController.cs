using Aadyam.SDS.API.Filters;
using Aadyam.SDS.API.Models;
using Aadyam.SDS.Business.BusinessConstant;
using Aadyam.SDS.Business.Model.Login;
using Aadyam.SDS.Business.Model.User;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;

namespace Aadyam.SDS.API.Controllers
{
    public class LoginController : BaseApiController
    {
        [HttpPost]
        [Route("Login/CreateLogs")]
        public IHttpActionResult CreateLogs([FromBody]LoggerModel model)
        {
            LoggerModel _LoggerModel = new LoggerModel();
            _unitOfWork.GetLoggerInstance.LogException(model);
            return Ok();
        }

        [HttpPost]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("Login/UpdatePassword")]
        public IHttpActionResult UpdatePassword([FromBody]UserModel model)
        {
            var result = _unitOfWork.loginRepository.ChangePassword(model);

            if (result != null)
                return Ok(result);

            return BadRequest("-1");
        }

        //forgot password reset
        [HttpPost]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("Login/UpdateforgotPassword")]
        public IHttpActionResult UpdateforgotPassword([FromBody]ResetPasswordModel model)
        {
            int result = _unitOfWork.loginRepository.ChangeforgotPassword(model);

            if (result > 0)
                return Ok(result);

            return BadRequest("-1");
        }

        decimal LogId = 0;

        // Send OTP SMS for activate user/Login user first time only       
        [HttpPost]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("Login/SendOTPSMS")]
        public IHttpActionResult SendOTPSMS([FromBody]UserModel user)
        {
            if (user == null)
                return BadRequest(BusinessCont.InvalidClientRqst);
            Task<LoginDetails> accessToken = null;
            LoginDetails loginDetails = null;
            try
            {
                loginDetails = new LoginDetails();
                loginDetails = _unitOfWork.loginRepository.SendOTPSMS(user);
                if (loginDetails.UserInfo != null)
                {
                    loginDetails.UserInfo.Source = BusinessCont.MobSource;
                    if (loginDetails != null)
                    {
                        // Create & Return the access token which contains JWT and Refresh Token
                        accessToken = CreateAccessToken(loginDetails, "");
                        accessToken.Result.Status = BusinessCont.SuccessStatus;
                        return Ok(new { authToken = accessToken });
                    }
                }
                else
                {
                    accessToken = NULLObject();
                    accessToken.Result.Status = BusinessCont.FailStatus;
                    accessToken.Result.ExMsg = BusinessCont.UnauthorizedUser;
                }
            }
            catch (Exception ex)
            {
                loginDetails.Status = BusinessCont.FailStatus;
                loginDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "SendOTPSMS", "MobileNo= " + user.MobileNo, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(new { authToken = accessToken });
        }

        private async Task<LoginDetails> NULLObject()
        {
            LoginDetails loginDetails = new LoginDetails();

            return new LoginDetails()
            {
                UserInfo = null,
                Token = "",
                expiration = DateTime.Now,
                refresh_token = ""
            };
        }

        [HttpGet]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("Login/ADSLoginVerification/{Username}/{Password}")]
        public IHttpActionResult ADSLoginVerification(string UserName, string Password)
        {
            string result = null;
            try
            {
                result = _unitOfWork.UserRepository.OfficerValidate(UserName,Password);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "ADSLoginVerification", "UserName= " + UserName + "  Password= " + Password, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(result);
        }

        #region Create New Token From Another Server 
        [HttpPost]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("Login/GenerateTokenByLogin")]
        public async Task<IHttpActionResult> GenerateTokenByLogin([FromBody]UserModel user)
        {
            // We will return Generic 500 HTTP Server Status Error
            // If we receive an invalid payload
            if (user == null)
            {
                return InternalServerError(); // Error 500
            }

            switch (user.GrantType)
            {
                case "password":
                    return await GenerateNewToken(user, user.UserDtls,BusinessCont.LoginFromSDS);
                case "refresh_token":
                    return await RefreshToken(user, user.UserDtls,BusinessCont.LoginFromSDS);
                default:
                    // not supported - return a HTTP 401 (Unauthorized)
                    return Unauthorized();
            }
        }
        #endregion

        #region New Refresh Token Based Authentication
        [HttpPost]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("Login/LoginDetails2")]
        public async Task<IHttpActionResult> LoginDetails2([FromBody]UserModel user)
        {
            try
            {
                switch (user.GrantType)
                {
                    case "password":
                        return await GenerateNewToken(user, null, BusinessCont.LoginFromSDS);
                    case "refresh_token":
                        return await RefreshToken(user, null, BusinessCont.LoginFromSDS);
                    default:
                        // not supported - return a HTTP 401 (Unauthorized)
                        return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "LoginDetails2", "UserName- " + user.Username + ", Password- " + user.Password, BusinessCont.FailStatus, ex.Message);
                throw ex;
            }
        }
                
        // Method to Create New JWT and Refresh Token
        private async Task<IHttpActionResult> GenerateNewToken(UserModel model, LoginAuth UserDtls,string User)
        {
            // check if there's an user with the given username
            //await
            LoginDetails loginDetails = new LoginDetails();
            try
            {
                if (UserDtls == null)
                {
                    if (User == BusinessCont.LoginFromSDS)
                        loginDetails = _unitOfWork.UserRepository.GetLoginDetails(model.Username, model.Password);
                    else
                        loginDetails = _unitOfWork.loginRepository.CDCMSLoginDetails(model.DistCode, model.ProfileId);
                }
                else
                {
                    loginDetails.UserInfo = UserDtls;
                    loginDetails.Status = BusinessCont.SuccessStatus;
                }

                // Validate credentials
                if (loginDetails.UserInfo != null)
                {
                    if (loginDetails.UserInfo.ActiveStatus == "Y")
                    {
                        // username & password matches: create the refresh token
                        var newRtoken = CreateRefreshToken(WebConfigSettings.ClientIdSecret, loginDetails.UserInfo.UserId);
                       
                        // Delete Previous if exists and Add new refresh token to Database
                        _unitOfWork.UserRepository.CreateAfetrDeletingRefreshToken(newRtoken);

                        // Create & Return the access token which contains JWT and Refresh Token
                        var accessToken = await CreateAccessToken(loginDetails, newRtoken.RefreshValue);
                        return Ok(new { authToken = accessToken });
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "GenerateNewToken", null, BusinessCont.FailStatus, ex.Message);
            }
            return Unauthorized();
        }

        // Create access Tokenm
        private async Task<LoginDetails> CreateAccessToken(LoginDetails loginDetails, string refreshToken)
        {
            string UserName = "";
            int RoleId = 0;
            string DisplayName = String.IsNullOrEmpty(loginDetails.UserInfo.DisplayName) ? "" : loginDetails.UserInfo.DisplayName;
            double tokenExpiryTime = Convert.ToDouble(WebConfigSettings.WebTokenExpireTimeinHrs);
            if (loginDetails.UserInfo.Source == BusinessCont.MobSource)
            {
                UserName = loginDetails.UserInfo.StaffRefNo;
                RoleId = loginDetails.UserInfo.RoleId;
                tokenExpiryTime = WebConfigSettings.MobileTokenExpireTimeinHrs;
            }
            if (loginDetails.UserInfo.Source == BusinessCont.WebSource)
            {
                UserName = loginDetails.UserInfo.UserName;
                RoleId = loginDetails.UserInfo.RoleId;
                tokenExpiryTime = WebConfigSettings.WebTokenExpireTimeinHrs;
            }

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(WebConfigSettings.JwtSecretKey));

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.NameIdentifier, UserName),
                        new Claim(ClaimTypes.Role,RoleId.ToString()),
                        new Claim("LoggedOn", DateTime.Now.ToString()),
                        new Claim("DisplayName", loginDetails.UserInfo.DisplayName!=null? loginDetails.UserInfo.DisplayName:""),
                        new Claim("RefNo", loginDetails.UserInfo.RefNo!=null? loginDetails.UserInfo.RefNo:""),
                        new Claim("RoleId", RoleId.ToString())
                     }),

                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                Issuer = WebConfigSettings.AppDomain,
                Audience = WebConfigSettings.AppDomain,
                Expires = DateTime.UtcNow.AddHours(tokenExpiryTime)
            };

            // Generate token
            var newtoken = tokenHandler.CreateToken(tokenDescriptor);
            var encodedToken = tokenHandler.WriteToken(newtoken);

            return new LoginDetails()
            {
                UserInfo = loginDetails.UserInfo,
                Token = encodedToken,
                expiration = newtoken.ValidTo,
                refresh_token = refreshToken
            };
        }
        private async Task<IHttpActionResult> MobileGenerateNewToken(LoginDetails loginDetails)
        {
            // Validate credentials
            if (loginDetails != null)
            {
                // username & password matches: create the refresh token
                var newRtoken = CreateRefreshToken(WebConfigSettings.ClientIdSecret, loginDetails.UserInfo.UserId);
                // Create & Return the access token which contains JWT and Refresh Token
                var accessToken = await CreateAccessToken(loginDetails, newRtoken.RefreshValue);
                return Ok(new { authToken = accessToken });
            }
            return Unauthorized();
        }


        private RefreshToken CreateRefreshToken(string clientId, long userId)
        {
            return new RefreshToken()
            {
                ClientKey = clientId,
                UserId = userId,
                RefreshValue = Guid.NewGuid().ToString("N"),
                CreatedDate = DateTime.UtcNow,
                ExpiryTime = DateTime.UtcNow.AddHours(WebConfigSettings.MobileTokenExpireTimeinHrs)
            };
        }

        // Method to Refresh JWT and Refresh Token
        private async Task<IHttpActionResult> RefreshToken(UserModel model, LoginAuth UserDtls,string User)
        {
            
            try
            {
                LoginDetails userDetails = new LoginDetails();
                // check if the received refreshToken exists for the given clientId                
                if (UserDtls == null)
                {
                    if (User==BusinessCont.LoginFromSDS)
                        userDetails = _unitOfWork.UserRepository.GetLoginDetails(model.Username, model.Password);
                    else
                        userDetails = _unitOfWork.loginRepository.CDCMSLoginDetails(model.DistCode, model.ProfileId);
                }
                else
                {
                    userDetails.UserInfo = UserDtls;
                    userDetails.Status = BusinessCont.SuccessStatus;
                }
                if (userDetails == null)
                {
                    //if user doesnt exists
                    return Unauthorized();
                }

                var resultUserRT = new RefreshToken();
                if (userDetails != null)
                {
                    //if user exists, check if refresh toen exists or not
                    resultUserRT = _unitOfWork.UserRepository.GetRefreshTokenByUserId(Convert.ToInt32(userDetails.UserInfo.UserId));
                }

                if (resultUserRT == null)
                {
                    // refresh token not found or invalid (or invalid clientId)
                    return Unauthorized();
                }

                // check if refresh token is expired
                if (resultUserRT.ExpiryTime < DateTime.UtcNow)
                {
                    return Unauthorized();
                }

                //if resultUserRT!=null
                // generate a new refresh token 
                var rtNew = CreateRefreshToken(resultUserRT.ClientKey, resultUserRT.UserId);

                // Delete Previous if exists and Add new refresh token to Database
                _unitOfWork.UserRepository.CreateAfetrDeletingRefreshToken(rtNew);

                var response = await CreateAccessToken(userDetails, rtNew.RefreshValue);

                return Ok(new { authToken = response });

            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }
        #endregion

        #region Test apis
        //Test       
        [Route("Login/AllowAnonymous")]
        [AllowAnonymous]
        [OverrideAuthorization]
        [HttpGet]
        public IHttpActionResult AllowAnonymous()
        {
            return Ok("AllowAnonymous");
        }

        [Route("Login/Authorize1")]
        [HttpGet]
        public IHttpActionResult Authorize1()
        {
            return Ok("Authorize1");
        }

        [Route("Login/Authorize2")]
        [HttpPost]
        public IHttpActionResult Authorize2(UserModel model)
        {
            _unitOfWork.loginRepository.TestExceptionFilter();
            return Ok("Authorize(Roles =2, 1");
        }

        [Route("Login/Authorize3")]
        [HttpPost]
        public IHttpActionResult Authorize3(UserModel model)
        {
            return Ok("Authorize3");
        }
        #endregion

        #region Create Login
        //creating Distributor Login
        [HttpPost]
        [Route("Login/CreateLogin")]
        public IHttpActionResult CreateLogin([FromBody]List<UserModelForActivation> _user)
        {
            if (_user == null)
                return BadRequest(BusinessCont.InvalidClientRqst);
            DistributorDetails _details = null;
            try
            {
                _details = new DistributorDetails();
                _details.LoginId = _unitOfWork.adminRepository.CreateLogin(_user);
            }
            catch (Exception ex)
            {
                _details.Status = BusinessCont.FailStatus;
                _details.ExMsg = ex.Message;
            }
            return Ok(_details);
        }


        //creating Distributor SDS Issue tracker Login
        [HttpPost]
        [Route("Login/CreateLoginforSDSIssueTracker")]
        public IHttpActionResult CreateLoginforSDSIssueTracker([FromBody]List<UserModelForActivation> _user)
        {
            if (_user == null)
                return BadRequest(BusinessCont.InvalidClientRqst);
            DistributorDetails _details = null;
            try
            {
                _details = new DistributorDetails();
                _details.LoginId = _unitOfWork.adminRepository.CreateLoginForSDSIssueTracker(_user);
            }
            catch (Exception ex)
            {
                _details.Status = BusinessCont.FailStatus;
                _details.ExMsg = ex.Message;
            }
            return Ok(_details);
        }
        #endregion

        //to get the UserDetails by UserName
        [HttpPost]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("Login/GetUserDetailsByUserName")]
        public IHttpActionResult GetUserDetailsByUserName([FromBody] UserModel user)
        {
            LoginDetails GetUserModel = new LoginDetails();
            try
            {
                GetUserModel = _unitOfWork.loginRepository.GetUserDetailsByUsername(user.Username);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, 0, 0, "GetUserDetailsByUserName", "Username= " + user.Username, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(GetUserModel);
        }

        //To send forgot password link thru email
        [HttpPost]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("Login/SendEmail")]
        public LoginDetails SendEmail(EmailSend emailTo)
        {
            EmailSend model = new EmailSend();
            LoginDetails loginDetails = null;
            loginDetails = new LoginDetails();
            int ResetId = 0;
            try
            {
                ResetId = _unitOfWork.loginRepository.SaveResetPasswordLog(emailTo, 1);
                if (ResetId > 0)
                //saving reset password details
                {
                    model = _unitOfWork.loginRepository.sendEmailViaWebApi(emailTo, ResetId);
                    if (model != null)
                    {
                        loginDetails.Status = model.Status;
                        loginDetails.ExMsg = model.Exceptionmsg;
                    }
                    else
                    {
                        loginDetails.ExMsg = BusinessCont.UnauthorizedUser;
                        loginDetails.ExMsg = model.Exceptionmsg;
                        loginDetails.Status = model.Status;
                    }
                }
            }
            catch (Exception ex)
            {
                loginDetails.Status = BusinessCont.FailStatus;
                loginDetails.ExMsg = ex.Message;
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "SendEmail", null, BusinessCont.FailStatus, ex.Message);
            }
            return loginDetails;
        }


        //to get Reset password details
        [HttpGet]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("Login/GetResetPasswordLog/{ResetId}")]
        public ResetPasswordModel GetResetPasswordLog(int ResetId)
        {
            ResetPasswordModel resetPasswordModel = new ResetPasswordModel();
            try
            {
                resetPasswordModel = _unitOfWork.loginRepository.GetResetPasswordLog(ResetId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "GetResetPasswordLog", "ResetId= " + ResetId, BusinessCont.FailStatus, ex.Message);
            }
            return resetPasswordModel;
        }

        [HttpPost]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("Login/CDCMSLogin")]
        public async Task<IHttpActionResult> CDCMSLogin([FromBody]UserModel user)
        {
            if (user == null)
                return InternalServerError();

            switch (user.GrantType)
            {
                case "password":
                    return await GenerateNewToken(user, null, BusinessCont.LoginFromCDCMS);
                case "refresh_token":
                    return await RefreshToken(user, null,BusinessCont.LoginFromCDCMS);
                default:
                    return Unauthorized();
            }
        }

        [HttpPost]
        [Route("Login/SendNotification")]
        public void SendNotification([FromBody]Notification notification)
        {
            try
            {                
                _unitOfWork.loginRepository.SendNotification(notification.DistributorId, notification.StaffRefNo, notification.Title, notification.Message);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, notification.DistributorId, 0, 0, "SendNotification", null, BusinessCont.FailStatus, ex.Message);
            }
        }

        [HttpGet]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("Login/Login1")]
        public HttpResponseMessage GetAntiForgeryToken()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            HttpCookie cookie = HttpContext.Current.Request.Cookies["X-CSRF-TOKEN"];

            string cookieToken;
            string formToken;
            AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;
            AntiForgery.GetTokens(cookie == null ? "" : cookie.Value, out cookieToken, out formToken);

            AntiForgeryTokenModel content = new AntiForgeryTokenModel
            {
                AntiForgeryToken = formToken
            };

            response.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            
            if (!string.IsNullOrEmpty(cookieToken))
            {
                
                response.Headers.Add("X-CSRF-TOKEN", cookieToken);
                response.Headers.Add("withCredentials","true");

               // response.Headers.Server.
                response.Headers.AddCookies(new[]
                {
                    new CookieHeaderValue("X-CSRF-TOKEN", cookieToken)
                    {
                        Expires = DateTimeOffset.Now.AddMinutes(10),
                        Path = "/"
                    }
                });
            }

            return response;
        }

        [HttpPost]
        [Route("Login/GetAppConfiguration")]
        public List<AppConfiguration> GetAppConfiguration([FromBody]AppConfiguration Config)
        {
            return _unitOfWork.loginRepository.GetAppConfiguration(Config.Key);
        }

        [HttpPost]
        [Route("Login/AddUpdateApiconfigurationMaster")]
        public IHttpActionResult AddUpdateAppconfigurationMaster([FromBody]AppConfiguration Appconfiguration)
        {
            if (Appconfiguration == null)
                return BadRequest(BusinessCont.InvalidClientRqst);
            try
            {
                Appconfiguration.Id = _unitOfWork.loginRepository.AddApiconfigurationMaster(Appconfiguration);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "AddUpdateApiconfigurationMaster", null, BusinessCont.FailStatus, ex.Message);
            }
            return Ok(Appconfiguration);
        }

        [HttpPost]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("Login/SaveLog")]
        public SaveLogModel SaveLog([FromBody]SaveLogModel LogDtls)
        {
            return _unitOfWork.loginRepository.SaveLog(LogDtls.saveLogStr);
        }

        [ValidateAntiForgeryTokenFilter]
        [HttpPost]
        [Route("login")]
        public HttpResponseMessage Login(UserModel model)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpPost]
        [OverrideAuthorization]
        [AllowAnonymous]
        [Route("Login/SendSMS")]
        public string SendSMS([FromBody]UserModel user)
        {
            return _unitOfWork.loginRepository.SendSMS(user);
        }

        [HttpPost]
        [Route("Login/QueryBuilder")]
        [AllowAnonymous]
        [OverrideAuthorization]
        public int QueryBuilder(QueryBuilder queryBuilder)
        {
            int RetValue = 0, AppConfigAPITimeout = 0;
            var AppConfig = BusinessCont.GetAppConfiguration();
            AppConfigAPITimeout = Convert.ToInt32(AppConfig.Where(a => a.Key == BusinessCont.AppConfigAPITimeout).Select(a => a.Value).FirstOrDefault());
            RetValue = _unitOfWork.adminRepository.QueryBuilder(queryBuilder.Query, AppConfigAPITimeout);
            return RetValue;
        }

        #region Issue Tracker Login Session
        [HttpPost]
        [Route("Login/CreateIssueTrackerSession")]
        public IssueTrackerLogin CreateIssueTrackerSession([FromBody]IssueTrackerLogin _model)
        {
            IssueTrackerLogin model = new IssueTrackerLogin();
            try
            {
                model.ProfileId = "";
                model.ProfileId = _unitOfWork.loginRepository.CreateIssueTrackerLoginSession(_model);

                model.Status= BusinessCont.SuccessStatus;
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(LogId, 0, 0, 0, 0, "CreateIssueTrackerSession", null, BusinessCont.FailStatus, ex.Message);
                model.Status = BusinessCont.FailStatus;
            }
            return model;
        }
        #endregion

    }
}
