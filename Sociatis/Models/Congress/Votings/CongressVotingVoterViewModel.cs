using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Congress.Votings
{
    public class CongressVotingVoterViewModel
    {
        public ImageViewModel Avatar { get; set; }
        public string Name { get; set; }
        public string PartyName { get; set; }

        public CongressVotingVoterViewModel(Entities.Citizen citizen)
        {
            Avatar = new ImageViewModel(citizen.Entity.ImgUrl);
            Name = citizen.Entity.Name;

            if (citizen.PartyMember != null)
                PartyName = citizen.PartyMember.Party.Entity.Name;
        }
    }
}