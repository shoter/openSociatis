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
    public class ChangePresidentCadenceLengthViewModel : CongressVotingViewModel
    {
        [DisplayName("New president cadence length")]
        [Range(7, 90)]
        public int NewCadenceLength { get; set; }




        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangePresidentCadenceLength;
        public ChangePresidentCadenceLengthViewModel() { }
        public ChangePresidentCadenceLengthViewModel(int countryID) : base(countryID) { }
        public ChangePresidentCadenceLengthViewModel(CongressVoting voting)
        :base(voting)
        {
        }

        public ChangePresidentCadenceLengthViewModel(FormCollection values)
        :base(values)
        {
        }
        public override StartCongressVotingParameters CreateVotingParameters()
        {
            return new ChangePresidentCadenceLengthVotingParameters(NewCadenceLength);
        }
    }
}