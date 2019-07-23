using Common.Extensions;
using Entities.enums;
using Entities.Extensions;
using Entities.Models.Market;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Helpers;
using Sociatis.Models.Houses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using WebServices;
using WebUtils;
using WebUtils.Extensions;

namespace Sociatis.Controllers
{
    public class HouseController : ControllerBase
    {
        private readonly IHouseRepository houseRepository;
        private readonly IHouseService houseService;
        private readonly IEquipmentRepository equipmentRepository;
        private readonly IHouseFurnitureRepository houseFurnitureRepository;
        private readonly IHouseChestService houseChestService;
        private readonly IHouseChestItemRepository houseChestItemRepository;
        private readonly IEquipmentItemRepository equipmentItemRepository;
        private readonly IMarketOfferRepository marketOfferRepository;
        private readonly IMarketService marketService;
        private readonly ISellHouseService sellHouseService;

        public HouseController(IPopupService popupService, IHouseRepository houseRepository,
            IHouseService houseService, IEquipmentRepository equipmentRepository, IHouseFurnitureRepository houseFurnitureRepository,
            IHouseChestService houseChestService, IHouseChestItemRepository houseChestItemRepository, IEquipmentItemRepository equipmentItemRepository,
            IMarketOfferRepository marketOfferRepository, IMarketService marketService, ISellHouseService sellHouseService) : base(popupService)
        {
            this.houseRepository = houseRepository;
            this.houseService = houseService;
            this.equipmentRepository = equipmentRepository;
            this.houseFurnitureRepository = houseFurnitureRepository;
            this.houseChestService = houseChestService;
            this.houseChestItemRepository = houseChestItemRepository;
            this.equipmentItemRepository = equipmentItemRepository;
            this.marketOfferRepository = marketOfferRepository;
            this.marketService = marketService;
            this.sellHouseService = sellHouseService;
        }

        [Route("House")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult Index()
        {
            if (SessionHelper.CurrentEntity.Is(EntityTypeEnum.Citizen) == false)
                return RedirectBackWithError("Only citizens can have houses!");

            var houses = houseRepository.GetOwnedHouses(SessionHelper.CurrentEntity.EntityID);

            var vm = new HouseIndexViewModel(houses);

            return View(vm);
        }

        [Route("House/{houseID:long}")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult View(long houseID)
        {
            var house = houseRepository.GetById(houseID);
            var result = houseService.CanViewHouse(house, SessionHelper.CurrentEntity);
            if (result.IsError)
                return RedirectBackWithError(result);

            var vm = new HouseViewModel(house, houseService.GetHouseRights(house, SessionHelper.CurrentEntity));

            return View(vm);
        }

        [Route("House/{houseID:long}/Chest")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult Chest(long houseID)
        {
            var house = houseRepository.GetById(houseID);
            var result = houseService.CanModifyHouse(house, SessionHelper.CurrentEntity);
            if (result.IsError)
                return RedirectBackWithError(result);

            var equipment = equipmentRepository.GetByEntityID(SessionHelper.LoggedCitizen.ID);
            var chest = houseFurnitureRepository.GetHouseChest(houseID);

            var vm = new HouseChestViewModel(house, equipment, chest, houseService.GetHouseRights(house, SessionHelper.CurrentEntity));

            return View(vm);
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult DropItem(long houseID, int itemID, int amount)
        {
            var house = houseRepository.GetById(houseID);
            var result = houseService.CanViewHouse(house, SessionHelper.CurrentEntity);
            if (result.IsError)
                return RedirectBackWithError(result);


            var item = houseChestItemRepository.GetItem(houseID, itemID);

            result = houseChestService.CanDropItem(item, amount);
            if (result.IsError)
                return RedirectBackWithError(result);

            houseChestService.RemoveItem(item, amount);
            return RedirectBackWithSuccess("Item has been removed!");
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult TransferToChest(long houseID, int itemID, int amount)
        {
            var house = houseRepository.GetById(houseID);
            var entity = SessionHelper.CurrentEntity;

            var result = houseService.CanViewHouse(house, entity);
            if (result.IsError)
                return RedirectBackWithError(result);

            var item = equipmentItemRepository.GetById(itemID);
            var chest = houseFurnitureRepository.GetHouseChest(houseID);

            result = houseChestService.CanTransferItemToChest(entity, item, chest, amount);
            if (result.IsError)
                return RedirectBackWithError(result);

            houseChestService.TransferItemToChest(entity, item, chest, amount);
            return RedirectBackWithSuccess("Item has been transfered to chest!");
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult TransferToCitizen(long houseID, int itemID, int amount)
        {
            var house = houseRepository.GetById(houseID);
            var entity = SessionHelper.CurrentEntity;

            var result = houseService.CanViewHouse(house, entity);
            if (result.IsError)
                return RedirectBackWithError(result);

            var item = houseChestItemRepository.GetItem(houseID, itemID);
            var chest = houseFurnitureRepository.GetHouseChest(houseID);

            result = houseChestService.CanTransferItemToCitizen(entity, item, SessionHelper.LoggedCitizen, amount);
            if (result.IsError)
                return RedirectBackWithError(result);

            houseChestService.TransferItemToCitizen(entity, item, SessionHelper.LoggedCitizen, amount);
            return RedirectBackWithSuccess("Item has been transfered to you!");
        }


        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult BuyCMPost(int houseID, PagingParam pagingParam)
        {
            return RedirectToAction("BuyCM", new RouteValueDictionary { { "houseID", houseID }, { "pagingParam.PageNumber", pagingParam.PageNumber }});
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("House/{houseID:long}/BuyConstructionMaterials/{pagingParam.PageNumber:int=1}")]
        public ActionResult BuyCM(long houseID, PagingParam pagingParam)
        {
            var house = houseRepository.GetById(houseID);

            var result = houseService.CanViewHouse(house, SessionHelper.CurrentEntity);
            if (result.IsError)
                return RedirectBackWithError(result);

            int[] availableProducts = {(int)ProductTypeEnum.ConstructionMaterials, (int)ProductTypeEnum.UpgradePoints };
            CompanyTypeEnum[] companies = new CompanyTypeEnum[] { CompanyTypeEnum.Manufacturer };
            IQueryable<MarketOfferModel> offers = marketOfferRepository.GetAvailableOffers(0, 1, house.Region.Country, companies, availableProducts);
            if (offers.Count() > 0)
                offers = offers.Apply(pagingParam);


            var vm = new HouseBuyCMViewModel(house, pagingParam, offers, houseService.GetHouseRights(house, SessionHelper.CurrentEntity));

            return View(vm);
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult BuyHouse(long houseID)
        {
            var house = houseRepository.GetById(houseID);

            var result = sellHouseService.CanBuy(house, SessionHelper.CurrentEntity);
            if (result.IsError)
                return RedirectBackWithError(result);

            sellHouseService.Buy(house, SessionHelper.CurrentEntity);

            AddSuccess("You bought a house");
            return RedirectToAction(nameof(View), "house", new { houseID = houseID });
        }

        [HttpGet]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("House/{houseID:long}/SellHouse")]
        public ActionResult SellHouse(long houseID)
        {
            var house = houseRepository.GetById(houseID);

            var result = houseService.CanModifyHouse(house, SessionHelper.CurrentEntity);
            if (result.IsError)
                return RedirectBackWithError(result);

            var rights = houseService.GetHouseRights(house, SessionHelper.CurrentEntity);

            var vm = new HouseSellViewModel(house, rights);

            return View(vm);
        }
        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("House/{houseID:long}/SellHouse")]
        public ActionResult SellHouse(long houseID, decimal price)
        {
            var house = houseRepository.GetById(houseID);

            var result = houseService.CanModifyHouse(house, SessionHelper.CurrentEntity);

            if (result.isSuccess)
                result = sellHouseService.CanSellHouse(house, price);


            if (result.IsError)
                return RedirectBackWithError(result);

            sellHouseService.SellHouse(house, price);

            AddSuccess("House is now on sell");
            return RedirectToAction(nameof(View), "House", new { houseID = houseID });
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult BuyOffer(long houseID, int offerID, int amount)
        {
            var house = houseRepository.GetById(houseID);
            var entity = SessionHelper.CurrentEntity;

            var result = houseService.CanViewHouse(house, entity);
            if (result.IsError)
                return RedirectBackWithError(result);

            var offer = marketOfferRepository.GetById(offerID);

            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                result = houseService.CanBuyOffer(offer, amount, house, SessionHelper.CurrentEntity);

                if (result.IsError)
                    return RedirectBackWithError(result);

                int productID = offer.ProductID;
                marketService.Buy(offer, SessionHelper.LoggedCitizen.Entity, amount);

                var item = equipmentRepository.GetEquipmentItem(SessionHelper.CurrentEntity.EquipmentID.Value, productID, 1);
                var chest = houseFurnitureRepository.GetHouseChest(houseID);
                houseChestService.TransferItemToChest(SessionHelper.CurrentEntity, item, chest, amount);
                trs.Complete();
            }

            return RedirectBackWithSuccess("Item has been transfered!");
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("house/{houseID:long}/Furniture")]
        public ActionResult Furniture(long houseID)
        {
            var house = houseRepository.GetById(houseID);

            var result = houseService.CanViewHouse(house, SessionHelper.CurrentEntity);

            var furniture = houseFurnitureRepository.GetFurniture(houseID);

            var vm = new HouseFurnitureListViewModel(house, furniture, houseService.GetHouseRights(house, SessionHelper.CurrentEntity));

            return View(vm);
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult UpgradeFurniture(long houseID, int furnitureTypeID)
        {
            var house = houseRepository.GetById(houseID);

            var result = houseService.CanModifyHouse(house, SessionHelper.CurrentEntity);
            if (result.IsError)
                return RedirectBackWithError(result);
            var furnitureType = (FurnitureTypeEnum)furnitureTypeID;

            result = houseService.CanUpgradeFurniture(house, furnitureType);
            if (result.IsError)
                return RedirectBackWithError(result);

            houseService.UpgradeFurniture(house, furnitureType);

            return RedirectBackWithSuccess($"{furnitureType.ToHumanReadable().FirstUpper()} has been upgraded!");
        }

        [HttpGet]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("House/{houseID:long}/CreateFurniture")]
        public ActionResult CreateFurniture(long houseID)
        {
            var house = houseRepository.GetById(houseID);

            var result = houseService.CanModifyHouse(house, SessionHelper.CurrentEntity);
            if (result.IsError)
                return RedirectBackWithError(result);

            var rights = houseService.GetHouseRights(house, SessionHelper.CurrentEntity);

            var unbuiltFurniture = houseFurnitureRepository.GetUnbuiltFurniture(houseID);

            if (unbuiltFurniture.Any() == false)
                return RedirectBackWithError("You had created all possible furniture types for this house!");

            var vm = new HouseCreateFurnitureViewModel(house, rights, unbuiltFurniture);

            return View(vm);
        }

        [HttpPost]
        [Route("House/{houseID:long}/CreateFurniture")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult CreateFurniture(long houseID, int furnitureTypeID)
        {
            var house = houseRepository.GetById(houseID);
            var furnitureType = (FurnitureTypeEnum)furnitureTypeID;

            var result = houseService.CanBuildFurniture(house, furnitureType, SessionHelper.CurrentEntity);
            if (result.IsError)
                return RedirectBackWithError(result);

            houseService.BuildFurniture(house, furnitureType);

            return RedirectToAction(nameof(HouseController.Furniture), "House", new { houseID = houseID });
        }
    }
}
