using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BaseClass;
using HospitalManagementApi.Models.DaLayer;
using HospitalManagementApi.Models.BLayer;

namespace HospitalManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HodManagmentController : ControllerBase
    {
        /// <summary>
        /// Insert service registration application
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("savehodregistration")]
      // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> SaveHODRegistration([FromBody] BlHod appParam)
        {
            DlHod dl = new DlHod();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.registrationYear = Convert.ToInt32(DateTime.Now.Year.ToString());
            appParam.userId = 0;//Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnBool rb = await dl.RegistorNewHodOffice(appParam);
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
        ///Get All HOD List
        /// </summary>         
                         
        /// <returns></returns>
        [HttpGet("getallhodlist/{vid?}/{rid?}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetAllHODList(Int16 vid=0, Int16 rid = 0)
        {
            DlHod dl = new DlHod();
            //string clientIP = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetAllHODList(vid,rid);
            return dt;
        }
        /// <summary>
        ///Get All HOD List
        /// </summary> 
        /// <returns></returns>
        [HttpGet("getallhodlistbyid/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetAllHODListById(Int64 Id)
        {
            DlHod dl = new DlHod();
            //string clientIP = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetAllHODListById(Id);
            return dt;
        }
        /// <summary>
        /// Verification of HOD registration application
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("verifyhodregistration")]
         [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> VerifyHODRegistration([FromBody] Verification appParam)
        {
            DlHod dl = new DlHod();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);            
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.date = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.VerifyHodOffice(appParam);
            if (rb.status)
            {
                rs.message = "Successfully Verified";
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
        ///Save Ticket Type
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("savetickettype")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> SaveTicketType([FromBody] BlTicketType appParam)
        {
            DlHod dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);          
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.SaveTicketType(appParam);
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
        ///Update Ticket Type
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("updatetickettype")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> UpdateTicketType([FromBody] BlTicketType appParam)
        {
            DlHod dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);            
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.UpdateTicketType(appParam);
            if (rb.status)
            {
                rs.message = "Data Updated Successfully";
                rs.status = true;
                rs.value = rb.message;
            }
            else
            {
                //====Failure====
                rs.message = "Failed to Update data " + rb.message;
                rs.status = false;
            }
            return rs;
        }
        /// <summary>
        ///Get All Ticket Type List
        /// </summary>         

        /// <returns></returns>
        [HttpGet("getalltickettype/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetAllTicketTypeList(Int32 Id)
        {
            DlHod dl = new();
            //string clientIP = Utilities.GetRemoteIPAddress(this.HttpContext, true);
           // Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetAllTicketType(Id);
            return dt;
        }
        /// <summary>
        ///Get All Ticket Type List
        /// </summary> 
        /// <returns></returns>
        [HttpGet("gettickettypebyid/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetAllTicketTypeById(Int32 Id)
        {
            DlHod dl = new();
            //string clientIP = Utilities.GetRemoteIPAddress(this.HttpContext, true);
          //  Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetTicketTypeById(Id);
            return dt;
        }


        /// <summary>
        ///Save Ticket Category
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("saveticketcategory")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> SaveTicketCategory([FromBody] BlCategory appParam)
        {
            DlHod dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.SaveTicketCategory(appParam);
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
        ///Update Ticket Category
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("updateticketcategory")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> UpdateTicketCategory([FromBody] BlCategory appParam)
        {
            DlHod dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.UpdateTicketCategory(appParam);
            if (rb.status)
            {
                rs.message = "Data Updated Successfully";
                rs.status = true;
                rs.value = rb.message;
            }
            else
            {
                //====Failure====
                rs.message = "Failed to Update data " + rb.message;
                rs.status = false;
            }
            return rs;
        }

        /// <summary>
        ///Get All Ticket Category List
        /// </summary> 
        /// <returns></returns>
        [HttpGet("getallticketcategory/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetAllTicketCategoryList(Int64 Id)
        {
            DlHod dl = new();            
           // Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetAllTicketCategory(Id);
            return dt;
        }
        /// <summary>
        ///Get Ticket Category By Id
        /// </summary> 
        /// <returns></returns>
        [HttpGet("getticketcategorybyid/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetTicketCategoryById(Int32 Id)
        {
            DlHod dl = new();            
            //Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetTicketCategoryById(Id);
            return dt;
        }

        /// <summary>
        ///Save Ticket Sub Category
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("savesubcategory")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> SaveTicketSubCategory([FromBody] BlSubCategory appParam)
        {
            DlHod dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.SaveTicketSubCategory(appParam);
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
        ///Update Sub Category
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("updatesubcategory")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> UpdateTicketSubCategory([FromBody] BlSubCategory appParam)
        {
            DlHod dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.UpdateTicketSubCategory(appParam);
            if (rb.status)
            {
                rs.message = "Data Updated Successfully";
                rs.status = true;
                rs.value = rb.message;
            }
            else
            {
                //====Failure====
                rs.message = "Failed to Update data " + rb.message;
                rs.status = false;
            }
            return rs;
        }


        /// <summary>
        ///Get All Ticket Sub Category List
        /// </summary> 
        /// <returns></returns>
        [HttpGet("ticketsubcategory/{Id}/{tcid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetAllSubCategory(Int32 Id, Int32 tcid)
        {
            DlHod dl = new();
            //Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetAllTicketSubCategory(Id,tcid);
            return dt;
        }
        /// <summary>
        ///Get Ticket Sub Category By Id
        /// </summary> 
        /// <returns></returns>
        [HttpGet("ticketSubcatbyid/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetSubCategoryById(Int32 Id)
        {
            DlHod dl = new();
            //Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetTicketSubCategoryById(Id);
            return dt;
        }

    }
}
