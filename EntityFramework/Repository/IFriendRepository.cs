using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface IFriendRepository : IRepository<Friend>
    {
        void AddFriendRequest(Citizen proposer, Citizen other);
        List<FriendRequest> GetFriendRequests(Citizen citizen);
        void RemoveFriendshipRequest(FriendRequest request);
        bool AnyFriendRequestBetween(Citizen citizen, Citizen other);
        bool AreFriends(Citizen citizen, Citizen other);
        /// <summary>
        /// Returns null if not found
        /// </summary>
        FriendRequest GetFriendRequest(int requestID);
        IQueryable<FriendRequest> GetFriendRequestsQuery(Citizen citizen);
        FriendRequest GetFriendRequest(int proposerID, int otherID);
        Friend GetFriend(int firstID, int secondID);
        IQueryable<Citizen> GetFriends(int citizenID);

    }
}
