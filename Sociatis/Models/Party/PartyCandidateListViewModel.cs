using Entities;
using Entities.enums;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Party
{
    public class PartyCandidateListViewModel : List<PartyCandidateViewModel>
    {
        public PartyInfoViewModel Info { get; set; }
        public VotingStatusEnum VotingStatus { get; set; }
        public bool CanVote { get; set; } = false;

        public PartyCandidateListViewModel(PartyPresidentVoting voting)
        {
            var candidates = voting.PartyPresidentCandidates.ToList();
            Info = new PartyInfoViewModel(voting.Party);

            foreach(var candidate in candidates)
            {
                Add(new PartyCandidateViewModel(candidate));
            }

            VotingStatus = (VotingStatusEnum)voting.VotingStatusID;

            var entity = SessionHelper.CurrentEntity;

            if(entity.EntityTypeID == (int)(EntityTypeEnum.Citizen) && voting.VotingStatusID == (int)VotingStatusEnum.Ongoing)
            {
                if (voting.PartyPresidentVotes.Any(v => v.CitizenID == entity.EntityID) == false)
                    CanVote = true;
            }
        }
    }
}