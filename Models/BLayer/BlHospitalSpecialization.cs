namespace HospitalManagementApi.Models.BLayer
{
    public class BlHospitalSpecialization
    {
        public Int16 CRUD { get; set; }
        public long? hospitalRegNo { get; set; }
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        public string? clientIp { get; set; }
        public List<BlHospitalSpecializationItems>? Bl { get; set; }
    }
    public class BlHospitalSpecializationItems
    {
        public Int16 specializationTypeId { get; set; }
        public string specializationTypeName { get; set; }
        public Int16 levelOfCareId { get; set; }
        public string levelOfCareName { get; set; }
        public Int32 specializationId { get; set; }
        public string? specializationName { get; set; }
    }

    public class BlDoctorSpecialization
    {
        public Int16 CRUD { get; set; }
        public long? doctorRegNo { get; set; }
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        public string? clientIp { get; set; }
        public List<BlHospitalSpecializationItems>? Bl { get; set; }
    }

    public class BlDoctorAvailability
    {
        public long? doctorRegNo { get; set; }
        public Int32? scheduleTimeId { get; set; }
        public DateTime? date { get; set; }
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        public string? clientIp { get; set; }
    }
}
