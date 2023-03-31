using BaseClass;
using DmfPortalApi.Models.AppClass;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementApi.Models.BLayer
{
    public class BlPatient
    {

        public long? patientRegNo { get; set; }
        public string? patientNameEnglish { get; set; }
        public string? patientNameLocal { get; set; }
        public Int16? stateId { get; set; }
        public Int16? districtId { get; set; }
        public string? address { get; set; }
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])(?=.*[a-z]).{8,15}$", ErrorMessage = "Invalid email address")]
        public string? emailId { get; set; }
        public string? mobileNo { get; set; }
        public YesNo? active { get; set; }

        public YesNo? isVerified { get; set; }
        
        public string? clientIp { get; set; }
        public string? entryDateTime { get; set; }
        public Int64? userId { get; set; }
        public int? registrationYear { get; set; }        


    }
    public class BlPatientOtp
    {

        public long? patientRegNo { get; set; }
        public Int32? OTP { get; set; }
        public string? mobileNo { get; set; }
        public YesNo? active { get; set; }
        public string? password { get; set; }
        public YesNo? isVerified { get; set; }

        public string? clientIp { get; set; }
        public string? entryDateTime { get; set; }
        public Int64? userId { get; set; }
       


    }

}
