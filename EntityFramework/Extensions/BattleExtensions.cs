using Common.utilities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class BattleExtensions
    {
        public static TimeSpan GetTimeLeft(this Battle battle, int currentDay)
        {
            return TimeHelper.CalculateTimeLeft(
                startDay: battle.StartDay,
                currentDay: currentDay,
                eventInDayLength: 1,
                startDateTime: battle.StartTime,
                currentDateTime: DateTime.Now);
        }

        public static BattleStatusEnum GetBattleStatus(this Battle battle)
        {
            if (battle.Active)
                return BattleStatusEnum.OnGoing;

            if (battle.WonByAttacker == true)
                return BattleStatusEnum.AttackerWin;
            return BattleStatusEnum.DefenderWin;
        }

        public static Country GetAttacker(this Battle battle)
        {
            if (battle.AttackerInitiatedBattle)
                return battle.War.Attacker;
            return battle.War.Defender;
        }

        public static Country GetDefender(this Battle battle)
        {
            if (battle.AttackerInitiatedBattle)
                return battle.War.Defender;
            return battle.War.Attacker;
        }
    }
}
