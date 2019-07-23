using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums
{
    public enum MonetaryOfferTypeEnum
    {
        Buy = 1,
        Sell = 2
    }

    public static class MonetaryOfferTypeEnumExtension
    {
        public static string ToHumanReadable(this MonetaryOfferTypeEnum type)
        {
            switch(type)
            {
                case MonetaryOfferTypeEnum.Buy:
                    return "buy";
                case MonetaryOfferTypeEnum.Sell:
                    return "sell";
            }

            return type.ToString();
        }
    }
}
