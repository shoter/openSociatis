
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
    public class FriendRepository : RepositoryBase<Friend, SociatisEntities>, IFriendRepository
    {
        public FriendRepository(SociatisEntities context) : base(context)
        {
        }

        public Friend GetFriend(int firstID, int secondID)
        {
            int[] ids = { firstID, secondID };
            return FirstOrDefault(f => ids.Contains(f.ProposerCitizenID) && ids.Contains(f.SecondCitizenID));
        }

        public IQueryable<Citizen> GetFriends(int citizenID)
        {
            return
                Where(f => f.ProposerCitizenID == citizenID)
                .Select(f => f.SecondCitizen)
                .Union(
                    Where(f => f.SecondCitizenID == citizenID)
                    .Select(f => f.ProposerCitizen));
        }

        public void AddFriendRequest(Citizen proposer, Citizen other)
        {
            FriendRequest req = new FriendRequest()
            {
                ProposerCitizen = proposer,
                SecondCitizen = other
            };

            context.FriendRequests.Add(req);
        }

        public List<FriendRequest> GetFriendRequests(Citizen citizen)
        {
            return GetFriendRequestsQuery(citizen)
            .ToList();
        }

        public FriendRequest GetFriendRequest(int proposerID, int otherID)
        {
            return context.FriendRequests
                .FirstOrDefault(f => f.ProposerCitizenID == proposerID && f.SecondCitizenID == otherID);
        }

        public IQueryable<FriendRequest> GetFriendRequestsQuery(Citizen citizen)
        {
            return context.FriendRequests.Where(f => f.ProposerCitizenID == citizen.ID || f.SecondCitizenID == citizen.ID);
           
        }

        public FriendRequest GetFriendRequest(int requestID)
        {
            return context.FriendRequests.FirstOrDefault(r => r.ID == requestID);
        }



        public void RemoveFriendshipRequest(FriendRequest request)
        {
            context.FriendRequests.Remove(request);
        }

        public bool AnyFriendRequestBetween(Citizen citizen, Citizen other)
        {
            int[] ids = { citizen.ID, other.ID };
            return context.FriendRequests
                .Any(f => ids.Contains(f.ProposerCitizenID) && ids.Contains(f.SecondCitizenID));
        }

        public static Expression<Func<Friend, bool>> areFriends(Citizen c)
        {
            return f =>
                f.ProposerCitizenID == c.ID ||
                f.SecondCitizenID == c.ID;
        }

        public bool AreFriends(Citizen citizen, Citizen other)
        {
            int[] ids = { citizen.ID, other.ID };
            return
                Any(f => ids.Contains(f.ProposerCitizenID) && ids.Contains(f.SecondCitizenID));
        }
    }
}
