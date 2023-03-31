namespace HospitalManagementApi.Models.BLayer
{
    public class BlHospitalFacility
    {
        public Int16 CRUD { get; set; }
        public long? hospitalRegNo { get; set; }
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        public string? clientIp { get; set; }
        public List<BlHospitalFacilityItems>? Bl { get; set; }
    }
    public class BlHospitalFacilityItems
    {
        public Int32 facilityId { get; set; }
        public Int16? hospitalFacilityId { get; set; }
        public string? hospitalFacility { get; set; }
        public string? remarks { get; set; }
    }
}
