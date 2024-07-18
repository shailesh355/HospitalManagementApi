using BaseClass;
namespace HospitalManagementApi.Models.BLayer
{
    public class BlCommon
    {
        Utilities util = new();
        public BlCommon()
        {

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
}
