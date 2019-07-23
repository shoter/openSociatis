using Common;
using Common.Extensions;
using Common.Transactions;
using Entities.enums;
using Entities.Extensions;
using Entities.Models.Hotels;
using Entities.Models.Market;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Helpers;
using Sociatis.Models.Hotels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebUtils;
using WebUtils.Attributes;
using WebUtils.Extensions;
using WebUtils.Forms.Select2;

namespace Sociatis.Controllers
{
    public class HotelController : ControllerBase
    {
        private readonly IHotelService hotelService;
        private readonly IHotelRepository hotelRepository;
        private readonly IHotelRoomRepository hotelRoomRepository;
        private readonly ITransactionScopeProvider transactionScopeProvider;
        private readonly IWalletRepository walletRepository;
        private readonly IMarketOfferRepository marketOfferRepository;
        private readonly ICitizenRepository citizenRepository;
        private readonly IHotelManagerRepository hotelManagerRepository;
        public HotelController(IPopupService popupService, IHotelRepository hotelRepository, IHotelService hotelService,
            IHotelRoomRepository hotelRoomRepository, ITransactionScopeProvider transactionScopeProvider,
            IWalletRepository walletRepository, IMarketOfferRepository marketOfferRepository, ICitizenRepository citizenRepository,
            IHotelManagerRepository hotelManagerRepository) : base(popupService)
        {
            this.hotelRepository = hotelRepository;
            this.hotelService = hotelService;
            this.hotelRoomRepository = hotelRoomRepository;
            this.transactionScopeProvider = transactionScopeProvider;
            this.walletRepository = walletRepository;
            this.marketOfferRepository = marketOfferRepository;
            this.citizenRepository = citizenRepository;
            this.hotelManagerRepository = hotelManagerRepository;
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Hotel/{hotelID:int}")]
        public ActionResult View(int hotelID)
        {
            var info = hotelRepository.GetHotelInfo(hotelID, SessionHelper.CurrentEntity.EntityID);
            if (info == null)
                return RedirectBackWithError("Hotel does not exist!");

            var main = hotelRepository.GetHotelMain(hotelID);
            var vm = new HotelViewModel(info, main);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Hotels")]
        [Route("Hotels/{regionID:int}")]
        public ActionResult Index(int? regionID)
        {
            regionID = regionID ?? SessionHelper.CurrentEntity.GetCurrentRegion().ID;

            var hotels = hotelRepository.Where(h => h.RegionID == regionID)
                .ToList();

            var vm = new HotelIndexViewModel(regionID, hotels);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult RemoveManager(int hotelID, int managerID)
        {
            var hotel = hotelRepository.GetById(hotelID);
            var manager = hotelRepository.GetManager(hotelID, managerID);
            var result = hotelService.CanManageManager(hotel, SessionHelper.CurrentEntity, manager.HotelRights);

            if (result.IsError)
                return RedirectBackWithError(result);

            hotelRepository.RemoveManager(hotelID, managerID);
            AddSuccess("Manager has been removed!");
            return RedirectBack();
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Hotel/{hotelID:int}/Inventory")]
        [EntityAuthorize(EntityTypeEnum.Citizen, EntityTypeEnum.Organisation, EntityTypeEnum.Company, EntityTypeEnum.Hotel)]
        public ActionResult Inventory(int hotelID)
        {
            var info = hotelRepository.GetHotelInfo(hotelID, SessionHelper.CurrentEntity.EntityID);
            if (info == null)
                return RedirectToHomeWithError("Company does not exist!");

            var rights = hotelService.GetHotelRigths(hotelRepository.GetById(hotelID), SessionHelper.CurrentEntity);

            if (rights.CanManageEquipment == false)
                return RedirectToHomeWithError("You cannot see this inventory!");

            var vm = new HotelEquipmentViewModel(info, hotelRepository.GetEquipmentForHotel(hotelID));

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Hotel/{hotelID:int}/Managers")]
        public ActionResult Managers(int hotelID)
        {
            var info = hotelRepository.GetHotelInfo(hotelID, SessionHelper.CurrentEntity.EntityID);
            if (info == null)
                return RedirectBackWithError("Hotel does not exist!");

            var managers = hotelRepository.GetManagers(hotelID);

            var vm = new HotelManagersViewModel(info, managers);

            return View(vm);
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult UpdateManager(int hotelID, int managerID, int priority, List<HotelUpdateRightsViewModel> rights)
        {
            try
            {
                var hotel = hotelRepository.GetById(hotelID);
                var manager = hotelManagerRepository.Get(hotelID, managerID);



                var hotelRights = new HotelRights()
                {
                    Priority = priority
                };
                foreach (var right in Enums.ToArray<HotelRightsEnum>())
                {
                    hotelRights[right] = rights.First(r => r.HotelRights == right).Value;
                }

                var result = hotelService.CanUpdateManager(manager, SessionHelper.CurrentEntity, hotelRights);
                if (result.IsError)
                    return JsonError(result);

                hotelService.UpdateManager(manager, hotelRights);
                return JsonSuccess("Manager has been updated!");
            }
            catch (Exception e)
            {
                return JsonDebugOnlyError(e);
            }
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult AddManager(int hotelID, int citizenID)
        {
            var hotel = hotelRepository.GetById(hotelID);
            var citizen = citizenRepository.GetById(citizenID);

            var result = hotelService.CanAddManager(hotel, SessionHelper.CurrentEntity, citizen);
            if (result.IsError)
                return RedirectBackWithError(result);

            hotelService.AddManager(hotel, SessionHelper.CurrentEntity, citizen);
            AddSuccess("New manager has been added!");
            return RedirectBack();
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Hotel/{hotelID:int}/MakeDelivery/{productID=0}/{quality=0}/{pagingParam.PageNumber:int=1}")]
        public ActionResult MakeDelivery(int hotelID, PagingParam pagingParam, int productID, int quality)
        {
            pagingParam = pagingParam ?? new PagingParam();

            var hotel = hotelRepository.GetById(hotelID);

            var result = hotelService.CanMakeDeliveries(hotel, SessionHelper.CurrentEntity);
            if (result.IsError)
                return RedirectBackWithError(result);

            int[] availableProducts = { (int)ProductTypeEnum.Fuel, (int)ProductTypeEnum.ConstructionMaterials };

            CompanyTypeEnum[] companies = new CompanyTypeEnum[] { CompanyTypeEnum.Manufacturer };
            IQueryable<MarketOfferModel> offers = marketOfferRepository.GetAvailableOffers(productID, quality, hotel.Region.Country, companies, availableProducts);
            if (offers.Count() > 0)
                offers = offers.Apply(pagingParam);

            var rights = hotelService.GetHotelRigths(hotel, SessionHelper.CurrentEntity);
            var info = hotelRepository.GetHotelInfo(hotelID, SessionHelper.CurrentEntity.EntityID);
            var vm = new HotelMakeDeliveryViewModel(info, hotel, rights, offers, pagingParam, quality, productID);

            return View(vm);
        }



        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Hotel/{hotelID:int}/rooms")]
        public ActionResult Rooms(int hotelID)
        {
            var info = hotelRepository.GetHotelInfo(hotelID, SessionHelper.CurrentEntity.EntityID);
            if (info == null)
                return RedirectBackWithError("Hotel does not exist!");



            var rooms = hotelRoomRepository.GetHotelRooms(hotelID);

            var vm = new HotelRoomsViewModel(info, rooms);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Hotel/{hotelID:int}/SetPrices")]
        [HttpGet]
        public ActionResult SetPrices(int hotelID)
        {
            var info = hotelRepository.GetHotelInfo(hotelID, SessionHelper.CurrentEntity.EntityID);
            if (info == null)
                return RedirectBackWithError("Hotel does not exist!");
            if (info.HotelRights.CanSetPrices == false)
                return RedirectBackWithError("You cannot manage prices in this hotel!");

            var price = hotelRepository.GetGotelPrice(hotelID);
            var vm = new HotelPricesViewModel(price, info);
            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Hotel/{hotelID:int}/SetPrices")]
        [HttpPost]
        public ActionResult SetPrices(int hotelID, decimal? price1, decimal? price2, decimal? price3, decimal? price4, decimal? price5)
        {
            var info = hotelRepository.GetHotelInfo(hotelID, SessionHelper.CurrentEntity.EntityID);
            if (info == null)
                return RedirectBackWithError("Hotel does not exist!");
            if (info.HotelRights.CanSetPrices == false)
                return RedirectBackWithError("You cannot manage prices in this hotel!");

            hotelService.SetPrices(hotelID, price1, price2, price3, price4, price5);
            AddSuccess("Prices has been set!");

            var price = hotelRepository.GetGotelPrice(hotelID);
            var vm = new HotelPricesViewModel(price, info);
            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Hotel/{hotelID:int}/CreateRoom")]
        [HttpGet]
        public ActionResult CreateRoom(int hotelID)
        {
            var info = hotelRepository.GetHotelInfo(hotelID, SessionHelper.CurrentEntity.EntityID);
            if (info == null)
                return RedirectBackWithError("Hotel does not exist!");
            if (info.HotelRights.CanBuildRooms == false)
                return RedirectBackWithError("You cannot manage prices in this hotel!");

            var vm = new HotelCreateRoomViewModel(info);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Hotel/{hotelID:int}/CreateRoom")]
        [HttpPost]
        public ActionResult CreateRoom(int hotelID, int quality)
        {
            var hotel = hotelRepository.GetById(hotelID);

            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                var result = hotelService.CanBuildNewRoom(hotel, quality, SessionHelper.CurrentEntity);
                if (result.IsError)
                    return RedirectBackWithError(result);

                hotelService.BuildRoom(hotel, quality);
                trs.Complete();
            }

            return RedirectToAction("Rooms", "Hotel", new { hotelID = hotelID });
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Hotel/{hotelID:int}/Wallet")]
        [HttpGet]
        public ActionResult Wallet(int hotelID)
        {
            var hotel = hotelRepository.GetById(hotelID);

            var result = hotelService.CanSeeWallet(hotel, SessionHelper.CurrentEntity);
            if (result.IsError)
                return RedirectBackWithError(result);

            var info = hotelRepository.GetHotelInfo(hotelID, SessionHelper.CurrentEntity.EntityID);
            var walletID = hotel.Entity.WalletID;
            var money = walletRepository.GetMoney(walletID).ToList();

            var vm = new HotelWalletViewModel(info, money);

            return View(vm);

        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult RemoveRoom(int roomID)
        {
            var room = hotelRoomRepository.GetById(roomID);
            var result = hotelService.CanRemoveRoom(room, SessionHelper.CurrentEntity);
            if (result.IsError)
                return RedirectBackWithError(result);
            hotelService.RemoveRoom(room);

            AddSuccess("Room removed!");
            return RedirectBack();
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [AjaxOnly]
        public JsonResult CreateRoomCost(int hotelID, int quality)
        {
            try
            {
                var hotel = hotelRepository.GetById(hotelID);
                if (hotel == null)
                    return JsonError("Not found");

                return JsonData(
                    hotelService.CalculateConstructionCostForNewRoom(hotel, quality));

            }
            catch (Exception e)
            {
                return JsonDebugOnlyError(e);
            }
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [AjaxOnly]
        public JsonResult CanCreateRoom(int hotelID, int quality)
        {
            try
            {
                var hotel = hotelRepository.GetById(hotelID);
                var result = hotelService.CanBuildNewRoom(hotel, quality, SessionHelper.CurrentEntity);
                if (result.IsError)
                    return JsonError(result);
                return JsonSuccess("OK");
            }
            catch (Exception e)
            {
                return JsonDebugOnlyError(e);
            }
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult RentRoom(int hotelID, int quality, int nights)
        {
            var hotel = hotelRepository
                    .Include(h => h.HotelPrice)
                    .Include(h => h.Region)
                    .First(h => h.ID == hotelID);

            var entity = SessionHelper.CurrentEntity;

            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                var result = hotelService.CanRentRoom(hotel, quality, nights, entity);
                if (result.IsError)
                    return RedirectBackWithError(result);

                hotelService.RentRoom(hotel, quality, nights, entity.Citizen);

                trs.Complete();
                AddSuccess("Room has been rented!");
                return RedirectToAction("Rooms", "Hotel", new { hotelID = hotelID });
            }
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult GetRoomCost(int hotelID, int quality, int nights)
        {
            try
            {
                if (nights <= 0)
                    return JsonError("You cannot rent room for less than 1 night!");
                var hotel = hotelRepository
                    .Include(h => h.HotelPrice)
                    .Include(h => h.Region)
                    .First(h => h.ID == hotelID);


                var cost = hotelService.CalculateRoomCost(hotel, quality, nights);

                if (cost == null)
                    return JsonError("Room is not for rent!");

                return JsonData(cost.TotalCost);
            }
            catch (Exception e)
            {
                return JsonDebugOnlyError(e);
            }
        }


        public JsonResult CanRentRoom(int hotelID, int quality, int nights)
        {
            try
            {
                var hotel = hotelRepository
                    .Include(x => x.HotelRooms)
                    .Include(x => x.HotelPrice)
                    .Include(x => x.Region)
                    .FirstOrDefault(h => h.ID == hotelID);

                var result = hotelService.CanRentRoom(hotel, quality, nights, SessionHelper.CurrentEntity);
                if (result.IsError)
                    return JsonError(result);

                return JsonSuccess("");
            }
            catch (Exception e)
            {
                return JsonDebugOnlyError(e);
            }


        }



        public JsonResult GetHotels(Select2Request request, int? regionID)
        {
            string search = request.Query.Trim().ToLower();

            var query = hotelRepository
                .Where(h => h.Entity.Name.ToLower().Contains(search));
            if (regionID.HasValue)
                query = query.Where(h => h.RegionID == regionID);

             var retQuery = query.OrderBy(h => h.Entity.Name)
            .Select(h => new Select2Item()
            {
                id = h.ID,
                text = h.Entity.Name
            });

            return Select2Response(retQuery, request);
        }
    }
}