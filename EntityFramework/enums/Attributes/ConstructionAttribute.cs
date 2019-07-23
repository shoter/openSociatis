using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums.Attributes
{
    public class ConstructionAttribute : Attribute
    {
        public bool OneTime { get; set; }
        public ConstructionAttribute(bool oneTime = false)
        {
            OneTime = oneTime;
        }
    }
}
