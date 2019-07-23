using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Groups
{
    public static class ProductGroups
    {
        public static List<ProductTypeEnum> Raws = new List<ProductTypeEnum>()
        {
            ProductTypeEnum.Oil,
            ProductTypeEnum.Grain,
            ProductTypeEnum.Iron,
            ProductTypeEnum.TeaLeaf,
            ProductTypeEnum.Wood,
        };

        public static List<ProductTypeEnum> Consumables = new List<ProductTypeEnum>()
        {
            ProductTypeEnum.MovingTicket,
            ProductTypeEnum.Weapon,
            ProductTypeEnum.Bread,
            ProductTypeEnum.Tea,
        };

        public static List<ProductTypeEnum> Manufactured = new List<ProductTypeEnum>()
        {
            ProductTypeEnum.Paper,
            ProductTypeEnum.Fuel,
            //Consumables will be added in constructor
        };

        public static List<ProductTypeEnum> All { get; set; } = new List<ProductTypeEnum>();

        static ProductGroups()
        {
            foreach (ProductTypeEnum productType in Enum.GetValues(typeof(ProductTypeEnum)))
                All.Add(productType);

            Manufactured.AddRange(Consumables);
        }
    }
}
