using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities.enums;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewChangeHotelTaxViewModel : ViewVotingBaseViewModel
    {
        public decimal NewTax { get; set; }

        public ViewChangeHotelTaxViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
            : base(voting, isPlayerCongressman, canVote)
        {
            NewTax = decimal.Parse(voting.Argument1);
        }
    }
}
