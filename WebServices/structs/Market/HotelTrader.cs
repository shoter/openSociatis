using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Operations;
using Entities;
using Entities.enums;

namespace WebServices.structs.Market
{
    public class HotelTrader : Trader
    {
        private readonly Hotel hotel;
        public HotelTrader(Entity entity, IEquipmentService equipmentService) : base(entity, equipmentService)
        {
            hotel = entity.Hotel;
            RegionID = hotel.RegionID;
        }

        public override int? RegionID { get;  }

        public override TraderTypeEnum TraderType => TraderTypeEnum.Hotel;

        protected override MethodResult canBuy(MarketOffer offer, ITrader seller, ProductTypeEnum productType)
        {
            if (seller.TraderType == TraderTypeEnum.Shop)
                return new MethodResult("You cannot buy that!");

            return MethodResult.Success;
        }
    }
}
