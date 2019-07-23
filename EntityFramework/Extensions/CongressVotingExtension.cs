using Common.utilities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class CongressVotingExtension
    {
        public static TimeSpan GetTimeLeft(this CongressVoting voting, int currentDay)
        {
            return TimeHelper.CalculateTimeLeft(voting.StartDay, currentDay, 1, voting.StartTime, DateTime.Now);
        }

        public static string GetName(this CongressVoting voting)
        {
            return $"Voting #{voting.ID}";
        }

        public static string GetStatus(this CongressVoting voting)
        {
            return voting.GetStatusEnum().ToHumanReadable();
        }

        public static VotingStatusEnum GetStatusEnum(this CongressVoting voting)
        {
            return (VotingStatusEnum)voting.VotingStatusID;
        }

        public static VotingTypeEnum GetVotingType(this CongressVoting voting)
        {
            return (VotingTypeEnum)voting.VotingTypeID;
        }

        public static CongressVotingRejectionReasonEnum GetRejectionReason(this CongressVoting voting)
        {
            return (CongressVotingRejectionReasonEnum)voting.RejectionReasonID;
        }

    }
}
