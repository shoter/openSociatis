using Entities;
using Entities.enums;
using Entities.Extensions;
using Entities.Models.Market;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Code.Json.MarketOffer;
using Sociatis.Helpers;
using Sociatis.Models.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebServices;
using WebUtils;
using WebUtils.Attributes;
using WebUtils.Extensions;

namespace Sociatis.Controllers
{
    public class MarketOfferController : ControllerBase
    {

        readonly IMarketOfferRepository marketOfferRepository;
        readonly IMarketService marketService;
        readonly IWalletService walletService;
        readonly IEquipmentRepository equipmentRepository;
        readonly ICountryRepository countryRepository;
        readonly ICompanyService companyService;
        private readonly IEquipmentService equipmentService;
        private readonly IEntityRepository entityRepository;


        public MarketOfferController(IMarketOfferRepository marketOfferRepository, IMarketService marketService, IWalletService walletService,
            IEquipmentRepository equipmentRepository, ICountryRepository countryRepository, ICompanyService companyService
            , IPopupService popupService, IEquipmentService equipmentService, IEntityRepository entityRepository) : base(popupService)
        {
            this.marketOfferRepository = marketOfferRepository;
            this.marketService = marketService;
            this.walletService = walletService;
            this.equipmentRepository = equipmentRepository;
            this.countryRepository = countryRepository;
            this.companyService = companyService;
            this.equipmentService = equipmentService;
            this.entityRepository = entityRepository;
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult MarketOffersPost(int countryID, PagingParam pagingParam, int productID, int quality)
        {
            return RedirectToAction("MarketOffers", new RouteValueDictionary { { "countryID", countryID }, { "pagingParam.PageNumber", pagingParam.PageNumber }, { "productID", productID }, { "quality", quality } });
        }


        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public JsonResult CalculateTotal(int offerID, int amount, int? buyerID)
        {
            Entity buyer = SessionHelper.CurrentEntity;
            if (buyerID.HasValue)
                buyer = entityRepository.GetById(buyerID.Value);

            var offer = marketOfferRepository.GetById(offerID);
            var cost = marketService.GetOfferCost(offer, buyer, amount);


            return Json(new JsonCalculateTotalModel(cost));
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Country/{countryID:int}/MarketOffers/{productID=0}/{quality=0}/{pagingParam.PageNumber:int=1}")]
        public ActionResult MarketOffers(int countryID, PagingParam pagingParam, int productID, int quality)
        {
            pagingParam = pagingParam ?? new PagingParam();

            var country = countryRepository.GetById(countryID);

            if (country == null)
                return RedirectToHomeWithError("Country does not exist!");
            int[] availableProducts = { (int)ProductTypeEnum.Bread, (int)ProductTypeEnum.MovingTicket, (int)ProductTypeEnum.Tea, (int)ProductTypeEnum.Weapon };

            CompanyTypeEnum[] companies = new CompanyTypeEnum[] { CompanyTypeEnum.Shop };
            IQueryable<MarketOfferModel> offers = marketOfferRepository.GetAvailableOffers(productID, quality, country, companies, availableProducts);

            if (offers.Count() > 0)
                offers = offers.Apply(pagingParam);

            var countries = countryRepository.GetAll().ToList();


            var vm = new CountryMarketOffersListViewModel(SessionHelper.CurrentEntity, country, offers.ToList(), countries, availableProducts, pagingParam, quality, productID);

            return View(vm);
        }


        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult ResourceOffersPost(int countryID, PagingParam pagingParam, int productID, int quality)
        {
            return RedirectToAction("ResourceOffers", new RouteValueDictionary { { "countryID", countryID }, { "pagingParam.PageNumber", pagingParam.PageNumber }, { "productID", productID }, { "quality", quality } });
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [AjaxOnly]
        public JsonResult RemoveOffer(int offerID)
        {
            try
            {
                var offer = marketOfferRepository.GetById(offerID);

                if (offer == null)
                    return JsonError("Offer does not exist!");

                var rights = companyService.GetCompanyRights(offer.Company, SessionHelper.CurrentEntity, SessionHelper.LoggedCitizen);

                if (rights.CanPostMarketOffers == false)
                    return JsonError("You do not have sufficient rights to do that!");

                marketService.RemoveOffer(offer.ID);
                return JsonSuccess("Offer sucessfully removed!");
            }
            catch (Exception e)
            {
                return JsonDebugOnlyError(e);
            }
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Country/{countryID:int}/ResourceOffers/{productID=0}/{quality=0}/{pagingParam.PageNumber:int=1}")]
        public ActionResult ResourceOffers(int countryID, PagingParam pagingParam, int productID, int quality)
        {
            pagingParam = pagingParam ?? new PagingParam();

            var country = countryRepository.GetById(countryID);

            if (country == null)
                return RedirectToHomeWithError("Country does not exist!");


            int[] availableProducts = { (int)ProductTypeEnum.Bread, (int)ProductTypeEnum.MovingTicket, (int)ProductTypeEnum.Tea, (int)ProductTypeEnum.Weapon, ProductTypeEnum.Fuel.ToInt(),
            ProductTypeEnum.Grain.ToInt(), ProductTypeEnum.Iron.ToInt(), ProductTypeEnum.MedicalSupplies.ToInt(), ProductTypeEnum.Oil.ToInt(), ProductTypeEnum.TeaLeaf.ToInt(), ProductTypeEnum.Wood.ToInt(),
            ProductTypeEnum.Paper.ToInt(), ProductTypeEnum.ConstructionMaterials.ToInt()};

            CompanyTypeEnum[] companies = new CompanyTypeEnum[] { CompanyTypeEnum.Manufacturer, CompanyTypeEnum.Producer };
            IQueryable<MarketOfferModel> offers = marketOfferRepository.GetAvailableOffers(productID, quality, country, companies, availableProducts);

            if (offers.Count() > 0)
                offers = offers.Apply(pagingParam);

            var countries = countryRepository.GetAll().ToList();


            var vm = new CountryMarketOffersListViewModel(SessionHelper.CurrentEntity, country, offers.ToList(), countries, availableProducts, pagingParam, quality, productID);

            return View("MarketOffers", vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult HousesPost(int countryID, PagingParam pagingParam)
        {
            return RedirectToAction("Houses", new RouteValueDictionary { { "countryID", countryID }, { "pagingParam.PageNumber", pagingParam.PageNumber } });
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Country/{countryID:int}/Houses/{quality=0}/{pagingParam.PageNumber:int=1}")]
        public ActionResult Houses(int countryID, PagingParam pagingParam)
        {
            pagingParam = pagingParam ?? new PagingParam();

            var country = countryRepository.GetById(countryID);

            if (country == null)
                return RedirectToHomeWithError("Country does not exist!");


            int[] availableProducts = { (int)ProductTypeEnum.House };

            CompanyTypeEnum[] companies = new CompanyTypeEnum[] { CompanyTypeEnum.Construction };
            IQueryable<MarketOfferModel> offers = marketOfferRepository.GetAvailableOffers((int)ProductTypeEnum.House, 1, country, companies, availableProducts);

            if (SessionHelper.CurrentEntity.Is(EntityTypeEnum.Citizen))
                offers = offers.Where(o => o.CompanyRegionID == SessionHelper.LoggedCitizen.RegionID);

            if (offers.Count() > 0)
                offers = offers.Apply(pagingParam);

            var countries = countryRepository.GetAll().ToList();

            var vm = new CountryMarketOffersListViewModel(SessionHelper.CurrentEntity, country, offers.ToList(), countries, availableProducts, pagingParam, 1, (int)ProductTypeEnum.House);
            vm.DisableShowingQuantity();
            vm.QualityList = new List<SelectListItem>();

            return View("MarketOffers", vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult BuyOffer(int offerID, int amount)
        {
            var offer = marketOfferRepository.GetById(offerID);

            var entity = SessionHelper.CurrentEntity;


            var result = marketService.CanBuyOffer(offer, amount, entity);

            if (result.IsError)
                return RedirectBackWithError(result);

            marketService.Buy(offer, entity, amount);

            return RedirectBackWithInfo(string.Format("You sucessfuly bought {0} {1}", amount, MarketOfferExtensions.GetProductType(offer).ToHumanReadable()));
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult BuyOfferWithWallet(int entityID, int offerID, int amount, int walletID)
        {
            var result = walletService.CanAccessWallet(walletID, SessionHelper.CurrentEntity.EntityID);
            if (result.IsError)
                return RedirectBackWithError(result);

            var offer = marketOfferRepository.GetById(offerID);

            var entity = entityRepository.GetById(entityID);

            result = marketService.CanBuyOffer(offer, amount, entity, walletID);

            if (result.IsError)
                return RedirectBackWithError(result);

            marketService.Buy(offer, entity, amount, walletID);

            return RedirectBackWithInfo(string.Format("You sucessfuly bought {0} {1}", amount, MarketOfferExtensions.GetProductType(offer).ToHumanReadable()));
        }
    }
}