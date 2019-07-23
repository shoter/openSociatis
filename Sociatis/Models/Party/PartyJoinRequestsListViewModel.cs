using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Party
{
    public class PartyJoinRequestsListViewModel
    {
        public PartyInfoViewModel Info { get; set; }
        public List<PartyJoinRequestsListItemViewModel> Requests { get; set; }

        public PartyJoinRequestsListViewModel(Entities.Party party, List<PartyJoinRequest> requests)
        {
            Info = new PartyInfoViewModel(party);
            Requests = requests.Select(request => new PartyJoinRequestsListItemViewModel(request)).ToList();
        }
    }
}