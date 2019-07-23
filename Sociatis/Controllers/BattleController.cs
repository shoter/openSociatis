using Common.DataTables;
using Common.Maths;
using DataTables.AspNet.Core;
using DataTables.AspNet.Mvc5;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Code.Filters;
using Sociatis.Code.Json.Battle;
using Sociatis.Helpers;
using Sociatis.Models.Battle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.Helpers;
using WebUtils.Attributes;

namespace Sociatis.Controllers
{
    [DayChangeAuthorize]
    public class BattleController : ControllerBase
    {
        private readonly IBattleRepository battleRepository;
        private readonly IBattleService battleService;
        private readonly IWarRepository warRepository;
        private readonly IWarService warService;
        private readonly IProductRepository productRepository;
        private readonly IEquipmentRepository equipmentRepository;

        public BattleController(IBattleRepository battleRepository, IBattleService battleService, IWarRepository warRepository, IWarService warService,
            IProductRepository productRepository, IEquipmentRepository equipmentRepository, IPopupService popupService) : base(popupService)
        {
            this.battleRepository = battleRepository;
            this.battleService = battleService;
            this.warRepository = warRepository;
            this.warService = warService;
            this.productRepository = productRepository;
            this.equipmentRepository = equipmentRepository;
        }

        [Route("Battle/{battleID:int}")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult View(int battleID)
        {
            var battle = battleRepository.GetById(battleID);

            if (battle == null)
                return NoBattleRedirect();

            var vm = new BattleViewModel(battle, battleRepository, battleService, warRepository, warService);


            return View(vm);

        }

        [IsCitizen]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public JsonResult DamageCalculation(int battleID, int weaponQuality, bool isAttacker)
        {
            try
            {
                var citizen = SessionHelper.CurrentEntity.Citizen;
                var battle = battleRepository.GetById(battleID);

                if (battle == null)
                    return JsonError("Battle does not exist!");

                if (weaponQuality > 0)
                    if (citizen.Entity.GetEquipmentItem(ProductTypeEnum.Weapon, weaponQuality, productRepository).Amount == 0)
                        return JsonError("You do not have such weapon in inventory!");


                var info = new
                {
                    damage = battleService.CalculateDamage(citizen, battle, weaponQuality, isAttacker),
                    weaponBonus = battleService.CalculateWeaponBonus(weaponQuality),
                    healthModifier = Percentage.ConvertToPercent(battleService.CalculateHealthModifier(citizen.HitPoints)),
                    militaryRankModifier = Percentage.ConvertToPercent(battleService.CalculateMilitaryRankModifier((double)citizen.MilitaryRank)),
                    weaponAndStrengthModifier = Percentage.ConvertToPercent(battleService.CalculateStrengthModifier((double)citizen.Strength, weaponQuality)),
                    distanceModifier = Percentage.ConvertToPercent(battleService.CalculateDistanceModifier(citizen, battle, isAttacker)),
                    developmentModifier = Percentage.ConvertToPercent(battleService.CalculateDevelopmentModifier(battle, isAttacker)),
                    overallModifier = Percentage.ConvertToPercent(battleService.CalculateOverallModifier(citizen, battle, weaponQuality, isAttacker))
                };

                return JsonData(info);
            }
            catch (Exception e)
            {
                return JsonDebugError(e);
            }
        }

        [IsCitizen]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public JsonResult Fight(int battleID, int weaponQuality, bool isAttacker)
        {
            try
            {
                var citizen = SessionHelper.CurrentEntity.Citizen;
                var battle = battleRepository.GetById(battleID);

                if (battle == null)
                    return JsonError("Battle does not exist!");

                if (citizen == null)
                    return JsonError("You are not a citizen!");

                if (citizen.HitPoints < 30)
                    return JsonError("You must have more than 30 HP to fight!");



                if (weaponQuality > 0)
                    if (citizen.Entity.GetEquipmentItem(ProductTypeEnum.Weapon, weaponQuality, productRepository).Amount == 0)
                        return JsonError("You do not have such weapon in inventory!");

                if (battle.War.IsTrainingWar == false && battle.GetTimeLeft(GameHelper.CurrentDay).TotalSeconds < 0)
                    return JsonError("You cannot take part in the fight!");

                WarSideEnum warSide = WarSideEnum.Attacker;
                if (isAttacker == false)
                    warSide = WarSideEnum.Defender;

                if (warService.CanFightAs(battle, battle.War, citizen, warSide) == false)
                    return JsonError("You cannot fight in that battle!");

                var damage = battleService.CalculateDamage(citizen, battle, weaponQuality, isAttacker);
                battleService.ParticipateInBattle(citizen, battle, isAttacker, weaponQuality);

                return Json(new JsonFightModel(damage, citizen.HitPoints));
            }
            catch (Exception e)
            {
                return JsonDebugError(e);
            }
        }

        public ActionResult NoBattleRedirect()
        {
            return RedirectToHomeWithError("Battle does not exist!");
        }


        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Battle/StatisticsAjax")]
        [HttpPost]
        [AjaxOnly]
        public JsonResult StatisticsAjax(IDataTablesRequest request, int battleID)
        {
            var data = battleRepository.GetParticipantsForBattle(battleID);

            var dataCount = data.Count();

            if (string.IsNullOrWhiteSpace(request.Search.Value) == false)
            {
                var query = request.Search.Value.Trim().ToLower();
                data = data.Where(p => p.Name.ToLower().Contains(query));
            }

            var dataFilteredCount = data.Count();

            if (request.Columns.Get("name").Sort != null)
                data = data.OrderBy(p => p.Name, request.Columns.Get("name").Sort);
            else if (request.Columns.Get("side").Sort != null)
                data = data.OrderBy(p => p.IsAttacker, request.Columns.Get("side").Sort);
            else if (request.Columns.Get("dmg").Sort != null)
                data = data.OrderBy(p => p.Damage, request.Columns.Get("dmg").Sort);
            else
                data = data.OrderByDescending(p => p.ID);

            data = data.Skip(request.Start).Take(request.Length);

            var war = warRepository.GetWarAssociatedWithBattle(battleID);
            var attackerImgUrl = Images.GetCountryFlag(war.AttackerCountryID).Path;
            var defenderImgUrl = Images.GetCountryFlag(war.DefenderCountryID).Path;

            var dataPage = data.Select(p => new
            {
                id = p.ID,
                name = p.Name,
                sideImgUrl = p.IsAttacker ? attackerImgUrl : defenderImgUrl,
                damage = p.Damage,
                imgUrl = p.ImgUrl

            }).ToList();


            // Response creation. To create your response you need to reference your request, to avoid
            // request/response tampering and to ensure response will be correctly created.
            var response = DataTablesResponse.Create(request, dataCount, dataFilteredCount, dataPage);

            // Easier way is to return a new 'DataTablesJsonResult', which will automatically convert your
            // response to a json-compatible content, so DataTables can read it when received.
            return new DataTablesJsonResult(response, JsonRequestBehavior.DenyGet);

        }
    }
}