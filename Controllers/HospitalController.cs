using BaseClass;
using HospitalManagementApi.Models.BLayer;
using HospitalManagementApi.Models.DaLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static BaseClass.ReturnClass;

namespace HospitalManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HospitalController : ControllerBase
    {

        /// <summary>
        /// Insert service registration application
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("savehospitalregistration")]
        public async Task<ReturnClass.ReturnString> SaveHospital([FromBody] BlHospital appParam)
        {
            DlHospital dl = new DlHospital();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.registrationYear = Convert.ToInt32(DateTime.Now.Year.ToString());
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.RegisterNewHospital(appParam);
            if (rb.status)
            {
                rs.message = "Hospital Registered Successfully";
                rs.status = true;
                rs.value = rb.message;
            }
            else
            {
                //====Failure====
                rs.message = "Failed to Register Hospital, " + rb.message;
                rs.status = false;
            }
            return rs;
        }
        /// <summary>
        /// Insert service registration application
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("updatehospitalregistration")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> updateHospital([FromBody] BlHospital appParam)
        {
            DlHospital dl = new DlHospital();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.registrationYear = Convert.ToInt32(DateTime.Now.Year.ToString());
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.UpdateNewHospital(appParam);
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
        [HttpGet("getallhospital/{vid?}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetAllHospital(Int16 vid = 0)
        {
            DlHospital dl = new DlHospital();
            //string clientIP = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetAllHospitalList(vid);
            return dt;
        }
        /// <summary>
        ///Get All HOD List
        /// </summary> 
        /// <returns></returns>
        [HttpGet("gethospitalbyid/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GethospitalById(Int64 Id)
        {
            DlHospital dl = new DlHospital();
            //string clientIP = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetHospitalById(Id);
            return dt;
        }

        /// <summary>
        /// Verification of HOD registration application
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("verifyregistration")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> VerifyHospitalRegistration([FromBody] VerificationDetail appParam)
        {
            DlHospital dl = new DlHospital();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.date = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.VerifyHospital(appParam);
            if (rb.status)
            {
                rs.message = "Successfully Verified";
                rs.status = true;
                rs.value = rb.error;
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
        /// Passwrod Reset
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("resetpassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> ResetPassword([FromBody] ResetPassword appParam)
        {
            DlHospital dl = new DlHospital();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);

            ReturnClass.ReturnBool rb = await dl.ResetPassword(appParam);
            if (rb.status)
            {
                rs.message = "Successfully Reset";
                rs.status = true;
                rs.value = rb.error;
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
        /// Update hospital Info - registration 
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("updatehospitalinfo")]
        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> UpdateHospitalInfo([FromBody] BlHospital appParam)
        {
            DlHospital dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.UpdateHospitalInfo(appParam);
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
        /// Search By Hospital Name OR Specialization
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("searchhs")]
        public async Task<ReturnClass.ReturnDataTable> SearchHS([FromBody] Filter appParam)
        {
            DlHospital dl = new DlHospital();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            ReturnClass.ReturnDataTable dt = await dl.SearchHS(appParam);
            return dt;
        }

        /// <summary>
        /// Get Hospital profile, NACH , license and Other documents
        /// </summary>
        /// <param name="hospitalRegNo"></param>        
        /// <returns></returns>
        [HttpPost("gethospitaldoc")]
        public async Task<ReturnClass.ReturnDataTable> GetHospitalProfile(Int64 hospitalRegNo)
        {
            DlHospital dl = new DlHospital();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            ReturnClass.ReturnDataTable dt = await dl.GetHospitalDoc(hospitalRegNo);
            return dt;
        }


        /// <summary>
        /// Save/Update hospital Main Contact,Info,Rohini 
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("saveupdatehospitalmi")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> SaveUpdateHospitalMI([FromBody] BlHospitalMI appParam)
        {
            DlHospital dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            if (appParam != null)
            {
                appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
                appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
                appParam.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                try
                {
                    ReturnClass.ReturnBool rb = await dl.UpdateHospitalInfoMI(appParam);
                    if (rb.status)
                    {
                        rs.message = "Data Updated Successfully.";
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
                    rs.message = "Could not save , " + ex.Message;
                    rs.status = false;
                }
            }
            else
            {
                rs.messageInt = "check all parameters !";
                rs.message = "something went wrong !";
                rs.status = false;
            }
            return rs;
        }

        /// <summary>
        /// Get Hospital profile Info
        /// </summary>
        /// <param name="hospitalRegNo"></param>        
        /// <returns></returns>
        [HttpGet("gethospitalinfomi/{hospitalRegNo}")]
        public async Task<ReturnClass.ReturnDataSet> GetHospitalInfoMI(Int64 hospitalRegNo)
        {
            DlHospital dl = new DlHospital();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            ReturnClass.ReturnDataSet ds = await dl.GetHospitalInfoMI(hospitalRegNo);
            return ds;
        }

        ///// <summary>
        ///// Get Hospital Financial Iinformation
        ///// </summary>
        ///// <param name="hospitalRegNo"></param>        
        ///// <returns></returns>
        //[HttpGet("gethospitalfi/{hospitalRegNo}")]
        //public async Task<ReturnClass.ReturnDataTable> GetHospitalFinancialInfo(Int64 hospitalRegNo)
        //{
        //    DlHospital dl = new DlHospital();
        //    ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
        //    ReturnClass.ReturnDataTable dt = await dl.GetHospitalFinancialInfo(hospitalRegNo);
        //    return dt;
        //}

        /// <summary>
        /// Upload 
        /// </summary>
        /// <param name="bl"></param>
        /// <returns></returns>
        [HttpPost("uploadhospitaldocs")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnString> SaveWorkProgressBacklogDocumentsAsync(HospitalDocs bl)
        {
            DlHospital blh = new();
            ReturnString rs = new ReturnString();
            bl.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            bl.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            ReturnClass.ReturnBool succeded = await blh.UploadDocument(bl);
            if (succeded.status)
            {
                rs.message = succeded.message;
                rs.status = succeded.status;
            }
            else
            {
                //====Failure====
                rs.message = succeded.message;
            }
            return rs;
        }

        /// <summary>
        /// Get Hospital profile Info Docs
        /// </summary>
        /// <param name="hospitalRegNo"></param>        
        /// <returns></returns>
        [HttpGet("gethospitalinfomidoc/{hospitalRegNo}")]
        public async Task<ReturnClass.ReturnDataSet> GetHospitalInfoMIDoc(Int64 hospitalRegNo)
        {
            DlHospital dl = new DlHospital();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            ReturnClass.ReturnDataSet ds = await dl.GetHospitalInfoMIDoc(hospitalRegNo);
            return ds;
        }

        /// <summary>
        /// Get Hospital profile & Logo 
        /// </summary>
        /// <param name="hospitalRegNo"></param>        
        /// <returns></returns>
        [HttpGet("gethospitalprofilelogo/{hospitalRegNo}")]
        public async Task<ReturnClass.ReturnDataSet> GetHospitalProfileLogo(Int64 hospitalRegNo)
        {
            DlHospital dl = new DlHospital();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            ReturnClass.ReturnDataSet ds = await dl.GetHospitalProfileLogo(hospitalRegNo);
            return ds;
        }

        /// <summary>
        /// Get Hospital Preview
        /// </summary>
        /// <param name="hospitalRegNo"></param>        
        /// <returns></returns>
        [HttpGet("gethospitalpreview/{hospitalRegNo}")]
        public async Task<ReturnClass.ReturnDataSet> GetHospitalPreview(Int64 hospitalRegNo)
        {
            DlHospital dl = new DlHospital();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            ReturnClass.ReturnDataSet ds = await dl.GetHospitalPreview(hospitalRegNo);
            return ds;
        }
    }
}
