using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class PartyExtensions
    {
        public static PartyPresidentVoting GetLastPresidentVoting(this Party party)
        {
            return party.PartyPresidentVotings
                .OrderByDescending(v => v.ID)
                .First();
        }

        /// <summary>
        /// Can return null
        /// </summary>
        public static PartyMember GetPresident(this Party party)
        {
            return party.PartyMembers
                .FirstOrDefault(member => member.PartyRoleID == (int)PartyRoleEnum.President);
        }

        public static JoinMethodEnum GetJoinMethod(this Party party)
        {
            return (JoinMethodEnum)party.JoinMethodID;
        }
    }
}
