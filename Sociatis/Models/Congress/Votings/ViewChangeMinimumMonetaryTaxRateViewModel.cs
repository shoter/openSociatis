using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.enums;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewChangeMinimumMonetaryTaxRateViewModel : ViewVotingBaseViewModel
    {
        public double NewTaxRate { get; set; }

        public ViewChangeMinimumMonetaryTaxRateViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
        : base(voting, isPlayerCongressman, canVote)
        {
            NewTaxRate = double.Parse(voting.Argument1);
        }
    }
}
