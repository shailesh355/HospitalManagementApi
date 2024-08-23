using BaseClass;
using System.Net.Mail;
namespace HospitalManagementApi.Models.BLayer
{
    public class BlCommon
    {
        Utilities util = new();
        public BlCommon()
        {

        }
        private int smsValidity;


        /// <summary>
        /// Validity in Minutes
        /// </summary>
        public int smsvalidity
        {
            get { return this.smsValidity; }
        }
        //public enum YesNo
        //{
        //    No = 0,
        //    Yes = 1
        //}

        public enum CRUD
        {
            Create = 1,
            Read = 2,
            Update = 3,
            Delete = 4,
        }


    }
    public class BlMenuLabel
    {
        public Int16 CRUD { get; set; }
        public Int32? id { get; set; }
        public Int32? groupingId { get; set; }
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        public string? clientIp { get; set; }
        public string? labelname { get; set; }
        public Int32? sortOrder { get; set; }
        public string? category { get; set; }
        public string? categoryname { get; set; }
        public string? description { get; set; }
    }
    public enum DocumentImageGroup
    {
        Hospital = 1,
        Doctor = 2,
        Website = 3,
        Mobile = 4,
    }
    public enum DocumentType
    {
        ProfilePic = 1,
        ProfileLogo = 2,
        ProfileDocument = 3,
        NACH = 4,
        License = 5,
        OtherDocument = 6,
        SDD = 7,
        DoctorProfilePic = 8,
        HospitalImages = 9,
        HospitalPAN = 10,
        DoctorHospitalImages = 11,
        DoctorWorkArea = 12,
        WebsiteBanner = 13,
        MobileBanner = 14,
    }

    public class AlertMessageBody
    {
        public string? clientIp { get; set; }
        public long? mobileNo { get; set; }
        public string? smsBody { get; set; }
        public LanguageSupported smsLanguage { get; set; }
        public string? msgId { get; set; }
        public Int16 msgCategory { get; set; }
        public bool isOtpMsg { get; set; } = false;
        public long? OTP { get; set; }
        public long? emailOTP { get; set; }
        public long? applicationId { get; set; } = 0;
        public int? actionId { get; set; } = 0;
        public string? emailToReceiver { get; set; }
        public string? emailBody { get; set; }
        public List<Attachment>? emailAttachment { get; set; }
        public string? emailSubject { get; set; }
        public long loginId { get; set; }
        public string? messageServerResponse { get; set; }
        public Int16 smsTemplateId { get; set; }
    }
    public class UserLoginWithOTP
    {
        public string emailId { get; set; } = "";
        public string id { get; set; } = "0";
        public string? captchaId { get; set; } = "";
        public string? userEnteredCaptcha { get; set; } = "";
        public string requestToken { get; set; }
    }
    public class SendOtp
    {
        public string? msgId { get; set; } = "";
        public string? emailId { get; set; } = "";
        public long? mobileNo { get; set; }
        public string? clientIp { get; set; } = "";
        public string? msgType { get; set; } = "";
        public Int32? OTP { get; set; } = 0;
        public long? id { get; set; } = 0;
        public Int16? loginFor { get; set; } =0;
        public string? userId { get; set; } = "";
        public string requestToken { get; set; } = "";
    }
}
