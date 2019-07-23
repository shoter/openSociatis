using Entities.enums;
using Entities.Extensions;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Country
{
    public class CountrySummaryViewModel : BaseEntitySummaryViewModel
    {
        public int CountryID { get; set; }
        public ImageViewModel Avatar { get; set; }
        public string CountryName { get; set; }
        public MoneyViewModel CountryMoney { get; set; }
        public MoneyViewModel AdminMoney { get; set; }
        public List<MoneyViewModel> ForeignMoney { get; set; } = new List<MoneyViewModel>(); 

        public CountrySummaryViewModel(Entities.Country country) : base(SessionHelper.Session)
        {
            CountryID = country.ID;
            var entity = country.Entity;
            CountryName = entity.Name;
            Avatar = Images.GetCountryFlag(CountryName).VM;

            AdminMoney = new MoneyViewModel(entity.Wallet.GetMoney(CurrencyTypeEnum.Gold, Persistent.Currencies.GetAll()));
            CountryMoney = new MoneyViewModel(entity.Wallet.GetMoney(country.CurrencyID, Persistent.Currencies.GetAll()));

            var foreignMoney = entity.Wallet.WalletMoneys
                .Where(wm => wm.CurrencyID != country.CurrencyID && wm.CurrencyID != (int)CurrencyTypeEnum.Gold && wm.Amount > 0)
                .OrderByDescending(wm => wm.Amount)
                .Take(3);

            foreach(var money in foreignMoney)
            {
                ForeignMoney.Add(new MoneyViewModel(Persistent.Currencies.First(c => c.ID == money.CurrencyID), money.Amount));
            }
                


        }
    }
}