using Entities;
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
    public class ChangePartyPresidentCadenceLengthViewModel : CongressVotingViewModel
    {
        [DisplayName("New cadence length")]
        [Range(15, 90)]
        public int NewCadenceLength { get; set; }



        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangePartyPresidentCadenceLength;

        public ChangePartyPresidentCadenceLengthViewModel() { }
        public ChangePartyPresidentCadenceLengthViewModel(int countryID) : base(countryID) { }
        public ChangePartyPresidentCadenceLengthViewModel(CongressVoting voting)
        :base(voting)
    {
        }

        public ChangePartyPresidentCadenceLengthViewModel(FormCollection values)
        :base(values)
        { 
        }


        public override StartCongressVotingParameters CreateVotingParameters()
        {
            return new ChangePartyPresidentCadenceLengthVotingParameters(NewCadenceLength);
        }
    }
}