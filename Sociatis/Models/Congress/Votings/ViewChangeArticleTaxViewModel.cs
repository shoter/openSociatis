using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities.enums;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewChangeArticleTaxViewModel : ViewVotingBaseViewModel
    {
        public double NewTax { get; set; }

        public ViewChangeArticleTaxViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
            :base(voting, isPlayerCongressman, canVote)
        {
            NewTax = double.Parse(voting.Argument1);
        }
    }
}
