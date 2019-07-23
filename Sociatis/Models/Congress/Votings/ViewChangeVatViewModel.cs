using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities.enums;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewChangeVatViewModel : ViewVotingBaseViewModel
    {
        public double NewVat { get; set; }


        public ViewChangeVatViewModel( Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
            :base(voting, isPlayerCongressman, canVote)
        {
            NewVat = double.Parse(voting.Argument1);
        }
    }
}