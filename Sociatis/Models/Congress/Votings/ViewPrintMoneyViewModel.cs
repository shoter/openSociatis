using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities.enums;
using WebServices;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewPrintMoneyViewModel : ViewVotingBaseViewModel
    {
        public int MoneyAmount { get; set; }
        public string CurrencySymbol { get; set; }
        public double GoldCost { get; set; }


        public ViewPrintMoneyViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
            : base(voting, isPlayerCongressman, canVote)
        {
            MoneyAmount = int.Parse(voting.Argument1);
            CurrencySymbol = Persistent.Countries.GetCountryCurrency(voting.CountryID).Symbol;
            GoldCost = double.Parse(voting.Argument2);
        }
    }

}