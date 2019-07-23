using Common.utilities;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using Sociatis.Helpers;
using Sociatis.Models.War;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;
using WebServices.Helpers;

namespace Sociatis.Models.Battle
{
    public class BattleViewModel
    {
        public int BattleID { get; set; }
        public WarInfoViewModel Info { get; set; }
        public double WallHealth { get; set; }
        public bool CanFightAsAttacker { get; set; }
        public bool CanFighstAsDefender { get; set; }
        public string RegionName { get; set; }
        public string TimeLeft { get; set; }
        public bool WaitingForResolve { get; set; }
        public bool CanFight { get; set; }
        public bool IsActive { get; set; }
        public bool? AttackerWon { get; set; } = null;
        public double? GoldTaken { get; set; }
        public int StartDay { get; set; }
        public bool AttackerInitiated { get; set; }
        public List<ShortBattleParticipantViewModel> AttackerBattleParticipants { get; set; } = new List<ShortBattleParticipantViewModel>();
        public List<ShortBattleParticipantViewModel> DefenderBattleParticipants { get; set; } = new List<ShortBattleParticipantViewModel>();

        public ShortBattleParticipantViewModel AttackerHero { get; set; }
        public ShortBattleParticipantViewModel DefenderHero { get; set; }

        public string AttackerName { get; set; }
        public string DefenderName { get; set; }

        public List<int> AvailableWeaponQualities { get; set; } = new List<int>();

        public BattleViewModel(Entities.Battle battle, IBattleRepository battleRepository, IBattleService battleService, IWarRepository warRepository, IWarService warService)
        {
            Info = new WarInfoViewModel(battle.War, warRepository, warService);

            initBasic(battle);
            initCanFight(battle, warService);
            initTime(battle);

            IsActive = battle.Active;
            CanFight = (CanFighstAsDefender || CanFightAsAttacker) && IsActive && WaitingForResolve == false;
            if (IsActive == false)
            {
                AttackerWon = battle.WonByAttacker;
                GoldTaken = (double?)battle.GoldTaken;
            }
            addRealLastParticipants(battle);
            addDummiesIfNeeded();
            AttackerInitiated = battle.AttackerInitiatedBattle;

            var attackerHero = battleService.GetBattleHero(battle, true);
            var defenderHero = battleService.GetBattleHero(battle, false);

            if(attackerHero != null)
            {
                AttackerHero = new ShortBattleParticipantViewModel(attackerHero);
            }
            if(defenderHero != null)
            {
                DefenderHero = new ShortBattleParticipantViewModel(defenderHero);
            }

            if (battle.War.IsTrainingWar)
            {
                CanFighstAsDefender = true;
                CanFight = SessionHelper.CurrentEntity.Is(EntityTypeEnum.Citizen);
                CanFightAsAttacker = true;
                RegionName = "Edge of the Earth";
                TimeLeft = "";
                WaitingForResolve = false;


                
            }
            if (CanFight)
            {
                AvailableWeaponQualities = battleService.GetUsableQualitiesOfWeapons(SessionHelper.LoggedCitizen);
            }

            AttackerName = Info.Info.Attacker.Name;
            DefenderName = Info.Info.Defender.Name;

            if (battle.AttackerInitiatedBattle == false)
            {
                AttackerName = Info.Info.Defender.Name;
                DefenderName = Info.Info.Attacker.Name;
            }

        }

        private void initBasic(Entities.Battle battle)
        {
            WallHealth = (double)Math.Round(battle.WallHealth, 1);
            BattleID = battle.ID;
            RegionName = battle.Region.Name;
            StartDay = battle.StartDay;
        }

        private void initCanFight(Entities.Battle battle, IWarService warService)
        {
            bool canFight = SessionHelper.CurrentEntity.GetEntityType() == Entities.enums.EntityTypeEnum.Citizen && SessionHelper.CurrentEntity.Citizen.HitPoints > 5;
            if (canFight)
            {
                var fightingSide = warService.GetFightingSide(battle.War, SessionHelper.CurrentEntity.Citizen);

                if (fightingSide == Entities.enums.WarSideEnum.Attacker)
                    CanFightAsAttacker = true;
                else if (fightingSide == Entities.enums.WarSideEnum.Defender)
                    CanFighstAsDefender = true;
                else
                {
                    if (battle.RegionID == SessionHelper.CurrentEntity.GetCurrentRegion().ID)
                        CanFighstAsDefender = CanFightAsAttacker = true;
                }

                if (!battle.AttackerInitiatedBattle)
                {
                    var att = CanFightAsAttacker;
                    CanFightAsAttacker = CanFighstAsDefender;
                    CanFighstAsDefender = att;
                }
            }
        }

        private void initTime(Entities.Battle battle)
        {
            var timeLeft = TimeHelper.CalculateTimeLeft(battle.StartDay, GameHelper.CurrentDay, 1, battle.StartTime);

            if (timeLeft.TotalSeconds > 0)
            {
                TimeLeft = string.Format("{0:00}:{1:00}:{2:00}", Math.Floor(timeLeft.TotalHours), timeLeft.Minutes, timeLeft.Seconds);
                WaitingForResolve = false;
            }
            else
            {
                TimeLeft = "00:00:00";
                WaitingForResolve = true;
            }
        }

        private void addRealLastParticipants(Entities.Battle battle)
        {
            var lastAttackers = battle.BattleParticipants.Where(p => p.IsAttacker).OrderByDescending(p => p.ID).Take(5).ToList();
            var lastDefenders = battle.BattleParticipants.Where(p => p.IsAttacker == false).OrderByDescending(p => p.ID).Take(5).ToList();

            foreach (var attacker in lastAttackers)
                AttackerBattleParticipants.Add(new ShortBattleParticipantViewModel(attacker));

            foreach (var defender in lastDefenders)
                DefenderBattleParticipants.Add(new ShortBattleParticipantViewModel(defender));

           // AttackerBattleParticipants.Reverse();
            //DefenderBattleParticipants.Reverse();
        }

        private void addDummiesIfNeeded()
        {
            while (AttackerBattleParticipants.Count < 5)
                AttackerBattleParticipants.Add(new ShortEmptyBattleParticipantViewModel());

            while (DefenderBattleParticipants.Count < 5)
                DefenderBattleParticipants.Add(new ShortEmptyBattleParticipantViewModel());
        }
    }
}