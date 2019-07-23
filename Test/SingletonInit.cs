using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Entities.Repository;
using WebServices.Helpers;
using WebServices;

namespace Test
{
    public class SingletonInit
    {
        public static void Init()
        {

            var configurationRepository = Ninject.Current.Get<IConfigurationRepository>();
            var currencyRepository = Ninject.Current.Get<ICurrencyRepository>();
            ICitizenService citizenService = Ninject.Current.Get<ICitizenService>();

            GameHelper.Init(configurationRepository, currencyRepository, citizenService);

            Persistent.Init();

        }
    }
}
