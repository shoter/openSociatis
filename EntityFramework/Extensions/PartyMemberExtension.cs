using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class PartyMemberExtension
    {
        public static bool Is(this PartyMember partyMember,PartyRoleEnum partyRole)
        {
            return partyMember.PartyRoleID == (int)partyRole;
        }
    }
}
