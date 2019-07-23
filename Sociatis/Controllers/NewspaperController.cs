using Common.Extensions;
using Common.Operations;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using Entities.Selector;
using Sociatis.ActionFilters;
using Sociatis.Code.Filters;
using Sociatis.Helpers;
using Sociatis.Models.Equipment;
using Sociatis.Models.Newspapers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebServices;
using WebServices.Helpers;
using WebServices.structs;
using WebUtils;
using WebUtils.Extensions;
using WebUtils.Forms.Select2;

namespace Sociatis.Controllers
{
    [DayChangeAuthorize]
    public class NewspaperController : ControllerBase
    {
        private readonly INewspaperService newspaperService;
        private readonly INewspaperRepository newspaperRepository;
        private readonly IArticleRepository articleRepository;
        private readonly ICitizenRepository citizenRepository;
        private readonly IUploadService uploadService;
        private readonly IWalletService walletService;
        private readonly IEquipmentRepository equipmentRepository;
        private readonly IConfigurationRepository configurationRepository;
        private readonly IEntityRepository entityRepository;

        public NewspaperController(INewspaperService newspaperService, INewspaperRepository newspaperRepository, IArticleRepository articleRepository, ICitizenRepository citizenRepository,
            IUploadService uploadService, IWalletService walletService, IEquipmentRepository equipmentRepository, IConfigurationRepository configurationRepository
            , IPopupService popupService, IEntityRepository entityRepository) : base(popupService)
        {
            this.newspaperService = newspaperService;
            this.newspaperRepository = newspaperRepository;
            this.articleRepository = articleRepository;
            this.citizenRepository = citizenRepository;
            this.uploadService = uploadService;
            this.walletService = walletService;
            this.equipmentRepository = equipmentRepository;
            this.configurationRepository = configurationRepository;
            this.entityRepository = entityRepository;
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Newspaper/{newspaperID:int}/Inventory")]
        public ActionResult Inventory(int newspaperID)
        {
            var newspaper = newspaperRepository.GetById(newspaperID);
            if (newspaper == null)
                return RedirectToHomeWithError("Company does not exist!");
            var vm = new EquipmentViewModel(newspaper.Entity.Equipment);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpGet]
        [Route("Newspaper/Create")]
        public ActionResult Create()
        {
            var entity = SessionHelper.CurrentEntity;
            var can = newspaperService.CanCreateNewspaper(entity);
            if (can.IsError)
                return RedirectBackWithError(can);

            var country = entity.GetCurrentCountry();
            var configuration = ConfigurationHelper.Configuration;

            var vm = new CreateNewspaperViewModel(country, configuration);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        [Route("Newspaper/Create")]
        public ActionResult Create(CreateNewspaperViewModel vm)
        {
            var entity = SessionHelper.CurrentEntity;
            var can = newspaperService.CanCreateNewspaper(entity);
            
            if (can.IsError)
                return RedirectBackWithError(can);
            var country = entity.GetCurrentCountry();

            var result = newspaperService.CanCreateNewspaper(entity, vm.Name, SessionHelper.LoggedCitizen);

            if(result.IsError)
                return RedirectBackWithError(result);

            if (ModelState.IsValid)
            {
                var newspaper = newspaperService.CreateNewspaper(entity, vm.Name, country.ID, SessionHelper.LoggedCitizen);
                AddSuccess("Newspaper {0} was sucessfuly created!", vm.Name);
                return RedirectToAction("View", new { newspaperID = newspaper.ID });
            }

            var configuration = ConfigurationHelper.Configuration;
            vm = new CreateNewspaperViewModel(country, configuration, vm.Name);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpGet]
        [Route("Newspaper/{newspaperID:int}/ChangeOwnership")]
        public ActionResult ChangeOwnership(int newspaperID)
        {
            var newspaper = newspaperRepository.GetById(newspaperID);
            var entity = SessionHelper.CurrentEntity;
            var citizen = SessionHelper.LoggedCitizen;

            MethodResult result = newspaperService.CanChangeOwnership(newspaper, entity, citizen);
            if (result.IsError)
                return RedirectBackWithError(result);


            var vm = new ChangeNewspaperOwnerViewModel(newspaper, newspaperService);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        [Route("Newspaper/{newspaperID:int}/ChangeOwnership")]
        public ActionResult ChangeOwnership(int newOwnerID, int newspaperID)
        {
            if (ModelState.IsValid)
            {
                var newspaper = newspaperRepository.GetById(newspaperID);
                var entity = SessionHelper.CurrentEntity;
                var citizen = SessionHelper.LoggedCitizen;
                var newOwner = entityRepository.GetById(newOwnerID);

                var result = newspaperService.CanChangeOwnership(newspaper, entity, citizen, newOwner);
                if (result.IsError)
                    AddError(result);
                else
                {
                    newspaperService.ChangeOwnership(newspaper, newOwner);
                    var currentEntity = SessionHelper.CurrentEntity;

                    if (currentEntity.EntityID == newspaperID)
                    {
                        SessionHelper.SwitchStack.Pop();
                    }

                    AddSuccess("Owner has been changed!");
                    return RedirectToAction("View", new { newspaperID = newspaperID });
                }
            }

            return ChangeOwnership(newspaperID);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]

        public JsonResult GetEligibleEntitiesForOwnershipChange(Select2Request request)
        {
            string search = request.Query.Trim().ToLower();
            var eligibleEntityTypesIDs = newspaperService.GetEligibleEntityTypeForOwnership()
                .Select(c => (int)c);


            var query = entityRepository
                .Where(entity => entity.Name.ToLower().Contains(search) &&
                eligibleEntityTypesIDs.Contains(entity.EntityTypeID))
                .OrderBy(entity => entity.Name)
                .Select(entity => new Select2Item()
                {
                    id = entity.EntityID,
                    text = entity.Name
                });

            return Select2Response(query, request);
        }



        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpGet]
        [Route("Newspaper/{newspaperID:int}/Journalists")]
        public ActionResult ManageJournalists(int newspaperID)
        {
            var newspaper = newspaperRepository.GetById(newspaperID);
            if (newspaper == null)
                return NewspaperDoesNotExistRedirect();

            var entity = SessionHelper.CurrentEntity;
            var rights = newspaperService.GetNewspaperRights(newspaper, entity, SessionHelper.LoggedCitizen);

            if (rights.HasFlag(NewspaperRightsEnum.CanManageJournalists) == false)
                return RedirectToHomeWithError("You cannot manage journalists!");

            var vm = new ManageJournalistsViewModel(newspaper, newspaperService);
            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        [Route("Newspaper/{newspaperID:int}/Journalists")]
        public ActionResult ManageJournalists(ManageJournalistsViewModel vm, int newspaperID)
        {
            var newspaper = newspaperRepository.GetById(newspaperID);
            if (newspaper == null)
                return NewspaperDoesNotExistRedirect();

            var entity = SessionHelper.CurrentEntity;
            var rights = newspaperService.GetNewspaperRights(newspaper, entity, SessionHelper.LoggedCitizen);

            if (rights.HasFlag(NewspaperRightsEnum.CanManageJournalists) == false)
                return RedirectToHomeWithError("You cannot manage journalists!");

            if(ModelState.IsValid)
            {
                var journalistsRights = new Dictionary<int, NewspaperRightsEnum>();
                foreach(var journalist in vm.Journalists)
                {
                    var journalistRights = journalist.GetRights();
                    journalistsRights.Add(journalist.CitizenID, journalistRights);
                }

                newspaperService.ChangeRights(new ChangeJournalistsRightsParam(newspaper, journalistsRights));
                AddSuccess("Rights were changed!");
            }

            vm = new ManageJournalistsViewModel(newspaper, newspaperService);
            return View(vm);
        }
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult RemoveJournalist(RemoveJournalistViewModel vm)
        {
            var newspaper = newspaperRepository.GetById(vm.NewspaperID);
            if (newspaper == null)
                return NewspaperDoesNotExistRedirect();

            var entity = SessionHelper.CurrentEntity;
            var rights = newspaperService.GetNewspaperRights(newspaper, entity, SessionHelper.LoggedCitizen);

            if (rights.HasFlag(NewspaperRightsEnum.CanManageJournalists) == false)
                return RedirectToHomeWithError("You cannot manage journalists!");

            var journalist = newspaperRepository.GetJournalist(vm.JournalistID, vm.NewspaperID);
            if(journalist == null)
                return RedirectToHomeWithError("Journalist does not exist!");
            newspaperService.RemoveJournalist(journalist);

            AddSuccess("Journalist was removed from newspaper!");
            return RedirectBack();
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult AddJournalist(AddJournalistViewModel vm)
        {
            var newspaper = newspaperRepository.GetById(vm.NewspaperID);
            if (newspaper == null)
                return NewspaperDoesNotExistRedirect();

            var entity = SessionHelper.CurrentEntity;
            var rights = newspaperService.GetNewspaperRights(newspaper, entity, SessionHelper.LoggedCitizen);

            if (rights.HasFlag(NewspaperRightsEnum.CanManageJournalists) == false)
                return RedirectToHomeWithError("You cannot manage journalists!");

            var citizen = citizenRepository.GetById(vm.CitizenID);
            if (citizen == null)
                return RedirectToHomeWithError("Citizen does not exist!");

            var result = newspaperService.CanAddJournalist(newspaper, citizen);
            if(ModelState.IsValid && result.IsError == false)
            {
                newspaperService.AddNewJournalist(newspaper, citizen);
                AddSuccess("{0} was added as journalist!", citizen.Entity.Name);
            }
            else if(result.IsError)
            {
                AddError(result);
            }
            else
            {
                AddError("Something went wrong");
            }

            return RedirectBack();


        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Newspaper/")]
        public ActionResult Index()
        {
            var entity = SessionHelper.CurrentEntity;
            var newspapers = newspaperRepository.GetActionableNewspapers(entity.EntityID).ToList();

            var vm = new NewspaperIndexViewModel(newspapers, newspaperService);
            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Newspaper/{newspaperID:int}")]
        public ActionResult View(int newspaperID)
        {
            var newspaper = newspaperRepository.GetById(newspaperID);
            if (newspaper == null)
                return NewspaperDoesNotExistRedirect();

            var vm = new ViewNewspaperViewModel(newspaper, newspaperService);

            return View(vm);
        }

        private ActionResult NewspaperDoesNotExistRedirect()
        {
            return RedirectToHomeWithError("Newspaper does not exist!");
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult BuyPayOnlyContent(int articleID)
        {
            var entity = SessionHelper.CurrentEntity;
            var article = articleRepository.GetById(articleID);
            if (article == null)
                return RedirectToHomeWithError("Article does not exist!");

            if (article.Price.HasValue == false)
                return RedirectToAction("ReadArticle", "Newspaper", new { articleID = articleID });

            decimal tax = article.Newspaper.Country.CountryPolicy.ArticleTax;

            var money = new Money()
            {
                Amount = article.Price.Value * (1 + tax),
                Currency = Persistent.Countries.GetCountryCurrency(article.Newspaper.CountryID)
            };

            if (walletService.HaveMoney(entity.WalletID, money) == false)
            {
                AddError("You do not have enough money to buy this article!");
                return RedirectToAction("ReadArticle", "Newspaper", new { articleID = articleID });
            }
            
            if((equipmentRepository.GetEquipmentItem(article.Newspaper.Entity.EquipmentID.Value, (int)ProductTypeEnum.Paper, 1)?.Amount ?? 0) == 0)
            {
                AddError("Newspaper does not have paper.");
                return RedirectToAction("ReadArticle", "Newspaper", new { articleID = articleID });
            }

            if(article.Buyers.Any(b => b.EntityID == entity.EntityID))
            {
                AddError("You already bought this article!");
                return RedirectToAction("ReadArticle", "Newspaper", new { articleID = articleID });
            }

            newspaperService.BuyArticle(article, entity);
            AddSuccess("You bought article!");
            return RedirectToAction("ReadArticle", "Newspaper", new { articleID = articleID });
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult Vote(int articleID, bool positive)
        {
            var article = articleRepository.GetById(articleID);
            if (article == null)
                return RedirectToHomeWithError("Article not found!");

            var newspaper = article.Newspaper;
            if (newspaper == null)
                return NewspaperDoesNotExistRedirect();

            var entity = SessionHelper.CurrentEntity;
            int score = positive ? 1 : -1;

            newspaperService.VoteOn(article, entity, score);
            AddSuccess("You successfully voted on article!");

            return RedirectToAction("ReadArticle", new { articleID = articleID });
            
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Newspaper/{newspaperID:int}/Write")]
        [HttpGet]
        public ActionResult WriteArticle(int newspaperID)
        {
            var newspaper = newspaperRepository.GetById(newspaperID);
            if (newspaper == null)
                return NewspaperDoesNotExistRedirect();

            var entity = SessionHelper.CurrentEntity;
            var rights = newspaperService.GetNewspaperRights(newspaper, entity, SessionHelper.LoggedCitizen);

            if (rights.HasFlag(NewspaperRightsEnum.CanWriteArticles) == false)
                return RedirectToHomeWithError("You cannot write articles!");

            var vm = new WriteArticleViewModel(newspaper, newspaperService);
            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Newspaper/{newspaperID:int}/Write")]
        [HttpPost]
        public ActionResult WriteArticle(WriteArticleViewModel vm, int newspaperID)
        {
            var newspaper = newspaperRepository.GetById(newspaperID);
            if (newspaper == null)
                return NewspaperDoesNotExistRedirect();

            var entity = SessionHelper.CurrentEntity;
            var rights = newspaperService.GetNewspaperRights(newspaper, entity, SessionHelper.LoggedCitizen);

            if (rights.HasFlag(NewspaperRightsEnum.CanWriteArticles) == false)
                return RedirectToHomeWithError("You cannot write articles!");

            if (ModelState.IsValid)
            {
                
                var article = newspaperService.CreateArticle(newspaper, SessionHelper.CurrentEntity, vm.Title, vm.Content, vm.Price, vm.PayOnlyContent, vm.ShortDescription, vm.ArticleImage, vm.Publish == true);
                if (article.IsError)
                    return RedirectBackWithError(article);
                return RedirectToAction("ReadArticle", new { articleID = article.ReturnValue.ID });
            }

            vm.Info = new NewspaperInfoViewModel(newspaper, newspaperService);
            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Newspaper/Article/{articleID:int}/Edit")]
        [HttpGet]
        public ActionResult EditArticle(int articleID)
        {
            var article = articleRepository.GetById(articleID);
            if (article == null)
                return RedirectToHomeWithError("Article not found!");

            var newspaper = article.Newspaper;
            if (newspaper == null)
                return NewspaperDoesNotExistRedirect();

            var entity = SessionHelper.CurrentEntity;
            var rights = newspaperService.GetNewspaperRights(newspaper, entity, SessionHelper.LoggedCitizen);

            if (rights.HasFlag(NewspaperRightsEnum.CanWriteArticles) == false)
                return RedirectToHomeWithError("You cannot write articles!");

            var vm = new WriteArticleViewModel(newspaper, article, newspaperService);
            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Newspaper/Article/{articleID:int}/Edit")]
        [HttpPost]
        public ActionResult EditArticle(WriteArticleViewModel vm, int articleID)
        {
            var article = articleRepository.GetById(articleID);
            if (article == null)
                return RedirectToHomeWithError("Article not found!");

            var newspaper = article.Newspaper;
            if (newspaper == null)
                return NewspaperDoesNotExistRedirect();

            var entity = SessionHelper.CurrentEntity;
            var rights = newspaperService.GetNewspaperRights(newspaper, entity, SessionHelper.LoggedCitizen);

            if (rights.HasFlag(NewspaperRightsEnum.CanWriteArticles) == false)
                return RedirectToHomeWithError("You cannot write articles!");

            if (ModelState.IsValid)
            {

               

                article.Content = vm.Content;
                article.PayOnlyContent = vm.PayOnlyContent ?? "";
                article.ShortDescription = vm.ShortDescription;
                article.Price = (decimal?)vm.Price;
                if (vm.EnablePayOnly == false)
                    article.Price = null;

                article.Title = vm.Title;
                if (vm.ArticleImage != null)
                {
                    var result = uploadService.UploadImage(vm.ArticleImage, WebServices.enums.UploadLocationEnum.Articles);
                    if (result.IsError)
                        AddError(result as MethodResult);
                    else
                        article.ImgURL = result;
                }
                article.Published = (vm.Publish == true);

                articleRepository.SaveChanges();

                return RedirectToAction("ReadArticle", new { articleID = article.ID });
            }

            vm.Info = new NewspaperInfoViewModel(newspaper, newspaperService);
            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Newspaper/Article/{articleID:int}")]
        public ActionResult ReadArticle(int articleID)
        {
            PagingParam pp = new PagingParam();
            var selector = new ArticleDomSelector(pp, SessionHelper.CurrentEntity.EntityID);
            var article = articleRepository.Where(a => articleID == a.ID).Apply(selector).First();
            pp.Records = article.CommentCount;

            var newspaper = newspaperRepository.GetById(article.NewspaperID);
            var vm = new ArticleViewModel(article, newspaper, newspaperService, pp);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult WriteComment(int articleID, string content)
        {
            newspaperService.CreateComment(articleID, SessionHelper.CurrentEntity.EntityID, content);
            AddSuccess("Comment was added!");
            return RedirectToAction("ReadArticle", new { articleID = articleID });
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpGet]
        [Route("Newspaper/{newspaperID:int}/Articles/{pagingParam.PageNumber:int=1}")]
        public ActionResult ManageArticles(int newspaperID, PagingParam pagingParam)
        {
            pagingParam = pagingParam ?? new PagingParam();

            var newspaper = newspaperRepository.GetById(newspaperID);
            if (newspaper == null)
                return NewspaperDoesNotExistRedirect();

            var entity = SessionHelper.CurrentEntity;
            var rights = newspaperService.GetNewspaperRights(newspaper, entity, SessionHelper.LoggedCitizen);

            if (rights.HasFlag(NewspaperRightsEnum.CanManageArticles) == false)
                return RedirectToHomeWithError("You cannot manage articles!");

            var articles = articleRepository
                .Where(a => a.NewspaperID == newspaperID)
                .OrderByDescending(a => a.ID)
                .Apply(pagingParam)
                .ToList();

            var vm = new ManageArticlesViewModel(newspaper, articles, newspaperService, pagingParam);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult DeleteArticle(int articleID)
        {
            var article = articleRepository.GetById(articleID);
            if (article == null)
                return RedirectToHomeWithError("Article does not exist!");

            var newspaper = article.Newspaper;
            if (newspaper == null)
                return NewspaperDoesNotExistRedirect();

            var entity = SessionHelper.CurrentEntity;
            var rights = newspaperService.GetNewspaperRights(newspaper, entity, SessionHelper.LoggedCitizen);

            if (rights.HasFlag(NewspaperRightsEnum.CanManageArticles) == false)
                return RedirectToHomeWithError("You cannot manage articles!");

            articleRepository.Remove(article);
            AddSuccess("Article was successfully deleted!");
            articleRepository.SaveChanges();
            return RedirectToAction("ManageArticles", "Newspaper", new { newspaperID = newspaper.ID });
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult ChangePublishOptions(int articleID, bool publish)
        {
            var article = articleRepository.GetById(articleID);
            if (article == null)
                return RedirectToHomeWithError("Article does not exist!");

            var newspaper = article.Newspaper;
            if (newspaper == null)
                return NewspaperDoesNotExistRedirect();

            var entity = SessionHelper.CurrentEntity;
            var rights = newspaperService.GetNewspaperRights(newspaper, entity, SessionHelper.LoggedCitizen);

            if (rights.HasFlag(NewspaperRightsEnum.CanManageArticles) == false)
                return RedirectToHomeWithError("You cannot manage articles!");

            article.Published = publish;
            AddSuccess("Article was successfully " + (publish ? "" : "un") + "published.");
            articleRepository.SaveChanges();
            return RedirectToAction("ManageArticles", "Newspaper", new { newspaperID = newspaper.ID });
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public JsonResult SetNewPrice(int newspaperID, int articleID, double price)
        {
            try
            {
                var newspaper = newspaperRepository.GetById(newspaperID);
                if (newspaper == null)
                    return JsonError("Newspaper does not exist!");

                var entity = SessionHelper.CurrentEntity;
                var rights = newspaperService.GetNewspaperRights(newspaper, entity, SessionHelper.LoggedCitizen);

                if (rights.HasFlag(NewspaperRightsEnum.CanManageArticles) == false)
                    return JsonError("You cannot manage articles!");

                var article = articleRepository.GetById(articleID);

                if (article.NewspaperID != newspaper.ID)
                    return JsonError("Wrong data!");

                if (price < 0)
                    return JsonError("Wrong price!");

                else if (price > 100000000)
                    return JsonError("Wrong price!");

                article.Price = (decimal)price;
                articleRepository.SaveChanges();

                return JsonSuccess("Price was changed!");
            }
            catch(Exception e)
            {
                return JsonDebugOnlyError(e);
            }

        }

        
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult ManageArticlesPost(int newspaperID, PagingParam pagingParam)
        {
            return RedirectToAction("ManageArticles", new RouteValueDictionary { { "pagingParam.PageNumber", pagingParam.PageNumber }, { "newspaperID", newspaperID } });
        }

    }
}