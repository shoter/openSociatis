using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SociatisCommon.Errors
{
    public enum ConstructionErrors
    {
        [Description("You are not a manager of this construction to do that!")]
        NotAManager,


    }
}
