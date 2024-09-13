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
        public string? firstName { get; set; }
        public string? timeslot { get; set; }
        public string? lastName { get; set; }
        public string? emailId { get; set; }
        public string? phoneNo { get; set; }
        public decimal? consultancyFee { get; set; }
        public decimal? bookingFee { get; set; }
        public decimal? videoCallFee { get; set; }
        public Int16? paymentMethodId { get; set; }
        public string? nameOnCard { get; set; }
        public string? cardNo { get; set; }
        public string? expiryMonth { get; set; }
        public string? expiryYear { get; set; }
        public string? cvv { get; set; }
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
        public string? clientIp { get; set; }
        public string? entryDateTime { get; set; }
        public Int64? userId { get; set; }

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
