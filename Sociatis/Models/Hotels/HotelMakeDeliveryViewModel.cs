using Entities;
using Entities.enums;
using Entities.Models.Hotels;
using Entities.Models.Market;
using Sociatis.Helpers;
using Sociatis.Models.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebServices;
using WebUtils;
using WebUtils.Mvc;

namespace Sociatis.Models.Hotels
{
    public class HotelMakeDeliveryViewModel
    {
        public HotelInfoViewModel Info { get; set; }
        public CountryMarketOffersListViewModel Market { get; set; }

        public CustomSelectList WalletIDs { get; set; } = new CustomSelectList();

        public HotelMakeDeliveryViewModel(HotelInfo info, Hotel hotel, HotelRights rights, IQueryable<MarketOfferModel> offers,
            PagingParam pp, int quality, int productID)
        {
            Info = new HotelInfoViewModel(info);
            WalletIDs.Add(SessionHelper.CurrentEntity.WalletID, "Your wallet");
            if (rights.CanUseWallet)
                WalletIDs.Add(hotel.Entity.WalletID, "Hotel's wallet");

            Market = new CountryMarketOffersListViewModel(hotel.Entity, hotel.Region.Country,  offers.ToList(),
                Persistent.Countries.GetAll().ToList(),
                new List<int>()
                {
                    (int)ProductTypeEnum.Fuel,
                    (int)ProductTypeEnum.ConstructionMaterials
                }, pp,
                quality,
                productID);
        }
    }
}
