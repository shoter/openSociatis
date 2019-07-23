using Sociatis.Models.Gifts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities;

namespace Sociatis.Models.Trades
{
    public class MoneyTradeViewModel : MoneyGiftViewModel
    {
        public MoneyTradeViewModel(WalletMoney money) : base(money)
        {
        }
    }
}