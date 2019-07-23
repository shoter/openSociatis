using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.structs;

namespace WebServices
{
    public interface ICitizenService
    {
        Citizen CreateCitizen(RegisterInfo info);
        double GetSkillIncreaseForWork(int citizenID, WorkTypeEnum amoun);
        void IncreaseSkill(int citizenID, WorkTypeEnum workType, double amount);
        void ReceiveBattleHeroMedal(Citizen citizen);
        void ReceiveWarHeroMedal(Citizen citizen);
        void ReceiveCongressMedal(Citizen citizen, double gold);
        void ReceivePresidentMedal(Citizen citizen, double gold);

        void ReceiveRessistanceHeroMedal(Citizen citizen, War war, int battleWonCount, double goldAmount);
        /// <summary>
        /// Returns amount of gold that was earned when received medal
        /// </summary>
        /// <param name="citizen"></param>
        /// <returns></returns>
        double ReceiveHardWorker(Citizen citizen);
        /// <summary>
        /// Make strength training for citizen
        /// </summary>
        /// <returns>How much strength did citizen gained through training</returns>
        double Train(Citizen citizen);


        /// <summary>
        /// Decides wheter to give medal to the citizen on sold article. +1 to sold articles for citizen
        /// </summary>
        /// <param name="citizen"></param>
        void OnSoldArticle(Citizen citizen);

        int CalculateExperienceForLevel(int level);
        int CalculateExperienceForNextLevel(int level);
        int CalculateExperienceForNextLevel(Citizen citizen);

        void GrantExperience(Citizen citizen, int amount);

        /// <param name="newPassword">in plain text. Will be translated to hash</param>
        void SetPassword(Citizen citizen, string newPassword);
        int GetActivePlayerCount();
    }
}
