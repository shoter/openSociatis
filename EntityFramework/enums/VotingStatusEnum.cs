using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums
{
    public enum VotingStatusEnum
    {
        Ongoing = 1,
        Finished = 2,
        NotStarted = 3,
        Accepted = 4,
        Rejected = 5,
        Started = 6,
    }

    public static class VotingStatusEnumExtensions
    {
        public static string ToHumanReadable(this VotingStatusEnum votingStatus)
        {
            switch(votingStatus)
            {
                case VotingStatusEnum.Ongoing:
                    return "On going";
                case VotingStatusEnum.Finished:
                    return "Finished";
                case VotingStatusEnum.NotStarted:
                    return "Not started";
                case VotingStatusEnum.Accepted:
                    return "Accepted";
                case VotingStatusEnum.Rejected:
                    return "Rejected";
                case VotingStatusEnum.Started:
                    return "Started";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
