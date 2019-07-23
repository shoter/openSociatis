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
using SociatisCommon.Errors.Trader;

namespace WebServices.structs.Market
{
    public class CitizenTrader : Trader
    {
        private Citizen citizen;
        public override int? RegionID => citizen.RegionID;
        public override TraderTypeEnum TraderType => TraderTypeEnum.Citizen;

        public CitizenTrader(Entity entity, IEquipmentService equipmentService) : base(entity, equipmentService)
        {
            citizen = entity.Citizen;
        }

        protected override MethodResult canBuy(MarketOffer offer, ITrader seller, ProductTypeEnum productType)
        {
            if (seller.TraderType != TraderTypeEnum.Shop && IsHouseOffer(offer, seller) == false)
                return new MethodResult(TraderErrors.YouCannotBuyThat);

            if (offer.CountryID.HasValue)
            {
                if (offer.CountryID != citizen.Region.CountryID)
                    return new MethodResult(TraderErrors.NotSelledInYourCountry);
            }
            else
            {
                if (seller.RegionID != citizen.RegionID)
                    return new MethodResult(TraderErrors.NotSelledInYourRegion);
            }



            return MethodResult.Success;
        }

        private static bool IsHouseOffer(MarketOffer offer, ITrader seller)
        {
            if (seller.TraderType == TraderTypeEnum.Company &&
                offer.ProductID == (int)ProductTypeEnum.House)
                return true;
            return false;
        }
    }
}
