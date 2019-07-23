using Entities.enums;
using Entities.Repository;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;
using WebServices.Helpers;

namespace CreateSociatisWorld
{
    public static class Hospitals
    {
        public static void Create()
        {
            var companyService = Ninject.Current.Get<ICompanyService>();
            var marketService = Ninject.Current.Get<IMarketService>();
            var equipmentService = Ninject.Current.Get<IEquipmentService>();
            var entityRepository = Ninject.Current.Get<IEntityRepository>();
            var companyRepository = Ninject.Current.Get<ICompanyRepository>();
            var countryRepository = Ninject.Current.Get<ICountryRepository>();
            var hospitalService = Ninject.Current.Get<IHospitalService>();
            var hospitalRepository = Ninject.Current.Get<IHospitalRepository>();
            IConfigurationRepository configurationRepository = Ninject.Current.Get<IConfigurationRepository>();
            ICurrencyRepository currencyRepository = Ninject.Current.Get<ICurrencyRepository>();
            ICitizenService citizenService = Ninject.Current.Get<ICitizenService>();

            GameHelper.Init(configurationRepository, currencyRepository, citizenService);
            ConfigurationHelper.Init(configurationRepository.GetConfiguration());
            Persistent.Init();

            foreach (var country in countryRepository.GetAll())
            {
                var region = country.
                    Regions.
                    OrderByDescending(r => r.Citizens.Count)
                    .First();

                if (companyRepository.Any(c => c.OwnerID == country.ID && c.ProductID == (int)ProductTypeEnum.MedicalSupplies))
                    continue;

                var hospital = companyService.CreateCompany($"{country.Entity.Name} - national hospital", ProductTypeEnum.MedicalSupplies, region.ID, country.ID);
                var hospitalEntity = entityRepository.GetById(hospital.ID);
                region.NationalHospitalID = hospital.ID;
                equipmentService.GiveItem(ProductTypeEnum.MedicalSupplies, 1000, 3, hospitalEntity.Equipment);


                var hos = hospitalRepository.GetById(hospital.ID);
                hos.Company.Quality = 3;


                hos.HealingEnabled = true;
                Console.WriteLine($"Created {country.Entity.Name}");
            }

            companyRepository.SaveChanges();
        }
    }
}
