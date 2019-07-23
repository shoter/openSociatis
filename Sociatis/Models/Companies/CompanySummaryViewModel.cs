using Entities;
using Entities.enums;
using Entities.Extensions;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Companies
{
    public class CompanySummaryViewModel : BaseEntitySummaryViewModel
    {

        public ImageViewModel Avatar { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public double Queue { get; set; }
        public MoneyViewModel CountryMoney { get; set; }
        public MoneyViewModel AdminMoney { get; set; }
        

        public CompanySummaryViewModel(Company company)
            : base(SessionHelper.Session)
        {
            var entity = company.Entity;

            AdminMoney = new MoneyViewModel(entity.Wallet.GetMoney(CurrencyTypeEnum.Gold, Persistent.Currencies.GetAll()));
            CountryMoney = new MoneyViewModel(entity.Wallet.GetMoney(company.Region.Country.CurrencyID, Persistent.Currencies.GetAll()));
            Avatar = new ImageViewModel(entity.ImgUrl);
            ID = entity.EntityID;
            Name = entity.Name;
            Queue = (double)company.Queue;
            Stock = company.GetProducedProductItem().Amount;

        }
    }
}