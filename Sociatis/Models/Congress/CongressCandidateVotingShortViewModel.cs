using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities.Extensions;
using Common.utilities;
using WebServices.Helpers;
using Entities.enums;

namespace Sociatis.Models.Congress
{
    public class CongressCandidateVotingShortViewModel
    {
        public int VotingDay { get; set; }
        public int DaysLeft { get; set; }
        public VotingStatusEnum VotingStatus { get; set; }

        public CongressCandidateVotingShortViewModel(Entities.CongressCandidateVoting voting)
        {
            VotingDay = voting.VotingDay;
            DaysLeft = voting.VotingDay - GameHelper.CurrentDay;
            VotingStatus = (VotingStatusEnum)voting.VotingStatusID;
        }
    }
}