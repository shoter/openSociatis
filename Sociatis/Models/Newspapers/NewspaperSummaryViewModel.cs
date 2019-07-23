using Entities;
using Entities.Extensions;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Newspapers
{
    public class NewspaperSummaryViewModel : BaseEntitySummaryViewModel
    {
        public ImageViewModel Avatar { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public MoneyViewModel CountryMoney { get; set; }
        public NewspaperSummaryViewModel(Newspaper newspaper)
            : base(SessionHelper.Session)
        {
            var entity = newspaper.Entity;

            CountryMoney = new MoneyViewModel(entity.Wallet.GetMoney(newspaper.Country.CurrencyID, Persistent.Currencies.GetAll()));
            Avatar = new ImageViewModel(entity.ImgUrl);
            ID = entity.EntityID;
            Name = entity.Name;

        }
    }
}