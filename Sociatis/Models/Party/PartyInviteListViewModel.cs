using Common.Extensions;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Party
{
    public class PartyInviteListViewModel
    {
        public List<PartyInviteListItemViewModel> Invites { get; set; } = new List<PartyInviteListItemViewModel>();

        public PartyInviteListViewModel(IQueryable<PartyInvite> invites)
        {
            Invites = invites.ToList(invite => new PartyInviteListItemViewModel(invite));
        }
    }
}