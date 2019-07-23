using Entities;
using Entities.enums;
using Sociatis.Helpers;
using Sociatis.Models.Citizens;
using Sociatis.Models.Congress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices.Helpers;
using Entities.Extensions;
using WebUtils.Forms.Select2;
using Sociatis.Controllers;

namespace Sociatis.Models.Party
{
    public class ViewPartyViewModel
    {
        public PartyInfoViewModel OverallInfo { get; set; }
        public int PartyID { get; set; }
        public ShortEntityInfoViewModel President { get; set; } = null;
        public List<ShortEntityInfoViewModel> Managers { get; set; } = new List<ShortEntityInfoViewModel>();
        public List<ShortEntityInfoViewModel> Members { get; set; } = new List<ShortEntityInfoViewModel>();
        public int OtherMemberCount { get; set; }
        public PartyRoleEnum PartyRole { get; set; }
        public bool CanCandidate { get; set; } = false;
        public JoinMethodEnum JoinMethod { get; set; }

        public int PendingInvites { get; set; }

        public int VotingDay { get; set; }
        public int DaysLeft { get; set; }
        public bool CanStartAsCandidate { get; set; } = true;
        public VotingStatusEnum VotingStatus { get; set; }
        public CongressCandidateVotingShortViewModel CongressCandidateVotingViewModel { get; set; }
        public bool Voted { get; set; } = false;
        public bool IsCandidate { get; set; }

        public Select2AjaxViewModel InviteSelector { get; set; }


        public ViewPartyViewModel(Entities.Party party)
        {
            InviteSelector = Select2AjaxViewModel.Create<CitizenController>(c => c.GetCitizens(null), "citizenID", null, "");
            InviteSelector.OnChange = "inviteCitizen";

            OverallInfo = new PartyInfoViewModel(party);
            JoinMethod = (JoinMethodEnum)party.JoinMethodID;
            PartyRole = OverallInfo.PartyRole;
            PartyID = party.ID;

            var members = party.PartyMembers
                .ToList();

            var president = members
                .FirstOrDefault(m => m.PartyRoleID == (int)PartyRoleEnum.President);
            if (president != null)
            {
                President = new ShortEntityInfoViewModel(president.Citizen.Entity);
            }

            Managers = members
                .Where(m => m.PartyRoleID == (int)PartyRoleEnum.Manager)
                .Take(3)
                .Select(m => new ShortEntityInfoViewModel(m.Citizen.Entity))
                .ToList();

            if(Managers.Count < 3)
            {
                Members = members
                .Where(m => m.PartyRoleID == (int)PartyRoleEnum.Member )
                .Take(3 - Managers.Count)
                .Select(m => new ShortEntityInfoViewModel(m.Citizen.Entity))
                .ToList();
            }

            OtherMemberCount = members.Count() - Managers.Count - Members.Count - (president == null ? 0 : 1);

            var partyVoting = party.PartyPresidentVotings.Last();
            var entity = SessionHelper.CurrentEntity;

            if (entity.EntityTypeID == (int)EntityTypeEnum.Citizen)
            {
                if (partyVoting.PartyPresidentCandidates.Any(c => c.CitizenID == entity.EntityID))
                {
                    CanStartAsCandidate = false;
                }
            }

            VotingDay = partyVoting.VotingDay;
            DaysLeft = Math.Abs(GameHelper.CurrentDay - VotingDay);
            var candidatesID = partyVoting.PartyPresidentCandidates
                .Select(c => c.CitizenID);

            VotingStatus = (VotingStatusEnum)partyVoting.VotingStatusID;
            IsCandidate = candidatesID.Contains(entity.EntityID);
            if (VotingStatus == VotingStatusEnum.NotStarted && entity.EntityTypeID == (int)EntityTypeEnum.Citizen &&
                 IsCandidate  == false
                )
            {
                CanCandidate = true;
            }

            if(VotingStatus == VotingStatusEnum.Ongoing)
            {
                if (partyVoting.PartyPresidentVotes.Any(v => v.CitizenID == entity.EntityID) == true)
                    Voted = true;
            }

            var country = party.Country;
            var congressVoting = country.GetLastCongressCandidateVoting();

            CongressCandidateVotingViewModel = new CongressCandidateVotingShortViewModel(congressVoting);

            InviteSelector.AddAdditionalData("partyid", PartyID);

            if (JoinMethod == JoinMethodEnum.Invite && PartyRole != PartyRoleEnum.NotAMember)
            {
                PendingInvites = party.PartyInvites.Count();
            }
        }


    }
}