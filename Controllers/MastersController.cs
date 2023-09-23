using BaseClass;
using HospitalManagementApi.Models.BLayer;
using HospitalManagementApi.Models.DaLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MastersController : ControllerBase
    {
        ///// <summary>
        ///// City Master
        ///// </summary>
        ///// <param name="bl"></param>        
        ///// <returns></returns>
        //[HttpPost("crdcity")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<ReturnClass.ReturnString> CUDOperation([FromBody] BlCity bl)
        //{
        //    DlMasters dl = new();
        //    ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
        //    bl.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
        //    bl.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
        //    bl.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
        //    ReturnClass.ReturnBool rb = await dl.CUDOperation(bl);
        //    if (rb.status)
        //    {
        //        rs.message = "City Saved Successfully";
        //        rs.status = true;
        //        rs.value = rb.message;
        //    }
        //    else
        //    {
        //        //====Failure====
        //        rs.message = "Failed to save city " + rb.message;
        //        rs.status = false;
        //    }
        //    return rs;
        //}
    }
}
