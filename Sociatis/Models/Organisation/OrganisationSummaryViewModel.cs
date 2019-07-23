using Entities.enums;
using Entities.Extensions;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Organisation
{
    public class OrganisationSummaryViewModel : BaseEntitySummaryViewModel
    {
        public int OrganisationID { get; set; }
        public ImageViewModel Avatar { get; set; }
        public string Name { get; set; }
        public MoneyViewModel CountryMoney { get; set; }
        public MoneyViewModel AdminMoney { get; set; }

        public OrganisationSummaryViewModel(Entities.Organisation organisation) : base(SessionHelper.Session)
        {
            OrganisationID = organisation.ID;
            var entity = organisation.Entity;
            Avatar = new ImageViewModel(entity.ImgUrl);
            Name = entity.Name;


            AdminMoney = new MoneyViewModel(entity.Wallet.GetMoney(CurrencyTypeEnum.Gold, Persistent.Currencies.GetAll()));
            CountryMoney = new MoneyViewModel(entity.Wallet.GetMoney(organisation.Country.CurrencyID, Persistent.Currencies.GetAll()));
        }
    }
}