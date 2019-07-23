using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities.enums;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewChangeNormalCongressVotingWinPercentageViewModel : ViewVotingBaseViewModel
    {
        public double WinPercentage { get; set; }

        public ViewChangeNormalCongressVotingWinPercentageViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
            :base(voting, isPlayerCongressman, canVote)
        {
            WinPercentage = double.Parse(voting.Argument1);
        }
    }
}