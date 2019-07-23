using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.structs;

namespace WebServices
{
    public interface IProductService
    {
        /// <summary>
        /// Returns raws required to produce 1 unit of product at desired quality
        /// </summary>
        List<ProductRequirement> GetRequiredRaws(ProductTypeEnum productType, int quality);

        /// <summary>
        /// Returns struct with taxes for designated product.
        /// If some kind of tax is not defined then it has value of default values from country policy
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="countryID"></param>
        /// <returns></returns>
        ProductAllTaxes GetAllTaxesForProduct(int productID, int? countryID, int? foreignCountryID);

        int GetTeaHealedAmount(int quality);

        List<ProductTypeEnum> GetRequiredProductTypes(ProductTypeEnum productType);

        ProductTax GetVatTax(int productID, int? countryID);

        decimal GetFuelCostForTranposrt(Path path, ProductTypeEnum productType, int quality, int amount);
        decimal GetFuelCostForTranposrt(double distance, ProductTypeEnum productType, int quality, int amount);

        bool CanUseQuality(ProductTypeEnum productType);

        bool CanUseFertility(ProductTypeEnum productType);
    }
}
