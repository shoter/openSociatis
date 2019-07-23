using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Operations;
using Entities;
using Entities.Repository;
using WebServices.Helpers;
using Weber.Html;

namespace WebServices
{
    public class FriendService : BaseService, IFriendService
    {
        private readonly IFriendRepository friendRepository;
        private readonly IWarningService warningService;
        private readonly IPopupService popupService;

        public FriendService(IFriendRepository friendRepository, IWarningService warningService, IPopupService popupService)
        {
            this.friendRepository = friendRepository;
            this.warningService = Attach(warningService);
            this.popupService = Attach(popupService);
        }

        public MethodResult CanRemoveFriend(Citizen first, Citizen second)
        {
            if (friendRepository.AreFriends(first, second) == false)
                return new MethodResult("You are not friends!");

            return MethodResult.Success;
        }

        public void RemoveFriend(Citizen first, Citizen second)
        {
            var friendship = friendRepository.GetFriend(first.ID, second.ID);

            using (NoSaveChanges)
            {
                string link = EntityLinkCreator.Create(first.Entity).ToHtmlString();

                warningService.AddWarning(second.ID, $"{link} removed you as friend.");
            }

            friendRepository.Remove(friendship);
            friendRepository.SaveChanges();
        }

        public void AcceptRequest(FriendRequest request)
        {
            var friendshipIsMagic = new Friend()
            {
                ProposerCitizenID = request.ProposerCitizenID,
                SecondCitizenID = request.SecondCitizenID,
                DayCreated = GameHelper.CurrentDay
            };
            friendRepository.Add(friendshipIsMagic);

            using (NoSaveChanges)
            {
                string link = EntityLinkCreator.Create(request.SecondCitizen.Entity).ToHtmlString();

                popupService.AddSuccess("You accepted friendship request.");
                warningService.AddWarning(request.ProposerCitizenID, $"{link} accepted your friendship request.");
            }


            friendRepository.RemoveFriendshipRequest(request);
            ConditionalSaveChanges(friendRepository);
        }

        public void DeclineRequest(FriendRequest request)
        {
            using (NoSaveChanges)
            {
                string link = EntityLinkCreator.Create(request.SecondCitizen.Entity).ToHtmlString();

                popupService.AddInfo("You rejected friendship request.");
                warningService.AddWarning(request.ProposerCitizenID, $"{link} declined your friendship request.");
            }


            friendRepository.RemoveFriendshipRequest(request);
            ConditionalSaveChanges(friendRepository);
        }

        public void RemoveRequest(FriendRequest request)
        {
            using (NoSaveChanges)
            {
                string link = EntityLinkCreator.Create(request.ProposerCitizen.Entity).ToHtmlString();

                popupService.AddInfo("You canceled friendship request.");
                warningService.AddWarning(request.SecondCitizenID, $"{link} canceled friendship request.");
            }


            friendRepository.RemoveFriendshipRequest(request);
            ConditionalSaveChanges(friendRepository);
        }

        public MethodResult CanAcceptReuqest(Citizen citizen, FriendRequest request)
        {
            if (citizen.ID == request.SecondCitizenID)
                return MethodResult.Success;

            return new MethodResult("You cannot accept this request!");
        }

        public MethodResult CanDeclineRequest(Citizen citizen, FriendRequest request)
        {
            if (citizen.ID == request.SecondCitizenID)
                return MethodResult.Success;

            return new MethodResult("You cannot accept this request!");
        }

        public MethodResult CanRemoveRequest(Citizen citizen, FriendRequest request)
        {
            if (citizen.ID == request.ProposerCitizenID)
                return MethodResult.Success;

            return new MethodResult("You cannot accept this request!");
        }

        public MethodResult CanSendFriendRequest(Citizen proposer, Citizen other)
        {
            if (friendRepository.AreFriends(proposer, other))
                return new MethodResult("You are already friends!");

            if (friendRepository.AnyFriendRequestBetween(proposer, other))
                return new MethodResult("There is already friend request between you!");

            return MethodResult.Success;
        }


        public void SendFriendRequest(Citizen proposer, Citizen other)
        {
            friendRepository.AddFriendRequest(proposer, other);

            using (NoSaveChanges)
            {
                string link = EntityLinkCreator.Create(proposer.Entity).ToHtmlString();

                warningService.AddWarning(other.ID, $"You received friendship request from {link}.");
                popupService.AddInfo($"You've sent friend request to {other.Entity.Name}.");
            }

            ConditionalSaveChanges(friendRepository);
        }

        public bool AreFriends(Citizen citizen, Citizen other)
        {
            return friendRepository.AreFriends(citizen, other);
        }

        public bool HasSentFriendRequest(Citizen proposer, Citizen other)
        {
            return friendRepository.GetFriendRequestsQuery(proposer)
                .Any(r => r.SecondCitizenID == other.ID);
        }
    }
}
