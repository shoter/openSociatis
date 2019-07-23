using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.MonetaryMarket
{
    public class MonetaryOfferCost
    {
        public double OfferCost { get; set; }
        public double TaxCost { get; set; }
        public double Sum { get { return OfferCost + TaxCost; } }
        public Currency Currency { get; set; }
    }
}
