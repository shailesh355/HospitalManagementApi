using BaseClass;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HospitalManagementApi.Models.BLayer;
using HospitalManagementApi.Models.DaLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace HospitalManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        readonly DlCategory dl = new();

        [HttpGet("getmenulist")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<List<ListValue>> Menu()
        {
            List<ListValue> lv = await dl.GetMenu();
            return lv;
        }
        /// <summary>
        /// Add Category Labels
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("saveupdatecatlabel")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> CUDCatOperation([FromBody] BlMenuLabel appParam)
        {
           
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp =  Utilities.GetRemoteIPAddress(this.HttpContext, true);
            string asd  = User.FindFirst("userId")?.Value;
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            try
            {
                ReturnClass.ReturnBool rb = await dl.CUDCatOperation(appParam);
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
            }
            catch (Exception ex)
            {
            }
            return rs;
        }
    }
}
