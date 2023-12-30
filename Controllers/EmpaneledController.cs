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
    public class EmpaneledController : Controller
    {

        /// <summary>
        /// CRUD for Empaneled
        /// </summary>
        /// <param name="bl"></param>        
        /// <returns></returns>
        [HttpPost("crdempaneled")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> CUDOperation([FromBody] BlEmpaneled bl)
        {
            DlEmpaneled dl = new();
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
        /// Get empaneled details 
        /// </summary>
        /// <param name="hospitalRegNo"></param>        
        /// <param name="empaneledTypeId"></param>        
        /// <returns></returns>
        [HttpGet("getempaneleddetail/{hospitalRegNo}/{empaneledTypeId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetEmpaneledDetail(Int64 hospitalRegNo, Int16 empaneledTypeId)
        {
            DlEmpaneled dl = new();
            //string clientIP = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetEmpaneledDetail(hospitalRegNo, empaneledTypeId);
            return dt;
        }

        /// <summary>
        /// CRUD for Empaneled
        /// </summary>
        /// <param name="bl"></param>        
        /// <returns></returns>
        [HttpPost("crddoctorempaneled")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> CUDDoctorOperation([FromBody] BlDoctorEmpaneled bl)
        {
            DlEmpaneled dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            bl.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            bl.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            bl.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.CUDDoctorOperation(bl);
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
        /// Get empaneled details 
        /// </summary>
        /// <param name="doctorRegNo"></param>        
        /// <param name="empaneledTypeId"></param>        
        /// <returns></returns>
        [HttpGet("getdoctorempaneleddetail/{doctorRegNo}/{empaneledTypeId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetDoctorEmpaneledDetail(Int64 doctorRegNo, Int16 empaneledTypeId)
        {
            DlEmpaneled dl = new();
            //string clientIP = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetDoctorEmpaneledDetail(doctorRegNo, empaneledTypeId);
            return dt;
        }

        /// <summary>
        /// CRUD for Provider Empaneled
        /// </summary>
        /// <param name="bl"></param>        
        /// <returns></returns>
        [HttpPost("crdproviderempaneled")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> CUDProviderEmpaneled([FromBody] BlProviderEmpaneled bl)
        {
            DlEmpaneled dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            bl.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            bl.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            bl.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.CUDProviderEmpaneled(bl);
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
        /// Get provider empaneled details 
        /// </summary>
        /// <param name="hospitalRegNo"></param>        
        /// <param name="empaneledTypeId"></param>        
        /// <param name="empaneledId"></param>        
        /// <returns></returns>
        [HttpGet("getproviderempaneleddetail/{hospitalRegNo}/{empaneledTypeId}/{empaneledId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GeProviderEmpaneledDetail(Int64 hospitalRegNo, Int16 empaneledTypeId, Int32 empaneledId)
        {
            DlEmpaneled dl = new();
            //string clientIP = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetProviderEmpaneledDetail(hospitalRegNo, empaneledTypeId, empaneledId);
            return dt;
        }


        /// <summary>
        /// CRUD for Empaneled
        /// </summary>
        /// <param name="bl"></param>        
        /// <returns></returns>
        [HttpPost("crdempaneledinsurance")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> CUDEmpInsOperation([FromBody] BlEmpaneledIns bl)
        {
            DlEmpaneled dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            bl.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            bl.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            bl.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.CUDEmpInsOperation(bl);
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
        /// </summary>
        /// <param name="hospitalRegNo"></param>        
        /// <param name="empaneledTypeId"></param>        
        /// <returns></returns>
        [HttpGet("getempaneledinsurance/{hospitalRegNo}/{empaneledTypeId}")]
        public async Task<ReturnClass.ReturnDataSet> GetHospitalPreview(Int64 hospitalRegNo, Int16 empaneledTypeId)
        {
            DlEmpaneled dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            ReturnClass.ReturnDataSet ds = await dl.GetEmpaneledProviderDetail(hospitalRegNo, empaneledTypeId);
            return ds;
        }

    }
}
