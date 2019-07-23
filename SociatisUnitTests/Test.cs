using Common;
using Entities;
using Entities.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;
using WebServices.BigParams;
using WebServices.structs;

namespace SociatisUnitTests
{
    [TestClass]
    public class Test
    {
        IAuthService authService;
        IRegionRepository regionsRepository;
        ICountryRepository countryRepository;
        ICountryService countryService;

        [TestMethod]
        public void Testerek()
        {
            authService = Ninject.Current.Get<IAuthService>();
            regionsRepository = Ninject.Current.Get<IRegionRepository>();
            countryRepository = Ninject.Current.Get<ICountryRepository>();
            countryService = Ninject.Current.Get<ICountryService>();

           /* var country = countryRepository.GetById(1);

            countryService.CreateNewPresidentVoting(country, 50);
            countryRepository.SaveChanges();*/
        }

        
    }
}
