namespace HospitalManagementApi.Models.BLayer
{
    public class BlEmpaneled
    {
        public Int16? CRUD { get; set; }
        public long? hospitalRegNo { get; set; }
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        public string? clientIp { get; set; }
        public List<BlEmpaneledItems>? Bl { get; set; }
    }
    public class BlEmpaneledItems
    {
        public long? rowId { get; set; }
        public Int32? empaneledId { get; set; }
        public Int16? empaneledTypeId { get; set; }
        public string? empaneledTypeName { get; set; }
        public string? headName { get; set; }
    }

    public class BlDoctorEmpaneled
    {
        public Int16 CRUD { get; set; }

        public long? doctorRegNo { get; set; }
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        public string? clientIp { get; set; }
        public List<BlEmpaneledItems>? Bl { get; set; }
    }

    public class BlProviderEmpaneled
    {
        public Int16 CRUD { get; set; }
        public long? hospitalRegNo { get; set; }
        public Int16? empaneledTypeId { get; set; }
        public string? empaneledTypeName { get; set; }
        public Int32? empaneledId { get; set; }
        public string? empaneledName { get; set; }
        public string? providerName { get; set; }
        public string? fromDate { get; set; }
        public string? toDate { get; set; }
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        public string? clientIp { get; set; }
    }

    public class BlEmpaneledIns
    {
        public Int16? CRUD { get; set; }
        public long? hospitalRegNo { get; set; }
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        public string? clientIp { get; set; }
        public List<BlEmpaneledItemsIns>? blEmpaneledItemsIns { get; set; }
        public List<BlProviderEmpaneled>? blProviderItemsIns { get; set; }
    }
    public class BlEmpaneledItemsIns
    {
        public long? rowId { get; set; }
        public Int32? empaneledId { get; set; }
        public Int16? empaneledTypeId { get; set; }
        public string? empaneledTypeName { get; set; }
        public string? headName { get; set; }
    }
}
