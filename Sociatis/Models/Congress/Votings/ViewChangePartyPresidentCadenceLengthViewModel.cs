using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities.enums;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewChangePartyPresidentCadenceLengthViewModel : ViewVotingBaseViewModel
    {
        public int NewCadenceLength { get; set; }


        public ViewChangePartyPresidentCadenceLengthViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
            :base(voting, isPlayerCongressman, canVote)
        {
            NewCadenceLength = int.Parse(voting.Argument1);
        }
    }
}