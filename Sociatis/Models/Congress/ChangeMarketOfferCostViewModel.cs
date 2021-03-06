﻿using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices.structs.Votings;

namespace Sociatis.Models.Congress
{
    public class ChangeMarketOfferCostViewModel : CongressVotingViewModel
    {
        [DisplayName("New market offer cost")]
        [Range(0, 10000)]
        public double OfferCost { get; set; }




        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeMarketOfferCost;

        public ChangeMarketOfferCostViewModel() { }
        public ChangeMarketOfferCostViewModel(int countryID) : base(countryID) { }
        public ChangeMarketOfferCostViewModel(CongressVoting voting)
        :base(voting)
        {
        }

        public ChangeMarketOfferCostViewModel(FormCollection values)
        :base(values)
        {
        }
        public override StartCongressVotingParameters CreateVotingParameters()
        {
            return new ChangeMarketOfferCostVotingParameters(OfferCost);
        }
    }
}