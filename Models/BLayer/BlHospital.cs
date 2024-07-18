using BaseClass;
using DmfPortalApi.Models.AppClass;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementApi.Models.BLayer
{
    public class BlHospital
    {

        public long? hospitalRegNo { get; set; }
        public string? hospitalNameEnglish { get; set; }
        public string? hospitalNameLocal { get; set; }
        public Int16? stateId { get; set; } = 0;
        public Int16? districtId { get; set; } = 0;
        public string? address { get; set; }
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])(?=.*[a-z]).{8,15}$", ErrorMessage = "Invalid email address")]
        public string? emailId { get; set; }
        public string? mobileNo { get; set; }
        public YesNo? active { get; set; }

        public YesNo? isVerified { get; set; }
        public string? verificationDate { get; set; }
        public int? verifiedByLoginId { get; set; }
        public RegistrationStatus? registrationStatus { get; set; }
        public string? clientIp { get; set; }
        public string? entryDateTime { get; set; }
        public Int64? userId { get; set; }
        public int? registrationYear { get; set; }
        public string? password { get; set; }
        public Int32? cityId { get; set; } = 0;
        public string? pinCode { get; set; }
        public string? phoneNumber { get; set; }
        public string? landMark { get; set; }
        public string? fax { get; set; }
        public Int16? isCovid { get; set; }
        public decimal? latitude { get; set; }
        public decimal? longitude { get; set; }
        public Int16? typeOfProviderId { get; set; }
        public string? website { get; set; }
        public Int16? natureOfEntityId { get; set; }
        public string? rohiniId { get; set; }
        public string? cityName { get; set; }


    }
    public class VerificationHospital
    {
        public long hospitalRegNo { get; set; }
        public YesNo? isRegistrationDocumentUploaded { get; set; }
        public YesNo? isVerified { get; set; }
        public RegistrationStatus registrationStatus { get; set; }

    }
    public class ResetPassword
    {

        public string oldPassword { get; set; }
        public string Password { get; set; }
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        public string? clientIp { get; set; }


    }
    public class VerificationDetail
    {
        public List<VerificationHospital> VerificationHospital { get; set; }
        public string? clientIp { get; set; }
        public string? date { get; set; }
        public Int64? userId { get; set; }
    }

    public class Filter
    {
        public string? hospitalSpec { get; set; }
        public Int16? districtId { get; set; }
    }

    public class BlHospitalMI
    {
        public Int16 CRUD { get; set; }
        public long? hospitalRegNo { get; set; }
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        public string? clientIp { get; set; }
        public List<BlMainContactItems>? BLMC { get; set; }
        public Int32? cityId { get; set; }
        public string? pinCode { get; set; }
        public string? phoneNumber { get; set; }
        public string? landMark { get; set; }
        public string? fax { get; set; }
        public Int16? isCovid { get; set; }
        public decimal? latitude { get; set; }
        public decimal? longitude { get; set; }
        public Int16? typeOfProviderId { get; set; }
        public string? website { get; set; }
        public Int16? natureOfEntityId { get; set; }
        public string? rohiniId { get; set; }
        public string licenseExpiryDate { get; set; }
        public Int16? isNABH { get; set; }
        public Int16? isNABL { get; set; }
        public Int16? isISO { get; set; }
        public string registeredWith { get; set; }
        public string anyOtherCertification { get; set; }
        public string typeOfProviderName { get; set; }
        public string natureOfEntityName { get; set; }
        public string cityName { get; set; }
        public Int16? hospitalTypeId { get; set; }
        public string? hospitalTypeName { get; set; }
        public string nabhCertificationLevel { get; set; }

    }

    public class HospitalDocs
    {
        public long hospitalRegNo { get; set; }
        public Int64? userId { get; set; }
        public string? clientIp { get; set; }
        public string? entryDateTime { get; set; }
        public List<BlDocumentNew>? BlDocument { get; set; }
    }

    public class HomeSearch
    {

        public Int16? searchTypeId { get; set; } = 0;
        public string? searchedText { get; set; } = null;
        public Int64? userId { get; set; }
        public string? clientIp { get; set; }
        public Int16? districtId { get; set; } = 0;
        public Double? lat{ get; set; }
        public Double? longi { get; set; }
        public Int16? isMobileView { get; set; } = 0;
        public Int16? searchSubCategoryTypeId { get; set; } = 0;
        public Int16? specializationId { get; set; } = 0;
        public Int16? genderId { get; set; } = 0;

    }

}
