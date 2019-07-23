using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Wallet
{
    public class WalletViewModel
    {
        public List<MoneyViewModel> Money { get; set; } = new List<MoneyViewModel>();
        public WalletViewModel(List<WalletMoney> money)
        {
            foreach(var singleMoney in money)
            {
                Money.Add(new MoneyViewModel(singleMoney));
            }
        }
    }
}