using BaseClass;
using DmfPortalApi.Models.AppClass;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementApi.Models.BLayer
{
    public class BlDoctor
    {

        public long? doctorRegNo { get; set; } = 0; 
        public string? doctorNameEnglish { get; set; } = "";
        public string? doctorNameLocal { get; set; } = "";
        public Int16? stateId { get; set; } = 0;
        public Int16? districtId { get; set; } = 0;
        public string? address { get; set; } = "";
        //[RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])(?=.*[a-z]).{8,15}$", ErrorMessage = "Invalid email address")]
        public string? emailId { get; set; } = "";
        public string? mobileNo { get; set; } = "";
        public YesNo? active { get; set; } = YesNo.No;

        public YesNo? isVerified { get; set; } = 0;
        public string? verificationDate { get; set; } = "";
        public int? verifiedByLoginId { get; set; } = 0;
        public RegistrationStatus? registrationStatus { get; set; } = RegistrationStatus.Pending;
        public string? clientIp { get; set; } = "";
        public string? entryDateTime { get; set; } = "";
        public Int64? userId { get; set; } = 0;
        public int? registrationYear { get; set; } = 0;
        public string? password { get; set; } = "";
        public Int32? cityId { get; set; } = 0;
        public string? pinCode { get; set; } = "";
        public string? phoneNumber { get; set; } = "";
        public string? landMark { get; set; } = "";
        public string? fax { get; set; } = "";
        public string? cityName { get; set; } = "";
        public string? firstName { get; set; } = "";
        public string? middleName { get; set; } = "";
        public string? lastName { get; set; } = "";


    }
    public class VerificationDoctor
    {
        public long doctorRegNo { get; set; }
        public YesNo? isRegistrationDocumentUploaded { get; set; }
        public YesNo? isVerified { get; set; }
        public RegistrationStatus registrationStatus { get; set; }

    }
    public class FilterDoctor
    {
        public string? DoctorSpec { get; set; }
        public Int16? districtId { get; set; }
    }

    public class BlDoctorAcademic
    {
        public Int16 CRUD { get; set; }
        public long? doctorRegNo { get; set; }
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        public string? clientIp { get; set; }
        public List<BlDoctorAcademicItems>? Bl { get; set; }
    }
    public class BlDoctorAcademicItems
    {
        public Int16 degreePgId { get; set; }
        public string degreePgName { get; set; }

        public Int16 specialityId { get; set; }
        public string specialityName { get; set; }

        public string? collegeName { get; set; }
        public Int32 passingYear { get; set; }
    }
    public class BlDoctorExperience
    {
        public Int16 CRUD { get; set; }
        public long? doctorRegNo { get; set; }
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        public string? clientIp { get; set; }
        public List<BlDoctorExperienceItems>? Bl { get; set; }
    }
    public class BlDoctorExperienceItems
    {
        public long? hospitalRegNo { get; set; }
        public string hospitalNameEnglish { get; set; }
        public string hospitalNameLocal { get; set; }
        public Int32 yearFrom { get; set; }
        public Int32 yearTo { get; set; }
        public Int16 designationId { get; set; }
        public string designationName { get; set; }
        public string hospitalNameOther { get; set; }
    }

    public class BlDoctorWorkArea
    {
        public long? doctorRegNo { get; set; }
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        public string? clientIp { get; set; }
        public List<BlDoctorWorkAreaItems>? Bl { get; set; }
    }
    public class BlDoctorWorkAreaItems
    {
        public long? hospitalRegNo { get; set; }
        public string? hospitalNameEnglish { get; set; }
        public Int16? consultancyTypeId { get; set; }
        public string? consultancyTypeName { get; set; }
        public decimal? price { get; set; }
        public Int16? stateId { get; set; }
        public string? stateName { get; set; }
        public Int16? districtId { get; set; }
        public string? districtName { get; set; }
        public string? address1 { get; set; }
        public string? address2 { get; set; }
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])(?=.*[a-z]).{8,15}$", ErrorMessage = "Invalid email address")]
        //public YesNo? active { get; set; }
        public YesNo? isVerified { get; set; }
        public string? verificationDate { get; set; }
        public int? verifiedByLoginId { get; set; }
        //public RegistrationStatus? registrationStatus { get; set; }
        //public int? registrationYear { get; set; }
        public string? clientIp { get; set; }
        public string? entryDateTime { get; set; }
        public Int64? userId { get; set; }
        //public string? password { get; set; }
        public Int32? cityId { get; set; }
        public string? cityName { get; set; }
        public string? pinCode { get; set; }
        public string? phoneNumber { get; set; }
        public string? landMark { get; set; }
        public string? fax { get; set; }
        //public string? specialization { get; set; }
        public string? hospitalAddress { get; set; }
        public List<BlDocumentNew>? BlDocument { get; set; }
    }

    public class VerificationDoctorDetail
    {
        public List<VerificationDoctor> VerificationDoctor { get; set; }
        public string? clientIp { get; set; }
        public string? date { get; set; }
        public Int64? userId { get; set; }
    }
    public class DoctorProfilePart1
    {
        //public List<VerificationDoctor> VerificationDoctor { get; set; }
        public long? doctorRegNo { get; set; }
        public string? firstName { get; set; }
        public string? middleName { get; set; }
        public string? lastName { get; set; }
        public string? phoneNumber { get; set; }
        public Int16? genderId { get; set; }
        public string? genderName { get; set; }
        public string? dateOfBirth { get; set; }
        public string? aboutMe { get; set; }
        public string? clientIp { get; set; }
        public string? date { get; set; }
        public Int64? userId { get; set; }
        public string? address1 { get; set; }
        public string? address2 { get; set; }
        public Int32? cityId { get; set; }
        public string? cityName { get; set; }
        public Int16? stateId { get; set; }
        public string? stateName { get; set; }
        public Int16? countryId { get; set; }
        public string? countryName { get; set; }
        public string? pinCode { get; set; }
        public List<BlDocumentNew>? BlDocument { get; set; }
        public string? specialization { get; set; }
        public Int16? specializationId { get; set; }

    }
    public class DoctorAward
    {
        public long? doctorRegNo { get; set; }
        public string? clientIp { get; set; }
        public string? date { get; set; }
        public Int64? userId { get; set; }
        public List<DoctorAwardItems>? items { get; set; }
    }
    public class DoctorAwardItems
    {
        public string? awardName { get; set; }
        public string? awardYear { get; set; }
    }

    public class DoctorMembership
    {
        public long? doctorRegNo { get; set; }
        public string? clientIp { get; set; }
        public string? date { get; set; }
        public Int64? userId { get; set; }
        public List<DoctorMembershipItems>? items { get; set; }
    }
    public class DoctorMembershipItems
    {
        public string? membershipName { get; set; }
    }

    public class DoctorAddOns
    {
        public long? doctorRegNo { get; set; }
        public string? clientIp { get; set; }
        public string? date { get; set; }
        public Int64? userId { get; set; }
        public List<DoctorAddOnsItems>? items { get; set; }
    }
    public class DoctorAddOnsItems
    {
        public string certificateName { get; set; }
        public Int32 year { get; set; }
        public string reason { get; set; }
    }
    public class DoctorIndaminity
    {
        public long? doctorRegNo { get; set; }
        public string? clientIp { get; set; }
        public string? date { get; set; }
        public Int64? userId { get; set; }
        public Int16 isIndaminity { get; set; }
    }

    public class DoctorMCR
    {
        public long? doctorRegNo { get; set; }
        public string? clientIp { get; set; }
        public string? date { get; set; }
        public Int64? userId { get; set; }
        public List<DoctorMCRItems>? items { get; set; }
    }
    public class DoctorMCRItems
    {
        public string? remark { get; set; }
        public Int32? year { get; set; }
        public string? mcrName { get; set; }
    }

    public class DoctorScheduleDate
    {
        public Int32? scheduleDateId { get; set; }
        public long? doctorRegNo { get; set; }
        public Int16? dayId { get; set; }
        public string? day { get; set; }
        public Int16? isActive { get; set; }
        public string? clientIp { get; set; }
        public string? date { get; set; }
        public Int64? userId { get; set; }
        public List<DoctorScheduleTime>? items { get; set; }
    }
    public class DoctorScheduleTime
    {
        public Int32? scheduleTimeId { get; set; }
        public Int32? scheduleDateId { get; set; }
        public string? fromTime { get; set; }
        public string? toTime { get; set; }
        public Int16? isActive { get; set; }
        public Int16? patientLimit { get; set; }
    }

    public class DoctorScheduleDatewise
    {
        public long? doctorRegNo { get; set; }
        public Int16? isActive { get; set; }
        public string? clientIp { get; set; }
        public Int64? userId { get; set; }
        public List<DoctorScheduleDatewiseTime>? items { get; set; }
    }
    public class DoctorScheduleDatewiseTime
    {

        public long? scheduleTimeId { get; set; }
        public string? scheduleDate { get; set; }
        public string? fromTime { get; set; }
        public string? toTime { get; set; }
        public Int16? isActive { get; set; }
    }

    public class BlDoctorWorkAreaItemsDoc
    {
        public long? hospitalRegNo { get; set; }
        public string? hospitalNameEnglish { get; set; }
        public Int16? consultancyTypeId { get; set; }
        public string? consultancyTypeName { get; set; }
        public decimal? price { get; set; }
        public string? hospitalAddress { get; set; }
        public List<BlDocument>? BlDocument { get; set; }
    }

}
