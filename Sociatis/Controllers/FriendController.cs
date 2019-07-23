using Common.Operations;
using Entities.enums;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Helpers;
using Sociatis.Models.Friends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Controllers
{
    public class FriendController : ControllerBase
    {
        private readonly IFriendService friendService;
        private readonly ICitizenRepository citizenRepository;
        private readonly IFriendRepository friendRepository;

        public FriendController(IPopupService popupService, IFriendService friendService, ICitizenRepository citizenRepository,
            IFriendRepository friendRepository) : base(popupService)
        {
            this.friendService = friendService;
            this.citizenRepository = citizenRepository;
            this.friendRepository = friendRepository;
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Friend/SendFriendRequest/{citizenID:int}")]
        public ActionResult SendFriendRequest(int citizenID)
        {
            var citizen = citizenRepository.GetById(citizenID);

            if (citizen == null)
                return RedirectToHomeWithError("Citizen with this ID does not exist!");

            var currentCitizen = SessionHelper.CurrentEntity.Citizen;

            if (currentCitizen == null)
                return RedirectToHomeWithError("You must be an citizen to make friends.");

            MethodResult result;
            if ((result = friendService.CanSendFriendRequest(currentCitizen, citizen)).IsError)
                return RedirectBackWithError(result);

            friendService.SendFriendRequest(currentCitizen, citizen);

            return RedirectToAction("View", "Citizen", new { citizenID = citizenID });
        }

        [Route("Citizen/{citizenID:int}/Friends")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult Friends(int citizenID)
        {
            var citizen = citizenRepository.GetById(citizenID);

            if (citizen == null)
                return RedirectToHomeWithError("Citizen does not exist!");

            var friends = friendRepository.GetFriends(citizenID);

            var vm = new FriendsViewModel(citizen, friends, friendService);

            return View(vm);
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Friend/Requests/{requestID:int}/Accept")]
        public ActionResult AcceptFriendRequest(int requestID)
        {
            var request = friendRepository.GetFriendRequest(requestID);

            if (request == null)
                return RedirectToHomeWithError("Request does not exist!");

            var currentCitizen = SessionHelper.CurrentEntity.Citizen;

            if (currentCitizen == null)
                return RedirectToHomeWithError("You must be an citizen to make friends.");

            MethodResult result;
            if ((result = friendService.CanAcceptReuqest(currentCitizen, request)).IsError)
                return RedirectToHomeWithError(result);

            friendService.AcceptRequest(request);

            return RedirectToAction("View", "Citizen", new { citizenID = request.ProposerCitizenID });
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Friend/Requests/{requestID:int}/Decline")]
        public ActionResult DeclineFriendRequest(int requestID)
        {
            var request = friendRepository.GetFriendRequest(requestID);

            if (request == null)
                return RedirectToHomeWithError("Request does not exist!");

            var currentCitizen = SessionHelper.CurrentEntity.Citizen;

            if (currentCitizen == null)
                return RedirectToHomeWithError("You must be an citizen to make friends.");

            MethodResult result;
            if ((result = friendService.CanDeclineRequest(currentCitizen, request)).IsError)
                return RedirectToHomeWithError(result);

            friendService.DeclineRequest(request);

            return RedirectToAction("View", "Citizen", new { citizenID = request.ProposerCitizenID });
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Friend/Requests/req-{requestID:int}/Cancel")]
        public ActionResult CancelFriendRequest(int requestID)
        {
            var request = friendRepository.GetFriendRequest(requestID);

            if (request == null)
                return RedirectToHomeWithError("Request does not exist!");

            var currentCitizen = SessionHelper.CurrentEntity.Citizen;

            if (currentCitizen == null)
                return RedirectToHomeWithError("You must be an citizen to make friends.");

            MethodResult result;
            if ((result = friendService.CanRemoveRequest(currentCitizen, request)).IsError)
                return RedirectToHomeWithError(result);

            friendService.RemoveRequest(request);

            return RedirectToAction("View", "Citizen", new { citizenID = request.SecondCitizenID });
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Friend/Requests/{citizenID:int}/Cancel")]
        public ActionResult CancelFriendRequestByCitizen(int citizenID)
        {
            var currentCitizen = SessionHelper.CurrentEntity.Citizen;

            if (currentCitizen == null)
                return RedirectToHomeWithError("You must be an citizen to make friends.");

            var request = friendRepository.GetFriendRequest(currentCitizen.ID, citizenID);

            if (request == null)
                return RedirectToHomeWithError("Request does not exist!");

            return CancelFriendRequest(request.ID);
        }


        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Friend/Friends/{citizenID:int}/Remove")]
        public ActionResult RemoveFriend(int citizenID)
        {
            var currentCitizen = SessionHelper.CurrentEntity.Citizen;

            if (currentCitizen == null)
                return RedirectToHomeWithError("You must be an citizen to make friends.");

            var otherCitizen = citizenRepository.GetById(citizenID);

            if (otherCitizen == null)
                return RedirectToHomeWithError("Citizen does not exist");

            MethodResult result;
            if ((result = friendService.CanRemoveFriend(currentCitizen, otherCitizen)).IsError)
                return RedirectToHomeWithError(result);

            friendService.RemoveFriend(currentCitizen, otherCitizen);
            return RedirectToAction("View", "Citizen", new { citizenID = citizenID });
        }

    }
}