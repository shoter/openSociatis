using Entities.enums;
using Entities.Extensions;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices.Helpers;

namespace Sociatis.Models.Battle
{
    public class SummaryBattleViewModel
    {
        public ImageViewModel AttackerImage { get; set; }
        public string AttackerName { get; set; }

        public ImageViewModel DefenderImage { get; set; }
        public string DefenderName { get; set; }

        public BattleStatusEnum BattleStatus { get; set; }
        public string BattleStatusText { get; set; }

        public string RegionName { get; set; }
        public int WallHealth { get; set; }
        public int BattleID { get; set; }
        public string TimeLeft { get; set; } = "-";
        public bool CanFight { get; set; } = true;

        public SummaryBattleViewModel(Entities.Battle battle)
        {
            AttackerName = battle.War.Attacker.Entity.Name;
            DefenderName = battle.War.Defender.Entity.Name;

            AttackerImage = Images.GetCountryFlag(AttackerName).VM;
            DefenderImage = Images.GetCountryFlag(DefenderName).VM;

            WallHealth = (int)battle.WallHealth;
            RegionName = battle.Region.Name;
            BattleID = battle.ID;

            BattleStatus = battle.GetBattleStatus();
            if (BattleStatus != BattleStatusEnum.OnGoing)
                CanFight = false;
            BattleStatusText = BattleStatus.ToHumanReadable();

            var timeLeft = battle.GetTimeLeft(GameHelper.CurrentDay);

            if (timeLeft.TotalSeconds > 0)
                TimeLeft = string.Format("{0:00}:{1:00}:{2:00}",  Math.Floor(timeLeft.TotalHours), timeLeft.Minutes, timeLeft.Seconds);
            else
                CanFight = false;

            if (battle.War.IsTrainingWar)
            {
                CanFight = true;
                AttackerImage = Images.Placeholder.VM;
                DefenderImage = AttackerImage;
                RegionName = "Edge of the Earth";
                AttackerName = "Chuck Norris";
                DefenderName = "Bruce Lee";

            }
        }
    }
}