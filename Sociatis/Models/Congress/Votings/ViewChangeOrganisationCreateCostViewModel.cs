using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities.enums;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewChangeOrganisationCreateCostViewModel : ViewVotingBaseViewModel
    {
        public double CreateCost { get; set; }
        public string CurrencySymbol { get; set; }


        public ViewChangeOrganisationCreateCostViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
            :base(voting, isPlayerCongressman, canVote)
        {
            CreateCost = double.Parse(voting.Argument1);
            CurrencySymbol = voting.Country.Currency.Symbol;
        }
    }
}