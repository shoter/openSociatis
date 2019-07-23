using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities.enums;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewChangePresidentCadenceLengthViewModel : ViewVotingBaseViewModel
    {
        public int CadenceLength { get; set; }


        public ViewChangePresidentCadenceLengthViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
            :base(voting, isPlayerCongressman, canVote)
        {
            CadenceLength = int.Parse(voting.Argument1);
        }
    }
}