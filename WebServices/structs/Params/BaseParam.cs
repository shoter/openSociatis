using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.structs.Params.Attributes;

namespace WebServices.structs.Params
{
    public class BaseParam
    {

        public bool IsValid
        {
            get
            {
                foreach(var property in this.GetType().GetProperties())
                {
                    var attributes = property.GetCustomAttributes(typeof(IParamAttribute), true) as IParamAttribute[];
                    foreach (var attribute in attributes)
                    {
                        if (attribute.Validate(property.GetValue(this)) == false)
                            return false;
                    }
                }
                return true;
            }
        }
    }
}
