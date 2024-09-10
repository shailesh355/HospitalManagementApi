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
    public class PatientController : Controller
    {

        /// <summary>
        /// Insert service registration application
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("savepatientregistration")]
        public async Task<ReturnClass.ReturnString> SavePatient([FromBody] BlPatient appParam)
        {
            DlPatient dl = new DlPatient();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.registrationYear = Convert.ToInt32(DateTime.Now.Year.ToString());
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.RegisterNewPatient(appParam);
            if (rb.status)
            {
                rs.message = "Patient Registered Successfully,Please Verify OTP";
                rs.status = true;
                rs.value = rb.value;
                rs.any_id = rb.error;
            }
            else
            {
                //====Failure====
                rs.message = "Failed to Register Patient, " + rb.message;
                rs.status = false;
            }
            return rs;
        }

        /// <summary>
        /// Insert service registration application
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("otpverify")]
        public async Task<ReturnClass.ReturnString> OTPVerify([FromBody] BlPatientOtp appParam)
        {
            DlPatient dl = new DlPatient();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.VerifyOTP((long)appParam.patientRegNo, (Int32)appParam.OTP, appParam.mobileNo);
            if (rb.status)
            {
                rs.message = "Successfully Verify";
                rs.status = true;
                rs.value = appParam.patientRegNo.ToString();
                rs.any_id = rb.error;
            }
            else
            {
                //====Failure====
                rs.message = "OTP Not Matched, " + rb.message;
                rs.status = false;
            }
            return rs;
        }

        /// <summary>
        /// Insert service registration application
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("setpassword")]
        public async Task<ReturnClass.ReturnString> SetPassword([FromBody] BlPatientOtp appParam)
        {
            DlPatient dl = new DlPatient();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            appParam.entryDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            ReturnClass.ReturnBool rb = await dl.SetPassword(appParam);
            if (rb.status)
            {
                rs.message = "Registration Completed!!!";
                rs.status = true;
                rs.value = appParam.patientRegNo.ToString();
                rs.any_id = rb.error;
            }
            else
            {
                //====Failure====
                rs.message = "Password Not Set, " + rb.message;
                rs.status = false;
            }
            return rs;
        }

        /// <summary>
        ///Get Patient history
        /// </summary>         
        /// <returns></returns>
        [HttpGet("getpatientbookinghistory/{patientRegNo}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetPatientSlotsHistory(Int64 patientRegNo)
        {
            DlPatient dl = new();
            ReturnClass.ReturnDataTable dt = await dl.GetPatientSlotsHistory(patientRegNo);
            return dt;
        }

        /// <summary>
        ///Get Patient
        /// </summary>         
        /// <returns></returns>
        [HttpGet("getappointmentcalender/{doctorRegNo}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> GetAppointmentCalender(Int64 doctorRegNo)
        {
            DlPatient dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetAppointmentCalender(doctorRegNo);
            return dt;
        }
        /// <summary>
        /// Submit Appointment
        /// </summary>
        /// <param name="appParam"></param>        
        /// <returns></returns>
        [HttpPost("addwallet")]
       [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnString> AddWallet([FromBody] BlAddWallet appParam)
        {
            DlPatient dl = new DlPatient();
            ReturnClass.ReturnString rs = new ReturnClass.ReturnString();
            appParam.clientIp = Utilities.GetRemoteIPAddress(this.HttpContext, true);            
            appParam.userId = Convert.ToInt64(User.FindFirst("userId")?.Value);            
            ReturnClass.ReturnBool rb = await dl.AddWallet(appParam);
            if (rb.status)
            {
                rs.status = true;
                rs.value= rb.value;
                rs.message = rb.message;
            }
            else
            {
                //====Failure====
                rs.message = "Failed to Submit Appointment, " + rb.message;
                rs.status = false;
            }
            return rs;
        }
        /// <summary>
        ///Get check wallet balance
        /// </summary>         
        /// <returns></returns>
        [HttpGet("checkwalletbalance")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> checkWalletBalance()
        {
            DlPatient dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetwalletlistByUser(userId);
            return dt;
        }
        /// <summary>
        ///Get check wallet history
        /// </summary>         
        /// <returns></returns>
        [HttpGet("checkwallethistory")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ReturnClass.ReturnDataTable> checkWalletHistory()
        {
            DlPatient dl = new();
            Int64 userId = Convert.ToInt64(User.FindFirst("userId")?.Value);
            ReturnClass.ReturnDataTable dt = await dl.GetwalletlistHistoryByUser(userId);
            return dt;
        }
        /*       
        
           INSERT INTOewalletreleasedtransaction(patientRegNo,actionId,doctorRegNo,walletReleasedAmount,discountAmount,serviceCharge,gst,
												igst,otherCharges,totalReleasedAmount,Remark,transactionNo,entryDateTime,userId,clientIp) 
									VALUES
									(@patientRegNo,@actionId,@doctorRegNo,@walletReleasedAmount,@discountAmount,@serviceCharge,@gst,
										@igst,@otherCharges,@totalReleasedAmount,@Remark,@transactionNo,@entryDateTime,@userId,@clientIp) 

         */

    }
}
