using Common.Transactions;
using Entities;
using Entities.Repository;
using Hangfire;
using Ninject;
using Ninject.Web.Common;
using Sociatis.Code.Providers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.App_Start
{
    public static class NinjectHangFire
    {
        private static IKernel kernel;
        public static IKernel Kernel
        {
            get
            {
                if (kernel == null)
                    kernel = CreateKernel();
                return kernel;
            }
        }   
        public static IKernel CreateKernel()
        {
            kernel = new StandardKernel();
            try
            {
               // kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                Hangfire.GlobalConfiguration.Configuration.UseNinjectActivator(kernel);

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<DbContext>().ToSelf().InBackgroundJobScope();
            kernel.Bind<SociatisEntities>().ToSelf().InBackgroundJobScope();
            kernel.Bind<ISessionRepository>().To<SessionRepository>().InBackgroundJobScope();
            kernel.Bind<ICitizenRepository>().To<CitizenRepository>().InBackgroundJobScope();
            kernel.Bind<IRegionRepository>().To<RegionRepository>().InBackgroundJobScope();
            kernel.Bind<ICountryRepository>().To<CountryRepository>().InBackgroundJobScope();
            kernel.Bind<IEntityRepository>().To<EntityRepository>().InBackgroundJobScope();
            kernel.Bind<ICitizenService>().To<CitizenService>().InBackgroundJobScope();
            kernel.Bind<IAuthService>().To<AuthService>().InBackgroundJobScope();
            kernel.Bind<ITransactionsService>().To<TransactionsService>().InBackgroundJobScope();
            kernel.Bind<IConfigurationRepository>().To<ConfigurationRepository>().InBackgroundJobScope();
            kernel.Bind<IWalletRepository>().To<WalletRepository>().InBackgroundJobScope();
            kernel.Bind<ITransactionLogRepository>().To<TransactionLogRepository>().InBackgroundJobScope();
            kernel.Bind<IWalletService>().To<WalletService>().InBackgroundJobScope();
            kernel.Bind<ICountryService>().To<CountryService>().InBackgroundJobScope();
            kernel.Bind<IEntityService>().To<EntityService>().InBackgroundJobScope();
            kernel.Bind<ICurrencyRepository>().To<CurrencyRepository>().InBackgroundJobScope();
            kernel.Bind<ICompanyService>().To<CompanyService>().InBackgroundJobScope();
            kernel.Bind<ICompanyRepository>().To<CompanyRepository>().InBackgroundJobScope();
            kernel.Bind<IOrganisationRepository>().To<OrganisationRepository>().InBackgroundJobScope();
            kernel.Bind<IOrganisationService>().To<OrganisationService>().InBackgroundJobScope();
            kernel.Bind<ICompanyManagerRepository>().To<CompanyManagerRepository>().InBackgroundJobScope();
            kernel.Bind<IFileService>().To<FileService>().InBackgroundJobScope();
            kernel.Bind<IJobOfferRepository>().To<JobOfferRepository>().InBackgroundJobScope();
            kernel.Bind<IJobOfferService>().To<JobOfferService>().InBackgroundJobScope();
            kernel.Bind<ICompanyEmployeeRepository>().To<CompanyEmployeeRepository>().InBackgroundJobScope();
            kernel.Bind<IWorldService>().To<WorldService>().InBackgroundJobScope();
            kernel.Bind<IEquipmentRepository>().To<EquipmentRepository>().InBackgroundJobScope();
            kernel.Bind<IEquipmentService>().To<EquipmentService>().InBackgroundJobScope();
            kernel.Bind<IProductService>().To<ProductService>().InBackgroundJobScope();
            kernel.Bind<IProductRepository>().To<ProductRepository>().InBackgroundJobScope();
            kernel.Bind<IContractRepository>().To<ContractRepository>().InBackgroundJobScope();
            kernel.Bind<IContractService>().To<ContractService>().InBackgroundJobScope();
            kernel.Bind<IMessageService>().To<MessageService>().InBackgroundJobScope();
            kernel.Bind<IMessageRepository>().To<MessageRepository>().InBackgroundJobScope();
            kernel.Bind<ICongressVotingRepository>().To<CongressVotingRepository>().InBackgroundJobScope();
            kernel.Bind<IPartyMemberRepository>().To<PartyMemberRepository>().InBackgroundJobScope();
            kernel.Bind<ICongressVotingService>().To<CongressVotingService>().InBackgroundJobScope();
            kernel.Bind<IPartyRepository>().To<PartyRepository>().InBackgroundJobScope();
            kernel.Bind<IPartyService>().To<PartyService>().InBackgroundJobScope();
            kernel.Bind<IUploadService>().To<UploadService>().InBackgroundJobScope();
            kernel.Bind<ICongressCandidateVotingRepository>().To<CongressCandidateVotingRepository>().InBackgroundJobScope();
            kernel.Bind<ICongressCandidateService>().To<CongressCandidateService>().InBackgroundJobScope();
            kernel.Bind<ICongressmenRepository>().To<CongressmenRepository>().InBackgroundJobScope();
            kernel.Bind<IPresidentVotingRepository>().To<PresidentVotingRepository>().InBackgroundJobScope();
            kernel.Bind<IRegionService>().To<RegionService>().InBackgroundJobScope();
            kernel.Bind<ITravelService>().To<TravelService>().InBackgroundJobScope();
            kernel.Bind<IMarketOfferRepository>().To<MarketOfferRepository>().InBackgroundJobScope();
            kernel.Bind<IMarketService>().To<MarketService>().InBackgroundJobScope();
            kernel.Bind<IProductTaxRepository>().To<ProductTaxRepository>().InBackgroundJobScope();
            kernel.Bind<IWarningRepository>().To<WarningRepository>().InBackgroundJobScope();
            kernel.Bind<IWarningService>().To<WarningService>().InBackgroundJobScope();
            kernel.Bind<IEmployeeService>().To<EmployeeService>().InBackgroundJobScope();
            kernel.Bind<IWarRepository>().To<WarRepository>().InBackgroundJobScope();
            kernel.Bind<IMilitaryProtectionPactRepository>().To<MilitaryProtectionPactRepository>().InBackgroundJobScope();
            kernel.Bind<ITruceRepository>().To<TruceRepository>().InBackgroundJobScope();
            kernel.Bind<IBattleRepository>().To<BattleRepository>().InBackgroundJobScope();
            kernel.Bind<IWarService>().To<WarService>().InBackgroundJobScope();
            kernel.Bind<IBattleService>().To<BattleService>().InBackgroundJobScope();
            kernel.Bind<IArticleRepository>().To<ArticleRepository>().InBackgroundJobScope();
            kernel.Bind<INewspaperRepository>().To<NewspaperRepository>().InBackgroundJobScope();
            kernel.Bind<INewspaperService>().To<NewspaperService>().InBackgroundJobScope();
            kernel.Bind<IEmbargoRepository>().To<EmbargoRepository>().InBackgroundJobScope();
            kernel.Bind<IBugReportRepository>().To<BugReportRepository>().InBackgroundJobScope();
            kernel.Bind<IEmbargoService>().To<EmbargoService>().InBackgroundJobScope();
            kernel.Bind<IDebugDayChangeRepository>().To<DebugDayChangeRepository>().InBackgroundJobScope();
            kernel.Bind<IMonetaryOfferRepository>().To<MonetaryOfferRepository>().InBackgroundJobScope();
            kernel.Bind<IMonetaryTransactionRepository>().To<MonetaryTransactionRepository>().InBackgroundJobScope();
            kernel.Bind<IMonetaryMarketService>().To<MonetaryMarketService>().InBackgroundJobScope();
            kernel.Bind<ICountryPresidentService>().To<CountryPresidentService>().InBackgroundJobScope();
            kernel.Bind<IPopupService>().To<PopupService>().InBackgroundJobScope();
            kernel.Bind<IFriendRepository>().To<FriendRepository>().InBackgroundJobScope();
            kernel.Bind<IFriendService>().To<FriendService>().InBackgroundJobScope();
            kernel.Bind<IEquipmentItemRepository>().To<EquipmentItemRepository>().InBackgroundJobScope();
            kernel.Bind<IPolygonRepository>().To<PolygonRepository>().InBackgroundJobScope();
            kernel.Bind<IResourceRepository>().To<ResourceRepository>().InBackgroundJobScope();
            kernel.Bind<IPartyJoinRequestRepository>().To<PartyJoinRequestRepository>().InBackgroundJobScope();
            kernel.Bind<IPartyInviteRepository>().To<PartyInviteRepository>().InBackgroundJobScope();
            kernel.Bind<ICountryTreasureService>().To<CountryTreasureService>().InBackgroundJobScope();
            kernel.Bind<IReservedEntityNameRepository>().To<ReservedEntityNameRepository>().InBackgroundJobScope();
            kernel.Bind<ICongressVotingReservedMoneyRepository>().To<CongressVotingReservedMoneyRepository>().InBackgroundJobScope();
            kernel.Bind<IVotingGreetingMessageRepository>().To<VotingGreetingMessageRepository>().InBackgroundJobScope();
            kernel.Bind<IShoutboxMessageRepository>().To<ShoutboxMessageRepository>().InBackgroundJobScope();
            kernel.Bind<IShoutboxChannelRepository>().To<ShoutboxChannelRepository>().InBackgroundJobScope();
            kernel.Bind<IShoutBoxService>().To<ShoutBoxService>().InBackgroundJobScope();
            kernel.Bind<IGiftTransactionRepository>().To<GiftTransactionRepository>().InBackgroundJobScope();
            kernel.Bind<IGiftService>().To<GiftService>().InBackgroundJobScope();
            kernel.Bind<ITradeService>().To<TradeService>().InBackgroundJobScope();
            kernel.Bind<ITradeRepository>().To<TradeRepository>().InBackgroundJobScope();
            kernel.Bind<ITradeMoneyRepository>().To<TradeMoneyRepository>().InBackgroundJobScope();
            kernel.Bind<ITradeProductRepository>().To<TradeProductRepository>().InBackgroundJobScope();
            kernel.Bind<IHospitalRepository>().To<HospitalRepository>().InBackgroundJobScope();
            kernel.Bind<IHospitalService>().To<HospitalService>().InBackgroundJobScope();
            kernel.Bind<IStartService>().To<StartService>().InBackgroundJobScope();
            kernel.Bind<ISummaryService>().To<SummaryService>().InBackgroundJobScope();
            kernel.Bind<IMilitaryProtectionPactOfferRepository>().To<MilitaryProtectionPactOfferRepository>().InBackgroundJobScope();
            kernel.Bind<IMPPService>().To<MPPService>().InBackgroundJobScope();
            kernel.Bind<IDevIssueRepository>().To<DevIssueRepository>().InBackgroundJobScope();
            kernel.Bind<IDevIssueCommentRepository>().To<DevIssueCommentRepository>().InBackgroundJobScope();
            kernel.Bind<IDevIssueService>().To<DevIssueService>().InBackgroundJobScope();
            kernel.Bind<IUploadAvatarService>().To<UploadAvatarService>().InBackgroundJobScope();
            kernel.Bind<IUploadRepository>().To<UploadRepository>().InBackgroundJobScope();
            kernel.Bind<IEmailService>().ToProvider<EmailServiceProvider>().InBackgroundJobScope();
            kernel.Bind<IRemovalService>().To<RemovalService>().InBackgroundJobScope();
            kernel.Bind<INewDayRepository>().To<NewDayRepository>().InBackgroundJobScope();
            kernel.Bind<IDefenseSystemService>().To<DefenseSystemService>().InBackgroundJobScope();
            kernel.Bind<ITransactionScopeProvider>().To<StandardTransactionScopeProvider>().InBackgroundJobScope();
            kernel.Bind<IConstructionRepository>().To<ConstructionRepository>().InBackgroundJobScope();
            kernel.Bind<IConstructionService>().To<ConstructionService>().InBackgroundJobScope();
            kernel.Bind<IHotelRepository>().To<HotelRepository>().InBackgroundJobScope();
            kernel.Bind<IHotelService>().To<HotelService>().InBackgroundJobScope();
            kernel.Bind<IHotelRoomRepository>().To<HotelRoomRepository>().InBackgroundJobScope();
            kernel.Bind<IHotelTransactionsService>().To<HotelTransactionsService>().InBackgroundJobScope();
            kernel.Bind<IHotelManagerRepository>().To<HotelManagerRepository>().InBackgroundJobScope();
            kernel.Bind<IBusinessRepository>().To<BusinessRepository>().InBackgroundJobScope();
            kernel.Bind<IMahService>().To<MahService>().InBackgroundJobScope();
            kernel.Bind<IHouseRepository>().To<HouseRepository>().InBackgroundJobScope();
            kernel.Bind<IHouseService>().To<HouseService>().InBackgroundJobScope();
            kernel.Bind<IHouseFurnitureRepository>().To<HouseFurnitureRepository>().InBackgroundJobScope();
            kernel.Bind<IHouseChestService>().To<HouseChestService>().InBackgroundJobScope();
            kernel.Bind<IHouseChestItemRepository>().To<HouseChestItemRepository>().InBackgroundJobScope();
            kernel.Bind<HouseDayChangeProcessor>().ToSelf().InBackgroundJobScope();
            kernel.Bind<ISellHouseRepository>().To<SellHouseRepository>().InBackgroundJobScope();
            kernel.Bind<ISellHouseService>().To<SellHouseService>().InBackgroundJobScope();
            kernel.Bind<IHouseTransactions>().To<HouseTransactions>().InBackgroundJobScope();
            kernel.Bind<ICompanyFinanceSummaryRepository>().To<CompanyFinanceSummaryRepository>().InBackgroundJobScope();
            kernel.Bind<ICompanyFinanceSummaryService>().To<CompanyFinanceSummaryService>().InBackgroundJobScope();
            kernel.Bind<ICompanyFinanceService>().To<CompanyFinanceService>().InBackgroundJobScope();
            kernel.Bind<ICountryEventService>().To<CountryEventService>().InBackgroundJobScope();
            kernel.Bind<IBattleEventService>().To<BattleEventService>().InBackgroundJobScope();
            kernel.Bind<IWarEventService>().To<WarEventService>().InBackgroundJobScope();
            kernel.Bind<IEventRepository>().To<EventRepository>().InBackgroundJobScope();
            kernel.Bind<ICountryEventRepository>().To<CountryEventRepository>().InBackgroundJobScope();
        }
    }
}