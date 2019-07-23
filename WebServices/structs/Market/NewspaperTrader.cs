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
    public class NewspaperTrader : Trader
    {
        private readonly Newspaper newspaper;
        public NewspaperTrader(Entity entity, IEquipmentService equipmentService) : base(entity, equipmentService)
        {
            newspaper = entity.Newspaper;
        }

        public override int? RegionID => null;

        public override TraderTypeEnum TraderType => TraderTypeEnum.Newspaper;

        protected override MethodResult canBuy(MarketOffer offer, ITrader seller, ProductTypeEnum productType)
        {
            if (seller.TraderType == TraderTypeEnum.Shop)
                return new MethodResult("You cannot buy that!");

            return MethodResult.Success;
        }
    }
}
