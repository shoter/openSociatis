using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using System.Net.Mail;
using System.Net;
using WebServices.Emails;

namespace WebServices
{
    public class EmailService : IEmailService
    {
        private readonly int smtpPort;
        private readonly string smtpAddress;
        private readonly string user;
        private readonly string password;

        public EmailService(int smtpPort, string smtpAddress, string user, string password)
        {
            this.smtpPort = smtpPort;
            this.smtpAddress = smtpAddress;
            this.user = user;
            this.password = password;
        }
        public void SendEmail(string to, string subject, string body)
        {
            using (var mail = new MailMessage("donotreply@sociatis.net", to))
            using (var smtp = new SmtpClient(smtpAddress, smtpPort))
            {


                smtp.EnableSsl = false;
                smtp.Credentials = new NetworkCredential(user, password);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                //    smtp.UseDefaultCredentials = false;
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                smtp.Send(mail);
            }

        }

        public void SendPasswordChangeEmailOnForgot(Citizen citizen, string newPassword)
        {
            var subject = "Lost Password";
            var body = EmailTemplate.LostPassword
                .GetBody(subject, citizen.Entity.Name, newPassword);

            SendEmail(citizen.Email, subject, body);
        }

        public void InformAboutException(Exception e)
        {
            SendEmail(
                to: "damian@laczak.net.pl",
                subject: "Exception",
                body: e.ToString()
                );
        }
    }
}
