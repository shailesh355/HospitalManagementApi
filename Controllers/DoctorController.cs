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
    public class DoctorController : Controller
    {

        /// <summary>
        /// Insert service registration application
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("savedoctorregistration")]
        public async Task<ReturnClass.ReturnString> SaveHospital([FromBody] BlDoctor appParam)
        {
            DlDoctor dl = new DlDoctor();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.registrationYear = Convert.ToInt32(DateTime.Now.Year.ToString());
            //appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.RegisterNewDoctor(appParam);
            if (rb.status)
            {
                rs.message = "Doctor registered successfully and Pending for the Approval";
                rs.status = true;
                rs.value = rb.message;
            }
            else
            {
                //====Failure====
                rs.message = "Failed to Register Doctor, " + rb.message;
                rs.status = false;
            }
            return rs;
        }

        /// <summary>
        /// Save Update Doctor Academic
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("saveupdatedoctoracademic")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> SaveUpdateDoctorAcademic([FromBody] BlDoctorAcademic appParam)
        {
            DlDoctor dl = new DlDoctor();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.SaveUpdateDoctorAcademic(appParam);
            if (rb.status)
            {
                rs.message = "Doctor Academic Saved Successfully";
                rs.status = true;
                rs.value = rb.message;
            }
            else
            {
                //====Failure====
                rs.message = "Failed to Update Doctor Acedmic, " + rb.message;
                rs.status = false;
            }
            return rs;
        }

        /// <summary>
        /// Save Update Doctor Experience
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("saveupdatedoctorexp")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> SaveUpdateDoctorExperience([FromBody] BlDoctorExperience appParam)
        {
            DlDoctor dl = new DlDoctor();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.SaveUpdateDoctorExperience(appParam);
            if (rb.status)
            {
                rs.message = "Doctor Experience Saved Successfully";
                rs.status = true;
                rs.value = rb.message;
            }
            else
            {
                //====Failure====
                rs.message = "Failed to Update Doctor Experience, " + rb.message;
                rs.status = false;
            }
            return rs;
        }

        /// <summary>
        /// Save Update Doctor Experience
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("saveupdatedoctorworkarea")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> SaveUpdateDoctorWorkArea([FromBody] BlDoctorWorkArea appParam)
        {
            DlDoctor dl = new DlDoctor();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.SaveUpdateDoctorWorkArea(appParam);
            if (rb.status)
            {
                rs.message = "Doctor Work Area Saved Successfully";
                rs.status = true;
                rs.value = rb.message;
            }
            else
            {
                //====Failure====
                rs.message = "Failed to Update Work Area, " + rb.message;
                rs.status = false;
            }
            return rs;
        }

        /// <summary>
        ///Get All Doctor List
        /// </summary>         
        /// <returns></returns>
        [HttpGet("getalldoctorlist/{vid?}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetAllDoctorList(Int16 vid = 0)
        {
            DlDoctor dl = new();
            //string clientIP = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetAllDoctorList(vid);
            return dt;
        }

        /// <summary>
        /// Verification of Doctor registration application
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("verifydoctorregistration")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> VerifyHospitalRegistration([FromBody] VerificationDoctorDetail appParam)
        {
            DlDoctor dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.date = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.VerifyDoctor(appParam);
            if (rb.status)
            {
                rs.message = rb.message;
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
        /// Doctor Profile Setting Save Part 1
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("savedrprofpartone")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> SaveDoctorProfilePartOne([FromBody] DoctorProfilePart1 appParam)
        {
            DlDoctor dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.date = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.SaveDoctorProfilePartOne(appParam);
            if (rb.status)
            {
                rs.message = "Profile Updated Successfully.";
                rs.status = true;
                rs.value = rb.error;
            }
            else
            {
                //====Failure====
                rs.message = "Failed to update " + rb.message;
                rs.status = false;
            }
            return rs;
        }

        /// <summary>
        /// Doctor Award
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("savedraward")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> SaveUpdateDoctorAward([FromBody] DoctorAward appParam)
        {
            DlDoctor dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.date = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.SaveUpdateDoctorAward(appParam);
            if (rb.status)
            {
                rs.message = "Award Saved Successfully.";
                rs.status = true;
                rs.value = rb.error;
            }
            else
            {
                //====Failure====
                rs.message = "Failed to add " + rb.message;
                rs.status = false;
            }
            return rs;
        }

        /// <summary>
        /// Doctor MCR
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("savedrmcr")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> SaveUpdateDoctorMCR([FromBody] DoctorMCR appParam)
        {
            DlDoctor dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.date = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.SaveUpdateDoctorMCR(appParam);
            if (rb.status)
            {
                rs.message = "MCR Saved Successfully.";
                rs.status = true;
                rs.value = rb.error;
            }
            else
            {
                //====Failure====
                rs.message = "Failed to add " + rb.message;
                rs.status = false;
            }
            return rs;
        }

        /// <summary>
        /// Doctor MCR
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("savedrmembership")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> SaveUpdateMembership([FromBody] DoctorMembership appParam)
        {
            DlDoctor dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.date = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.SaveUpdateMembership(appParam);
            if (rb.status)
            {
                rs.message = "Membership Saved Successfully.";
                rs.status = true;
                rs.value = rb.error;
            }
            else
            {
                //====Failure====
                rs.message = "Failed to add " + rb.message;
                rs.status = false;
            }
            return rs;
        }


        /// <summary>
        /// Doctor AddOns 
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("savedraddons")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> SaveUpdateAddOns([FromBody] DoctorAddOns appParam)
        {
            DlDoctor dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.date = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.SaveUpdateAddOns(appParam);
            if (rb.status)
            {
                rs.message = "Add On Certificate Saved Successfully.";
                rs.status = true;
                rs.value = rb.error;
            }
            else
            {
                //====Failure====
                rs.message = "Failed to add " + rb.message;
                rs.status = false;
            }
            return rs;
        }

        /// <summary>
        /// Doctor Indaminity 
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("savedrindaminity")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> SaveUpdateIndaminity([FromBody] DoctorIndaminity appParam)
        {
            DlDoctor dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.date = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.SaveUpdateIndaminity(appParam);
            if (rb.status)
            {
                rs.message = "Indaminity Saved Successfully.";
                rs.status = true;
                rs.value = rb.error;
            }
            else
            {
                //====Failure====
                rs.message = "Failed to add " + rb.message;
                rs.status = false;
            }
            return rs;
        }
        /// <summary>
        ///Get Doctor Info
        /// </summary>         
        /// <returns></returns>
        [HttpGet("getdoctorinfo/{doctorRegNo?}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetDoctorInfoPart1(Int64 doctorRegNo = 0)
        {
            DlDoctor dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetDoctorInfo(doctorRegNo);
            return dt;
        }
        /// <summary>
        ///Get Doctor Clinic Info
        /// </summary>         
        /// <returns></returns>
        [HttpGet("getdoctorclinicinfo/{doctorRegNo?}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<List<BlDoctorWorkAreaItemsDoc>> GetDoctorClinicInfo(Int64 doctorRegNo = 0)
        {
            DlDoctor dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            List<BlDoctorWorkAreaItemsDoc> bl = await dl.GetDoctorClinicInfo(doctorRegNo);
            return bl;
        }

        /// <summary>
        ///Get Doctor Education Info
        /// </summary>         
        /// <returns></returns>
        [HttpGet("getdoctoreducationinfo/{doctorRegNo?}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetDoctorEducationInfo(Int64 doctorRegNo = 0)
        {
            DlDoctor dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetDoctorEducationInfo(doctorRegNo);
            return dt;
        }

        /// <summary>
        ///Get Doctor Experience Info
        /// </summary>         
        /// <returns></returns>
        [HttpGet("getdoctorexperienceinfo/{doctorRegNo?}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetDoctorExperienceInfo(Int64 doctorRegNo = 0)
        {
            DlDoctor dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetDoctorExperienceInfo(doctorRegNo);
            return dt;
        }
        /// <summary>
        ///Get Doctor Award Info
        /// </summary>         
        /// <returns></returns>
        [HttpGet("getdoctorawardinfo/{doctorRegNo?}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetDoctorAwardInfo(Int64 doctorRegNo = 0)
        {
            DlDoctor dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetDoctorAwardInfo(doctorRegNo);
            return dt;
        }

        /// <summary>
        ///Get Doctor Membership Info
        /// </summary>         
        /// <returns></returns>
        [HttpGet("getdoctormembershipinfo/{doctorRegNo?}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetDoctorMembershipInfo(Int64 doctorRegNo = 0)
        {
            DlDoctor dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetDoctorMembershipInfo(doctorRegNo);
            return dt;
        }
        /// <summary>
        ///Get Doctor Add ons
        /// </summary>         
        /// <returns></returns>
        [HttpGet("getdoctoraddons/{doctorRegNo?}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetDoctorAddons(Int64 doctorRegNo = 0)
        {
            DlDoctor dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetDoctorAddons(doctorRegNo);
            return dt;
        }

        /// <summary>
        ///Get Doctor indaminity
        /// </summary>         
        /// <returns></returns>
        [HttpGet("getdoctorindaminity/{doctorRegNo?}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetDoctorIndaminity(Int64 doctorRegNo = 0)
        {
            DlDoctor dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetDoctorIndaminity(doctorRegNo);
            return dt;
        }

        /// <summary>
        ///Get Doctor Registration Info
        /// </summary>         
        /// <returns></returns>
        [HttpGet("getdoctorregistrationinfo/{doctorRegNo?}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetDoctorRegistrationInfo(Int64 doctorRegNo = 0)
        {
            DlDoctor dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetDoctorRegistrationInfo(doctorRegNo);
            return dt;
        }

        /// <summary>
        ///Get Doctor Profile & Logo
        /// </summary>         
        /// <returns></returns>
        [HttpGet("getdoctorprofilelogo/{doctorRegNo?}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetDoctorProfileLogo(Int64 doctorRegNo = 0)
        {
            DlDoctor dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetDoctorProfileLogo(doctorRegNo);
            return dt;
        }

        /// <summary>
        /// Doctor Schedule Date Time
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("saveupdatedoctorschedule")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> SaveUpdateDoctorScheduleDateTime([FromBody] DoctorScheduleDate appParam)
        {
            DlDoctor dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.date = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            if (appParam.items.Count == 0)
            {
                rs.message = "No Time slots entered.";
                rs.status = false;
            }
            else
            {
                ReturnClass.ReturnBool rb = await dl.SaveUpdateDoctorScheduleDateTime(appParam);
                if (rb.status)
                {
                    rs.message = "Schedule Saved Successfully.";
                    rs.status = true;
                    rs.value = rb.error;
                }
                else
                {
                    //====Failure====
                    rs.message = "Failed to Submit " + rb.message;
                    rs.status = false;
                }
            }
            return rs;
        }

        /// <summary>
        ///Get Doctor Schedule by dayId
        /// </summary>         
        /// <returns></returns>
        [HttpGet("getdoctorschedule/{doctorRegNo}/{dayId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetDoctorScheduleTimings(Int64 doctorRegNo, Int16 dayId = 0)
        {
            DlDoctor dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetDoctorScheduleTimings(doctorRegNo, dayId);
            return dt;
        }

        /// <summary>
        ///Delete Doctor Schedule Time by scheduleTimeId
        /// </summary>         
        /// <returns></returns>
        [HttpGet("deldoctorschedule/{scheduleTimeId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> DeleteScheduleTime(Int32 scheduleTimeId)
        {
            DlDoctor dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            ReturnClass.ReturnBool rb = await dl.DeleteScheduleTime(scheduleTimeId);
            if (rb.status)
            {
                rs.message = rb.message;
                rs.status = rb.status;
            }
            else
            {
                //====Failure====
                rs.message = "Failed to Delete " + rb.message;
                rs.status = rb.status;
            }
            return rs;
        }

        /// <summary>
        ///Get Counters of Verified Doctors and Patients
        /// </summary>         
        /// <returns></returns>
        [HttpGet("getverifiedcounters")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataSet> GetVerifiedCounters()
        {
            DlDoctor dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataSet ds = await dl.GetVerifiedCounters();
            return ds;
        }

        /// <summary>
        ///Get List of Verified Doctors and Patients Doctor  = 3 
        /// </summary>         
        /// <returns></returns>
        [HttpGet("getdoctorpatientlimlist/{role}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetDoctorPatientLimList(Int16 role)
        {
            DlDoctor dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetDoctorPatientLimList(role);
            return dt;
        }

        /// <summary>
        ///Get Doctor All Info
        /// </summary>         
        /// <returns></returns>
        [HttpGet("getallinfodoctor/{doctorRegNo?}")]
        public async Task<ReturnClass.ReturnDataSet> GetAllDoctorInfo(Int64 doctorRegNo = 0)
        {
            DlDoctor dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataSet ds = await dl.GetAllDoctorInfo(doctorRegNo);
            return ds;
        }

        /// <summary>
        /// Check existance of Date on Doctor
        /// </summary>
        /// <param name="doctorRegNo"></param>        
        /// <param name="month"></param>        
        /// <returns></returns>
        [HttpGet("checkrecordsdatewise/{doctorRegNo}/{year}/{month}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Int32> CheckDoctorDatewiseSchedule(Int64 doctorRegNo, Int32 year, Int16 month)
        {
            DlDoctor dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            Int32 records = await dl.CheckDoctorDatewiseSchedule(doctorRegNo, month, year);
            return records;
        }

        /// <summary>
        ///Get date wise timing
        /// </summary>         
        /// <returns></returns>
        [HttpGet("getdatewisedoctortiming/{doctorRegNo}/{year}/{month}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetDoctorDatewiseScheduleTime(Int64 doctorRegNo, Int32 year, Int16 month)
        {
            DlDoctor dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetDoctorDatewiseScheduleTime(doctorRegNo, month, year);
            return dt;
        }

        /// <summary>
        /// Save Update Doctor Datetime Schedule
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("savebulkslotdatewise")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> saveBulkSlotDatewise([FromBody] DoctorScheduleDatewise appParam)
        {
            DlDoctor dl = new DlDoctor();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnBool rb = await dl.saveBulkSlotDatewise(appParam);
            if (rb.status)
            {
                rs.message = rb.message;
                rs.status = rb.status;
                rs.value = rb.value;
            }
            else
            {
                //====Failure====
                rs.message = rb.message;
                rs.status = rb.status;
            }
            return rs;
        }

        /// <summary>
        /// Doctor Schedule Date Time
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("savesingleslotdatewise")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> saveSingleSlotDatewise([FromBody] DoctorScheduleDatewise appParam)
        {
            DlDoctor dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            if (appParam.items.Count == 0)
            {
                rs.message = "No Time slots entered.";
                rs.status = false;
            }
            else
            {
                ReturnClass.ReturnBool rb = await dl.saveSingleSlotDatewise(appParam);
                if (rb.status)
                {
                    rs.message = rb.message;
                    rs.status = rb.status;
                    rs.value = rb.error;
                }
                else
                {
                    //====Failure====
                    rs.message = "Failed to Submit " + rb.message;
                    rs.status = false;
                }
            }
            return rs;
        }

        /// <summary>
        ///Delete Doctor Schedule slot Datewise
        /// </summary>         
        /// <returns></returns>
        [HttpGet("delsingleslotdatewise/{scheduleTimeId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> delSingleSlotDatewise(Int32 scheduleTimeId)
        {
            DlDoctor dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            ReturnClass.ReturnBool rb = await dl.delSingleSlotDatewise(scheduleTimeId);
            if (rb.status)
            {
                rs.message = rb.message;
                rs.status = rb.status;
                rs.value = rb.value;
            }
            else
            {
                //====Failure====
                rs.message = "Failed to Delete " + rb.message;
                rs.status = rb.status;
            }
            return rs;
        }


        /// <summary>
        ///Get All Doctor List HOME
        /// </summary>         
        /// <returns></returns>
        [HttpGet("getalldoctorlisthome")]
        public async Task<ReturnClass.ReturnDataTable> GetAllDoctorListHome()
        {
            DlDoctor dl = new();
            ReturnClass.ReturnDataTable dt = await dl.GetAllDoctorListHome();
            return dt;
        }


        /// <summary>
        ///Patient Appointment timeslot from current date
        /// </summary>         
        /// <returns></returns>
        [HttpGet("getdoctornextweekslots/{doctorRegNo}")]
        public async Task<ReturnClass.ReturnDataTable> GetDoctorSlots(Int64 doctorRegNo)
        {
            DlDoctor dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetDoctorSlots(doctorRegNo);
            return dt;
        }


        /// <summary>
        /// Save Patient Booking
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("appointdoctor")]
        public async Task<ReturnClass.ReturnString> AppointDoctor([FromBody] BlDoctorAppointment appParam)
        {
            DlDoctor dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.AppointDoctor(appParam);
            if (rb.status)
            {
                rs.message = rb.message;
                rs.status = true;
                rs.value = rb.value;
            }
            else
            {
                //====Failure====
                rs.message = rb.message;
                rs.status = false;
            }
            return rs;
        }


        /// <summary>
        ///Patient Appointment timeslot from current date
        /// </summary>         
        /// <returns></returns>
        [HttpGet("getdoctorlisthomespecial/{specializationId}")]
        public async Task<ReturnClass.ReturnDataTable> GetAllDoctorListHomeSpecialization(Int16 specializationId)
        {
            DlDoctor dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetAllDoctorListHomeSpecialization(specializationId);
            return dt;
        }

        /// <summary>
        /// Verification of HOD registration application
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("rollbackregistration")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> RollbackHospitalRegistration([FromBody] VerificationDoctorDetail appParam)
        {
            DlDoctor dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.date = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.RollbackDocterRegistration(appParam);
            if (rb.status)
            {
                rs.message = "Successfully Rolledback.";
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
        /// CRUD for Doctor Specialization
        /// </summary>
        /// <param name="bl"></param>        
        /// <returns></returns>
        [HttpPost("crddoctorspec")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> CUDDoctorOperation([FromBody] BlDoctorSpecialization bl)
        {
            DlDoctor dl = new();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            bl.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            bl.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            bl.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.CUDDoctorOperation(bl);
            if (rb.status)
            {
                rs.message = "Specialization Saved Successfully";
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
        /// Get Doctor Specialization details 
        /// </summary>
        /// <param name="doctorRegNo"></param>        
        /// <returns></returns>
        [HttpGet("getdoctorspecdetail/{doctorRegNo}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetDoctorSpecDetail(Int64 doctorRegNo)
        {
            DlDoctor dl = new();
            //string clientIP = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetDoctorSpecDetail(doctorRegNo);
            return dt;
        }

        /// <summary>
        ///Get Doctor Home Page List
        /// </summary>         
        /// <returns></returns>
        [HttpGet("getdoctorhomelist")]
        public async Task<ReturnClass.ReturnDataTable> GetDoctorHomeList()
        {
            DlDoctor dl = new();
            ReturnClass.ReturnDataTable dt = await dl.GetDoctorHomeList();
            return dt;
        }

        /// <summary>
        ///Get Doctor Schedule  
        /// </summary>         
        /// <returns></returns>
        [HttpGet("getdoctorschedtimecalender/{doctorRegNo}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetDoctorScheduleTimingsCalender(Int64 doctorRegNo)
        {
            DlDoctor dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetDoctorScheduleTimingsCalender(doctorRegNo);
            return dt;
        }

        /// <summary>
        ///
        /// </summary>         
        /// <returns></returns>
        [HttpPost("doctornotavailable")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> NotAvailableDoctor(BlDoctorAvailability bl)
        {
            DlDoctor dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            ReturnClass.ReturnBool rb = await dl.NotAvailableDoctor(bl);
            if (rb.status)
            {
                rs.message = rb.message;
                rs.status = rb.status;
            }
            else
            {
                //====Failure====
                rs.message = "Failed to Update " + rb.message;
                rs.status = rb.status;
            }
            return rs;
        }

        /// <summary>
        /// 
        /// </summary>         
        /// <returns></returns>
        [HttpPost("doctoravailable")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> AvailableDoctor(BlDoctorAvailability bl)
        {
            DlDoctor dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            ReturnClass.ReturnBool rb = await dl.AvailableDoctor(bl);
            if (rb.status)
            {
                rs.message = rb.message;
                rs.status = rb.status;
            }
            else
            {
                //====Failure====
                rs.message = "Failed to Update " + rb.message;
                rs.status = rb.status;
            }
            return rs;
        }
    }
}
