using Entities.enums;
using System;
using Entities.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities.Repository;
using WebServices.Helpers;
using WebServices;

namespace Sociatis.Models.Country
{
    public class DeclareWarInformationsViewModel
    {
        public MoneyViewModel GoldNeeded { get; set; }
        public MoneyViewModel CountryGold { get; set; }
        public string CountryName { get; set; }
        public List<ShortCountryInfoViewModel> AttackerAllies { get; set; } = new List<ShortCountryInfoViewModel>();
        public List<ShortCountryInfoViewModel> DefenderAllies { get; set; } = new List<ShortCountryInfoViewModel>();

        public DeclareWarInformationsViewModel(Entities.Country declaringCountry, double goldNeeded, List<Entities.Country> attackerAllies, List<Entities.Country> defenderAllies)
        {
            GoldNeeded = new MoneyViewModel(CurrencyTypeEnum.Gold, (decimal)goldNeeded);
            foreach (var ally in attackerAllies)
                AttackerAllies.Add(new ShortCountryInfoViewModel(ally));
            foreach (var ally in defenderAllies)
                DefenderAllies.Add(new ShortCountryInfoViewModel(ally));

            CountryGold = new MoneyViewModel(declaringCountry.Entity.Wallet.GetMoney(CurrencyTypeEnum.Gold, Persistent.Currencies.GetAll()));
            CountryName = declaringCountry.Entity.Name;
        }
    }
}