using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class MarketOfferExtensions
    {
        public static ProductTypeEnum GetProductType(this MarketOffer offer)
        {
            return (ProductTypeEnum)offer.ProductID;
        }
    }
}
