using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.structs.MonetaryMarket
{
    public class ActualBuySellOffers
    {
        public double BuyPrice { get; set; }
        public int BuyVolume { get; set; }

        public double SellPrice { get; set; }
        public int SellVolume { get; set; }
    }
}
