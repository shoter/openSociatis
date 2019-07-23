using Common;
using Common.Operations;
using Common.utilities;
using Entities;
using Entities.enums;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebServices.BigParams.auth;
using WebServices.enums;
using WebServices.Helpers;
using WebServices.structs;

namespace WebServices
{
    public class AuthService : BaseService, IAuthService
    {
        ICitizenRepository citizensRepository;
        ISessionRepository sessionRepository;
        IEntityRepository entitiesRepository;
        ICountryRepository countriesRepository;
        IRegionRepository regionsRepository;
        IConfigurationRepository configurationRepository;
        ITransactionsService transactionService;
        ICitizenService citizensService;
        IEmailService emailService;

        Random rand = new Random();
        public AuthService(ICitizenRepository citizensRepository, ISessionRepository sessionRepository, ICountryRepository countriesRepository,
            IRegionRepository regionsRepository, IConfigurationRepository configurationRepository, ITransactionsService transactionService,
            IEntityRepository entitiesRepository, ICitizenService citizensService, IEmailService emailService)
        {
            this.citizensRepository = citizensRepository;
            this.sessionRepository = sessionRepository;
            this.regionsRepository = regionsRepository;
            this.configurationRepository = configurationRepository;
            this.transactionService = Attach(transactionService);
            this.countriesRepository = countriesRepository;
            this.entitiesRepository = entitiesRepository;
            this.citizensService = Attach(citizensService);
            this.emailService = Attach(emailService);
        }

        public Session Login(Credentials credentials, string userIP)
        {
            var citizen = citizensRepository.FirstOrDefault(u => u.Entity.Name == credentials.Username);

            if (citizen != null && citizen.Password == credentials.PasswordHash)
            {
                CreateSessionParameters parameters = new CreateSessionParameters()
                {
                    IP = userIP,
                    RememberMe = credentials.RememberMe,
                    CitizenID = citizen.ID
                };
                var session = CreateSession(parameters);
                return session;
            }
            return null;
        }

        /// <summary>
        /// No checks are being made here. Function assumes that you pass proper data.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Session CreateSession(CreateSessionParameters parameters)
        {

            var session =  new Session()
            {
                CitizenID = parameters.CitizenID,
                Cookie = CreateCookie(parameters.IP, DateTime.Now, parameters.CitizenID),
                ExpirationDate = DateTime.Now + TimeSpan.FromHours(1),
                IP = parameters.IP,
                RememberMe = parameters.RememberMe
            };

            if (parameters.RememberMe)
                session.ExpirationDate = session.ExpirationDate.AddDays(7);

            var citizen = citizensRepository.GetById(session.CitizenID);
            citizen.LastActivityDay = GameHelper.CurrentDay;

            sessionRepository.Add(session);
            sessionRepository.SaveChanges();
            return session;
        }

        public string CreateCookie(string IP, DateTime time, int citizenID)
        {
            string cookie = IP + "|" + time.ToString() + citizenID.ToString();
            for (int i = 0; i < 10; ++i)
                cookie += rand.Next('a', 'z').ToString();
            return Base64.Base64Encode(cookie);
        }




        public void ForgotPassword(string email)
        {
            var citizen = citizensRepository.GetByEmail(email);
            if (citizen != null)
            {
                var newPassword = RandomGenerator.GenerateString(10);
                citizensService.SetPassword(citizen, newPassword);
                emailService.SendPasswordChangeEmailOnForgot(citizen, newPassword);
            }
        }

        public void Logoff(Citizen citizen)
        {
            var sessions = sessionRepository.Where(s => s.CitizenID == citizen.ID)
                .ToList();

            foreach (var session in sessions)
                sessionRepository.Remove(session);

            sessionRepository.SaveChanges();
        }

        public Citizen Register(RegisterInfo info)
        {
            var country = countriesRepository.GetById(info.CountryID);
            if (country == null)
                throw new Exception("Country not found");
            var region = regionsRepository.GetById(info.RegionID);
            if (region == null)
                throw new Exception("Region not found");

            Citizen citizen = citizensService.CreateCitizen(info);
            return citizen;
        }

        public MethodResult CanChangePassword(Citizen citizen, string oldPassword, string newPassword)
        {
            if (oldPassword == newPassword)
                return new MethodResult("The new password is the same as old!");

            return CheckPassword(newPassword);
        }

        public MethodResult CheckPassword(string password)
        {
            if (password.Length < 6)
                return new MethodResult("Password must have length of at least 6 characters!");

            return MethodResult.Success;
        }

        public void ChangePassword(Citizen citizen, string newPassword)
        {
            citizensService.SetPassword(citizen, newPassword);
        }

        public Session Switch(SwitchSessionParameters parameters)
        {
            var entity = entitiesRepository.GetById(parameters.EntityID);
            CreateSessionParameters createSessionParameters = new CreateSessionParameters()
            {
                IP = parameters.IP,
                RememberMe = parameters.OldSession.RememberMe,
                CitizenID = parameters.OldSession.CitizenID,
            };
            var entityOldSession = sessionRepository.GetById(parameters.OldSession.ID);
            sessionRepository.Remove(entityOldSession.ID);

            var session = CreateSession(createSessionParameters);
            sessionRepository.Add(session);
            sessionRepository.SaveChanges();

            return session;
        }
    }
}
