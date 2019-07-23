using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities.enums;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewChangeCongressVotingLengthViewModel : ViewVotingBaseViewModel
    {
        public int NewCongressVotingLength { get; set; }

        public ViewChangeCongressVotingLengthViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
            :base(voting, isPlayerCongressman, canVote)
        {
            NewCongressVotingLength = int.Parse(voting.Argument1);
        }
    }
}