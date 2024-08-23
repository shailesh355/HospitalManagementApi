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
                rs.message = "Patient Registered Successfully";
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
        /*
         INSERT INTO ewalletmaster (patientRegNo,walletAmount,walletReleasedAmount,walletBalanceAmount,actionId,
									Remark,entryDateTime,userId,clientIp)
 									VALUES
 									(@patientRegNo,@walletAmount,@walletReleasedAmount,@walletBalanceAmount,@actionId,
					@Remark,@entryDateTime,@userId,@clientIp)
        
INSERT INTOewalletreleasedtransaction(patientRegNo,actionId,doctorRegNo,walletReleasedAmount,discountAmount,serviceCharge,gst,
												igst,otherCharges,totalReleasedAmount,Remark,transactionNo,entryDateTime,userId,clientIp) 
									VALUES
									(@patientRegNo,@actionId,@doctorRegNo,@walletReleasedAmount,@discountAmount,@serviceCharge,@gst,
										@igst,@otherCharges,@totalReleasedAmount,@Remark,@transactionNo,@entryDateTime,@userId,@clientIp) 
	


INSERT INTOewallettransaction(patientRegNo,actionId,walletAmount,walletReleasedAmount,walletBalanceAmount,Remark,
										transactionNo,entryDateTime,userId,clientIp) 
											VALUES
											(@patientRegNo,@actionId,@walletAmount,@walletReleasedAmount,@walletBalanceAmount,@Remark,
										@transactionNo,@entryDateTime,@userId,@clientIp)
         */

    }
}
