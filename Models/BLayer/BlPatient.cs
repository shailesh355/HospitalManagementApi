using BaseClass;
using DmfPortalApi.Models.AppClass;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementApi.Models.BLayer
{
    public class BlPatient
    {
        public long? patientRegNo { get; set; }
        public string? patientId { get; set; }
        public string? patientNameEnglish { get; set; }
        public string? patientNameLocal { get; set; }
        public Int16? stateId { get; set; }
        public Int16? districtId { get; set; }
        public string? address { get; set; }
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])(?=.*[a-z]).{8,15}$", ErrorMessage = "Invalid email address")]
        public string? emailId { get; set; }
        public string? mobileNo { get; set; }
        public YesNo? active { get; set; }
        public YesNo? isVerified { get; set; }
        public string? clientIp { get; set; }
        public string? entryDateTime { get; set; }
        public Int64? userId { get; set; }
        public int? registrationYear { get; set; }
        public string? genderId { get; set; } = "";
        public Int32? pinCode { get; set; } = 0;
        public string? password { get; set; } = "";

    }
    public class BlPatientOtp
    {

        public long? patientRegNo { get; set; }
        public Int32? OTP { get; set; }
        public string? mobileNo { get; set; }
        public YesNo? active { get; set; }
        public string? password { get; set; }
        public YesNo? isVerified { get; set; }
        public string? clientIp { get; set; }
        public string? entryDateTime { get; set; }
        public Int64? userId { get; set; }

    }

    public class BlDoctorAppointment
    {
        public long? Id { get; set; }
        public long? doctorRegNo { get; set; }
        public long? patientRegNo { get; set; }
        public long? scheduleTimeId { get; set; }
        public string? timeslot { get; set; }
        public DateTime? scheduleDate { get; set; }
        public decimal? consultancyFee { get; set; }
        public decimal? bookingFee { get; set; }
        public decimal? videoCallFee { get; set; }
        public decimal? appointmentStatusId { get; set; } = (Int16)AppointmentStatus.PendingConfirmation;
        public string? appointmentStatusName { get; set; } = "Pending Confirmation";
        public Int64? userId { get; set; }
        public string? entryDateTime { get; set; }
        public string? clientIp { get; set; }
        public string? remark { get; set; }
    }

    public class BlAddWallet
    {
        public long? patientRegNo { get; set; }
        public decimal? walletAmount { get; set; }
        public decimal? walletReleasedAmount { get; set; }
        public decimal? walletBalanceAmount { get; set; }
        public Int32? actionId { get; set; }
        public string? Remark { get; set; }
        public Int64? transactionNo { get; set; }
        public string? razorPaytransactionNo { get; set; }
        public string? clientIp { get; set; }
        public string? entryDateTime { get; set; }
        public Int64? userId { get; set; }
        public Int16? paymentStatus { get; set; }
        public string? paymentStatusName { get; set; }
        public string? successURL { get; set; }
        public string? razorpay_payment_id { get; set; }
        public string? razorpay_signature { get; set; }
        ,
                                        

    }

    public class CreatePaymentOrder
    {
        public long? transactionNo { get; set; }
        public string? OrderId { get; set; }
        public string? RazorpayKey { get; set; }
        public int? amount { get; set; }
        public string? Currency { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
        public string checkOutURL { get; set; }


    }
    public class BlAppointmentTransaction
    {
        public long? patientRegNo { get; set; }
        public long? appointmentNo { get; set; }
        public decimal? walletAmount { get; set; }
        public decimal? walletReleasedAmount { get; set; }
        public decimal? walletBalanceAmount { get; set; }
        public Int16? actionId { get; set; }
        public string? Remark { get; set; }
        public string? transactionNo { get; set; }
        public string? clientIp { get; set; }
        public string? entryDateTime { get; set; }
        public Int64? userId { get; set; }

    }
    public class BlAppointmentReleasedTransaction
    {
        public long? patientRegNo { get; set; }
        public long? doctorRegNo { get; set; }
        public decimal? walletReleasedAmount { get; set; }
        public decimal? discountAmount { get; set; }
        public decimal? serviceCharge { get; set; }
        public decimal? igst { get; set; }
        public decimal? otherCharges { get; set; }
        public decimal? totalReleasedAmount { get; set; }
        public Int16? actionId { get; set; }
        public string? Remark { get; set; }
        public string? transactionNo { get; set; }
        public string? clientIp { get; set; }
        public string? entryDateTime { get; set; }
        public Int64? userId { get; set; }

    }
    public class BlUpdateWallet
    {
        public long? patientRegNo { get; set; }
        public decimal? walletAmount { get; set; }
        public string? Remark { get; set; }
        public string? transactionNo { get; set; }
        public string? clientIp { get; set; }
        public Int64? userId { get; set; }
        public Int16? paymentStatus { get; set; }

    }
}
