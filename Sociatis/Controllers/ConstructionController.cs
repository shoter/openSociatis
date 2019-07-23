using Common.DataTables;
using Common.Extensions;
using Common.Operations;
using DataTables.AspNet.Core;
using DataTables.AspNet.Mvc5;
using Entities.enums;
using Entities.Models.Market;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Code.Filters;
using Sociatis.Helpers;
using Sociatis.Models.Constructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebServices;
using WebUtils;
using WebUtils.Extensions;
using WebUtils.Attributes;
using Sociatis.Models.Market;
using Sociatis.Code.Json.MarketOffer;
using Entities;

namespace Sociatis.Controllers
{
    [DayChangeAuthorize]
    public class ConstructionController : ControllerBase
    {
        private readonly IConstructionRepository constructionRepository;
        private readonly IConstructionService constructionService;
        private readonly ICountryRepository countryRepository;
        private readonly IMarketOfferRepository marketOfferRepository;
        private readonly IMarketService marketService;
        public ConstructionController(IConstructionRepository constructionRepository, IPopupService popupService, IConstructionService constructionService,
            ICountryRepository countryRepository, IMarketOfferRepository marketOfferRepository, IMarketService marketService) : base(popupService)
        {
            this.constructionRepository = constructionRepository;
            this.constructionService = constructionService;
            this.countryRepository = countryRepository;
            this.marketOfferRepository = marketOfferRepository;
            this.marketService = marketService;
        }
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult AcceptConstruction(int constructionID)
        {
            var construction = constructionRepository.GetById(constructionID);
            var result = constructionService.CanFinishOneTimeConstruction(construction, SessionHelper.CurrentEntity, SessionHelper.LoggedCitizen);

            if (result.IsError)
                return RedirectBackWithError(result);

            var finishResult = constructionService.FinishOneTimeConstruction(construction);
            AddSuccess("Construction was accepted!");

            if(finishResult is Hotel)
        
                    return RedirectToAction("View", "Hotel", new { hotelID = (finishResult as Hotel).ID });

            return RedirectToHome();
        }

      
    }
}