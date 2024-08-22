using HospitalManagementApi.Models.Balayer;
using HospitalManagementApi.Models.DaLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaseClass;
using ceiPortalApi.Models.Blayer.UserAgent;
using static BaseClass.ReturnClass;
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
            Utilities util=new Utilities();   
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
        //[HttpPost("Checkemailforlogin")]
        //public async Task<ReturnBool> CheckUserAccountExist([FromBody] UserLoginWithOTP ulr)
        //{
        //    Utilities util = new Utilities();
        //    //string captchaVerificationUrl = Utilities.GetAppSettings("CaptchaVerificationURL", "URL").message;
        //    ReturnBool rbBuild = util.GetAppSettings("Build", "Version");
        //    string buildType = rbBuild.message;
        //    string accessPath = "URL";
        //   // string captchaVerificationUrl = util.GetAppSettings("CaptchaVerificationURL", buildType, accessPath).message;
        //    ReturnBool rb = new(); //await dlCommon.VerifyCaptchaAsync(captchaID: ulr.captchaId, userEnteredCaptcha: ulr.userEnteredCaptcha, captchaVerificationUrl);
        //    rb.status = true;
        //    UserLoginResponse userLoginResponse = new();
        //    if (rb.status)
        //    {
        //        rb.status = false;
        //        ReturnDataTable dt1 = await dl.CheckUserAccountForLogin(ulr.emailId);
        //        if (dt1.status)
        //        {
        //            // Block Industrialist login
        //            //if (ulr.emailId.ToLower().Trim() == "abhishek96")
        //            //{
        //            //    rb.message = "Valid Email Id";
        //            //    rb.status = true;
        //            //    rb.message1 = dt1.table.Rows[0]["role_id"].ToString();
        //            //    rb.value = dt1.table.Rows[0]["userId"].ToString();
        //            //    rb.error = dt1.table.Rows[0]["isUserMigrate"].ToString();
        //            //}
        //            //else if (dt1.table.Rows[0]["role_id"].ToString().Trim() == "04")
        //            //{
        //            //    rb.message = "Unauthorized User, only Department User can login.";
        //            //    rb.status = false;
        //            //    rb.value = "401";
        //            //}
        //            //else
        //            //{
        //            rb.message = "Valid Email Id";
        //            rb.status = true;
        //            rb.message1 = dt1.table.Rows[0]["role_id"].ToString();
        //            rb.value = dt1.table.Rows[0]["userId"].ToString();
        //            rb.error = dt1.table.Rows[0]["isUserMigrate"].ToString();
        //            // }

        //        }
        //        else
        //            rb.message = "Invalid User Id";

        //    }
        //    else
        //        rb.message = rb.message;

        //    return rb;
        //}
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
