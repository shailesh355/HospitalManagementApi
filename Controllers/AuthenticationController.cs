using HospitalManagementApi.Models.Balayer;
using HospitalManagementApi.Models.DaLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaseClass;
using ceiPortalApi.Models.Blayer.UserAgent;
using static BaseClass.ReturnClass;
using System.Text.RegularExpressions;
using HospitalManagementApi.Models.BLayer;
namespace HospitalManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        DlCommon dlCommon = new DlCommon();
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userParam)
        {
            Utilities util = new Utilities();
            CaptchaReturnType ct = new CaptchaReturnType();
            DlCommon dc = new DlCommon();
            //ct.captchaID = userParam.captchaId;
            //ct.userEnteredCaptcha = userParam.userEnteredCaptcha;
            // string captcha_verification_url = util.GetAppSettings("CaptchaVerificationURL", "URL").message;
            ReturnBool rb = new ReturnBool(); // dc.VerifyCaptcha(ct, captcha_verification_url);
            rb.status = true;
            if (rb.status)
            {
                LoginTrail lt = new LoginTrail();
                UserAgent ua = new UserAgent(Request.Headers["User-Agent"]);
                lt.userAgent = Request.Headers["User-Agent"];
                lt.clientBrowser = ua.Browser.Name + " " + ua.Browser.Version;
                lt.clientOs = ua.OS.Name + " " + ua.OS.Version;
                lt.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
                //ReturnClass.ReturnDataTable dt = new ReturnClass.ReturnDataTable();
                DlAuthentication auth = new DlAuthentication();
                User user = await auth.AuthenticateUser(userParam.emailId, userParam.password, lt);
                if (user == null)
                {
                    return Ok(new
                    {
                        message = "Email ID or Password is incorrect",
                        Active = "false"
                    });
                }
                return Ok(user);
            }
            else
                return Ok(new
                {
                    message = "Invalid Captcha!!! Please enter correct captcha value",
                    Active = "false"
                });
        }
        [HttpGet("GenerateRequestToken")]
        public async Task<ReturnString> GenerateRequestToken()
        {
            return await dlCommon.GenerateRequestToken();
        }
        [HttpPost("Checkemailmobile")]
        public async Task<UserOTPResponse> CheckUserAccountExist([FromBody] UserLoginWithOTP ulr)
        {
            ReturnString rb = new();
            ReturnBool rb1 = new();
            DlAuthentication dl = new DlAuthentication();
            UserOTPResponse userOTPResponse = new();
            UserResponse user = new UserResponse();
            userOTPResponse.isOTPSent = false;
            rb1 = await dlCommon.VerifyRequestToken(ulr.requestToken);
            //rb1.status = true;
            if (!rb1.status)
            {
                userOTPResponse.isOTPSent = false;
                userOTPResponse.isUserValid = false;
                userOTPResponse.message = "Invalid Request Token.";
                return userOTPResponse;

            }
            userOTPResponse.isUserValid = true;
            Int16 loginBy = 0;//Email=1,mobile=2,other=0;
            Match match = Regex.Match(ulr.emailId,
                        @"^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+$",
                        RegexOptions.IgnoreCase);
            if (match.Success)
                loginBy = 1;
            else
            {
                match = Regex.Match(ulr.emailId,
                                      @"^[6-9]\d{9}$", RegexOptions.IgnoreCase);
                if (match.Success)
                    loginBy = 2;
            }
            if (loginBy == 0)
            {
                userOTPResponse.isOTPSent = false;
                userOTPResponse.message = "Invalid Login Details.";
                return userOTPResponse;
            }
            else if (loginBy == 1)//For Email            
                user = await dl.CheckEmail(ulr.emailId!);

            else if (loginBy == 2)//For Mobile            
                user = await dl.CheckMobile(ulr.emailId!);

            if ((bool)user.isAuthenticated!)
            {
                SendOtp sendOtp = new SendOtp();
                sendOtp.emailId = user.emailId;
                sendOtp.mobileNo = Convert.ToInt64(user.mobileNo);
                sendOtp.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
                rb = await dlCommon.SendOTP(sendOtp);
                if (rb.status)
                {
                    userOTPResponse.isOTPSent = true;
                    userOTPResponse.emailId = user.emailId;
                    userOTPResponse.mobileNo = user.mobileNo;
                    userOTPResponse.msgId = rb.msg_id;
                    userOTPResponse.message = rb.any_id;
                    userOTPResponse.otpCounter = rb.value;
                }
            }
            return userOTPResponse;
        }

        [HttpPost("authenticatewithotp")]
        public async Task<IActionResult> AuthenticateWithOTP([FromBody] SendOtp userParam)
        {
            Utilities util = new Utilities();
            CaptchaReturnType ct = new CaptchaReturnType();
            DlCommon dc = new DlCommon();
            ReturnBool rb = new ReturnBool();
            rb = await dc.VerifyRequestToken(userParam.requestToken);
            //ct.captchaID = userParam.captchaId;
            //ct.userEnteredCaptcha = userParam.userEnteredCaptcha;
            // string captcha_verification_url = util.GetAppSettings("CaptchaVerificationURL", "URL").message;
            // dc.VerifyCaptcha(ct, captcha_verification_url);
            //rb.status = true;
            if (rb.status)
            {
                LoginTrail lt = new LoginTrail();
                UserAgent ua = new UserAgent(Request.Headers["User-Agent"]);
                lt.userAgent = Request.Headers["User-Agent"];
                lt.clientBrowser = ua.Browser.Name + " " + ua.Browser.Version;
                lt.clientOs = ua.OS.Name + " " + ua.OS.Version;
                lt.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
                //ReturnClass.ReturnDataTable dt = new ReturnClass.ReturnDataTable();
                DlAuthentication auth = new DlAuthentication();
                User user = await auth.AuthenticateUserByOTP(userParam.emailId!, userParam.mobileNo.ToString()!, userParam.OTP.ToString()!, userParam.msgId!, lt);
                if (user == null)
                {
                    return Ok(new
                    {
                        message = "Email ID or Password is incorrect",
                        Active = "false"
                    });
                }
                return Ok(user);
            }
            else
                return Ok(new
                {
                    //message = "Invalid Captcha!!! Please enter correct captcha value",
                    message = "Invalid Request Token..",
                    Active = "false"
                });
        }


    }
}
