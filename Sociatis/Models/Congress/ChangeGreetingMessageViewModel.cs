using Entities;
using Entities.enums;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices.structs.Votings;

namespace Sociatis.Models.Congress
{
    public class ChangeGreetingMessageViewModel : CongressVotingViewModel
    {
        [DisplayName("Greeting Message")]
        public string GreetingMessage { get; set; }
        public string CurrentMessage { get; set; }


        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeGreetingMessage;

        public ChangeGreetingMessageViewModel() { }
        public ChangeGreetingMessageViewModel(int countryID) : base(countryID)
        {
            loadCurrentMessage(CountryID);
        }
        public ChangeGreetingMessageViewModel(CongressVoting voting)
        : base(voting)
        {
            loadCurrentMessage(CountryID);
        }

        public ChangeGreetingMessageViewModel(FormCollection values)
        : base(values)
        {
            loadCurrentMessage(CountryID);
        }

        void loadCurrentMessage(int countryID)
        {
            var countryRepository = DependencyResolver.Current.GetService<ICountryRepository>();

            CurrentMessage = countryRepository.GetById(countryID).GreetingMessage;
        }

        void loadCurrentMessage()
        {
            var countryRepository = DependencyResolver.Current.GetService<ICountryRepository>();
        }


        public override StartCongressVotingParameters CreateVotingParameters()
        {

            return new ChangeGreetingMessageVotingParameters(GreetingMessage);
        }
    }
}