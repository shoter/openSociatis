using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface IEmailService
    {

        void SendEmail(string to, string subject, string body);

        void SendPasswordChangeEmailOnForgot(Citizen citizen, string newPassword);

        void InformAboutException(Exception e);
    }
}
