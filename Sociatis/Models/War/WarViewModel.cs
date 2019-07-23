using Entities.Repository;
using Sociatis.Helpers;
using Sociatis.Models.Battle;
using Sociatis.Models.Country;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.War
{
    public class WarViewModel
    {
        public WarInfoViewModel Info { get; set; }
        public List<SummaryBattleViewModel> ActiveBattles { get; set; } = new List<SummaryBattleViewModel>();

        public WarViewModel(Entities.War war, IWarService warService, IWarRepository warRepository)
        {
            Info = new WarInfoViewModel(war, warRepository, warService);

            foreach(var battle in war.Battles.OrderByDescending(b => b.Active).ThenByDescending(b => b.ID).ToList())
            {
                ActiveBattles.Add(new SummaryBattleViewModel(battle));
            }
        }
    }
}