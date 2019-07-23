using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums
{
    [Flags]
    public enum NewspaperRightsEnum
    {
        None = 0,
        CanWriteArticles = 1 << 0,
        CanManageJournalists = 1 << 1,
        CanManageArticles = 1 << 2,
        CanSwitchInto = 1 << 3,
        CanSeeWallet = 1 << 4,

        Full = 0xFF,
    }

    public static class NewspaperRightsEnumExtensions
    {
        public static string ToHumanReadable(this NewspaperRightsEnum rights)
        {

            if (rights == 0)
                return "none";

            string msg = "";

            foreach(NewspaperRightsEnum right in Enum.GetValues(typeof(NewspaperRightsEnum)))
            {
                if (right == 0 || right == NewspaperRightsEnum.Full)
                    continue;

                if (rights.HasFlag(right))
                    msg += ToHumanReadableHelper(right) + ", ";
            }

            return msg.Substring(0, msg.Length - 2);
        }

        private static string ToHumanReadableHelper(NewspaperRightsEnum rights)
        {

            switch (rights)
            {
                case NewspaperRightsEnum.CanWriteArticles:
                    return "can write articles";
                case NewspaperRightsEnum.CanManageArticles:
                    return "can manage articles";
                case NewspaperRightsEnum.CanManageJournalists:
                    return "can manage journalists";
                case NewspaperRightsEnum.CanSwitchInto:
                    return "can switch into";
                case NewspaperRightsEnum.CanSeeWallet:
                    return "can see wallet";
                case NewspaperRightsEnum.Full:
                    return "full rights";
                case NewspaperRightsEnum.None:
                    return "no rights";
            }
#if !DEBUG
            return rights.ToString();
#else
            throw new NotImplementedException("ToHumanReadable NewspaperRightsEnum - " + rights.ToString());
#endif
        }
    }
}
