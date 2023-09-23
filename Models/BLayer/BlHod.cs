using BaseClass;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementApi.Models.BLayer
{
    public class BlHod
    {
        public long? hodOfficeId { get; set; }
        public string? hodOfficeName { get; set; }
        public int baseDeptId { get; set; }
        public int orgType { get; set; }
        public int hodOfficeLevel { get; set; }
        public int hodOfficeStateId { get; set; }
        public int hodOfficeDistrictId { get; set; }
        public string? hodOfficeDistrictname { get; set; }
        public YesNo hodOfficeIsUrbanRural { get; set; }
        public int hodOfficePinCode { get; set; }
        public int officeCount { get; set; }
        
        public string? hodOfficeAddress { get; set; }
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])(?=.*[a-z]).{8,15}$", ErrorMessage = "Invalid email address")]
        public string? hodOfficeEmailId { get; set; }
        public string? hodOfficePhoneNumber { get; set; }
        public string? hodOfficeFaxNumber { get; set; }
       // [RegularExpression(@"http(s)?://([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&=]*)?", ErrorMessage = "Invalid Website address")]
        public string? hodOfficeWebsite { get; set; }
        public YesNo isRegistrationDocumentUploaded { get; set; }
        public YesNo isVerified { get; set; }
        public string? verificationDate { get; set; }
        public int verifiedByLoginId { get; set; }
        public RegistrationStatus registrationStatus { get; set; }
        public string? clientIp { get; set; }
        public string? registrationDate { get; set; }
        public string? resubmissionDate { get; set; }
        public string? applicantName { get; set; }
        public int applicantDesignationCode { get; set; }
        [RegularExpression(@"^[5-9]{1}[0-9]{9}", ErrorMessage = "A valid 10 digit mobile number is required")]
        public long applicantMobileNumber { get; set; }
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])(?=.*[a-z]).{8,15}$", ErrorMessage = "Invalid email")]
        public string? applicantEmailId { get; set; }
        public string? applicantPassword { get; set; }
        public YesNo isParichayLogin { get; set; }
        public string? clienIp { get; set; }
        public Int32? registrationYear { get; set; }
        public Int64? userId { get; set; }
    }
    public class VerificationHod
    {
        public long hodOfficeId { get; set; }       
        public YesNo ? isRegistrationDocumentUploaded { get; set; }
        public YesNo? isVerified { get; set; }        
        public RegistrationStatus  registrationStatus { get; set; }
      
    }
    public class Verification
    {
        public List<VerificationHod> verificationHods { get; set; }
        public string? clientIp { get; set; }
        public string? date { get; set; }
        public Int64? userId { get; set; }
    }

    public class BlTicketType
    {
        public long? hodOfficeId { get; set; }       
        public Int32? ticketTypeId { get; set; }
        public string ticketTypeName { get; set; }
        public string ticketTypeNameLocal { get; set; }
        public Int16? active { get; set; }
        public string? clientIp { get; set; }     
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        

    }
    public class BlCategory
    {
        public long? hodOfficeId { get; set; }
        public Int32? ticketCatId { get; set; }
        public string categoryName { get; set; }
        public string categoryNameLocal { get; set; }
        public Int16? active { get; set; }
        public string? clientIp { get; set; }
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }


    }
    public class BlSubCategory
    {
        public long? hodOfficeId { get; set; }
        public Int32? ticketCatId { get; set; }
        public Int32? ticketSubCatId { get; set; }
        public string subCategoryName { get; set; }
        public string subCategoryNameLocal { get; set; }
        public Int16? active { get; set; }
        public string? clientIp { get; set; }
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }


    }

}
