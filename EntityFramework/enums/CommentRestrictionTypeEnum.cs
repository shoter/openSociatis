using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums
{
    public enum CommentRestrictionEnum
    {
        OnlyCongressmen = 1,
        CitizenCanView = 2,
        CitizenCanComment = 3
    }

    public static class CommentRestrictionEnumExtensions
    {
        public static string ToHumanReadable(this CommentRestrictionEnum restriction)
        {
            switch (restriction)
            {
                case CommentRestrictionEnum.CitizenCanComment:
                    return "citizens and congressmen can comment";
                case CommentRestrictionEnum.CitizenCanView:
                    return "congressmen can comment and citizens are able to view comments";
                case CommentRestrictionEnum.OnlyCongressmen:
                    return "only congressmen can view and comment";
            }
            throw new NotImplementedException();
        }
    }
}
