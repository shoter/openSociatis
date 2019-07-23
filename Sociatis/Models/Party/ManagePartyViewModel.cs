using Entities;
using Entities.enums;
using Sociatis.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sociatis.Models.Party
{
    public class ManagePartyViewModel : ViewModelBase
    {
        public int PartyID { get; set; }
        public PartyInfoViewModel OverallInfo { get; set; }
        public List<ManagePartyMemberViewModel> Members { get; set; } = new List<ManagePartyMemberViewModel>();
        public ImageViewModel PartyAvatar { get; set; }
        public JoinMethodEnum JoinMethod { get; set; }
        public PartyRoleEnum UserPartyRole { get; set; }

        public List<SelectListItem> PartyRoles { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> JoinMethods { get; set; } = new List<SelectListItem>();

        public ManagePartyViewModel() { }
        public ManagePartyViewModel(Entities.Party party, List<PartyRole> partyRoles, List<JoinMethod> joinMethods)
        {
            OverallInfo = new PartyInfoViewModel(party);
            this.UserPartyRole = OverallInfo.PartyRole;
            this.JoinMethod = (JoinMethodEnum)party.JoinMethodID;
            this.PartyAvatar = OverallInfo.PartyAvatar;
            this.PartyID = party.ID;

            partyRoles = partyRoles.Where(r => r.ID < (int)OverallInfo.PartyRole).ToList();

            foreach (var member in party.PartyMembers
                .Where(m => m.PartyRoleID < (int)UserPartyRole)
                
                .ToList())
            {
                Members.Add(new ManagePartyMemberViewModel(member));
            }

            PartyRoles = CreateSelectList(partyRoles, x => ((PartyRoleEnum)x.ID).ToHumanReadable(), x => x.ID, true);
            JoinMethods = CreateSelectList(joinMethods);
        }

    }
}