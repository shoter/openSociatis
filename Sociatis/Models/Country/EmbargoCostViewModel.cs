using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Country
{
    public class EmbargoCostViewModel
    {
        public MoneyViewModel Cost { get; set; }
        public int EmbargoedCountryID { get; set; }
        public int IssueCountryID { get; set; }
        public bool HaveMoney { get; set; }

        public EmbargoCostViewModel(Entities.Country issuedCountry, Entities.Country embargoedCountry, double cost, bool haveMoney)
        {
            Cost = new MoneyViewModel(CurrencyTypeEnum.Gold, (decimal)cost);
            EmbargoedCountryID = embargoedCountry.ID;
            IssueCountryID = issuedCountry.ID;
            HaveMoney = haveMoney;
        }
    }
}