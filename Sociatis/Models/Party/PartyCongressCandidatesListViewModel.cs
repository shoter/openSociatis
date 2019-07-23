using Entities;
using Entities.enums;
using Sociatis.Helpers;
using Sociatis.Models.Congress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Party
{
    public class PartyCongressCandidatesListViewModel : CongressCandidatesListViewModel
    {
        public bool CanAcceptCandidates { get; set; } = false;
        public PartyRoleEnum PartyRole { get; set; }

        public PartyCongressCandidatesListViewModel(List<CongressCandidate> candidates, IPartyService partyService, Entities.Party party)
            :base(candidates)
        {
            PartyRole = PartyRoleEnum.NotAMember;
            if (SessionHelper.CurrentEntity.Citizen != null)
            {
                PartyRole = partyService.GetPartyRole(SessionHelper.LoggedCitizen, party);
                if (partyService.CanAcceptCongressCandidates(SessionHelper.CurrentEntity.Citizen, party))
                    CanAcceptCandidates = true;
            }
        }
    }
}