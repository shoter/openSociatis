using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.structs.Params;
using WebServices.structs.Params.Attributes;

namespace Sociatis_Unit_Tests.Params
{
    public class TestParam : BaseParam
    {
        [Required]
        public int? RequiredValue { get; set; }

        public int? OptionalValue { get; set; }
    }
}
