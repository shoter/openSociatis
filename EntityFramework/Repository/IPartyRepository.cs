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
    public interface IPartyRepository : IRepository<Party>
    {
        IQueryable<Party> GetPartiesInCountry(int countryID);
        List<PartyJoinRequest> GetJoinRequestsForParty(int partyID);
        void AddJoinRequest(PartyJoinRequest request);
        void RemoveJoinRequest(PartyJoinRequest request);
        void AddPartyMember(PartyMember member);
        PartyMember GetMember(Expression<Func<PartyMember, bool>> predicate);
        List<PartyMember> GetMembers(int partyID);
        void RemovePartyMember(PartyMember member);
        List<PartyRole> GetPartyRoles();
        List<JoinMethod> GetJoinMethods();
        void AddPartyPresidentVoting(PartyPresidentVoting voting);
        PartyPresidentCandidate GetPartyPresidentCandidate(int candidateID);
        void AddPresidentCandidate(PartyPresidentCandidate candidate);
        void AddPartyPresidentVote(PartyPresidentVote vote);
    }
}
