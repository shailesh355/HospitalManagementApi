using BaseClass;
using HospitalManagementApi.Models.BLayer;
using HospitalManagementApi.Models.DaLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestimonialController : ControllerBase
    {
        DlTestimonial dlT = new();

        /// <summary>
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("submittestimonial")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> SubmitTestimonial([FromBody] BlTestimonial appParam)
        {
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnBool rb = await dlT.SubmitTestimonial(appParam);
            rs.message = rb.message;
            rs.status = rb.status;
            return rs;
        }

        /// <summary>
        /// </summary>         
        /// <returns></returns>
        [HttpGet("gettestimonials/{actionStatus?}")]
        public async Task<ReturnClass.ReturnDataTable> GetTestimonial(Int16 actionStatus = 0)
        {
            string clientIP = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dlT.GetTestimonial(actionStatus);
            return dt;
        }

        /// <summary>
        /// </summary>         
        /// <returns></returns>
        [HttpGet("gettestimonialbyid/{testimonialId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetTestimonialById(Int64 testimonialId = 0)
        {
            string clientIP = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dlT.GetTestimonialById(testimonialId);
            return dt;
        }

        /// <summary>
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("verifytestimonial")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> VerifyTestimonial([FromBody] BlTestimonialVerification appParam)
        {
            DlDoctor dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.actionDate = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dlT.VerifyTestimonial(appParam);
            rs.message = rb.message;
            rs.status = rb.status;
            return rs;
        }



    }
}
