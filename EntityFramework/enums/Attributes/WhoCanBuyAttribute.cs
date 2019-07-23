using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums.Attributes
{
    public class WhoCanBuyAttribute : Attribute
    {
        public TraderTypeEnum[] TraderTypes { get; private set; }

        public WhoCanBuyAttribute(params TraderTypeEnum[] entityTypes)
        {
            TraderTypes = entityTypes;
        }

        public bool Contains(TraderTypeEnum entityType)
        {
            return TraderTypes.Contains(entityType);
        }
    }
}
