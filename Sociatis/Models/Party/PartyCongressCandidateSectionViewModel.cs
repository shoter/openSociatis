using Sociatis.Models.Congress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities;
using WebServices;
using Sociatis.Helpers;
using Entities.Extensions;
using Entities.enums;
using WebServices.Helpers;

namespace Sociatis.Models.Party
{
    public class PartyCongressCandidateSectionViewModel : CongressCandidateSectionViewModel
    {
        public new PartyInfoViewModel Info { get; set; }
        public bool CanCandidate { get; set; } = false;
        public CongressCandidateStatusEnum PlayerCongressCandidateStatus { get; set; } = CongressCandidateStatusEnum.None;
        public string CandidateRegionName { get; set; } = string.Empty;
        public int PartyID { get; set; }
        public bool CanAcceptCandidates { get; set; } = false;

        

        public PartyCongressCandidateSectionViewModel(Entities.Party party, IPartyService partyService) : base(party.Country)
        {
            Info = new PartyInfoViewModel(party);
            PartyID = party.ID;

            var entity = SessionHelper.CurrentEntity;
            var lastVoting = party.Country.GetLastCongressCandidateVoting();
            
            
            var candidate = lastVoting.CongressCandidates.FirstOrDefault(c => c.CandidateID == entity.EntityID);

            if (candidate != null)
                PlayerCongressCandidateStatus = (CongressCandidateStatusEnum)candidate.CongressCandidateStatusID;
            if (entity.Citizen != null)
            {
                if(partyService.CanAcceptCongressCandidates(entity.Citizen, party))
                {
                    CanAcceptCandidates = true;
                }

                if (partyService.CanCandidateToCongress(entity.Citizen, party))
                {
                    CanCandidate = true;
                    CandidateRegionName = entity.GetCurrentRegion().Name;
                }
            }
        }
    }
}