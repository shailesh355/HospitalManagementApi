

using HospitalManagementApi.Models.BLayer;

namespace DmfPortalApi.Models.AppClass
{
    public class BlDocumentNew
    {
        //public List<IFormFile>? files { get; set; }
        public Int64 documentId { get; set; }
        public Int32 documentNumber { get; set; }
        public Int16 amendmentNo { get; set; }
        public DocumentType documentType { get; set; }
        public string? documentName { get; set; }
        public string? documentExtension { get; set; }
        public string? documentMimeType { get; set; }
        public string? clientIp { get; set; }
        public int stateId { get; set; }
        /// <summary>
        /// Refrenced for dpt_table_id 
        /// </summary>
        public DocumentImageGroup documentImageGroup { get; set; }
        public Int64? loginId { get; set; }
        //public List<byte[]>? documentInByte { get; set; }
        public long? userId { get; set; }
        public List<string>? documentInByteS { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public Int16? uploaded { get; set; } = 0;
        public Int16? deleted { get; set; }
    }
}
