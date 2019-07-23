using System.Linq;
using System.Web.Mvc;
using WebServices;
using Sociatis.ActionFilters;
using Entities.enums;
using Sociatis.Models.Home;
using Sociatis.Helpers;
using Entities.Extensions;
using Entities.Repository;
using Entities.Selector;
using WebServices.Helpers;
using System.Data.Entity.SqlServer;
using Common.Extensions;
using Sociatis.Code.Filters;

namespace Sociatis.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly ICongressCandidateService congressCandidateService;
        private readonly IWarRepository warRepository;
        private readonly IArticleRepository articleRepository;
        private readonly ICountryEventRepository countryEventRepository;

        public HomeController(ICongressCandidateService congressCandidateService, IWarRepository warRepository, IArticleRepository articleRepository
            , IPopupService popupService, ICountryEventRepository countryEventRepository) : base(popupService)
        {
            this.congressCandidateService = congressCandidateService;
            this.warRepository = warRepository;
            this.articleRepository = articleRepository;
            this.countryEventRepository = countryEventRepository;
        }
        
        public ActionResult DrodzyTesterzy()
        {
            return View(); 
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [DayChangeAuthorize]
        public ActionResult Index()
        {
            var articleSelector = new ArticleDomSelector(pagingParam: null, IncludeComments: false);

            var currentCountry = SessionHelper.CurrentEntity.GetCurrentCountry();
            var battles = warRepository.GetActiveBattles(currentCountry.ID).Take(3).ToList();

            var trainingBattle = warRepository.GetTrainingBattle();
            if (trainingBattle != null)
                battles.Add(trainingBattle);

            var newestArticles = articleRepository
                .Where(a => a.Newspaper.IsAdmin == false)
                .Apply(articleSelector)
                .OrderByDescending(a => a.ArticleID)
                .Where(a => a.Published)
                .Take(5)
                .ToList();

            var events = countryEventRepository.GetCountryEvents(currentCountry.ID, Persistent.Countries.GetAll(), 5);

            var popularArticles = articleRepository
            .OrderByDescending(a => a.VoteScore / (double)(1 + SqlFunctions.Square((1.3 * (GameHelper.CurrentDay - a.CreationDay + 1)))))
            .Where(a => a.Published)
            .Take(5)
            .Apply(articleSelector)
            .ToList();

            var adminArticles = articleRepository
                .OrderByDescending(a => a.CreationDate)
                .Where(a => a.Newspaper.IsAdmin && a.Published)
                .Take(5)
                .Apply(articleSelector)
                .ToList();


            var vm = new HomeIndexViewModel(currentCountry,
                battles,
                events,
                congressCandidateService,
                newestArticles,
                popularArticles,
                adminArticles);

            return View(vm);
        }

        public ActionResult DayChange()
        {
            if (GameHelper.IsDayChange == false)
                return RedirectToAction("Index");

            return View();
        }

    }
}