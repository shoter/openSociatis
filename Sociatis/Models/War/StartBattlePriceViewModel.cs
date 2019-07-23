using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.War
{
    public class StartBattlePriceViewModel
    {
        public MoneyViewModel Price { get; set; }
        
        public StartBattlePriceViewModel(MoneyViewModel price)
        {
            Price = price;
        }
    }
}