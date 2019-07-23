using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums
{
    public enum VisibilityOptionEnum
    {
        Admins = 4,
        Moderators = 3,
        Author = 2,
        Everyone = 1
    }

    public static class VisibilityOptionEnumExtensions
    {
        public static PlayerTypeEnum GetMinimumPlayerTypeToView(this VisibilityOptionEnum visibilityOption)
        {
            switch (visibilityOption)
            {
                case VisibilityOptionEnum.Admins:
                    return PlayerTypeEnum.Admin;
                case VisibilityOptionEnum.Moderators:
                    return PlayerTypeEnum.Moderator;
                default:
                    return PlayerTypeEnum.Player;
            }
        }
    }
}
