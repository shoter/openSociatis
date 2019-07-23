using Common.utilities;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using Sociatis.Helpers;
using Sociatis.Models.Avatar;
using Sociatis.Models.Infos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Models.Citizens
{
    public class CitizenInfoViewModel
    {
        public string CitizenName { get; set; }
        public int CitizenID { get; set; }
        public ImageViewModel Avatar { get; set; }
        public string RegionName { get; set; }
        public string CountryName { get; set; }
        public int CountryID { get; set; }
        public int RegionID { get; set; }
        public int? WorkingCompanyID { get; set; } = null;
        public string WorkingCompanyName { get; set; }
        public int? PartyID { get; set; } = null;
        public string PartyName { get; set; }
        public string MilitaryRankReadable { get; set; }

        public bool IsUnderControl { get; set; }

        public bool AreFriends { get; set; } = false;
        public bool HasSentFriendRequest { get; set; } = false;
        public bool HasReceivedFriendRequest { get; set; } = false;
        public int? friendRequestID { get; set; } 
        public bool CanAddFriend { get; set; } = false;
        public AvatarChangeViewModel AvatarChange { get; set; }

        public int CitizenshipCountryID { get; set; }
        public string CitizenshipCountryName { get; set; }

        public InfoMenuViewModel Menu { get; set; }

        public CitizenInfoViewModel(Entities.Citizen citizen, IFriendService friendService)
        {
            var entity = citizen.Entity;

            CitizenName = entity.Name;
            CitizenID = entity.EntityID;
            Avatar = new ImageViewModel(entity.ImgUrl);


            var region = citizen.Entity.GetCurrentRegion();
            var country = citizen.Entity.GetCurrentCountry();

            RegionName = region.Name;
            RegionID = region.ID;
            CountryName = country.Entity.Name;
            CountryID = country.ID;
            CitizenshipCountryID = citizen.CitizenshipID;
            CitizenshipCountryName = Persistent.Countries.GetById(CitizenshipCountryID).Entity.Name;

            if (citizen.CompanyEmployee != null)
            {
                WorkingCompanyID = citizen.CompanyEmployee.CompanyID;
                WorkingCompanyName = citizen.CompanyEmployee.Company.Entity.Name;
            }
            if (citizen.PartyMember != null)
            {
                PartyID = citizen.PartyMember.PartyID;
                PartyName = citizen.PartyMember.Party.Entity.Name;
            }

            IsUnderControl = citizen.ID == SessionHelper.CurrentEntity.EntityID;
            if (IsUnderControl)
            {
                AvatarChange = new AvatarChangeViewModel(citizen.ID);
            }
            MilitaryRankReadable = StringUtils.FirstToUpper(MilitaryRankEnumExtensions.GetRankForMilitaryRank((double)citizen.MilitaryRank).ToHumanReadable());

            var currentCitizen = SessionHelper.CurrentEntity.Citizen;

            friendshipInit(citizen, friendService, currentCitizen);
            createMenu();
        }

        private void friendshipInit(Entities.Citizen citizen, IFriendService friendService, Entities.Citizen currentCitizen)
        {
            if (currentCitizen != null && currentCitizen.ID != citizen.ID)
            {
                AreFriends = friendService.AreFriends(currentCitizen, citizen);
                HasSentFriendRequest = friendService.HasSentFriendRequest(currentCitizen, citizen);
                HasReceivedFriendRequest = friendService.HasSentFriendRequest(citizen, currentCitizen);
                if (HasReceivedFriendRequest)
                {
                    var friendRepo = DependencyResolver.Current.GetService<IFriendRepository>();
                    friendRequestID = friendRepo.GetFriendRequest(citizen.ID, currentCitizen.ID).ID;
                }
                CanAddFriend = !AreFriends && !HasSentFriendRequest && !HasReceivedFriendRequest;
            }
        }

        private void createMenu()
        {
            Menu = new InfoMenuViewModel();

            if (AreFriends)
            {
                Menu.AddItem(new InfoExpandableViewModel("Friend", "fa-user-circle-o")
                    .AddChild(new InfoActionViewModel("RemoveFriend", "Friend", "Remove", "fa-user-times", FormMethod.Post, new { citizenID = CitizenID })));
            }

            if (HasReceivedFriendRequest)
            {
                Menu.AddItem(new InfoExpandableViewModel("Friend Request", "fa-user-circle-o")
                    .AddChild(new InfoActionViewModel("AcceptFriendRequest", "Friend", "Accept", "fa-check", FormMethod.Post, new { requestID = friendRequestID }))
                    .AddChild(new InfoActionViewModel("DeclineFriendRequest", "Friend", "Decline", "fa-times", FormMethod.Post, new { requestID = friendRequestID })));
            }

            if (HasSentFriendRequest)
            {
                Menu.AddItem(new InfoExpandableViewModel("Friend request sent", "fa-envelope")
                    .AddChild(new InfoActionViewModel("CancelFriendRequestByCitizen", "Friend", "Cancel", "fa-user-times", FormMethod.Post)));
            }

            if (CanAddFriend)
            {
                Menu.AddItem(new InfoActionViewModel("SendFriendRequest", "Friend", "Add friend", "fa-user-plus", FormMethod.Post, new { citizenID = CitizenID }));
            }

            if (IsUnderControl)
            {
                Menu.AddItem(new InfoActionViewModel("Wallet", "Citizen", "Wallet", "fa-dollar"))
                    .AddItem(new InfoActionViewModel("Travel", "Citizen", "Travel", "fa-plane"));
            }
            if (SessionHelper.CurrentEntity.EntityID != CitizenID)
                Menu.AddItem(InfoExpandableViewModel.CreateExchange(CitizenID));
        }
    }
}