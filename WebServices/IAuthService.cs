using Common.Operations;
using Entities;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.BigParams.auth;
using WebServices.enums;
using WebServices.structs;

namespace WebServices
{
    public interface IAuthService
    {
        Citizen Register(RegisterInfo info); 
        Session Login(Credentials credentials, string userIP);
        /// <summary>
        /// Creates session and saves it to repository
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="userIP"></param>
        /// <returns>created session with ID</returns>
        Session CreateSession(CreateSessionParameters parameters);
        string CreateCookie(string IP, DateTime time, int citizenID);

        void ForgotPassword(string email);

        MethodResult CanChangePassword(Citizen citizen, string oldPassword, string newPassword);
        void ChangePassword(Citizen citizen, string newPassword);
        void Logoff(Citizen citizen);
    }
}
