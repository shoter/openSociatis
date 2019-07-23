using Entities;
using Sociatis.Models.Entit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Party
{
    public class PartyPendingInviteListViewModel
    {
        public PartyInfoViewModel Info { get; set; }
        public List<SmallEntityAvatarViewModel> CitizensWithInvites { get; set; }

        public PartyPendingInviteListViewModel(Entities.Party party, List<Entity> citizens)
        {
            Info = new PartyInfoViewModel(party);
            CitizensWithInvites = citizens.Select(c => new SmallEntityAvatarViewModel(c)).ToList();
        }
    }
}