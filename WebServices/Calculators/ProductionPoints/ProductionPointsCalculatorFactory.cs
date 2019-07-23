using Entities.enums;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.Calculators.ProductionPoints
{
    public class ProductionPointsCalculatorFactory
    {
        ProductTypeEnum? productType;

        public ProductionPointsCalculatorFactory SetProductType(ProductTypeEnum productType)
        {
            this.productType = productType;
            return this;
        }
        public IProductionPointsCalculator Create()
        {
            if (productType.HasValue == false)
                throw new Exception("You must set product type!");

            if (productType == ProductTypeEnum.Development)
            {
                return new DevelopmentProductionPointsCalculator();
            }
            else
            {
                return new DefaultProductionPointsCalculator();
            }
        }
    }
}
