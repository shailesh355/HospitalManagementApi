namespace HospitalManagementApi.Models.BLayer
{
    public class BlHospitalDocuments
    {
        public Int16 CRUD { get; set; }
        public Int32 hospitalDocumentsId { get; set; }
        public long hospitalRegNo { get; set; }
        public string hospitalRegistrationNo { get; set; }
        public string licenseExpiryDate { get; set; }
        public string NABHCertificationLevel { get; set; }
        public string registeredWith { get; set; }
        public string anyOtherCertification { get; set; }
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        public string? clientIp { get; set; }
    }
}
