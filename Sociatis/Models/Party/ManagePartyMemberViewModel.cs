using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Party
{
    public class ManagePartyMemberViewModel
    {
        public string Name { get; set; }
        public int CitizenID { get; set; }
        public PartyRoleEnum PartyRole { get; set; }
        public bool Kick { get; set; } = false;
        public ImageViewModel Avatar { get; set; }

        public ManagePartyMemberViewModel() { }

        public ManagePartyMemberViewModel(PartyMember member)
        {
            Name = member.Citizen.Entity.Name;
            CitizenID = member.CitizenID;
            PartyRole = (PartyRoleEnum)member.PartyRoleID;
            Avatar = new ImageViewModel(member.Citizen.Entity.ImgUrl);
        }

    }
}