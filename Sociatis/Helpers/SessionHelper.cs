using Common.Transactions;
using Entities;
using Entities.Repository;
using Entities.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.BigParams.auth;

namespace Sociatis.Helpers
{
    public class SessionHelper
    {
        private const string CitizenSession = "CITIZEN_SESSION";
        private const string SwitchStackString = "SWITCH_STACK";

        public static Citizen LoggedCitizen
        {
            get
            {
                if (Session == null)
                    return null;
                var citizenRepository = DependencyResolver.Current.GetService<ICitizenRepository>();
                return citizenRepository.GetById(Session.CitizenID);
            }
        }


        public static Stack<EntityDom> SwitchStack
        {
            get
            {
                var stack = HttpContext.Current.Session[SwitchStackString] as Stack<EntityDom>;
                if (stack == null)
                    return SwitchStack = new Stack<EntityDom>();
                return stack;
            }
            set
            {
                HttpContext.Current.Session[SwitchStackString] = value;
            }
        }

        public static Entity CurrentEntity
        {
            get
            {
                if (Session == null)
                    return null;
                if (SwitchStack.Count == 0)
                {
                    SwitchStack.Push(new EntityDom(LoggedCitizen.Entity));
                }

                var entityRepository = DependencyResolver.Current.GetService<IEntityRepository>();
                var entity = entityRepository.GetById(SwitchStack.Peek().ID);
                return entity;
            }
        }

        public static string ClientIP
        {
            get
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
        }

        public static Session Session
        {
            get
            {
                Session session = null;
                var sessionRepository = DependencyResolver.Current.GetService<ISessionRepository>();
                if (HttpContext.Current?.Session != null && HttpContext.Current?.Session[CitizenSession] != null)
                {
                    session = HttpContext.Current.Session[CitizenSession] as Session;
                    //session = sessionRepository.FirstOrDefault(s => s.ID == session.ID);
                }
                else if (HttpContext.Current.Request.Cookies != null && HttpContext.Current?.Request?.Cookies[CitizenSession] != null)
                {
                    var cookie = HttpContext.Current.Request.Cookies[CitizenSession].Value;

                    
                    session = sessionRepository
                        .FirstOrDefault(s => s.Cookie == cookie);
                    
                }
                if (session != null)
                {
                    if (session.IP != ClientIP && ClientIP != "::1" && ClientIP != "127.0.0.1")
                        session = null;
                    else if (session.ExpirationDate.CompareTo(DateTime.Now) < 0)
                    {
                        session = null;
                    }
                }

                return session;
            }
            set
            {
                HttpContext.Current.Session[CitizenSession] = value;
                HttpContext.Current.Response.Cookies.Add(new HttpCookie(CitizenSession, value.Cookie));
            }
        }

        public static void WipeSession()
        {
            HttpContext.Current.Session[CitizenSession] = null;
            HttpContext.Current.Response.Cookies.Remove(CitizenSession);
        }
    }
}