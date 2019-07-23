using Entities.Models.Hotels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Hotels
{
    public class HotelRoomMainViewModel
    {
        public decimal? Price { get; set; }
        public int Quality { get; set; }

        public bool CanUse => Price.HasValue;

        public HotelRoomMainViewModel(HotelMainRoom room)
        {
            Price = room.Price;
            Quality = room.Quality;
        }

    }
}