using Common;
using Entities.enums.Attributes;
using SociatisCommon.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums
{
    public enum ProductTypeEnum
    {
        [QualityProduct]
        [WhoCanBuy(TraderTypeEnum.Citizen, TraderTypeEnum.Company, TraderTypeEnum.Shop)]
        Bread = 1,
        [WhoCanBuy(TraderTypeEnum.Citizen, TraderTypeEnum.Company, TraderTypeEnum.Shop)]
        [QualityProduct]
        Weapon = 2,
        [RawProduct]
        [WhoCanBuy(TraderTypeEnum.Company)]
        Grain = 3,
        [RawProduct]
        [WhoCanBuy(TraderTypeEnum.Company)]
        Iron = 4,
        [WhoCanBuy(TraderTypeEnum.Citizen, TraderTypeEnum.Company, TraderTypeEnum.Shop)]
        [QualityProduct]
        Tea = 5,
        [WhoCanBuy(TraderTypeEnum.Company)]
        [RawProduct]
        Oil = 6,
        [WhoCanBuy(TraderTypeEnum.Company, TraderTypeEnum.Hotel, TraderTypeEnum.Newspaper, TraderTypeEnum.Shop)]
        Fuel = 7,
        [WhoCanBuy(TraderTypeEnum.Citizen, TraderTypeEnum.Company, TraderTypeEnum.Shop)]
        [QualityProduct]
        MovingTicket = 8,
        [RawProduct]
        [WhoCanBuy(TraderTypeEnum.Company)]
        TeaLeaf = 10,
        [RawProduct]
        [WhoCanBuy(TraderTypeEnum.Company)]
        Wood = 11,
        [WhoCanBuy(TraderTypeEnum.Company)]
        UpgradePoints = 12,
        [QualityProduct]
        [WhoCanBuy(TraderTypeEnum.Company)]
        MedicalSupplies = 13,
        [WhoCanBuy]
        SellingPower = 14,
        [WhoCanBuy(TraderTypeEnum.Company, TraderTypeEnum.Newspaper)]
        Paper = 15,
        [WhoCanBuy]
        Development = 16,
        [WhoCanBuy(TraderTypeEnum.Company, TraderTypeEnum.Hotel)]
        ConstructionMaterials = 17,
        [QualityProduct]
        [WhoCanBuy]
        Hospital = 18,
        [QualityProduct]
        [Construction(OneTime = true)]
        [WhoCanBuy]
        DefenseSystem = 19,
        [Construction(OneTime = false)]
        [WhoCanBuy(TraderTypeEnum.Citizen)]
        House = 20,
        [WhoCanBuy]
        [Construction(OneTime = true)]
        Hotel = 21
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ProductTypeEnumExtensions
    {
        public static string ToHumanReadable(this ProductTypeEnum productType)
        {
            switch (productType)
            {
                case ProductTypeEnum.Bread:
                    return "bread";
                case ProductTypeEnum.Weapon:
                    return "weapon";
                case ProductTypeEnum.Grain:
                    return "grain";
                case ProductTypeEnum.Iron:
                    return "iron";
                case ProductTypeEnum.Tea:
                    return "tea";
                case ProductTypeEnum.Oil:
                    return "oil";
                case ProductTypeEnum.Fuel:
                    return "fuel";
                case ProductTypeEnum.MovingTicket:
                    return "moving ticket";
                case ProductTypeEnum.TeaLeaf:
                    return "tea leaf";
                case ProductTypeEnum.Wood:
                    return "wood";
                case ProductTypeEnum.UpgradePoints:
                    return "upgrade points";
                case ProductTypeEnum.MedicalSupplies:
                    return "medical supplies";
                case ProductTypeEnum.SellingPower:
                    return "selling power";
                case ProductTypeEnum.Paper:
                    return "paper";
                case ProductTypeEnum.Development:
                    return "development";
                case ProductTypeEnum.ConstructionMaterials:
                    return "construction materials";
                case ProductTypeEnum.Hospital:
                    return "hospital";
                case ProductTypeEnum.House:
                    return "house";
                case ProductTypeEnum.DefenseSystem:
                    return "defense system";
                case ProductTypeEnum.Hotel:
                    return "hotel";
            }
            throw new Exception("Switch Enum Analyzer prevents going to this place");
        }

        public static int ToInt(this ProductTypeEnum productType)
        {
            return (int)productType;
        }

        public static bool IsDefined(this ProductTypeEnum productType)
        {
            return Enum.IsDefined(typeof(ProductTypeEnum), productType);
        }

        public static bool IsRaw(this ProductTypeEnum productType)
        {
            var type = typeof(ProductTypeEnum);
            var memberInfo = type.GetMember(productType.ToString()).First();

            return memberInfo.GetCustomAttributes(typeof(RawProductAttribute), true).Count() > 0;
        }

        public static bool CanShowQuality(this ProductTypeEnum productType)
        {
            var type = typeof(ProductTypeEnum);
            var memberInfo = type.GetMember(productType.ToString()).First();

            return memberInfo.GetCustomAttributes(typeof(QualityProductAttribute), true).Count() > 0;
        }

        public static WorkTypeEnum GetWorkType(this ProductTypeEnum productType)
        {
            switch (productType)
            {
                case ProductTypeEnum.Grain:
                case ProductTypeEnum.Oil:
                case ProductTypeEnum.Iron:
                case ProductTypeEnum.TeaLeaf:
                case ProductTypeEnum.Wood:
                    {
                        return WorkTypeEnum.Raw;
                    }
                case ProductTypeEnum.MedicalSupplies:
                case ProductTypeEnum.Weapon:
                case ProductTypeEnum.MovingTicket:
                case ProductTypeEnum.Bread:
                case ProductTypeEnum.Fuel:
                case ProductTypeEnum.Tea:
                case ProductTypeEnum.Paper:
                case ProductTypeEnum.UpgradePoints:
                    {
                        return WorkTypeEnum.Manufacturing;
                    }
                case ProductTypeEnum.Development:
                case ProductTypeEnum.ConstructionMaterials:
                case ProductTypeEnum.Hotel:
                case ProductTypeEnum.House:
                case ProductTypeEnum.DefenseSystem:
                case ProductTypeEnum.Hospital:
                    {
                        return WorkTypeEnum.Construction;
                    }
                case ProductTypeEnum.SellingPower:
                    {
                        return WorkTypeEnum.Selling;
                    }
            }

            throw new NotImplementedException();
        }

        public static CompanyTypeEnum GetCompanyTypeForProduct(this ProductTypeEnum productType)
        {
            switch (productType)
            {
                case ProductTypeEnum.Grain:
                case ProductTypeEnum.Oil:
                case ProductTypeEnum.Iron:
                case ProductTypeEnum.TeaLeaf:
                case ProductTypeEnum.Wood:
                    {
                        return CompanyTypeEnum.Producer;
                    }
                case ProductTypeEnum.MedicalSupplies:
                case ProductTypeEnum.Weapon:
                case ProductTypeEnum.MovingTicket:
                case ProductTypeEnum.Bread:
                case ProductTypeEnum.Fuel:
                case ProductTypeEnum.Tea:
                case ProductTypeEnum.Paper:
                case ProductTypeEnum.UpgradePoints:
                case ProductTypeEnum.ConstructionMaterials:
                    {
                        return CompanyTypeEnum.Manufacturer;
                    }
                case ProductTypeEnum.SellingPower:
                    return CompanyTypeEnum.Shop;
                case ProductTypeEnum.Development:
                    return CompanyTypeEnum.Developmenter;
                case ProductTypeEnum.House:
                case ProductTypeEnum.Hotel:
                case ProductTypeEnum.DefenseSystem:
                case ProductTypeEnum.Hospital:
                    return CompanyTypeEnum.Construction;
            }

            throw new NotImplementedException();
        }

        public static ResourceTypeEnum GetResourceType(this ProductTypeEnum productType)
        {
            switch (productType)
            {
                case ProductTypeEnum.Grain:
                    return ResourceTypeEnum.Grain;
                case ProductTypeEnum.Oil:
                    return ResourceTypeEnum.Oil;
                case ProductTypeEnum.TeaLeaf:
                    return ResourceTypeEnum.TeaLeaf;
                case ProductTypeEnum.Wood:
                    return ResourceTypeEnum.Wood;
                case ProductTypeEnum.Iron:
                    return ResourceTypeEnum.IronOre;
            }

            throw new NotImplementedException();
        }

        public static bool CanShowStockQuantityInCompanies(this ProductTypeEnum productType)
        {
            if (productType.IsOneTimeConstruction())
                return false;
            return true;
        }

        public static bool IsOneTimeConstruction(this ProductTypeEnum productType)
        {
            return productType.GetEnumAttribute<ConstructionAttribute>()?.OneTime ?? false;
        }




    }
}
