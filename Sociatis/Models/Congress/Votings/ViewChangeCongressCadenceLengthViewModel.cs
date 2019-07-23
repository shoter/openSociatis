using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities.enums;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewChangeCongressCadenceLengthViewModel : ViewVotingBaseViewModel
    {
        public int NewCadenceLength { get; set; }

        public ViewChangeCongressCadenceLengthViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
        :base(voting, isPlayerCongressman, canVote)
        {
            NewCadenceLength = int.Parse(voting.Argument1);
        }
    }
}