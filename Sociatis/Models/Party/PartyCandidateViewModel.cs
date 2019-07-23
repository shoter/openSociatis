using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Party
{
    public class PartyCandidateViewModel : ShortEntityInfoViewModel
    {
        public int VoteCount { get; set; }
        public int CandidateID { get; set; }

        public PartyCandidateViewModel(PartyPresidentCandidate candidate)
            :base(candidate.Citizen.Entity)
        {
            VoteCount = candidate.PartyPresidentVotes.Count();
            CandidateID = candidate.ID;
        }
    }
}