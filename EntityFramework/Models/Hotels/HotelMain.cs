using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Hotels
{
    public class HotelMain
    {
        public IEnumerable<HotelMainRoom> Rooms { get; set; }
        public string CurrencySymbol { get; set; }
    }
}
