using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities.enums;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewChangeMarketOfferCostViewModel : ViewVotingBaseViewModel
    {
        public double OfferCost { get; set; }
        public string CurrencySymbol { get; set; }

        public ViewChangeMarketOfferCostViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
            :base(voting, isPlayerCongressman, canVote)
        {
            OfferCost = double.Parse(voting.Argument1);
            CurrencySymbol = voting.Country.Currency.Symbol;
        }
    }
}