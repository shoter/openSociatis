using Common.utilities;
using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices.Helpers;

namespace Sociatis.Models.Congress
{
    public class CongressVotingsItemViewModel
    {
        public string ProposalName { get; set; }
        public string CreatorName { get; set; }
        public int CreatorID { get; set; }
        public string Ago { get; set; }
        public string StatusHumanReadable { get; set; }
        public int ID { get; set; }

        public CongressVotingsItemViewModel(CongressVoting voting)
        {
            ProposalName = string.Format("Proposal #{0} : {1}", voting.ID,((VotingTypeEnum)voting.VotingTypeID).ToHumanReadable());
            CreatorName = voting.Citizen.Entity.Name;
            CreatorID = voting.CreatorID;
            Ago = AgoHelper.DayAgo(GameHelper.CurrentDay, voting.StartDay);
            ID = voting.ID;

            StatusHumanReadable = ((VotingStatusEnum)voting.VotingStatusID).ToHumanReadable();
        }

    }
}