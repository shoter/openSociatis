using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface ICongressCandidateService
    {
        int GetCountryMaxCongressmenAmount(Country country);
        void VoteOnCongressCandidate(Citizen voter, CongressCandidate candidate);
        void ChangeCandidateStatus(CongressCandidate candidate, CongressCandidateStatusEnum status);
        CongressCandidateVoting CreateNewCongressCandidateVoting(Country country, int votingDay);
        /// <summary>
        /// Checks wheter desired citizen can vote in the congress voting for candidates
        /// </summary>
        /// <param name="citizen"></param>
        /// <param name="voting"></param>
        /// <returns></returns>
        bool CanVote(Citizen citizen, CongressCandidateVoting voting);
        void ProcessDayChange(int newDay);
        void Resign(CongressCandidate candidate);

        double GetGoldForCadency(int cadencyLength);

    }
}
