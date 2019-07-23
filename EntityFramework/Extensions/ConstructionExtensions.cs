using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class ConstructionExtensions
    {
        public static ProductTypeEnum GetProductType(this Construction construction)
        {
            return construction.Company.GetProducedProductType();
        }
    }
}
