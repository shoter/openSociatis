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
    public class ChangeNormalJobMarketFeeViewModel : CongressVotingViewModel
    {
        [DisplayName("New normal job market fee")]
        [Range(0, 1000)]
        public double NewJobFee { get; set; }



        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeNormalJobMarketFee;

        public ChangeNormalJobMarketFeeViewModel() { }
        public ChangeNormalJobMarketFeeViewModel(int countryID) : base(countryID) { }
        public ChangeNormalJobMarketFeeViewModel(CongressVoting voting)
        :base(voting)
    {
        }

        public ChangeNormalJobMarketFeeViewModel(FormCollection values)
        :base(values)
        {
        }


        public override StartCongressVotingParameters CreateVotingParameters()
        {
            return new ChangeNormalJobMarketFeeParameters(NewJobFee);
        }
    }
}