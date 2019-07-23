using Entities.enums;
using Entities.Extensions;
using Entities.Models.Hotels;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Hotels
{
    public class HotelViewModel
    {
        public HotelInfoViewModel Info { get; set; }
        public bool CanRentAnything { get; set; }
        public bool IsCitizen { get; set; }
        public List<HotelRoomMainViewModel> Rooms { get; set; }
        public string CurrencySymbol { get; set; }

        public HotelViewModel(HotelInfo info, HotelMain main)
        {
            IsCitizen = SessionHelper.CurrentEntity.Is(EntityTypeEnum.Citizen);
            Info = new HotelInfoViewModel(info);
            CurrencySymbol = main.CurrencySymbol;

            Rooms = main.Rooms.Select(r => new HotelRoomMainViewModel(r))
                .OrderBy(r => r.Quality).ToList();
        }
    }
}