namespace HospitalManagementApi.Models.BLayer
{
    public class BlBedComposition
    {
        public Int16 CRUD { get; set; }
        public long? hospitalRegNo { get; set; }
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        public string? clientIp { get; set; }
        public List<BlBedCompositionItems>? Bl { get; set; }
    }

    public class BlBedCompositionItems
    {
        public Int32 bedCompositionId { get; set; }
        public Int16 noOfBeds { get; set; }
        public Int16 rentPerDay { get; set; }

    }
}
