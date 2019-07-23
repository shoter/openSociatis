using Common.Operations;
using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.structs;
using WebServices.structs.Battles;

namespace WebServices
{
    public interface IWarService
    {
        /// <summary>
        /// Declares new war
        /// </summary>
        /// <returns>returns war ID</returns>
        MethodResult<int> DeclareWar(Country attacker, Country defender);
        MethodResult CanDeclareWar(Country attacker, Country defender);
        double GetNeededGoldToStartWar(Country attacker, Country defender);
        void EndWar(War war, bool informatAboutWarEnd = true);
        void SurrenderWar(War war, Entity entity);

        MethodResult CanSurrenderWar(War war, Entity entity);

        WarSideEnum GetWarSide(War war, Entity entity);
        WarSideEnum GetFightingSide(War war, Citizen citizen);
        bool CanFightAs(Battle battle, War war, Citizen citizen, WarSideEnum warSide);

        void ProcessDayChange(int newDay);

        double GetGoldNeededToStartBattle(War war, Region region);

        BattleHero GetWarHero(War war, bool isAttacker);

        MethodResult CanStartBattle(Entity entity, Country attackingCountry, War war, Region region);
        MethodResult CanStartBattle(Entity entity, Country attackingCountry, War war);

        /// <summary>
        /// Returns country in the war for which you can make military operations
        /// </summary>
        Country GetControlledCountryInWar(Entity entity, War war);

        void SendMessageToEveryoneInWar(War war, string message, params int[] excluding);

        MethodResult CanCancelSurrender(War war, Entity entity);
        void CancelSurrender(War war);

        Money GetNeededMoneyToStartRessistanceWar(Region region);
        War GetActiveRessistanceWar(Country country, int defenderCountryID);
        Money GetMoneyNeededToStartResistanceBattle(Region region);
        MethodResult CanStartRessistanceBattle(Citizen citizen, Region region);

        Battle StartRessistanceBattle(Citizen startingCitizen, Region defendingRegion, IBattleService battleService);
        MethodResult CanStartRessistanceBattle(Entity entity, Region region);

        bool AreAtWar(Country firstCountry, Country secondCountry);

        void TryToCreateTrainingWar();
    }
}
