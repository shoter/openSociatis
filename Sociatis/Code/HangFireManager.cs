using Elmah;
using Entities.Repository;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Ninject;
using Owin;
using Sociatis.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.Helpers;

namespace Sociatis.Code
{
    public class HangFireManager
    {

        public static void Init(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("SociatisHangfire");


            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new AuthorizationFilter() }
            });
            app.UseHangfireServer();

            DateTime localHour = new DateTime(2000, 1, 1, 0, 0, 0);
            localHour = localHour.ToUniversalTime();


            RecurringJob.AddOrUpdate("DayChange", () => TriggerDayChange(), Cron.Daily(localHour.Hour));
            RecurringJob.AddOrUpdate("Hourly", () => TriggerHourChange(), Cron.Hourly(localHour.Hour));
            RecurringJob.AddOrUpdate("DestroyTemp", () => TriggerHourChange(), Cron.Hourly(localHour.Hour));

            
        }


        public static void TriggerDayChange()
        {
            try
            {
                if (GameHelper.Initialized == false)
                    MvcApplication.Game_Start();
                var worldService = NinjectHangFire.Kernel.GetService<IWorldService>();

                worldService.ProcessDayChange();
            }
            catch (Exception e)
            {
                try
                {
                    var emailService = NinjectHangFire.Kernel.GetService<IEmailService>();
                    emailService.InformAboutException(e);
                    Elmah.ErrorLog.GetDefault(null).Log(new Error(e));
                }
                catch (Exception)
                { }
                throw;
            }
        }

        public static void TriggerDestroyTemp()
        {
            var uploadService = NinjectHangFire.Kernel.GetService<IUploadService>();
            uploadService.RemoveTemporaryFiles();
        }

        public static void TriggerHourChange()
        {
            if (GameHelper.Initialized == false)
                MvcApplication.Game_Start();

            var tradeService = NinjectHangFire.Kernel.GetService<ITradeService>();
            var citizenRepository = NinjectHangFire.Kernel.GetService<ICitizenRepository>();
            tradeService.CancelInactiveTrade();

            GameHelper.ActivePlayers = citizenRepository.GetActivePlayerCount(GameHelper.CurrentDay);

        }
    }
}