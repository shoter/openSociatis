using Entities.enums;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;
using WebUtils;

namespace Sociatis.Models.Country
{
    public class ShortWarInfoViewModel
    {
        public string StatusName { get; set; }
        public int WarID { get; set; }
        public ShortWarCountryInfoViewModel Attacker { get; set; }
        public ShortWarCountryInfoViewModel Defender { get; set; }
        public int StartDay { get; set; }
        public int? EndDay { get; set; }
        public string Duration { get; set; }
        public bool CanInitiateBattle { get; set; }
        public bool CanInitiateSurrender { get; set; }
        public bool CanCancelSurrender { get; set; }
        public string SurrenderText { get; set; }
        public string AfterSurrenderText { get; set; }
        public bool ShowAfterSurrenderText { get; set; }
        public int AttackerBattleWon { get; set; }
        public int DefenderBattleWon { get; set; }

        public ShortWarInfoViewModel(Entities.War war, IWarService warService)
        {
            WarID = war.ID;
            StatusName = war.Active ? "On going" : "Finished";
            Attacker = new ShortWarCountryInfoViewModel()
            {
                AllyCount = war.CountryInWars.Where(ciw => ciw.IsAttacker).Count(),
                Flag = Images.GetCountryFlag(war.Attacker.Entity.Name).VM,
                Name = war.Attacker.Entity.Name
            };

            Defender = new ShortWarCountryInfoViewModel()
            {
                AllyCount = war.CountryInWars.Where(ciw => ciw.IsAttacker == false).Count(),
                Flag = Images.GetCountryFlag(war.Defender.Entity.Name).VM,
                Name = war.Defender.Entity.Name
            };

            StartDay = war.StartDay;
            EndDay = war.EndDay;

            Duration = string.Format("{0} - {1}", war.StartDay, war.EndDay?.ToString() ?? "?");

            var entity = SessionHelper.CurrentEntity;
            var operatedCountry = warService.GetControlledCountryInWar(entity, war);

            CanInitiateBattle = warService.CanStartBattle(entity, operatedCountry, war).isSuccess;
            CanInitiateSurrender = warService.CanSurrenderWar(war, entity).isSuccess;

            var warSide = warService.GetWarSide(war, SessionHelper.CurrentEntity);
            if (war.AttackerOfferedSurrender.HasValue == false && CanInitiateSurrender)
            {
                SurrenderText = "Send surrender offer";
            }
            if (((war.AttackerOfferedSurrender == true && warSide == WarSideEnum.Defender) || (war.AttackerOfferedSurrender == false && warSide == WarSideEnum.Attacker)) 
                && CanInitiateSurrender)
            {
                SurrenderText = "Accept surrender";
            }
            else if (CanInitiateSurrender == false && war.AttackerOfferedSurrender.HasValue && warSide != WarSideEnum.None && war.Active)
            {
                ShowAfterSurrenderText = true;
                AfterSurrenderText = "Surrender sent";
                CanInitiateBattle = false;
            }

            CanCancelSurrender = warService.CanCancelSurrender(war, SessionHelper.CurrentEntity).isSuccess;

            AttackerBattleWon = war.Battles.Where(b => b.Active == false && b.WonByAttacker == true).Count();
            DefenderBattleWon = war.Battles.Where(b => b.Active == false && b.WonByAttacker == false).Count();

            if (war.IsTrainingWar)
            {
                Attacker = new ShortWarCountryInfoViewModel()
                {
                    AllyCount = 0,
                    Flag = Images.Placeholder.VM,
                    Name = "Chuck Norris"
                };
                Defender = new ShortWarCountryInfoViewModel()
                {
                    AllyCount = 0,
                    Flag = Images.Placeholder.VM,
                    Name = "Bruce Lee"
                };

                CanCancelSurrender = CanInitiateBattle = CanInitiateSurrender = false;
            }
        }
    }
}