namespace HospitalManagementApi.Models.BLayer
{
    public class BlInfrastructure
    {
        public Int16 CRUD { get; set; }
        public long? hospitalRegNo { get; set; }
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        public string? clientIp { get; set; }
        public List<BlInfrastructureItems>? Bl { get; set; }
    }
    public class BlInfrastructureItems
    {
        public Int32 infrastructureId { get; set; }
        public Int16? infrastructureFacilitiesId { get; set; }
        public string? infrastructureFacilities { get; set; }
        public Int16? medicalInfrastructureId { get; set; }
        public string? medicalInfrastructure { get; set; }
        public string? remarks { get; set; }
    }
}
