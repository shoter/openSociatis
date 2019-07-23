using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.structs.Votings;

namespace Sociatis.Models.Congress
{
    public class ChangeCitizenStartingMoneyViewModel : CongressVotingViewModel
    {
        [Range(0, 1000000)]
        public decimal Fee { get; set; }
        public string CurrencySymbol { get; set; }



        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeCitizenStartingMoney;

        public ChangeCitizenStartingMoneyViewModel()
        {
           
        }
        public ChangeCitizenStartingMoneyViewModel(int countryID) : base(countryID)
        {
            loadCurrency(CountryID);
        }
        public ChangeCitizenStartingMoneyViewModel(CongressVoting voting)
        : base(voting)
        {
            loadCurrency(CountryID);
        }

        public ChangeCitizenStartingMoneyViewModel(FormCollection values)
        : base(values)
        {
            loadCurrency(CountryID);
        }

        void loadCurrency(int countryID)
        {
            var currency = Persistent.Countries.GetCountryCurrency(countryID);
            CurrencySymbol = currency.Symbol;
        }


        public override StartCongressVotingParameters CreateVotingParameters()
        {
            return new ChangeCitizenStartingMoneyVotingParameters(Fee);
        }
    }
}