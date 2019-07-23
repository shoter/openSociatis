using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices.structs.Battles;

namespace Sociatis.Models.Battle
{
    public class ShortBattleParticipantViewModel
    {
        public double Damage { get; set; }
        public string CitizenName { get; set; }
        public int CitizenID { get; set; }
        public ImageViewModel Avatar { get; set; }
        protected ShortBattleParticipantViewModel() { }

        public ShortBattleParticipantViewModel(BattleHero hero)
        {
            Damage = hero.DamageDealt;
            Avatar = new ImageViewModel(hero.Citizen.Entity.ImgUrl);
            CitizenName = hero.Citizen.Entity.Name;
            CitizenID = hero.Citizen.ID;
        }

        public ShortBattleParticipantViewModel(BattleParticipant participant)
        {
            Damage = (double)participant.DamageDealt;
            Avatar = new ImageViewModel(participant.Citizen.Entity.ImgUrl);
            CitizenName = participant.Citizen.Entity.Name;
            CitizenID = participant.CitizenID;
        }
    }
}