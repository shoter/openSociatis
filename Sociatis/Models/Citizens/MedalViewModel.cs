using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Citizens
{
    public class MedalViewModel
    {
        public int HardWorkerCount { get; set; }
        public int SuperSoldierCount { get; set; }
        public int SuperJournalistCount { get; set; }
        public int CongressCount { get; set; }
        public int PresidentCount { get; set; }
        public int BattleHeroCount { get; set; }
        public int WarHeroCount { get; set; }
        public int RessistanceHeroCount { get; set; }
        public MedalViewModel(Citizen citizen)
        {
            HardWorkerCount = citizen.HardWorkerMedals;
            SuperSoldierCount = citizen.SuperSoldierMedals;
            SuperJournalistCount = citizen.SuperJournalistMedals;
            CongressCount = citizen.CongressMedals;
            PresidentCount = citizen.PresidentMedals;
            BattleHeroCount = citizen.BattleHeroMedals;
            WarHeroCount = citizen.WarHeroMedals;
            RessistanceHeroCount = citizen.RessistanceHeroMedals;
        }
    }
}