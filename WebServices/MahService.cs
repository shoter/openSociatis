using Common.Transactions;
using Entities.enums;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public class MahService : IMahService
    {
        private readonly ICompanyService companyService;
        private readonly ICompanyRepository companyRepository;
        private readonly IRegionRepository regionRepository;
        private readonly IHotelService hotelService;
        private readonly IEntityRepository entityRepository;
        private readonly IEquipmentService equipmentService;
        private readonly IMarketService marketService;
        private readonly IRemovalService removalService;
        private readonly ITransactionScopeProvider transactionScopeProvider;
        private readonly ICountryRepository countryRepository;
        public MahService(ICompanyService companyService, ICompanyRepository companyRepository, IRegionRepository regionRepository,
            IHotelService hotelService, IEntityRepository entityRepository, IEquipmentService equipmentService,
            IRemovalService removalService, IMarketService marketService, ITransactionScopeProvider transactionScopeProvider,
            ICountryRepository countryRepository)
        {
            this.companyService = companyService;
            this.companyRepository = companyRepository;
            this.regionRepository = regionRepository;
            this.entityRepository = entityRepository;
            this.hotelService = hotelService;
            this.equipmentService = equipmentService;
            this.removalService = removalService;
            this.marketService = marketService;
            this.transactionScopeProvider = transactionScopeProvider;
            this.countryRepository = countryRepository;
        }

        public void CreateHotels()
        {
            var existing = entityRepository.Where(e => e.Name.StartsWith("Admin hotel -"))
                .ToList();
            foreach (var e in existing)
                removalService.RemoveEntity(e);

            var regions = regionRepository
                .Where(r => r.Hotels
                .Any(c => c.Owner.Name == "admin") == false)
                .ToList();

            foreach (var region in regions)
            {
                var name = $"Admin hotel - {region.Name}";
                var hotel = hotelService.BuildHotel(name, region, entityRepository.First(e => e.Name == "admin"));
                hotel.Entity.Equipment.ItemCapacity = 35_000;

                equipmentService.GiveItem(Entities.enums.ProductTypeEnum.ConstructionMaterials, 35_000, 1, hotel.Entity.Equipment);
                hotel.HotelPrice.PriceQ1 = 0.1m;
            }
        }

        public void CreateHouses()
        {
            var existing = entityRepository.Where(e => e.Name.StartsWith("Admin house -"))
                .ToList();
            foreach (var e in existing)
                removalService.RemoveEntity(e);

            var regions = regionRepository
                .GetAll();

            foreach (var region in regions)
            {
                var name = $"Admin house - {region.Name}";
                var company = companyService.CreateCompany(name, ProductTypeEnum.House, region.ID, entityRepository.First(e => e.Name == "admin").EntityID);
                company.Entity.Equipment.ItemCapacity = 100_000;

                equipmentService.GiveItem(Entities.enums.ProductTypeEnum.House, 100_000, 1, company.Entity.Equipment);
                marketService.AddOffer(new BigParams.MarketOffers.AddMarketOfferParameters()
                {
                    Amount = 100_000,
                    CompanyID = company.ID,
                    CountryID = region.Country.ID,
                    Price = 25,
                    ProductType = ProductTypeEnum.House,
                    Quality = 1
                });
            }
        }

        public void CreateCM()
        {
            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                var existing = entityRepository.Where(e => e.Name.StartsWith("Admin CM -"))
                    .ToList();
                foreach (var e in existing)
                    removalService.RemoveCompany(e.Company);

                var countries = countryRepository
                    .GetAll();

                var regions = regionRepository
                    .Where(r => r.Companies
                    .Any(c => c.ProductID == (int)ProductTypeEnum.ConstructionMaterials && c.Owner.Name == "admin") == false)
                    .ToList();

                foreach (var country in countries)
                {
                    var region = country
                        .Regions
                        .OrderByDescending(r => r.Citizens.Count)
                        .FirstOrDefault();

                    if (region == null)
                        continue;

                    var name = $"Admin CM - {region.Name}";
                    var company = companyService.CreateCompany(name, ProductTypeEnum.ConstructionMaterials, region.ID, entityRepository.First(e => e.Name == "admin").EntityID);
                    company.Entity.Equipment.ItemCapacity = 100_000;

                    equipmentService.GiveItem(Entities.enums.ProductTypeEnum.ConstructionMaterials, 100_000, 1, company.Entity.Equipment);
                    marketService.AddOffer(new BigParams.MarketOffers.AddMarketOfferParameters()
                    {
                        Amount = 100_000,
                        CompanyID = company.ID,
                        CountryID = country.ID,
                        Price = 10,
                        ProductType = ProductTypeEnum.ConstructionMaterials,
                        Quality = 1
                    });

                }
                trs.Complete();
            }
        }

    }
}
