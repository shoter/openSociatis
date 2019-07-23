using Sociatis.Models.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities;

namespace Sociatis.Models.Country
{
    public class CountryWalletViewModel : WalletViewModel
    {
        public CountryInfoViewModel Info { get; set; }
        public CountryWalletViewModel(Entities.Country country, List<WalletMoney> money) : base(money)
        {
            Info = new CountryInfoViewModel(country);
        }
    }
}