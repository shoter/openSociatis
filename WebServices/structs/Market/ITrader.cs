using Common.Operations;
using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Market
{
    public interface ITrader
    {
        int? RegionID { get; }
        TraderTypeEnum TraderType { get; }

        MethodResult CanBuy(MarketOffer offer, ITrader seller);

    }
}
