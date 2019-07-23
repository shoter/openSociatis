using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums
{
    public enum LawAllowHolderEnum
    {
        President = 1,
        Congress = 2,
        PresidentAndCongress = 3
    }

    public static class LawAllowHolderEnumExtensions
    {
        public static string ToHumanReadableString(this LawAllowHolderEnum holder)
        {
            switch (holder)
            {
                case LawAllowHolderEnum.Congress:
                    return "congress";
                case LawAllowHolderEnum.President:
                    return "president";
                case LawAllowHolderEnum.PresidentAndCongress:
                    return "president and congress";
            }
            throw new NotImplementedException();
        }



    }
}
