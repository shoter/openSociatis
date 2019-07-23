using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs
{
    public class ProductRequirement
    {
        public ProductTypeEnum RequiredProductType { get; set; }
        public int Quantity { get; set; }

        public int Quality { get; set; } = 1;
    }
}
