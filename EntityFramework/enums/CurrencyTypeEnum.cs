using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums
{
    public enum CurrencyTypeEnum
    {
        PLN = 1,
        Gold = 2,
        DM = 3,
        Euro = 4,
        Dollar = 5,
        FrenchFranc = 6,
        SpainPeseta = 7,
        CroatianKuna = 8,
        SerbianDinar = 9,
        AlbanianLek = 10,
        PoundSterling = 11,
    }

    public static class CurrencyTypeEnumExtensions
    {
        public static string ToHumarReadable(this CurrencyTypeEnum currency)
        {
            switch(currency)
            {
                case CurrencyTypeEnum.DM:
                    return "dm";
                case CurrencyTypeEnum.Gold:
                    return "gold";
                case CurrencyTypeEnum.PLN:
                    return "pln";
            }
#if DEBUG
            throw new NotImplementedException("CurrencyType-HumanReadable-" + currency.ToString());
#else
            return currency.ToString();
#endif
        }
    }
}
