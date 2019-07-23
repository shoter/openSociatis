using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums
{
    public enum CompanyTypeEnum
    {
        Manufacturer = 1,
        Producer = 2,
        Shop = 3,
        Developmenter = 4,
        Construction = 5
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class CompanyTypeEnumExtensions
    {
        public static int ToInt(this CompanyTypeEnum companyType)
        {
            return (int)companyType;
        }
    }
}
