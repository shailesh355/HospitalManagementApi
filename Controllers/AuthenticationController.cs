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
        /*
         
         SELECT u.userId,u.userRole FROM userlogin u WHERE u.emailId='' AND u.active=1;
         */
        [HttpPost("Checkemailmobile")]
        public async Task<ReturnBool> CheckUserAccountExist([FromBody] UserLoginWithOTP ulr)
        {
            ReturnBool rb = new();
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
                rb.status = false;
                rb.message = "Invalid Login Details.";
                return rb;
            }         
            else if (loginBy == 1)//For Email
            {

                rb.status = false;
                rb.message = "Invalid Login Details.";
                return rb;
            }



            return rb;
        }
        //[HttpPost("sendotpforlogin")]
        //public async Task<ReturnString> SendOtpForLogin([FromBody] UserLoginWithOTP ulr)
        //{
        //    //string captchaVerificationUrl = Utilities.GetAppSettings("CaptchaVerificationURL", "URL").message;
        //    //ReturnBool rb = await dlCommon.VerifyCaptchaAsync(captchaID: ulr.captchaId, userEnteredCaptcha: ulr.userEnteredCaptcha, captchaVerificationUrl);
        //    ////rb.status = true;
        //    //UserLoginResponse userLoginResponse = new();
        //    ReturnString rs = new();
        //    //if (rb.status)
        //    //{
        //    rs = await dl.SendOtpForLogin(ulr.emailId, ulr.id);
        //    if (rs.status)
        //        rs.message = "OTP has been sent!!";
        //    //}
        //    //else
        //    //    rs.message = rb.message;

        //    return rs;
        //}

        //[AllowAnonymous]
        //[HttpPost("authenticatewithotp")]
        //public async Task<UserLoginResponse> AuthenticateWithOTP([FromBody] SendOtp ulr)
        //{
        //    // string captchaVerificationUrl = Utilities.GetAppSettings("CaptchaVerificationURL", "URL").message;
        //    // ReturnBool rb = await dlCommon.VerifyCaptchaAsync(captchaID: ulr.captchaId, userEnteredCaptcha: ulr.userEnteredCaptcha, captchaVerificationUrl);
        //    //rb.status = true;
        //    UserLoginResponse userLoginResponse = new();
        //    //if (rb.status)
        //    //{
        //    LoginTrail ltr = new();
        //    ltr.userAgent = Request.Headers[HeaderNames.UserAgent];
        //    BrowserContext br = Utilities.DetectBrowser(ltr.userAgent);

        //    ltr.clientOs = br.OS;
        //    ltr.clientBrowser = br.BrowserName;
        //    ltr.accessMode = VerifyAppKey(this.HttpContext);
        //    //ltr.loginSource = "";
        //    ltr.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
        //    ltr.logCategory = EventLogCategory.AccountAccess;

        //    userLoginResponse = await dl.CheckUserLoginwithOTP(ulr, ltr);
        //    //if(!userLoginResponse.isLoginSuccessful)
        //    //{
        //    //    UserLoginResponseFailure userLoginResponseFailure = new UserLoginResponseFailure();
        //    //    userLoginResponseFailure.message = userLoginResponse.message;
        //    //    return  userLoginResponseFailure;
        //    //}
        //    //}
        //    //else
        //    //    userLoginResponse.message = rb.message;
        //    return userLoginResponse;
        //}

        //[HttpGet("GenerateRequestToken")]
        //public async Task<ReturnString> GenerateRequestToken()
        //{
        //    return await dlCommon.GenerateRequestToken();
        //}
    }
}
