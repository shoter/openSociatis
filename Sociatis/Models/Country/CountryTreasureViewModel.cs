using Entities;
using Sociatis.Models.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Country
{
    public class CountryTreasureViewModel : WalletViewModel
    {
        public CountryInfoViewModel Info { get; set; }
        public CountryTreasureViewModel(Entities.Country country, List<WalletMoney> money) : base(money)
        {
            Info = new CountryInfoViewModel(country);
        }
    }
}