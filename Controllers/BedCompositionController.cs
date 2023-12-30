using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BaseClass;
using HospitalManagementApi.Models.DaLayer;
using HospitalManagementApi.Models.BLayer;
using HospitalManagementApi.Models.BLayer;

namespace HospitalManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BedCompositionController : Controller
    {
        /// <summary>
        /// CRUD for Bed Composition
        /// </summary>
        /// <param name="bl"></param>        
        /// <returns></returns>
        [HttpPost("crdbedcomposition")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> CUDOperation([FromBody] BlBedComposition bl)
        {
            DlBedComposition dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            bl.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            bl.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            bl.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.CUDOperation(bl);
            if (rb.status)
            {
                rs.message = "Data Saved Successfully";
                rs.status = true;
                rs.value = rb.message;
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
        /// Get bed composition details 
        /// </summary>
        /// <param name="hospitalRegNo"></param>        
        /// <returns></returns>
        [HttpGet("getbedcompdetail/{hospitalRegNo}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetBedCompositionDetail(Int64 hospitalRegNo)
        {
            DlBedComposition dl = new();
            //string clientIP = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetBedCompositionDetail(hospitalRegNo);
            return dt;
        }
    }
}
