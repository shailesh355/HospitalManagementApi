using BaseClass;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementApi.Models.BLayer
{
    public class BlEmployee
    {
        public long? hodOfficeId { get; set; }
        public Int16 stateId { get; set; }
        public long? employeeId { get; set; }
        public string employeeName { get; set; }
        public string mobileNo { get; set; }
        public string emailId { get; set; }
        public Int16 workingStatus { get; set; }
        public Int16 recruitmentType { get; set; }        
        public string? clientIp { get; set; }
        public Int32? registrationYear { get; set; }
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        public string? password { get; set; }
        
    }
    public class BlOffice
    {
        public long? hodOfficeId { get; set; }
        public long? OfficeId { get; set; }
        public string? OfficeName { get; set; }
        public int baseDeptId { get; set; }       
        public int officeLevel { get; set; }        
        public int districtId { get; set; }
        public string? districtname { get; set; }
        public YesNo urbanRural { get; set; }
        public int pinCode { get; set; }
       

        public string? officeAddress { get; set; }
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])(?=.*[a-z]).{8,15}$", ErrorMessage = "Invalid email address")]
        public string? officeEmailId { get; set; }
        public string? officePhoneNumber { get; set; }
      
        public YesNo isVerified { get; set; }
        public string? verificationDate { get; set; }
        public int verifiedByLoginId { get; set; }
        public RegistrationStatus registrationStatus { get; set; }
        public string? clientIp { get; set; }        
        
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        
    }
    public class BlEmpOffice
    {
        public Int16 stateId { get; set; }
        public int districtId { get; set; }
        public int? empOfficeId { get; set; }
        public int? chargeMappingKey { get; set; }        
        public long? employeeId { get; set; }
        public long? OfficeId { get; set; }        
        public int designationId { get; set; }
        public Int16 chargeType { get; set; }
        public Int16 userType { get; set; }
        public string? startDate { get; set; }
        public string? endDate { get; set; }
        public string? active { get; set; }     
        public string? clientIp { get; set; }
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }

    }


}
