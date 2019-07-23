using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums
{
    public enum BattleStatusEnum
    {
        OnGoing = 1,
        AttackerWin,
        DefenderWin,
        Started
    }

    public static class BattleStatusEnumExtensions
    {
        public static string ToHumanReadable(this BattleStatusEnum battleStatus)
        {
            switch(battleStatus)
            {
                case BattleStatusEnum.AttackerWin:
                    return "attacker won";
                case BattleStatusEnum.DefenderWin:
                    return "defender won";
                case BattleStatusEnum.OnGoing:
                    return "on going";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
