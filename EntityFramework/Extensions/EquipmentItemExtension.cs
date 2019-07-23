using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class EquipmentItemExtension
    {
        public static ProductTypeEnum GetProductType(this EquipmentItem item)
        {
            return (ProductTypeEnum)item.ProductID;
        }
    }
}
