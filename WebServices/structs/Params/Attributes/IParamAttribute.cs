using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Params.Attributes
{
    public interface IParamAttribute
    {
        bool Validate(object obj);
    }

}
