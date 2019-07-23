using Entities;
using Entities.enums;
using Sociatis.Models.Entit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Party
{
    public class PartyMemberListViewModel
    {
        public PartyInfoViewModel Info { get; set; }

        public SmallEntityAvatarViewModel President { get; set; }
        public List<SmallEntityAvatarViewModel> Managers { get; set; }
        public List<SmallEntityAvatarViewModel> Members { get; set; }

        public PartyMemberListViewModel(Entities.Party party, List<PartyMember> members)
        {
            Info = new PartyInfoViewModel(party);

            var president = members.FirstOrDefault(m => m.PartyRoleID == (int)PartyRoleEnum.President);
            if (president != null)
                President = new SmallEntityAvatarViewModel(president.Citizen.Entity);

            Managers = members.Where(m => m.PartyRoleID == (int)PartyRoleEnum.Manager)
                .Select(m => new SmallEntityAvatarViewModel(m.Citizen.Entity)).ToList();

            Members = members.Where(m => m.PartyRoleID == (int)PartyRoleEnum.Member)
                .Select(m => new SmallEntityAvatarViewModel(m.Citizen.Entity)).ToList();
        }
    }
}