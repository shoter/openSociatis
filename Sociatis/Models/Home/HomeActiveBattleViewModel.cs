using Sociatis.Helpers;
using Sociatis.Models.Battle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities;

namespace Sociatis.Models.Home
{
    public class HomeActiveBattleViewModel : SummaryBattleViewModel
    {
        public HomeActiveBattleViewModel(Entities.Battle battle) : base(battle)
        {

        }
    }
}