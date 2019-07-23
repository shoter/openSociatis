using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.enums;
using Entities.Extensions;
using Sociatis.Helpers;
using WebServices;

namespace Sociatis.Models.Hotels
{
    public class HotelSummaryViewModel : BaseEntitySummaryViewModel
    {
        public ImageViewModel Avatar { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public MoneyViewModel CountryMoney { get; set; }
        public MoneyViewModel AdminMoney { get; set; }
        public HotelSummaryViewModel(Hotel hotel) : base(SessionHelper.Session)
        {
            var entity = hotel.Entity;

            var countryCurrency = Persistent.Countries.GetCountryCurrency(hotel.Region.CountryID.Value);

            var money = entity.Wallet.GetMoney(countryCurrency.ID, Persistent.Currencies.GetAll());
            var adminMoney = entity.Wallet.GetMoney(CurrencyTypeEnum.Gold, Persistent.Currencies.GetAll());

            CountryMoney = new MoneyViewModel(money);
            AdminMoney = new MoneyViewModel(adminMoney);
            Avatar = new ImageViewModel(entity.ImgUrl);
            ID = entity.EntityID;
            Name = entity.Name;
        }
    }
}
