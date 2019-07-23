using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities.enums;
using WebServices;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewChangeMinimalWageViewModel : ViewVotingBaseViewModel
    {
        public double NewMinimalWage { get; set; }
        public string CurrencySymbol { get; set; }

        public ViewChangeMinimalWageViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
        : base(voting, isPlayerCongressman, canVote)
        {
            NewMinimalWage = double.Parse(voting.Argument1);
            CurrencySymbol = Persistent.Countries.GetCountryCurrency(voting.CountryID).Symbol;
        }


    }
}