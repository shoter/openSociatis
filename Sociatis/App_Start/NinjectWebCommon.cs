[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Sociatis.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(Sociatis.App_Start.NinjectWebCommon), "Stop")]

namespace Sociatis.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using WebServices;
    using Entities.Repository;
    using System.Data.Entity;
    using Entities;
    using Common.EntityFramework;
    using Sociatis.Code.Providers;
    using Common.Transactions;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

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
            kernel.Bind<DbContext>().ToSelf().InRequestScope();
            kernel.Bind<SociatisEntities>().ToSelf().InRequestScope();
            kernel.Bind<ISessionRepository>().To<SessionRepository>().InRequestScope();
            kernel.Bind<ICitizenRepository>().To<CitizenRepository>().InRequestScope();
            kernel.Bind<IRegionRepository>().To<RegionRepository>().InRequestScope();
            kernel.Bind<ICountryRepository>().To<CountryRepository>().InRequestScope();
            kernel.Bind<IEntityRepository>().To<EntityRepository>().InRequestScope();
            kernel.Bind<ICitizenService>().To<CitizenService>().InRequestScope();
            kernel.Bind<IAuthService>().To<AuthService>().InRequestScope();
            kernel.Bind<ITransactionsService>().To<TransactionsService>().InRequestScope();
            kernel.Bind<IConfigurationRepository>().To<ConfigurationRepository>().InRequestScope();
            kernel.Bind<IWalletRepository>().To<WalletRepository>().InRequestScope();
            kernel.Bind<ITransactionLogRepository>().To<TransactionLogRepository>().InRequestScope();
            kernel.Bind<IWalletService>().To<WalletService>().InRequestScope();
            kernel.Bind<ICountryService>().To<CountryService>().InRequestScope();
            kernel.Bind<IEntityService>().To<EntityService>().InRequestScope();
            kernel.Bind<ICurrencyRepository>().To<CurrencyRepository>().InRequestScope();
            kernel.Bind<ICompanyService>().To<CompanyService>().InRequestScope();
            kernel.Bind<ICompanyRepository>().To<CompanyRepository>().InRequestScope();
            kernel.Bind<IOrganisationRepository>().To<OrganisationRepository>().InRequestScope();
            kernel.Bind<IOrganisationService>().To<OrganisationService>().InRequestScope();
            kernel.Bind<ICompanyManagerRepository>().To<CompanyManagerRepository>().InRequestScope();
            kernel.Bind<IFileService>().To<FileService>().InRequestScope();
            kernel.Bind<IJobOfferRepository>().To<JobOfferRepository>().InRequestScope();
            kernel.Bind<IJobOfferService>().To<JobOfferService>().InRequestScope();
            kernel.Bind<ICompanyEmployeeRepository>().To<CompanyEmployeeRepository>().InRequestScope();
            kernel.Bind<IWorldService>().To<WorldService>().InRequestScope();
            kernel.Bind<IEquipmentRepository>().To<EquipmentRepository>().InRequestScope();
            kernel.Bind<IEquipmentService>().To<EquipmentService>().InRequestScope();
            kernel.Bind<IProductService>().To<ProductService>().InRequestScope();
            kernel.Bind<IProductRepository>().To<ProductRepository>().InRequestScope();
            kernel.Bind<IContractRepository>().To<ContractRepository>().InRequestScope();
            kernel.Bind<IContractService>().To<ContractService>().InRequestScope();
            kernel.Bind<IMessageService>().To<MessageService>().InRequestScope();
            kernel.Bind<IMessageRepository>().To<MessageRepository>().InRequestScope();
            kernel.Bind<ICongressVotingRepository>().To<CongressVotingRepository>().InRequestScope();
            kernel.Bind<IPartyMemberRepository>().To<PartyMemberRepository>().InRequestScope();
            kernel.Bind<ICongressVotingService>().To<CongressVotingService>().InRequestScope();
            kernel.Bind<IPartyRepository>().To<PartyRepository>().InRequestScope();
            kernel.Bind<IPartyService>().To<PartyService>().InRequestScope();
            kernel.Bind<IUploadService>().To<UploadService>().InRequestScope();
            kernel.Bind<ICongressCandidateVotingRepository>().To<CongressCandidateVotingRepository>().InRequestScope();
            kernel.Bind<ICongressCandidateService>().To<CongressCandidateService>().InRequestScope();
            kernel.Bind<ICongressmenRepository>().To<CongressmenRepository>().InRequestScope();
            kernel.Bind<IPresidentVotingRepository>().To<PresidentVotingRepository>().InRequestScope();
            kernel.Bind<IRegionService>().To<RegionService>().InRequestScope();
            kernel.Bind<ITravelService>().To<TravelService>().InRequestScope();
            kernel.Bind<IMarketOfferRepository>().To<MarketOfferRepository>().InRequestScope();
            kernel.Bind<IMarketService>().To<MarketService>().InRequestScope();
            kernel.Bind<IProductTaxRepository>().To<ProductTaxRepository>().InRequestScope();
            kernel.Bind<IWarningRepository>().To<WarningRepository>().InRequestScope();
            kernel.Bind<IWarningService>().To<WarningService>().InRequestScope();
            kernel.Bind<IEmployeeService>().To<EmployeeService>().InRequestScope();
            kernel.Bind<IWarRepository>().To<WarRepository>().InRequestScope();
            kernel.Bind<IMilitaryProtectionPactRepository>().To<MilitaryProtectionPactRepository>().InRequestScope();
            kernel.Bind<ITruceRepository>().To<TruceRepository>().InRequestScope();
            kernel.Bind<IBattleRepository>().To<BattleRepository>().InRequestScope();
            kernel.Bind<IWarService>().To<WarService>().InRequestScope();
            kernel.Bind<IBattleService>().To<BattleService>().InRequestScope();
            kernel.Bind<IArticleRepository>().To<ArticleRepository>().InRequestScope();
            kernel.Bind<INewspaperRepository>().To<NewspaperRepository>().InRequestScope();
            kernel.Bind<INewspaperService>().To<NewspaperService>().InRequestScope();
            kernel.Bind<IEmbargoRepository>().To<EmbargoRepository>().InRequestScope();
            kernel.Bind<IBugReportRepository>().To<BugReportRepository>().InRequestScope();
            kernel.Bind<IEmbargoService>().To<EmbargoService>().InRequestScope();
            kernel.Bind<IDebugDayChangeRepository>().To<DebugDayChangeRepository>().InRequestScope();
            kernel.Bind<IMonetaryOfferRepository>().To<MonetaryOfferRepository>().InRequestScope();
            kernel.Bind<IMonetaryTransactionRepository>().To<MonetaryTransactionRepository>().InRequestScope();
            kernel.Bind<IMonetaryMarketService>().To<MonetaryMarketService>().InRequestScope();
            kernel.Bind<ICountryPresidentService>().To<CountryPresidentService>().InRequestScope();
            kernel.Bind<IPopupService>().To<PopupService>().InRequestScope();
            kernel.Bind<IFriendRepository>().To<FriendRepository>().InRequestScope();
            kernel.Bind<IFriendService>().To<FriendService>().InRequestScope();
            kernel.Bind<IEquipmentItemRepository>().To<EquipmentItemRepository>().InRequestScope();
            kernel.Bind<IPolygonRepository>().To<PolygonRepository>().InRequestScope();
            kernel.Bind<IResourceRepository>().To<ResourceRepository>().InRequestScope();
            kernel.Bind<IPartyJoinRequestRepository>().To<PartyJoinRequestRepository>().InRequestScope();
            kernel.Bind<IPartyInviteRepository>().To<PartyInviteRepository>().InRequestScope();
            kernel.Bind<ICountryTreasureService>().To<CountryTreasureService>().InRequestScope();
            kernel.Bind<IReservedEntityNameRepository>().To<ReservedEntityNameRepository>().InRequestScope();
            kernel.Bind<ICongressVotingReservedMoneyRepository>().To<CongressVotingReservedMoneyRepository>().InRequestScope();
            kernel.Bind<IVotingGreetingMessageRepository>().To<VotingGreetingMessageRepository>().InRequestScope();
            kernel.Bind<IShoutboxMessageRepository>().To<ShoutboxMessageRepository>().InRequestScope();
            kernel.Bind<IShoutboxChannelRepository>().To<ShoutboxChannelRepository>().InRequestScope();
            kernel.Bind<IShoutBoxService>().To<ShoutBoxService>().InRequestScope();
            kernel.Bind<IGiftTransactionRepository>().To<GiftTransactionRepository>().InRequestScope();
            kernel.Bind<IGiftService>().To<GiftService>().InRequestScope();
            kernel.Bind<ITradeService>().To<TradeService>().InRequestScope();
            kernel.Bind<ITradeRepository>().To<TradeRepository>().InRequestScope();
            kernel.Bind<ITradeMoneyRepository>().To<TradeMoneyRepository>().InRequestScope();
            kernel.Bind<ITradeProductRepository>().To<TradeProductRepository>().InRequestScope();
            kernel.Bind<IHospitalRepository>().To<HospitalRepository>().InRequestScope();
            kernel.Bind<IHospitalService>().To<HospitalService>().InRequestScope();
            kernel.Bind<IStartService>().To<StartService>().InRequestScope();
            kernel.Bind<ISummaryService>().To<SummaryService>().InRequestScope();
            kernel.Bind<IMilitaryProtectionPactOfferRepository>().To<MilitaryProtectionPactOfferRepository>().InRequestScope();
            kernel.Bind<IMPPService>().To<MPPService>().InRequestScope();
            kernel.Bind<IDevIssueRepository>().To<DevIssueRepository>().InRequestScope();
            kernel.Bind<IDevIssueCommentRepository>().To<DevIssueCommentRepository>().InRequestScope();
            kernel.Bind<IDevIssueService>().To<DevIssueService>().InRequestScope();
            kernel.Bind<IUploadAvatarService>().To<UploadAvatarService>().InRequestScope();
            kernel.Bind<IUploadRepository>().To<UploadRepository>().InRequestScope();
            kernel.Bind<IRemovalService>().To<RemovalService>().InRequestScope();
            kernel.Bind<IEmailService>().ToProvider<EmailServiceProvider>().InRequestScope();
            kernel.Bind<INewDayRepository>().To<NewDayRepository>().InRequestScope();
            kernel.Bind<IConstructionService>().To<ConstructionService>().InRequestScope();
            kernel.Bind<IConstructionRepository>().To<ConstructionRepository>().InRequestScope();
            kernel.Bind<IDefenseSystemService>().To<DefenseSystemService>().InRequestScope();
            kernel.Bind<ITransactionScopeProvider>().To<StandardTransactionScopeProvider>().InRequestScope();
            kernel.Bind<IHotelService>().To<HotelService>().InRequestScope();
            kernel.Bind<IHotelRepository>().To<HotelRepository>().InRequestScope();
            kernel.Bind<IHotelRoomRepository>().To<HotelRoomRepository>().InRequestScope();
            kernel.Bind<IHotelTransactionsService>().To<HotelTransactionsService>().InRequestScope();
            kernel.Bind<IHotelManagerRepository>().To<HotelManagerRepository>().InRequestScope();
            kernel.Bind<IBusinessRepository>().To<BusinessRepository>().InRequestScope();
            kernel.Bind<IMahService>().To<MahService>().InRequestScope();
            kernel.Bind<IHouseService>().To<HouseService>().InRequestScope();
            kernel.Bind<IHouseRepository>().To<HouseRepository>().InRequestScope();
            kernel.Bind<IHouseFurnitureRepository>().To<HouseFurnitureRepository>().InRequestScope();
            kernel.Bind<IHouseChestService>().To<HouseChestService>().InRequestScope();
            kernel.Bind<IHouseChestItemRepository>().To<HouseChestItemRepository>().InRequestScope();
            kernel.Bind<HouseDayChangeProcessor>().ToSelf().InRequestScope();
            kernel.Bind<ISellHouseRepository>().To<SellHouseRepository>().InRequestScope();
            kernel.Bind<ISellHouseService>().To<SellHouseService>().InRequestScope();
            kernel.Bind<IHouseTransactions>().To<HouseTransactions>().InRequestScope();
            kernel.Bind<ICompanyFinanceSummaryRepository>().To<CompanyFinanceSummaryRepository>().InRequestScope();
            kernel.Bind<ICompanyFinanceSummaryService>().To<CompanyFinanceSummaryService>().InRequestScope();
            kernel.Bind<ICompanyFinanceService>().To<CompanyFinanceService>().InRequestScope();
            kernel.Bind<ICountryEventService>().To<CountryEventService>().InRequestScope();
            kernel.Bind<IBattleEventService>().To<BattleEventService>().InRequestScope();
            kernel.Bind<IWarEventService>().To<WarEventService>().InRequestScope();
            kernel.Bind<IEventRepository>().To<EventRepository>().InRequestScope();
            kernel.Bind<ICountryEventRepository>().To<CountryEventRepository>().InRequestScope();
        }
    }
}
