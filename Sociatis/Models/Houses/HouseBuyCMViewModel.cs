using Entities;
using Entities.enums;
using Entities.Models.Market;
using Sociatis.Helpers;
using Sociatis.Models.Market;
using SociatisCommon.Rights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;
using WebUtils;

namespace Sociatis.Models.Houses
{
    public class HouseBuyCMViewModel
    {
        public HouseInfoViewModel Info { get; set; }
        public CountryMarketOffersListViewModel Market { get; set; }

        public HouseBuyCMViewModel(House house, PagingParam pp, IQueryable<MarketOfferModel> offers, HouseRights houseRights)
        {
            Info = new HouseInfoViewModel(house, houseRights);

            Market = new CountryMarketOffersListViewModel(
                SessionHelper.CurrentEntity,
                SessionHelper.LoggedCitizen.Region.Country,
                offers.ToList(),
                Persistent.Countries.GetAll().ToList(),
                new List<int>()
                {
                    (int)ProductTypeEnum.ConstructionMaterials,
                    (int)ProductTypeEnum.UpgradePoints
                },
                pp,
                1,
                (int)ProductTypeEnum.ConstructionMaterials);
        }
    }
}
