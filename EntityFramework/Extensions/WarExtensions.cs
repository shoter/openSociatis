using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class WarExtensions
    {
        /// <summary>
        /// Depending on isAttacker it returns either attacker or defender from War. (Not countries in wars!)
        /// </summary>
        public static Country GetMainCountry(this War war, bool isAttacker)
        {
            return isAttacker ? war.Attacker : war.Defender;
        }

        public static Country GetMainCountry(this War war, WarSideEnum warSide)
        {
            if (warSide == WarSideEnum.None)
                return null;

            return GetMainCountry(war, warSide == WarSideEnum.Attacker);
        }

        /// <summary>
        /// Can return null if war have no battles.
        /// </summary>
        public static Battle GetLastBattle(this War war)
        {
            if (war.Battles.Any() == false)
                return null;

            return war.Battles
                .OrderByDescending(b => b.StartTime)
                .First();
        }
    }
}
