using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Operations;
using Entities;
using Entities.enums;
using Entities.enums.Attributes;
using Entities.Extensions;
using SociatisCommon.Errors.Trader;

namespace WebServices.structs.Market
{
    public abstract class Trader : ITrader
    {
        public abstract int? RegionID { get; }
        public abstract TraderTypeEnum TraderType { get; }
        public EntityTypeEnum EntityType { get; set; }
        public string TraderError { get; private set; }

        protected readonly IEquipmentService equipmentService;

        public Trader(Entity entity, IEquipmentService equipmentService)
        {
            EntityType = entity.GetEntityType();
            this.equipmentService = equipmentService;
        }

        public virtual MethodResult CanBuy(MarketOffer offer, ITrader seller)
        {
            var productType = (ProductTypeEnum)offer.ProductID;

            var whoCanBuy = productType.GetEnumAttribute<WhoCanBuyAttribute>();
            if (whoCanBuy.Contains(TraderType) == false)
                return new MethodResult(TraderErrors.YouCannotBuyThat);

            if (equipmentService.GetAllowedProductsForEntity(EntityType).Contains(productType) == false)
                return new MethodResult(TraderErrors.YouCannotHaveThat);
        
            return canBuy(offer, seller, productType);
        }

        protected abstract MethodResult canBuy(MarketOffer offer, ITrader seller, ProductTypeEnum productType);

    }
}
