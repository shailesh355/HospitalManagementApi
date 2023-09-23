using BaseClass;

namespace BaseClass
{
    public class SmsTemplates
    {
        public static SmsBody OtpMessage(string type, int otp)
        {
            var ms = new SmsBody
            {
                IsOtp = true,
                TemplateId = 89898,
                TemplateMessageBody = string.Format(@"OTP to retrieve your {0} of CEI Portal is {1}. Regards NIC Raipur", type, otp.ToString())
            };
            return ms;
        }
        public static SmsBody ApplicationSubmitToUserMessage(int applicationNo)
        {
            var ms = new SmsBody
            {
                TemplateId = 67198,
                TemplateMessageBody = string.Format(@"Your Application number {0} has been submiteed successfully. Regards NIC Raipur", applicationNo.ToString())
            };
            return ms;
        }
    }
}
