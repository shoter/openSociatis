using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;

namespace Sociatis.Helpers
{
    public class Images
    {
        public static Image EmptyBar { get; set; } = new Image("/Content/images/EmptyBar.png");
        public static Image Bread { get; set; } = new Image("/Content/Images/Product/Bread.png");
        public static Image UpgradePoints { get; set; } = new Image("/Content/Images/Product/UpgradePoints.png");
        public static Image Fuel { get; set; } = new Image("/Content/Images/Product/Fuel.png");
        public static Image Grain { get; set; } = new Image("/Content/Images/Product/Grain.png");
        public static Image HousingGoods { get; set; } = new Image("/Content/Images/Product/HousingGoods.png");
        public static Image Iron { get; set; } = new Image("/Content/Images/Product/Iron.png");
        public static Image Oil { get; set; } = new Image("/Content/Images/Product/Oil.png");
        public static Image Tea { get; set; } = new Image("/Content/Images/Product/Tea.png");
        public static Image Paper { get; set; } = new Image("/Content/Images/Product/Paper.png");
        public static Image OilDeposit { get; set; } = new Image("/Content/Images/Resources/Oil.png");
        public static Image IronOre { get; set; } = new Image("/Content/Images/Resources/Iron.png");
        public static Image Ticket { get; set; } = new Image("/Content/Images/Product/MovingTicket.png");
        public static Image TeaLeaf { get; set; } = new Image("/Content/Images/Product/TeaLeaf.png");
        public static Image Weapon { get; set; } = new Image("/Content/Images/Product/Weapon.png");
        public static Image Wood { get; set; } = new Image("/Content/Images/Product/Wood.png");
        public static Image MedicalSupplies { get; set; } = new Image("/Content/Images/Product/MedicalSupplies.png");
        public static Image SellingPower { get; set; } = new Image("/Content/Images/Product/SellingPower.png");
        public static Image Eating { get; set; } = new Image("/Content/Images/Icons/eating.png");
        public static Image Training { get; set; } = new Image("/Content/Images/Icons/training.png");
        public static Image Work { get; set; } = new Image("/Content/Images/Icons/Work.png");
        public static Image Cancel { get; set; } = new Image("/Content/Images/Icons/cancel.png");
        public static Image HpCross { get; set; } = new Image("/Content/Images/HPCross.png");
        public static Image Placeholder { get; set; } = new Image("/Content/Images/placeholder.png");
        public static Image MonetaryMarket { get; set; } = new Image("/Content/Images/Other/MonetaryMarket.png");
        public static Image UnderConstruction { get; set; } = new Image("/Content/Images/UnderConstruction.png");
        public static Image ConstructionMaterials { get; set; } = new Image("/Content/Images/Product/ConstructionMaterials.png");
        public static Image House { get; set; } = new Image("/Content/Images/Product/House.png");
        public static Image Hotel { get; set; } = new Image("/Content/Images/Product/Hotel.png");
        public static Image DefenseSystem { get; set; } = new Image("/Content/Images/Product/DefenseSystem.png");
        public static Image Hospital { get; set; } = new Image("/Content/Images/Product/Hospital.png");
        public static Image HousePlaceholder { get; set; } = new Image("/Content/Images/HousePlaceholder.png");

        public static Image GetCountryFlag(Country country)
        {
            return GetCountryFlag(country.ID);
        }

        public static Image GetCountryFlag(int countryID)
        {
            return GetCountryFlag(Persistent.Countries.GetById(countryID).Entity.Name);
        }
        public static Image GetCountryFlag(string countryName)
        {
            return new Image(string.Format("/Content/Images/Flags/{0}.png", countryName));
        }
        public static Image GetCountryCurrency(int countryID)
        {
            return GetCountryCurrency(Persistent.Countries.GetById(countryID));
        }
        public static Image GetCountryCurrency(Country country)
        {
            var currency = Persistent.Countries.GetCountryCurrency(country);
            return GetCountryCurrency(currency);
        }
        public static Image GetCountryCurrency(Currency currency)
        {
            if(currency.Name == "Gold")
                return new Image("/Content/Images/Currency/Gold.png");
            else
                return new Image(string.Format("/Content/Images/Currency/{0}.png", 
                    Persistent.Countries.First(c => c.CurrencyID == currency.ID).Entity.Name));
        }

        public static Image GetResourceImage(ResourceTypeEnum resourceType)
        {
            switch(resourceType)
            {
                case ResourceTypeEnum.Grain:
                    return Grain;
                case ResourceTypeEnum.IronOre:
                    return IronOre;
                case ResourceTypeEnum.Oil:
                    return OilDeposit;
                case ResourceTypeEnum.TeaLeaf:
                    return TeaLeaf;
                case ResourceTypeEnum.Wood:
                    return Wood;
            }

            throw new NotImplementedException("Resource type does not exist. Cannot get image for it. you baka");
        }

        public static Image GetProductImageHighRes(ProductTypeEnum productType)
        {
            switch (productType)
            {
                case ProductTypeEnum.Weapon:
                case ProductTypeEnum.Hospital:
                case ProductTypeEnum.House:
                case ProductTypeEnum.Hotel:
                case ProductTypeEnum.ConstructionMaterials:
                    return new Image(string.Format("/Content/Images/Product/Highres-{0}.png", productType.ToString()));
                default:
                    return new Image(string.Format("/Content/Images/Product/{0}.png", productType.ToString()));
            }
        }

        public static Image GetProductImage(ProductTypeEnum productType)
        {
            return new Image(string.Format("/Content/Images/Product/{0}.png", productType.ToString()));
        }
    }
}
