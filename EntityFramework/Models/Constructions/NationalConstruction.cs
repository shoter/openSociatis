using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Constructions
{
    public class NationalConstruction
    {
        public int ConstructionID { get; set; }
        public string ConstructionName { get; set; }
        public int Progress { get; set; }
        public int RegionID { get; set; }
        public string RegionName { get; set; }
        public int Quality { get; set; }
        public string OwnerName { get; set; }
        public int ProductTypeID { get; set; }
        public ProductTypeEnum ProductType => (ProductTypeEnum)ProductTypeID;
    }
}
