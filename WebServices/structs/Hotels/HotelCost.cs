using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Hotels
{
    public class HotelCost
    {
        public decimal BasePrice { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalCost => BasePrice * (1m + Tax);
        public Currency Currency { get; set; }
    }
}
