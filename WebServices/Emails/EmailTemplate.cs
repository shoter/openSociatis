using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WebServices.Emails
{
    public class EmailTemplate
    {
        private Mutex mutalisk = new Mutex();

        private static EmailTemplate lostPassword = new EmailTemplate(@"~\Content\Emails\LostPassword.html", 
            typeof(string), //Subject
            typeof(string), //Name
            typeof(string) //Password
            );

        private static string emailTemplate;

        public static EmailTemplate LostPassword => lostPassword;


        private Type[] emailArguments;

        public string Path { get; set; }

        /// <summary>
        /// Creates class that will have direct Path to the email template
        /// </summary>
        /// <param name="relativePath">like ~/Content/something.something</param>
        public EmailTemplate(string relativePath)
        {
            if (emailTemplate == null)
            {
                lock (mutalisk)
                {
                    emailTemplate = File.ReadAllText(System.IO.Path.Combine(HttpContext.Current.Server.MapPath(@"~\Content\Emails\template.html")));
                }
            }

            Path = System.IO.Path.Combine(HttpContext.Current.Server.MapPath(relativePath));
            if (File.Exists(Path) == false)
                throw new FileNotFoundException($"{relativePath} not found!", System.IO.Path.GetFileName(Path));
        }

        public EmailTemplate(string relativePath, params Type[] usedTypes) : this(relativePath)
        {
            emailArguments = usedTypes;
        }

        public string GetBody(params object[] parameters)
        {
            if (parameters.Length != emailArguments.Length)
                throw new ArgumentException($"Supplied {parameters.Length}, but you should supply {emailArguments.Length}!");

            for (int i = 0; i < parameters.Length; ++i)
            {
                Type parType = parameters[i].GetType();

                if (parType != emailArguments[i])
                    throw new ArgumentException($"Wrong type at {i}. Expected {emailArguments[i].Name} - found {parType.Name}!");
            }


            return
                emailTemplate.Replace("{{:BODY:}}",
                string.Format(File.ReadAllText(Path), parameters));
        }


    }
}
