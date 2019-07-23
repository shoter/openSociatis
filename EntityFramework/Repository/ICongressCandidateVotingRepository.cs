using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface ICongressCandidateVotingRepository : IRepository<CongressCandidateVoting>
    {
        void AddCandidate(CongressCandidate candidate);
        void AddVote(CongressCandidateVote vote);
        CongressCandidate GetCandidate(int candidateID);
        void RemoveCandidate(CongressCandidate candidate);
        CongressCandidate GetCandidateInNotStartedVoting(int citizenID);
        CongressCandidateVoting GetLastVotingForCountry(int countryID);

        CongressCandidate GetCongressCandidateNotApprovedInParty(int partyID, int citizenID);
    }
}
