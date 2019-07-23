using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.structs.Battles;

namespace WebServices
{
    public interface IBattleService
    {
        /// <param name="attackerSide">who is attacking (creating this battle)</param>
        /// <returns></returns>
        Battle CreateBattle(War war, int regionID, WarSideEnum attackerSide);
        double CalculateDamage(Citizen citizen, Battle battle, int weaponQuality, bool isAttacker);
        void ParticipateInBattle(Citizen citizen, Battle battle, bool isAttacker, int weaponQuality);
        void ProcessDayChange(int newDay);
        /// <summary>
        /// Can return null
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="isAttacker"></param>
        /// <returns></returns>
        BattleHero GetBattleHero(Battle battle, bool isAttacker);

        List<int> GetUsableQualitiesOfWeapons(Citizen citizen);

        double CalculateHealthModifier(double hitPoints);
        double CalculateMilitaryRankModifier(double militaryRank);

        double CalculateStrengthModifier(double str, int weaponQuality);
        double CalculateDistanceModifier(Citizen citizen, Battle battle, bool isAttacker);
        double CalculateDevelopmentModifier(Battle battle, bool isAttacker);

        double CalculateOverallModifier(Citizen citizen, Battle battle, int weaponQuality, bool isAttacker);
        double CalculateWeaponBonus(int weaponQuality);
        void EndBattle(Battle battle);

    }
}
