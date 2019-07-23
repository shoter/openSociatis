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
    public class ChangeCongressVotingLengthViewModel : CongressVotingViewModel
    {
        [DisplayName("New congress voting length")]
        [Range(12, 48)]
        public int NewCongressVotingLength { get; set; }



        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeCongressVotingLength;

        public ChangeCongressVotingLengthViewModel() { }
        public ChangeCongressVotingLengthViewModel(int countryID) : base(countryID) { }
        public ChangeCongressVotingLengthViewModel(CongressVoting voting)
            :base(voting)
        {
        }

        public ChangeCongressVotingLengthViewModel(FormCollection values)
            :base(values)
        {
        }


        public override StartCongressVotingParameters CreateVotingParameters()
        {
            return new ChangeCongressVotingLnegthParameters(NewCongressVotingLength);
        }
    }
}