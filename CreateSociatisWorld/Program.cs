using Common;
using Common.EntityFramework;
using Entities;
using Entities.enums;
using Entities.enums.Attributes;
using Entities.Repository;
using Ninject;
using Sociatis.Code;
using Sociatis_Test_Suite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using WebServices;
using WebServices.enums;
using WebServices.Helpers;

namespace CreateSociatisWorld
{
    class Program
    {
        private static ICurrencyRepository currencyRepository;
        private static IConfigurationRepository configurationRepository;
        static void Main(string[] args)
        {
            /*using (var scope = new TransactionScope(TransactionScopeOption.Required, SociatisTransactionOptions.RepeatableRead))
            {
                createTickets();

                Console.ReadKey();
                scope.Complete();
            }*/

            Hospitals.Create();

            //enumator();
            /*   var entities = new SociatisEntities();
               currencyRepository = new CurrencyRepository(entities);
               configurationRepository = new ConfigurationRepository(entities);

               if (currencyRepository.Any(c => c.ID == (int)CurrencyTypeEnum.Gold) == false)
               {
                   currencyRepository.Add(new Currency()
                   {
                       ID = (int)CurrencyTypeEnum.Gold,
                       Name = "Gold",
                       ShortName = "Gold",
                       Symbol = "Gold"
                   });
                   currencyRepository.SaveChanges();
                   Console.WriteLine("Gold created!");
               }

               if (configurationRepository.Any(x => true) == false)
               {
                   var con = new ConfigurationTable();
                   con.LastDayChange = DateTime.Today;
                   configurationRepository.Add(con);
                   configurationRepository.SaveChanges();
                   Console.WriteLine("Config created!");
               }

               // SingletonInit.Init();
                   GameHelper.CurrentDay = 1;
               Countries.Create();*/
              Console.ReadKey();
        }

        private static void enumator()
        {
            using (var context = new SociatisEntities())
            {
                var genericRepository = new GenericRepository<SociatisEntities>(context);

                new Enumator<VotingType, VotingTypeEnum>(genericRepository)
                    .CreateNewIfAble(
                    setName: (voting, name) => voting.Name = name,
                    setValue: (voting, value) =>
                    {
                        voting.ID = value;
                        voting.AlwaysVotable = isVotingEnumAlawaysVotable((VotingTypeEnum)value);

                    },
                    getName: votingType => votingType.ToString(),
                    getValue: votingType => (int)votingType,

                    isTheSame: (voting, votingType) => voting.ID == (int)votingType);

                new Enumator<CongressVotingRejectionReason, CongressVotingRejectionReasonEnum>(genericRepository)
                    .CreateNewIfAble(
                    setName: (reason, name) => reason.Name = name,
                    setValue: (reason, value) => reason.ID = value,

                    getName: reasonEnum => reasonEnum.ToString(),
                    getValue: reasonEnum => (int)reasonEnum,

                    isTheSame: (reason, reasonEnum) => reason.ID == (int)reasonEnum);

                new Enumator<TransactionType, TransactionTypeEnum>(genericRepository)
                    .CreateNewIfAble(
                    setName: (transfer, name) => transfer.Name = name,
                    setValue: (transfer, value) => transfer.ID = value,

                    getName: transferEnum => transferEnum.ToString(),
                    getValue: transferEnum => (int)transferEnum,

                    isTheSame: (transfer, transferEnum) => transfer.ID == (int)transferEnum);

                new Enumator<TradeStatus, TradeStatusEnum>(genericRepository)
                    .CreateNewIfAble(
                    setName: (status, name) => status.Name = name,
                    setValue: (status, value) => status.ID = value,

                    getName: statusEnum => statusEnum.ToString(),
                    getValue: statusEnum => (int)statusEnum,

                    isTheSame: (status, statusEnum) => status.ID == (int)statusEnum);

                new Enumator<Product, ProductTypeEnum>(genericRepository)
                    .CreateNewIfAble(
                    setName: (product, name) => product.Name = name,
                    setValue: (product, value) => product.ID = value,

                    getName: productType => productType.ToString(),
                    getValue: productType => (int)productType,

                    isTheSame: (product, productType) => product.ID == (int)productType);

                new Enumator<CompanyType, CompanyTypeEnum>(genericRepository)
                   .CreateNewIfAble(
                   setName: (companyType, name) => companyType.Name = name,
                   setValue: (companyType, value) => companyType.ID = value,

                   getName: companyTypeEnum => companyTypeEnum.ToString(),
                   getValue: companyTypeEnum => (int)companyTypeEnum,

                   isTheSame: (companyType, companyTypeEnum) => companyType.ID == (int)companyTypeEnum);

                new Enumator<DevIssueLabelType, DevIssueLabelTypeEnum>(genericRepository)
                   .CreateNewIfAble(
                   setName: (labelType, name) => labelType.Name = name,
                   setValue: (labelType, value) => labelType.ID = value,

                   getName: labelTypeEnum => labelTypeEnum.ToString(),
                   getValue: labelTypeEnum => (int)labelTypeEnum,

                   isTheSame: (labelType, labelTypeEnum) => labelType.ID == (int)labelTypeEnum);

                new Enumator<VisibilityOption, VisibilityOptionEnum>(genericRepository)
                   .CreateNewIfAble(
                   setName: (visibilityOption, name) => visibilityOption.Name = name,
                   setValue: (visibilityOption, value) => visibilityOption.ID = value,

                   getName: visibilityOptionEnum => visibilityOptionEnum.ToString(),
                   getValue: visibilityOptionEnum => (int)visibilityOptionEnum,

                   isTheSame: (visibilityOption, visibilityOptionEnum) => visibilityOption.ID == (int)visibilityOptionEnum);

                new Enumator<UploadLocation, UploadLocationEnum>(genericRepository).CreateNewIfAble();
                new Enumator<LawAllowHolder, LawAllowHolderEnum>(genericRepository).CreateNewIfAble();

                new Enumator<VotingStatus, VotingStatusEnum>(genericRepository).CreateNewIfAble();
                new Enumator<EntityType, EntityTypeEnum>(genericRepository).CreateNewIfAble();
                new Enumator<VotingType, VotingTypeEnum>(genericRepository).CreateNewIfAble();
                new Enumator<PresidentCandidateStatus, PresidentCandidateStatusEnum>(genericRepository).CreateNewIfAble();

                new Enumator<CongressCandidateStatus, CongressCandidateStatusEnum>(genericRepository).CreateNewIfAble(
                   setName: (playerType, name) => playerType.name = name,
                   setValue: (playerType, value) => playerType.ID = value,

                   getName: playerTypeEnum => playerTypeEnum.ToString(),
                   getValue: playerTypeEnum => (int)playerTypeEnum,

                   isTheSame: (playerType, playerTypeEnum) => playerType.ID == (int)playerTypeEnum);


                new Enumator<PlayerType, PlayerTypeEnum>(genericRepository).CreateNewIfAble(
                   setName: (playerType, name) => playerType.name = name,
                   setValue: (playerType, value) => playerType.ID = value,

                   getName: playerTypeEnum => playerTypeEnum.ToString(),
                   getValue: playerTypeEnum => (int)playerTypeEnum,

                   isTheSame: (playerType, playerTypeEnum) => playerType.ID == (int)playerTypeEnum);

                new Enumator<VoteType, VoteTypeEnum>(genericRepository).CreateNewIfAble();
                new Enumator<MonetaryOfferType, MonetaryOfferTypeEnum>(genericRepository).CreateNewIfAble();
                new Enumator<ProductTaxType, ProductTaxTypeEnum>(genericRepository).CreateNewIfAble();
            }
        }

        private static bool isVotingEnumAlawaysVotable(VotingTypeEnum value)
        {
            var type = typeof(VotingTypeEnum);
            var memInfo = type.GetMember(value.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(AlwaysVotableAttribute), false);
            return ((AlwaysVotableAttribute)attributes[0]).Value;
        }

        private static void createTickets()
        {
            var companyService = Ninject.Current.Get<ICompanyService>();
            var marketService = Ninject.Current.Get<IMarketService>();
            var equipmentService = Ninject.Current.Get<IEquipmentService>();
            var entityRepository = Ninject.Current.Get<IEntityRepository>();
            var companyRepository = Ninject.Current.Get<ICompanyRepository>();
            IConfigurationRepository configurationRepository = Ninject.Current.Get<IConfigurationRepository>();
            ICurrencyRepository currencyRepository = Ninject.Current.Get<ICurrencyRepository>();
            ICitizenService citizenService = Ninject.Current.Get<ICitizenService>();

            GameHelper.Init(configurationRepository, currencyRepository, citizenService);
            ConfigurationHelper.Init(configurationRepository.GetConfiguration());
            Persistent.Init();

            foreach (var country in Persistent.Countries.GetAll())
            {
                var ticketCompany = companyRepository.Where
                    (c => c.OwnerID == country.ID && c.ProductID == (int)ProductTypeEnum.SellingPower && c.MarketOffers.Any(off => off.ProductID == (int)ProductTypeEnum.MovingTicket) == false)
                    .First();

                equipmentService.GiveItem(ProductTypeEnum.MovingTicket, 25, 1, ticketCompany.Entity.Equipment);
                marketService.AddOffer(new WebServices.BigParams.MarketOffers.AddMarketOfferParameters()
                {
                    Amount = 25,
                    CompanyID = ticketCompany.ID,
                    CountryID = country.ID,
                    Price = 1,
                    ProductType = ProductTypeEnum.MovingTicket,
                    Quality = 1
                });
            }
        }


        private static void createNationalShoppingCenter(int countryID, int regionID)
        {



            var companyService = Ninject.Current.Get<ICompanyService>();
            var marketService = Ninject.Current.Get<IMarketService>();
            var equipmentService = Ninject.Current.Get<IEquipmentService>();
            var entityRepository = Ninject.Current.Get<IEntityRepository>();
            IConfigurationRepository configurationRepository = Ninject.Current.Get<IConfigurationRepository>();
            ICurrencyRepository currencyRepository = Ninject.Current.Get<ICurrencyRepository>();
            ICitizenService citizenService = Ninject.Current.Get<ICitizenService>();

            GameHelper.Init(configurationRepository, currencyRepository, citizenService);
            ConfigurationHelper.Init(configurationRepository.GetConfiguration());
            Persistent.Init();



            var country = entityRepository.GetById(countryID);

            //var fuel = companyService.CreateCompanyForCountry($"{country.Name} - national fuel", ProductTypeEnum.Fuel, regionID, country.Country, false);
            //var fuelEntity = entityRepository.GetById(fuel.ID);

            //equipmentService.GiveItem(ProductTypeEnum.Fuel, 4950, 1, fuelEntity.Equipment);
            //marketService.AddOffer(new WebServices.BigParams.MarketOffers.AddMarketOfferParameters()
            //{
            //    Amount = 1000,
            //    CompanyID = fuel.ID,
            //    CountryID = countryID,
            //    Price = 0.5,
            //    ProductType = ProductTypeEnum.Fuel,
            //    Quality = 1
            //});

            //Console.WriteLine($"{fuelEntity.Name} created!");

            var bread = companyService.CreateCompanyForCountry($"{country.Name} - national shop", ProductTypeEnum.SellingPower, regionID, country.Country, false);
            var breadEntity = entityRepository.GetById(bread.ID);
            equipmentService.GiveItem(ProductTypeEnum.Bread, 250, 1, breadEntity.Equipment);
            equipmentService.GiveItem(ProductTypeEnum.SellingPower, 4000, 1, breadEntity.Equipment);
            marketService.AddOffer(new WebServices.BigParams.MarketOffers.AddMarketOfferParameters()
            {
                Amount = 250,
                CompanyID = bread.ID,
                CountryID = countryID,
                Price = 1,
                ProductType = ProductTypeEnum.Bread,
                Quality = 1
            });

            Console.WriteLine($"{breadEntity.Name} created!");
        }
    }
}
