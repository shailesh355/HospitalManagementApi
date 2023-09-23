namespace HospitalManagementApi.Models.BLayer
{
    public class BlMasters
    {
    }
    public class BlCity
    {
        public Int16 CRUD { get; set; }
        public Int16 districtId { get; set; }
        public Int32 cityId { get; set; }
        public Int16 stateId { get; set; }
        public Int16 cityVersion { get; set; }
        public string cityNameEnglish { get; set; } = "";
        public string cityNameLocal { get; set; } = "";
        public string censusCityCode { get; set; } = "";
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        public string? clientIp { get; set; }
        public Int32 cityCount { get; set; }
        
    }
}
