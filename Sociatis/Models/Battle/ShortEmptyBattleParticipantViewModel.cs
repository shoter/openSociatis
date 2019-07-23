using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Battle
{
    public class ShortEmptyBattleParticipantViewModel : ShortBattleParticipantViewModel
    {
        public ShortEmptyBattleParticipantViewModel()
        {
            Avatar = Images.Placeholder.VM;
        }
    }
}