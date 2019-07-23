using Ninject.Activation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Code.Providers
{
    public class EmailServiceProvider : Provider<IEmailService>
    {
        protected override IEmailService CreateInstance(IContext context)
        {
            return new EmailService(Config.SmtpPort, Config.SmtpAddress, Config.EmailUsername, Config.EmailPassword);
        }
    }
}