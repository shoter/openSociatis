using Common.EntityFramework;
using Entities.enums;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class CongressCandidateVotingRepository : RepositoryBase<CongressCandidateVoting, SociatisEntities>, ICongressCandidateVotingRepository
    {
        public CongressCandidateVotingRepository(SociatisEntities context) : base(context)
        {
        }

        public void AddCandidate(CongressCandidate candidate)
        {
            context.CongressCandidates.Add(candidate);
        }
        public void RemoveCandidate(CongressCandidate candidate)
        {
            context.CongressCandidates.Remove(candidate);

        }

        public void AddVote(CongressCandidateVote vote)
        {
            context.CongressCandidateVotes.Add(vote);
        }

        public CongressCandidate GetCandidate(int candidateID)
        {
            return context.CongressCandidates.FirstOrDefault(cc => cc.ID == candidateID);
        }

        public CongressCandidate GetCandidateInNotStartedVoting(int citizenID)
        {
            return context.CongressCandidates
                .Where(cc => cc.CongressCandidateVoting.VotingStatusID == (int)VotingStatusEnum.NotStarted)
                .FirstOrDefault(cc => cc.CandidateID == citizenID);
        }

        public CongressCandidate GetCongressCandidateNotApprovedInParty(int partyID, int citizenID)
        {
            return context.CongressCandidates
                .Where(c => c.PartyID == partyID && c.CandidateID == citizenID && c.CongressCandidateStatusID == (int)CongressCandidateStatusEnum.WaitingForApproval)
                .OrderByDescending(c => c.ID)
                .FirstOrDefault();
        }

        public CongressCandidateVoting GetLastVotingForCountry(int countryID)
        {
            return Where(c => c.CountryID == countryID)
                .OrderByDescending(c => c.ID)
                .First();
        }
    }
}
