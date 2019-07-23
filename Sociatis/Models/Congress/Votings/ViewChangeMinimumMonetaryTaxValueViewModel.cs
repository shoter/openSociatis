using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.enums;
using WebServices;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewChangeMinimumMonetaryTaxValueViewModel : ViewVotingBaseViewModel
    {
        public string CurrencySymbol { get; set; }
        public double NewTaxValue { get; set; }

        public ViewChangeMinimumMonetaryTaxValueViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
        : base(voting, isPlayerCongressman, canVote)
        {
            CurrencySymbol = Persistent.Countries.GetCountryCurrency(voting.CountryID).Symbol;
            NewTaxValue = double.Parse(voting.Argument1);
        }
    }
}
