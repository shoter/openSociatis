using Entities.enums;
using Entities.Extensions;
using Sociatis.Helpers;
using Sociatis.Models.Avatar;
using Sociatis.Models.Infos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Models.Party
{
    public class PartyInfoViewModel
    {
        public ImageViewModel PartyAvatar { get; set; }
        public string PartyName { get; set; }
        public string PartyCountry { get; set; }
        public int PartyID { get; set; }
        public PartyRoleEnum PartyRole { get; set; }
        public JoinMethodEnum JoinMethod { get; set; }
        public bool IsActingAsParty { get; set; }
        public int MemberCount { get; set; }
        public int CongressmenCount { get; set; }
        public bool CanAcceptJoinOffer { get; set; }
        public int InviteID { get; set; }
        public bool CanSendJoinRequest { get; set; } = false;
        public bool HasSendJoinRequest { get; set; } = false;
        public bool CanSeeInvites { get; set; }
        public bool CanSeeRequests { get; set; }
        public int RequestID { get; set; }

        public InfoMenuViewModel Menu { get; set; } = new InfoMenuViewModel();

        public AvatarChangeViewModel AvatarChange { get; set; }

        public PartyInfoViewModel() { }
        public PartyInfoViewModel(Entities.Party party)
        {
            var citizen = SessionHelper.LoggedCitizen;
            var entity = SessionHelper.CurrentEntity;

            var isActingAsParty = entity.EntityID == party.ID;
            PartyRoleEnum userPartyRole = PartyRoleEnum.NotAMember;
            if(citizen.PartyMember != null && citizen.PartyMember.PartyID == party.ID)
            {
                userPartyRole = (PartyRoleEnum)citizen.PartyMember.PartyRoleID;
            }

            PartyAvatar = new ImageViewModel(party.Entity.ImgUrl);
            PartyName = party.Entity.Name;
            PartyCountry = party.Country.Entity.Name;
            JoinMethod = (JoinMethodEnum)party.JoinMethodID;
            PartyRole = userPartyRole;
            PartyID = party.ID;
            IsActingAsParty = isActingAsParty;

            MemberCount = party.PartyMembers.Count;
            CongressmenCount = party.PartyMembers.Where(pm => pm.Citizen.Congressmen.Any(c => c.CountryID == party.CountryID)).Count();

            if (entity.GetEntityType() == EntityTypeEnum.Citizen && PartyRole == PartyRoleEnum.NotAMember)
            {
                var partyService = DependencyResolver.Current.GetService<IPartyService>();
                if (party.JoinMethodID == (int)JoinMethodEnum.Invite)
                {
                    var invite = citizen
                        .PartyInvites.FirstOrDefault(i => i.PartyID == party.ID);
                    if (invite != null)
                    {
                        CanAcceptJoinOffer = true;
                        InviteID = invite.ID;
                    }
                }
                else if (party.JoinMethodID == (int)JoinMethodEnum.Request)
                {
                    var request = citizen.PartyJoinRequests.FirstOrDefault(i => i.PartyID == party.ID);
                    if (request != null)
                    {
                        HasSendJoinRequest = true;
                        RequestID = request.ID;
                    }
                    else if(partyService.CanSendJoinRequest(citizen, party, message: "").isSuccess)
                        CanSendJoinRequest = true;
                }

            }

            if (PartyRole >= PartyRoleEnum.Manager)
            {
                AvatarChange = new AvatarChangeViewModel(party.ID);
            }

            CanSeeInvites = JoinMethod == JoinMethodEnum.Invite && PartyRole >= PartyRoleEnum.Manager;
            CanSeeRequests = JoinMethod == JoinMethodEnum.Request && PartyRole >= PartyRoleEnum.Manager;

            createMenu();
        }

        private void createMenu()
        {
            if (PartyRole == PartyRoleEnum.NotAMember && JoinMethod == JoinMethodEnum.Open && SessionHelper.CurrentEntity.Is(EntityTypeEnum.Citizen))
            {
                Menu.AddItem(new InfoActionViewModel("Join", "Party", "Join", "fa-plus-square-o", FormMethod.Post, new { partyID = PartyID }));   
            }

            if (CanAcceptJoinOffer)
            {
                Menu.AddItem(new InfoActionViewModel("AcceptInviteNoAjax", "Party", "Accept Invite", "fa-handshake-o", FormMethod.Post, new { inviteID = InviteID }));
            }

            if (CanSendJoinRequest)
            {
                Menu.AddItem(new InfoCustomActionViewModel("Send Join Request", "fa-external-link", onClick: "openJoinRequest()"));
            }

            if (CanSeeInvites)
            {
                Menu.AddItem(new InfoActionViewModel("PartyInvites", "Party", "Invites", "fa-list-alt", new { partyID = PartyID }));
            }

            if (CanSeeRequests)
            {
                Menu.AddItem(new InfoActionViewModel("JoinRequests", "Party", "Requests", "fa-list-alt", new { partyID = PartyID }));
            }

            if (HasSendJoinRequest)
            {
                Menu.AddItem(new InfoActionViewModel("CancelJoinRequest", "Party", "Cancel Join Request", "fa-windows-close-o", FormMethod.Post, new { requestID = RequestID }));
            }

            if (PartyRole != PartyRoleEnum.NotAMember)
            {
                Menu.AddItem(new InfoActionViewModel("Leave", "Party", "Leave", "fa-minus-square-o", FormMethod.Post, new { partyID = PartyID }));
            }

            if (PartyRole >= PartyRoleEnum.Manager)
            {
                Menu.AddItem(new InfoActionViewModel("Manage", "Party", "Manage", "fa-cog", new { partyID = PartyID }));
            }

            if (PartyRole >= PartyRoleEnum.Manager && IsActingAsParty == false)
            {
                Menu.AddItem(InfoActionViewModel.CreateEntitySwitch(PartyID));
            }

            Menu.AddItem(new InfoActionViewModel("CongressCandidates", "Party", "Congress Candidates", "fa-users", new { partyID = PartyID }));

            if (SessionHelper.CurrentEntity.EntityID != PartyID)
                Menu.AddItem(InfoExpandableViewModel.CreateExchange(PartyID));
        }
    }
}