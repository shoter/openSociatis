using Entities;
using SociatisCommon.Rights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;

namespace Sociatis.Models.Houses
{
    public class HouseSellViewModel
    {
        public HouseInfoViewModel Info { get; set; }
        public decimal? Price { get; set; }
        public string CurrencySymbol { get; set; }

        public HouseSellViewModel(House house, HouseRights rights)
        {
            Info = new HouseInfoViewModel(house, rights);

            var currency = Persistent.Countries.GetCountryCurrency(house.Region.CountryID.Value);
            CurrencySymbol = currency.Symbol;
        }
    }
}
