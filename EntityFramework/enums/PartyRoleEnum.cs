using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums
{
    public enum PartyRoleEnum
    {
        Member = 1,
        Manager = 2,
        President = 3,

        NotAMember = 0
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class PartyRoleEnumExtensions
    {
        public static string ToHumanReadable(this PartyRoleEnum partyRole)
        {
            switch (partyRole)
            {
                case PartyRoleEnum.Manager:
                    return "manager";
                case PartyRoleEnum.Member:
                    return "member";
                case PartyRoleEnum.President:
                    return "president";
                case PartyRoleEnum.NotAMember:
                    return "not a member";
            }
            throw new NotImplementedException();
        }
    }
}
