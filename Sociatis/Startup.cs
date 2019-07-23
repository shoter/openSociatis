using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;
using Sociatis.Code;

[assembly: OwinStartupAttribute(typeof(Sociatis.Startup))]
namespace Sociatis
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            DataTables.AspNet.Mvc5.Configuration.RegisterDataTables();
            Weber.Helpers.HtmlHelper.Init(Config.WebsiteURL);
            HangFireManager.Init(app);
            
            

            
        }

        
    }
}
