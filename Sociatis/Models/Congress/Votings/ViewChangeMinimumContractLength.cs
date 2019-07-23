using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities;
using Entities.enums;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewChangeMinimumContractLength : ViewVotingBaseViewModel
    {
        public int NewLength { get; set; }

        public ViewChangeMinimumContractLength(CongressVoting voting, bool isPlayerCongressman, bool canVote) : base(voting, isPlayerCongressman, canVote)
        {
            NewLength = int.Parse(voting.Argument1);
        }
    }
}