using Entities.enums;
using Entities.Items;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Code.Filters;
using Sociatis.Code.Json;
using Sociatis.Helpers;
using Sociatis.Models;
using Sociatis.Models.Citizens;
using Sociatis.Models.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.PathFinding;
using WebUtils.Attributes;
using WebUtils.Forms.Select2;

namespace Sociatis.Controllers
{
    [DayChangeAuthorize]
    public class CitizenController : ControllerBase
    {
        ICitizenRepository citizenRepository;
        IEquipmentRepository equipmentRepository;
        IRegionRepository regionRepository;
        ITravelService travelService;
        IRegionService regionService;
        private readonly Entities.Repository.IWalletRepository walletRepository;
        private readonly IWarRepository warRepository;
        private readonly ICountryRepository countryRepository;
        private readonly ICitizenService citizenService;
        private readonly IFriendService friendService;
        private readonly IFriendRepository friendRepository;

        public CitizenController(ICitizenRepository citizenRepository, IEquipmentRepository equipmentRepository, IRegionRepository regionRepository
            , IRegionService regionService, ITravelService travelService, IWarRepository warRepository, Entities.Repository.IWalletRepository walletRepository,
            ICountryRepository countryRepository, ICitizenService citizenService, IPopupService popupService,
            IFriendService friendService, IFriendRepository friendRepository) : base(popupService)
        {
            this.citizenRepository = citizenRepository;
            this.equipmentRepository = equipmentRepository;
            this.regionRepository = regionRepository;
            this.regionService = regionService;
            this.travelService = travelService;
            this.warRepository = warRepository;
            this.walletRepository = walletRepository;
            this.countryRepository = countryRepository;
            this.citizenService = citizenService;
            this.friendService = friendService;
            this.friendRepository = friendRepository;
        }

        [Route("Citizen/{citizenID:int}")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult View(int citizenID)
        {
            var citizen = citizenRepository.GetById(citizenID);

            if (citizen == null)
                return RedirectToHomeWithError("Citizen does not exist!");

            var friends = friendRepository.GetFriends(citizenID).ToList();

            ViewCitizenViewModel vm = new ViewCitizenViewModel(citizen, friends, citizenService, friendService);

            return View(vm);
        }

        

        [Route("Citizen/Wallet")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult Wallet()
        {
            var citizen = SessionHelper.CurrentEntity.Citizen;
            if (citizen == null)
                return RedirectToHomeWithError("You are not a citizen!");

            var walletID = citizen.Entity.WalletID;
            var money = walletRepository.GetMoney(walletID).ToList();

            var vm = new CitizenWalletViewModel(citizen, money, friendService);

            return View(vm);

        }


        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Citizen/{ID:int}/Inventory")]
        public ActionResult Inventory(int ID)
        {
            var citizen = citizenRepository.GetById(ID);
            var vm = new InventoryViewModel(citizen.Entity.Equipment);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Citizen/Travel")]
        [HttpGet]
        public ActionResult Travel()
        {
            var entity = SessionHelper.CurrentEntity;

            if (entity.Citizen == null)
                return RedirectToHomeWithError("You are not a citizen!");

            var tickets = equipmentRepository
                .GetEquipmentItems(entity.EquipmentID.Value, (int)ProductTypeEnum.MovingTicket)
                .Select(t => new MovingTicket(t))
                .ToList();

            var vm = new CitizenTravelViewModel(entity.Citizen, tickets, friendService);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [AjaxOnly]
        [HttpPost]
        public ActionResult RegionsForTravel(int countryID)
        {
            try
            {
                var regions = regionRepository
                    .Where(r => r.CountryID == countryID)
                    .ToList().
                    Select(r => new SelectListItem()
                    {
                        Value = r.ID.ToString(),
                        Text = r.Name
                    }).ToList();

                return JsonSelectList(regions);
            }
            catch(Exception e)
            {
                return JsonDebugOnlyError(e);
            }
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Citizen/Travel")]
        [HttpPost]
        public ActionResult Travel(int regionID, int? ticketQuality)
        {
            var entity = SessionHelper.CurrentEntity;

            if (entity.Citizen == null)
                return RedirectToHomeWithError("You are not a citizen!");

            if (ticketQuality == null)
                return RedirectToHomeWithError("You have not chosen a ticket!");

            var ticket = equipmentRepository
                .GetEquipmentItem(entity.EquipmentID.Value, (int)ProductTypeEnum.MovingTicket, ticketQuality.Value);

            if (ticket == null)
                return RedirectBackWithError("You do not have ticket of this quality");

            var movingTicket = new MovingTicket(ticket);

            var startRegion = regionRepository.GetById(entity.Citizen.RegionID);
            var endRegion = regionRepository.GetById(regionID);

            if (travelService.CanTravel(entity.Citizen, startRegion, endRegion, movingTicket).IsError)
                return RedirectBackWithError("You cannot travel here!");

            travelService.Travel(entity.EntityID, startRegion, endRegion, movingTicket);

            AddInfo(string.Format("You successfuly moved to {0}", endRegion.Name));
            return RedirectToAction("View", new { CitizenID = entity.EntityID });
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Citizen/TravelSummary")]
        [HttpPost]
        public JsonResult TravelSummary(int regionID, int? ticketQuality)
        {

            if (ticketQuality == null)
                return JsonError("You did not have chosen the ticket!");

            try
            {
                var region = regionRepository.GetById(regionID);

                var entity = SessionHelper.CurrentEntity;

                if (entity.Citizen == null)
                    return JsonError("You are not a citizen!");

                var citizen = entity.Citizen;

                var startRegion = regionRepository.GetById(citizen.RegionID);
                var endRegion = regionRepository.GetById(regionID);

                var ticket = new MovingTicket(1, ticketQuality.Value);

                var path = regionService.GetPathBetweenRegions(startRegion, endRegion, new DefaultRegionSelector(), new WarsPenaltyCostCalculator(citizen, warRepository, regionService));
                var canTravel = travelService.CanTravel(citizen, startRegion, endRegion, ticket);

                var vm = new TravelSummaryViewModel(canTravel, path, ticket);

                var content = RenderPartialToString("~/Views/Citizen/TravelSummary.cshtml", vm);

                return Json(new JsonTravelSummary(content, canTravel.isSuccess));
            }
            catch(Exception e)
            {
                return JsonError(e);
                throw e; //we only logged the error
            }
        
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public JsonResult GetCitizens(Select2Request request)
        {
            string query = request.Query.Trim().ToLower();

            var citizens = citizenRepository
                .Where(citizen => citizen.Entity.Name.ToLower().Contains(query))
                .OrderBy(citizen => citizen.Entity.Name)
                .Select(citizen => new Select2Item()
                {
                    id = citizen.ID,
                    text = citizen.Entity.Name
                });

            return Select2Response(citizens, request);
        }
    }
}