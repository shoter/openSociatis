using Entities;
using Entities.enums;
using Entities.Repository;
using Moq;
using Sociatis_Test_Suite.Persistents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weber.Html;
using WebServices;
using WebServices.Helpers;
using WebServices.Html;

namespace Sociatis_Test_Suite
{
    public class SingletonInit
    {
        public static void Init()
        {
           var configurationRepository = new Mock<IConfigurationRepository>();

            configurationRepository.Setup(x => x.GetCurrentDay()).Returns(5);

            var currencyRepository = new Mock<ICurrencyRepository>();

            currencyRepository
                .SetupGet(x => x.Gold)
                .Returns( new Currency()
                {
                    ID = (int)CurrencyTypeEnum.Gold,
                    Name = "Gold",
                    ShortName = "Gold",
                    Symbol = "Gold"
                });

            GameHelper.Init(configurationRepository.Object, currencyRepository.Object, Mock.Of<ICitizenService>());
            LinkCreator.Current = new StringLinkCreator();
            Persistent.Init(new TestRegionPersistentRepository(), new TestCurrencyPersistentRepository(), new TestCountryPersistentRepository());

        }
    }
}
