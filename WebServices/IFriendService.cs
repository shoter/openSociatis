using Common.Operations;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface IFriendService
    {
        MethodResult CanSendFriendRequest(Citizen proposer, Citizen other);
        void SendFriendRequest(Citizen proposer, Citizen other);
        MethodResult CanAcceptReuqest(Citizen citizen, FriendRequest request);
        MethodResult CanDeclineRequest(Citizen citizen, FriendRequest request);
        MethodResult CanRemoveRequest(Citizen citizen, FriendRequest request);
        void AcceptRequest(FriendRequest request);
        void DeclineRequest(FriendRequest request);
        void RemoveRequest(FriendRequest request);

        bool AreFriends(Citizen citizen, Citizen other);
        bool HasSentFriendRequest(Citizen proposer, Citizen other);

        MethodResult CanRemoveFriend(Citizen first, Citizen second);
        void RemoveFriend(Citizen first, Citizen second);
    }
}
