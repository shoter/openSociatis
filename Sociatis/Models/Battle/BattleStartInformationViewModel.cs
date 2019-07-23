using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Battle
{
    public class BattleStartInformationViewModel
    {
        public MoneyViewModel GoldNeeded { get; set; }

        public BattleStartInformationViewModel(double goldNeeded)
        {
            GoldNeeded = new MoneyViewModel(CurrencyTypeEnum.Gold, (decimal)goldNeeded);
        }
    }
}