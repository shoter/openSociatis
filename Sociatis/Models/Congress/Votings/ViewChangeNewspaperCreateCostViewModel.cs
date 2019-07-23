using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities.enums;
using WebServices;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewChangeNewspaperCreateCostViewModel : ViewVotingBaseViewModel
    {
        public double NewCreateCost { get; set; }
        public string CurrencySymbol { get; set; }

        public ViewChangeNewspaperCreateCostViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
            :base(voting, isPlayerCongressman, canVote)
        {
            NewCreateCost = double.Parse(voting.Argument1);
            CurrencySymbol = Persistent.Countries.GetCountryCurrency(voting.CountryID).Symbol;
        }
    }
}