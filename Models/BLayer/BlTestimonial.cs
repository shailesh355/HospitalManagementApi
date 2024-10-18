using BaseClass;

namespace HospitalManagementApi.Models.BLayer
{
    public class BlTestimonial
    {
        public Int64 testimonialId { get; set; } = 0;
        public Int16 roleType { get; set; }
        public string fullName { get; set; }
        public string content{ get; set; }
        public Int16 actionStatus { get; set; }
        public string actionDate { get; set; }
        public Int64 userId { get; set; }
        public string clientIp { get; set; }
        
    }
    public class BlTestimonialVerification
    {
        public string clientIp { get; set; }
        public Int64 userId { get; set; }
        public List<Testimonials> collectionOfTestimonialsIds  { get; set; }
        public Int16 actionStatus { get; set; }
        public string actionDate { get; set; }

    }
    public class Testimonials
    {
        public long? testimonialsId { get; set; }
    }
}
