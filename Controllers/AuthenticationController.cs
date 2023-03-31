using HospitalManagementApi.Models.Balayer;
using HospitalManagementApi.Models.DaLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;
using System.Net;
using System.IO;
using BaseClass;
using ceiPortalApi.Models.Blayer.UserAgent;

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
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool(); // dc.VerifyCaptcha(ct, captcha_verification_url);
            rb.status = true;
            if (rb.status)
            {
                LoginTrail lt = new LoginTrail();
                UserAgent ua = new UserAgent(Request.Headers["User-Agent"]);
                lt.userAgent = Request.Headers["User-Agent"];
                lt.clientBrowser = ua.Browser.Name + " " + ua.Browser.Version;
                lt.clientOs = ua.OS.Name + " " + ua.OS.Version;

                lt.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
                ReturnClass.ReturnDataTable dt = new ReturnClass.ReturnDataTable();
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
    }
}
