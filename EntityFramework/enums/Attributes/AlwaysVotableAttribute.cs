using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AlwaysVotableAttribute : Attribute
    {
        public bool Value { get; set; }

        public AlwaysVotableAttribute(bool value = true)
        {
            Value = value;
        }
    }
}
