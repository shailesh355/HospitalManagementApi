#pragma warning disable CS1587 // XML comment is not placed on a valid language element
/// <summary>
/// Don't Add any additional enumerators in this code file. For Project Specific enumerator use CommonEnumerator.cs file
/// </summary>
namespace BaseClass
#pragma warning restore CS1587 // XML comment is not placed on a valid language element
{
    public enum HashingAlgorithmSupported
    {
        Md5,
        Sha256,
        Sha512
    }
    public enum LanguageSupported
    {
        Hindi = 1,
        English = 2,
    }
    public enum DBConnectionList
    {
        TransactionDb = 1,
        ReportingDb = 2
    }
    public enum Active
    {
        No = 0,
        Yes = 1
    }
    public enum YesNo
    {
        No = 0,
        Yes = 1
    }
    public enum UserRole
    {
        Admin = 1,
        Hospital = 2,
        Patient = 3,
        Doctor = 4,
        SuperAdmin = 5,
        User = 6,
    }
   public enum UserType
    {
        Receiver = 1,
        Replier = 2
    }
    public enum idPrefix
    {
        registrationId = 3,
        serviceRegistrationId = 2,
        employeeId = 2,
        officeId = 1,
        empOfficeMappingId = 2,
        liftServiceId = 4,
        chargeMappingKey = 5,
        doctorRegistrationId = 6,
        PatientRegistrationId = 7,
    }
    public enum MessageCategory
    {
        OTHER = 0,
        OTP = 1
    }
    public enum SMSSendType
    {
        Send = 1,
        Resend = 2,
    }
    public enum OTPStatus
    {
        Pending = 0,
        Verified = 1,
        Expired = 2,
    }
    public enum SmsEmailTemplate
    {
        OTPSWS = 3001,       
        Registration = 3002,


    }
    public enum AppointmentStatus
    {
        PendingConfirmation = 1,
        appointmentConfirm = 2,
        appointmentCancel = 3,
        ConsultwithDr = 4,
        WaitingLabReports = 5,
        PrescriptiononLabReports = 6,
        PrescriptionAndAppointmentClosed = 7,
        ReferredwithAppointmentClosed = 8,


    }
    public class data
    {
        public string reqid { get; set; } = "";
    }
    public class SandeshResponse
    {
        public string status { get; set; }
        public string notice { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public data data { get; set; }

    }
    public class sandeshMessageBody
    {
        public string? contact { get; set; }
        public string? message { get; set; }
        public string? projectName { get; set; }
        public Int16 msgPriority { get; set; }
        public Int16 msgCategory { get; set; }
        public bool? isOTP { get; set; } = false;
        public Int32? OTP { get; set; }
        public string? clientIp { get; set; }
        public long? templateId { get; set; } = 0;
    }
    public class emailSenderClass
    {
        public string? emailToId { get; set; }
        public string? emailToName { get; set; }
        public string? emailSubject { get; set; }
        public string? emailBody { get; set; }
    }
    public class SMSParam
    {
        public string? value1 { get; set; } = "";
        public string? value2 { get; set; } = "";
        public string? value3 { get; set; } = "";
        public string? value4 { get; set; } = "";
        public string? value5 { get; set; } = "";
        public string? value6 { get; set; } = "";
        public string? value7 { get; set; } = "";
        public string? value8 { get; set; } = "";
        public string? value9 { get; set; } = "";
        public string? value10 { get; set; } = "";

    }
    public enum SandeshmsgCategory
    {
        Info = 0,
        Alert = 1,
        EventType = 2,
    }
    public enum SandeshmsgPriority
    {
        Low = 0,
        Medium = 1,
        High = 2,
        HighVolatile = 3,
    }
    public enum ContactVerifiedType
    {
        Email = 1,
        Mobile = 2
    }
}