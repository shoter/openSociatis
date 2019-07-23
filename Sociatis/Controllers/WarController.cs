using Common.Exceptions;
using Common.Operations;
using Entities.enums;
using Entities.Extensions;
using Entities.QueryEnums;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Code.Filters;
using Sociatis.Helpers;
using Sociatis.Models.Battle;
using Sociatis.Models.War;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebUtils;
using WebUtils.Attributes;

namespace Sociatis.Controllers
{
    [DayChangeAuthorize]
    public class WarController : ControllerBase
    {
        private readonly IWarRepository warRepository;
        private readonly IWarService warService;
        private readonly IBattleService battleService;
        private readonly ICountryRepository countryRepository;
        private readonly IRegionRepository regionRepository;
        private readonly Entities.Repository.IWalletRepository walletRepository;


        public WarController(IWarRepository warRepository, IWarService warService, IBattleService battleService, ICountryRepository countryRepository, IRegionRepository regionRepository,
            Entities.Repository.IWalletRepository walletRepository, IPopupService popupService) : base(popupService)
        {
            this.warRepository = warRepository;
            this.warService = warService;
            this.battleService = battleService;
            this.countryRepository = countryRepository;
            this.regionRepository = regionRepository;
            this.walletRepository = walletRepository;
        }

        [SociatisAuthorize(PlayerTypeEnum.SuperAdmin)]
        [EntityAuthorize(EntityTypeEnum.Citizen)]
        [Route("War/{warID:int}/EndWar")]
        [HttpPost]
        public ActionResult EndWar(int warID)
        {
            var war = warRepository.GetById(warID);
            if (war == null)
                return NoWarRedirect();

            MethodResult surrenderPossibilityResult = warService.CanSurrenderWar(war, SessionHelper.CurrentEntity);

            if(surrenderPossibilityResult.IsError)
            {
                AddError(surrenderPossibilityResult);
                return RedirectToHome();
            }

            warService.EndWar(war);
            return RedirectBack();
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [EntityAuthorize(EntityTypeEnum.Citizen, EntityTypeEnum.Country)]
        [Route("War/{warID:int}/Surrender")]
        [HttpPost]
        public ActionResult SurrenderWar(int warID)
        {
            var war = warRepository.GetById(warID);
            if (war == null)
                return NoWarRedirect();

            MethodResult surrenderPossibilityResult = warService.CanSurrenderWar(war, SessionHelper.CurrentEntity);

            if (surrenderPossibilityResult.IsError)
                return RedirectToHomeWithError(surrenderPossibilityResult);

            warService.SurrenderWar(war, SessionHelper.CurrentEntity);
            return RedirectBack();
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [EntityAuthorize(EntityTypeEnum.Citizen)]
        [HttpPost]
        public ActionResult StartRessistanceBattle(int regionID)
        {
            var citizen = SessionHelper.LoggedCitizen;
            var region = regionRepository.GetById(regionID);

            var result = warService.CanStartRessistanceBattle(citizen, region);
            if (result.IsError)
                return RedirectBackWithError(result);

            var battle = warService.StartRessistanceBattle(citizen, region, battleService);

            return RedirectToAction("View", "Battle", new { battleID = battle.ID });
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [EntityAuthorize(EntityTypeEnum.Citizen)]
        [AjaxOnly]
        [HttpPost]
        public ActionResult GetBattleStartInformation(int warID, int regionID)
        {
            try
            {
                var entity = SessionHelper.CurrentEntity;
               

                

                var war = warRepository.GetById(warID);
                var attackingCountry = warService.GetControlledCountryInWar(entity, war);
                var region = regionRepository.GetById(regionID);
                var goldNeeded = warService.GetGoldNeededToStartBattle(war, region);

                var vm = new BattleStartInformationViewModel(goldNeeded);

                return PartialView(vm);
            }
            catch(UserReadableException e)
            {
                throw;
            }
            catch(Exception)
            {
                return Content("");
            }
        }


        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("War/{warID:int}")]
        public ActionResult View(int warID)
        {
            var war = warRepository.GetById(warID);

            if (war == null)
                return NoWarRedirect();

            var vm = new WarViewModel(war, warService, warRepository);

            return View(vm);
        }
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("War/{warID:int}/Start-Battle")]
        [HttpGet]
        public ActionResult StartBattle(int warID)
        {
            var war = warRepository.GetById(warID);
            

            if (war == null)
                return NoWarRedirect();
            var entity = SessionHelper.CurrentEntity;
            var operatedCountry = warService.GetControlledCountryInWar(entity, war);

            MethodResult result;

            if ((result = warService.CanStartBattle(SessionHelper.CurrentEntity, operatedCountry, war)).IsError)
            {
                AddError(result);
                return RedirectToAction("View", new { warID = warID });
            }

            var vm = new StartBattleViewModel(war, warRepository, warService);

            return View(vm);
        }

        public RedirectToRouteResult NoCountryRedirect()
        {
            AddError("Country was not found!");
            return RedirectToHome();
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("War/{warID:int}/Start-Battle")]
        [HttpPost]
        public ActionResult StartBattle(int warID, StartBattleViewModel vm)
        {
            var war = warRepository.GetById(warID);

            if (war == null)
                return NoWarRedirect();

            var entity = SessionHelper.CurrentEntity;
            var operatedCountry = warService.GetControlledCountryInWar(entity, war);

            MethodResult result;

            if ((result = warService.CanStartBattle(SessionHelper.CurrentEntity, operatedCountry, war)).IsError)
            {
                AddError(result);
                return RedirectToAction("View", new { warID = warID });
            }

            var warSide = warService.GetWarSide(war, SessionHelper.CurrentEntity);

            if (warSide == WarSideEnum.None)
            {
                AddError("You are not participating in this war!");
                return RedirectToAction("View", new { warID = warID });
            }


            var conquerableRegions = warRepository.GetAttackableRegions(war.ID, warSide == WarSideEnum.Attacker);

            if (conquerableRegions.Any(c => c.ID == vm.SelectedRegionID) == false)
            {
                AddError("You cannot attack this region!");
                return RedirectToAction("View", new { warID = warID });
            }

            var country = war.GetMainCountry(warSide);
            var walletID = country.Entity.WalletID;
            var region = regionRepository.GetById(vm.SelectedRegionID);
            var goldNeeded = warService.GetGoldNeededToStartBattle(war, region);

            if (walletRepository.HaveMoney(walletID, CurrencyTypeEnum.Gold, goldNeeded) == false)
            {
                AddError("You do not have enough gold to attack this region!");
                return RedirectToAction("View", new { warID = warID });
            }

            if(ModelState.IsValid)
            {
                Entities.Battle battle = battleService.CreateBattle(war, vm.SelectedRegionID, warSide);
                return RedirectToAction("View", "Battle",  new { battleID = battle.ID });
            }

            vm = new StartBattleViewModel(war, warRepository, warService);

            return View(vm);
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("War/{warID:int}/CancelSurrender")]
        public ActionResult CancelSurrender(int warID)
        {
            var war = warRepository.GetById(warID);

            if (war == null)
                return NoWarRedirect();
            var entity = SessionHelper.CurrentEntity;
            MethodResult result;
            if ((result = warService.CanCancelSurrender(war, entity)).IsError)
                return RedirectBackWithError(result);

            warService.CancelSurrender(war);
            return RedirectBack();


        }


        public ActionResult NoWarRedirect()
        {
            return RedirectToHomeWithError("War was not found");
        }
    }
}