using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Operations;
using Entities;
using Entities.enums;
using Entities.Extensions;

namespace WebServices.structs.Market
{
    public class CompanyTrader : Trader
    {
        private Company company;
        public CompanyTrader(Entity entity, IEquipmentService equipmentService) : base(entity, equipmentService)
        {
            company = entity.Company;
            traderType = getTraderType(company);
        }

        private TraderTypeEnum getTraderType(Company company)
        {
            switch (company.GetCompanyType())
            {
                case CompanyTypeEnum.Shop:
                    return TraderTypeEnum.Shop;
            }
            return TraderTypeEnum.Company;
        }

        public override int? RegionID => company.RegionID;
        private TraderTypeEnum traderType;
        public override TraderTypeEnum TraderType => traderType;

        protected override MethodResult canBuy(MarketOffer offer, ITrader seller, ProductTypeEnum productType)
        {
            if (seller.TraderType == TraderTypeEnum.Shop)
                return new MethodResult("You cannot buy that!");

            if (equipmentService.GetAllowedProductsForCompany(company).Contains(productType) == false)
                return new MethodResult("You cannot have that item!");

            return MethodResult.Success;
        }
    }
}
