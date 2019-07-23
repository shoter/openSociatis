using Entities.enums;
using Entities.Repository;
using Sociatis.Helpers;
using Sociatis.Models.Battle;
using Sociatis.Models.Country;
using Sociatis.Models.Infos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Models.War
{
    public class WarInfoViewModel
    {
        public ShortWarInfoViewModel Info { get; set; }
        public List<ShortWarCountryInfoViewModel> AttackerAllies { get; set; } = new List<ShortWarCountryInfoViewModel>();
        public List<ShortWarCountryInfoViewModel> DefendedAllies { get; set; } = new List<ShortWarCountryInfoViewModel>();
        public bool CanInitiateBattle { get; set; }
        public bool CanInitiateSurrender { get; set; }
        public string SurrenderText { get; set; }
        public string WarStateText { get; set; }
        public bool Active { get; set; }
        public bool CanCancelSurrender { get; set; }
        public bool IsRessistance { get; set; }

        public ShortBattleParticipantViewModel AttackerHero { get; set; }
        public ShortBattleParticipantViewModel DefenderHero { get; set; }

        public InfoMenuViewModel Menu { get; set; }

        public WarInfoViewModel(Entities.War war, IWarRepository warRepository, IWarService warService)
        {
            Info = new ShortWarInfoViewModel(war, warService);

            Active = war.Active;

            var countriesInWar = warRepository.GetCountriesInWar(war.ID);

            var countries = new
            {
                Attackers = countriesInWar.
                    Where(ciw => ciw.IsAttacker)
                    .Select(ciw => ciw.Country)
                    .ToList(),
                Defenders = countriesInWar
                .Where(ciw => ciw.IsAttacker == false)
                .Select(ciw => ciw.Country)
                .ToList()
            };

            IsRessistance = war.IsRessistanceWar;

            foreach (var attacker in countries.Attackers)
                AttackerAllies.Add(new ShortWarCountryInfoViewModel(attacker));
            foreach (var defender in countries.Defenders)
                DefendedAllies.Add(new ShortWarCountryInfoViewModel(defender));

            var entity = SessionHelper.CurrentEntity;
            var operatedCountry = warService.GetControlledCountryInWar(entity, war);

            CanInitiateBattle = warService.CanStartBattle(entity, operatedCountry, war).isSuccess;
            CanInitiateSurrender = warService.CanSurrenderWar(war, entity).isSuccess;

            if (Active)
            {
                WarStateText = "Active";
                if (war.AttackerOfferedSurrender == true)
                {
                    WarStateText = "Attacker offered surrender";
                }
                else if (war.AttackerOfferedSurrender == false)
                {
                    WarStateText = "Defender offered surrender";
                }
            }
            else
            {
                WarStateText = "Inactive";
            }

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


            CanCancelSurrender = warService.CanCancelSurrender(war, SessionHelper.CurrentEntity).isSuccess;


            var attackerHero = warService.GetWarHero(war, true);
            var defenderHero = warService.GetWarHero(war, false);

            if (attackerHero != null)
                AttackerHero = new ShortBattleParticipantViewModel(attackerHero);
            if (defenderHero != null)
                DefenderHero = new ShortBattleParticipantViewModel(defenderHero);

            if (war.IsTrainingWar)
            {
                AttackerAllies = new List<ShortWarCountryInfoViewModel>();
                DefendedAllies = new List<ShortWarCountryInfoViewModel>();
       
                CanCancelSurrender = CanInitiateBattle = CanInitiateSurrender = false;
            }

            //testAddAllies(war);

            createMenu();

        }

        private void testAddAllies(Entities.War war)
        {
            for (int i = 0; i < 10; ++i)
                AttackerAllies.Add(new ShortWarCountryInfoViewModel(war.Attacker));
            for (int i = 0; i < 10; ++i)
                AttackerAllies.Add(new ShortWarCountryInfoViewModel(war.Defender));
        }

        private void createMenu()
        {
            if (CanInitiateBattle || CanInitiateSurrender)
            {
                Menu = new InfoMenuViewModel();

                if (CanInitiateBattle)
                {
                    Menu.AddItem(new InfoActionViewModel("StartBattle", "War", "Start Battle", "fa-fighter-jet",  new { warID = Info.WarID }));
                }

                if (CanInitiateSurrender)
                {
                    Menu.AddItem(new InfoActionViewModel("SurrenderWar", "War", "Surrender war", "fa-flag-o", FormMethod.Post, new { warID = Info.WarID }));
                }

                if (CanCancelSurrender)
                {
                    Menu.AddItem(new InfoActionViewModel("CancelSurrender", "War", "Surrender war", "fa-times", FormMethod.Post, new { warID = Info.WarID }));
                }


            }
        }
    }
}