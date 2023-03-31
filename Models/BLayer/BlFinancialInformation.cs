namespace HospitalManagementApi.Models.BLayer
{
    public class BlFinancialInformation
    {
        public Int16 CRUD { get; set; }
        public long? hospitalRegNo { get; set; }
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        public string? clientIp { get; set; }
        public List<BlFinancialInformationItems>? Bl { get; set; }
    }

    public class BlFinancialInformationItems
    {
        public Int32 financialInformationId { get; set; }
        public string? accountNumber { get; set; }
        public string beneficiaryName { get; set; }
        public Int16 accountTypeId { get; set; }
        public string accountTypeName { get; set; }
        public string bankName { get; set; }
        public string bankAddress { get; set; }
        public string IFSCCode { get; set; }
        public string PANNo { get; set; }
        public string nameOnPAN { get; set; }
        public Int16 TDSExemptionPercent { get; set; }
        public Int16 TDSExemptionLimit { get; set; }
        public Int16 TDSExemptionPeriod { get; set; }
        public Int16? acive { get; set; } = 1;
    }
}
