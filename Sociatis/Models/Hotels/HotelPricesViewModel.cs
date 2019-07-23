using Entities;
using Entities.Models.Hotels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis.Models.Hotels
{
    public class HotelPricesViewModel
    {
        public HotelInfoViewModel Info { get; set; }
        public decimal? PriceQ1 { get; set; }
        public decimal? PriceQ2 { get; set; }
        public decimal? PriceQ3 { get; set; }
        public decimal? PriceQ4 { get; set; }
        public decimal? PriceQ5 { get; set; }

        public HotelPricesViewModel(HotelPrice price, HotelInfo info)
        {
            Info = new HotelInfoViewModel(info);
            initPrices(price);
        }

        private void initPrices(HotelPrice price)
        {
            PriceQ1 = price.PriceQ1;
            PriceQ2 = price.PriceQ2;
            PriceQ3 = price.PriceQ3;
            PriceQ4 = price.PriceQ4;
            PriceQ5 = price.PriceQ5;
        }
    }
}
