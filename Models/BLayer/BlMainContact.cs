using System.ComponentModel.DataAnnotations;
namespace HospitalManagementApi.Models.BLayer
{
    public class BlMainContact
    {
        public Int16 CRUD { get; set; }
        public long? hospitalRegNo { get; set; }
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        public string? clientIp { get; set; }
        public List<BlMainContactItems>? Bl { get; set; }
    }

    public class BlMainContactItems
    {
        public Int32? mainContactId { get; set; }
        public Int16? designationId { get; set; }
        public string? designationName { get; set; }
        public string contactPersonName { get; set; }
        public string mobileNo { get; set; }
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])(?=.*[a-z]).{8,15}$", ErrorMessage = "Invalid email address")]
        public string emailId { get; set; }
    }
}
