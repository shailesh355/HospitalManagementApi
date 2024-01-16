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
    public class HospitalSpecializationController : ControllerBase
    {
        /// <summary>
        /// CRUD for Specialization
        /// </summary>
        /// <param name="bl"></param>        
        /// <returns></returns>
        [HttpPost("crdhospitalspec")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> CUDOperation([FromBody] BlHospitalSpecialization bl)
        {
            DlHospitalSpecialization dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            bl.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            bl.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            bl.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.CUDOperation(bl);
            if (rb.status)
            {
                rs.message = rb.message;
                rs.status = true;
                rs.value = rb.value;
            }
            else
            {
                //====Failure====
                rs.message = "Failed to save data " + rb.message;
                rs.status = false;
            }
            return rs;
        }

        /// <summary>
        /// Get Hosptal Specialization details 
        /// </summary>
        /// <param name="hospitalRegNo"></param>        
        /// <returns></returns>
        [HttpGet("gethospitalspecdetail/{hospitalRegNo}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetHospitalSpecDetail(Int64 hospitalRegNo)
        {
            DlHospitalSpecialization dl = new();
            //string clientIP = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetHospitalSpecDetail(hospitalRegNo);
            return dt;
        }

       
    }
}
