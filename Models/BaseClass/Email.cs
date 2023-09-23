using System.Net;
using System.Net.Mail;
namespace BaseClass
{
    public class Email
    {
        private string _senderEmail;
        private string _password;
        private string _hostUrl;
        private int _port;
        private Utilities util = new();
        public Email()
        {
            try
            {
                this._senderEmail = util.GetAppSettings("EmailConfiguration", "SenderEmail").message;
                this._password = util.GetAppSettings("EmailConfiguration", "Password").message;
                this._hostUrl = util.GetAppSettings("EmailConfiguration", "HostUrl").message;
                this._port = Convert.ToInt32(util.GetAppSettings("EmailConfiguration", "Port").message);
            }
            catch { throw; }
        }
        public ReturnClass.ReturnBool Send(string ToAddres, string emailSubject, string emailBody, List<Attachment> Attachments)
        {
            ReturnClass.ReturnBool rb = new();
            try
            {
                MailMessage message = new();
                SmtpClient smtp = new();
                message.From = new MailAddress(_senderEmail);
                message.To.Add(new MailAddress(ToAddres));
                message.Subject = emailSubject;
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = emailBody;
                smtp.Port = _port;
                smtp.Host = _hostUrl;
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = false;
                try
                {
                    if (Attachments.Count > 0)
                    {
                        foreach (Attachment attachment in Attachments)
                            message.Attachments.Add(attachment);
                    }
                }
                catch
                {

                }
                smtp.Credentials = new NetworkCredential(_senderEmail, _password);
                smtp.Send(message);
                rb.status = true;
                rb.message = "Email Sent Successfully";
            }
            catch (Exception ex)
            {
                rb.message = "Failed to send email";
                WriteLog.Error("email(error)", ex);
            }
            return rb;
        }

        public ReturnClass.ReturnBool Send(string ToAddres, string emailSubject, string emailBody, List<Attachment> Attachments, string ccAddress)
        {
            ReturnClass.ReturnBool rb = new();
            try
            {
                MailMessage message = new();
                SmtpClient smtp = new();
                message.From = new MailAddress(_senderEmail);
                message.To.Add(new MailAddress(ToAddres));
                message.CC.Add(new MailAddress(ccAddress));
                message.Subject = emailSubject;
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = emailBody;
                smtp.Port = _port;
                smtp.Host = _hostUrl;
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = false;
                try
                {
                    if (Attachments.Count > 0)
                    {
                        foreach (Attachment attachment in Attachments)
                            message.Attachments.Add(attachment);
                    }
                }
                catch
                {

                }
                smtp.Credentials = new NetworkCredential(_senderEmail, _password);
                smtp.Send(message);
                rb.status = true;
                rb.message = "Email Sent Successfully";
            }
            catch (Exception ex)
            {
                rb.message = "Failed to send email";
                WriteLog.Error("email(error)", ex);
            }
            return rb;
        }
    }
}