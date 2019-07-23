using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class HotelExtensions
    {
        public static decimal? GetRoomPrice(this HotelPrice price, int quality)
        {
            switch (quality)
            {
                case 1:
                    return price.PriceQ1;
                case 2:
                    return price.PriceQ2;
                case 3:
                    return price.PriceQ3;
                case 4:
                    return price.PriceQ4;
                case 5:
                    return price.PriceQ5;
            }
            throw new NotImplementedException();
        }
    }
}
