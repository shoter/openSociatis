using Entities.Repository;
using Sociatis.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebServices;
using WebServices.Helpers;
using WebUtils.Exceptions;
using WebUtils.ViewEngines;

namespace Sociatis
{
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            try
            {
                AreaRegistration.RegisterAllAreas();
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                configureViewEngines();
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);
#if !DEBUG
                 Persistent.Init();
                    Game_Start();
#endif
                Starter.Start();
                NinjectHangFire.CreateKernel();

                
            }
            catch (Exception e)
            {
                ExceptionMVCLogger.LogException(e, "Start");
                throw;
            }
        }

        protected void Application_AuthenticateRequest()
        {
            try
            {

                if (GameHelper.Initialized == false)
                {
                    Starter.Start();
                    Persistent.Init();
                    Game_Start();
                }
            }
            catch (Exception e)
            {
                ExceptionMVCLogger.LogException(e, "Start");
                throw;
            }
        }

        protected void Page(object sender, EventArgs e)
        {
            Exception exc = Server.GetLastError();

            ExceptionMVCLogger.LogException(exc, "App_Error");
            Server.ClearError();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exc = Server.GetLastError();

            ExceptionMVCLogger.LogException(exc, "App_Error");
        }

        private void configureViewEngines()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new CSharpRazorViewEngine());
        }

        public static void Game_Start()
        {
            IConfigurationRepository configurationRepository = DependencyResolver.Current.GetService<IConfigurationRepository>();
            ICurrencyRepository currencyRepository = DependencyResolver.Current.GetService<ICurrencyRepository>();
            ICitizenService citizenService = DependencyResolver.Current.GetService<ICitizenService>();
            IShoutBoxService shoutBoxService = DependencyResolver.Current.GetService<IShoutBoxService>();

            var startService = DependencyResolver.Current.GetService<IStartService>();
            startService.OnStart();

            GameHelper.Init(configurationRepository, currencyRepository, citizenService);
            ConfigurationHelper.Init(configurationRepository.GetConfiguration());
            shoutBoxService.MergeSameNameChannels();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            
            GameHelper.LastDayChangeRealTime = DateTime.Now;
#if FORCE_HTTPS
            if (!HttpContext.Current.Request.IsSecureConnection)
            {
                Response.Redirect("https://" + Request.ServerVariables["HTTP_HOST"]
                                             + HttpContext.Current.Request.RawUrl);
            }
#endif
        }

    }
}
