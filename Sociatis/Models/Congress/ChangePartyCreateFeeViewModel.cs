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
    public class ChangePartyCreateFeeViewModel : CongressVotingViewModel
    {
        [DisplayName("New party creation fee")]
        [Range(0, 10000)]
        public double CreationFee { get; set; }

        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangePartyCreateFee;

        public ChangePartyCreateFeeViewModel() { }
        public ChangePartyCreateFeeViewModel(int countryID) : base(countryID) { }

        public ChangePartyCreateFeeViewModel(CongressVoting voting)
        :base(voting)
        {
        }

        public ChangePartyCreateFeeViewModel(FormCollection values)
        :base(values)
        {
        }
        public override StartCongressVotingParameters CreateVotingParameters()
        {
            return new ChangePartyCreateFeeVotingParameters(CreationFee);
        }
    }
}