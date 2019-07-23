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
    public class ChangeCongressCadenceLengthViewModel : CongressVotingViewModel
    {
        [DisplayName("New congress cadence length")]
        [Range(15, 90)]
        public int NewCadenceLength { get; set; }



        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeCongressCadenceLength;

        public ChangeCongressCadenceLengthViewModel() { }
        public ChangeCongressCadenceLengthViewModel(int countryID) : base(countryID) { }
        public ChangeCongressCadenceLengthViewModel(CongressVoting voting)
        :base(voting)
    {
        }

        public ChangeCongressCadenceLengthViewModel(FormCollection values)
        :base(values)
        { 
        }


        public override StartCongressVotingParameters CreateVotingParameters()
        {
            return new ChangeCongressCadenceLengthParameters(NewCadenceLength);
        }
    }
}