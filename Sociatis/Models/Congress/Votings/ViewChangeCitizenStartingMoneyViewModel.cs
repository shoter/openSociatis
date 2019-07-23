using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities.enums;
using WebServices;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewChangeCitizenStartingMoneyViewModel : ViewVotingBaseViewModel
    {
        public decimal Fee { get; set; }
        public string CurrencySymbol { get; set; }

        public ViewChangeCitizenStartingMoneyViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
        :base(voting, isPlayerCongressman, canVote)
    {
            Fee = decimal.Parse(voting.Argument1);
            CurrencySymbol = Persistent.Countries.GetCountryCurrency(voting.CountryID).Symbol;
        }
    }
}