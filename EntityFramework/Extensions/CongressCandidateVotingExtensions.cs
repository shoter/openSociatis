using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class CongressCandidateVotingExtensions
    {
        public static bool Is(this CongressCandidateVoting voting, VotingStatusEnum votingStatus)
        {
            return voting.VotingStatusID == (int)votingStatus;
        }

        public static bool HasVoted(this CongressCandidateVoting voting, Citizen citizen)
        {
            return voting.CongressCandidateVotes
                .Any(v => v.CitizenID == citizen.ID);
        }
    }
}
