using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class PartyRepository : RepositoryBase<Party, SociatisEntities>, IPartyRepository
    {
        public PartyRepository(SociatisEntities context) : base(context)
        {
        }

        public IQueryable<Party> GetPartiesInCountry(int countryID)
        {
            return Where(c => c.CountryID == countryID);
        }

        public void AddPartyMember(PartyMember member)
        {
            context.PartyMembers.Add(member);
        }

        public List<PartyJoinRequest> GetJoinRequestsForParty(int partyID)
        {
            var requests = context.PartyJoinRequests
                .Where(r => r.PartyID == partyID)
                .ToList();

            return requests;
        }

        public void AddPartyPresidentVote(PartyPresidentVote vote)
        {
            context.PartyPresidentVotes.Add(vote);
        }

        public PartyPresidentCandidate GetPartyPresidentCandidate(int candidateID)
        {
            return context.PartyPresidentCandidates
                .OrderByDescending(c => c.PartyPresidentVotingID)
                .First(c => c.ID == candidateID);
        }

        public void AddPartyPresidentVoting(PartyPresidentVoting voting)
        {
            context.PartyPresidentVotings.Add(voting);
        }

        public PartyMember GetMember(Expression<System.Func<PartyMember, bool>> predicate)
        {
            return context.PartyMembers
                .First(predicate);
        }
        public List<PartyMember> GetMembers(int partyID)
        {
            return context.PartyMembers
                .Where(p => p.PartyID == partyID)
                .ToList();
        }

        public void AddJoinRequest(PartyJoinRequest request)
        {
            context.PartyJoinRequests.Add(request);
        }

        public void RemoveJoinRequest(PartyJoinRequest request)
        {
            context.PartyJoinRequests.Remove(request);
        }

        public void RemovePartyMember(PartyMember member)
        {
            context.PartyMembers.Remove(member);
        }

        public List<PartyRole> GetPartyRoles()
        {
            return context.PartyRoles.ToList();
        }

        public List<JoinMethod> GetJoinMethods()
        {
            return context.JoinMethods.ToList();
        }

        public void AddPresidentCandidate(PartyPresidentCandidate candidate)
        {
            context.PartyPresidentCandidates.Add(candidate);
        }
    }
}
