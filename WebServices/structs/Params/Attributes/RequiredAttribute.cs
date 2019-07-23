using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Params.Attributes
{
    public class RequiredAttribute : Attribute, IParamAttribute
    {
        public bool IsRequired { get; set; }
        public RequiredAttribute(bool isRequired = true)
        {
            IsRequired = isRequired;
        }

        public bool Validate(object obj)
        {
            return obj != null || IsRequired == false;
        }
    }
}
