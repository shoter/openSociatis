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
    public class ChangeArticleTaxViewModel : CongressVotingViewModel
    {
        [DisplayName("New article tax")]
        [Range(0, 10000)]
        public double NewTax { get; set; }

        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeArticleTax;

        public ChangeArticleTaxViewModel() { }
        public ChangeArticleTaxViewModel(int countryID) : base(countryID) { }

        public ChangeArticleTaxViewModel(CongressVoting voting)
        :base(voting)
        {
        }

        public ChangeArticleTaxViewModel(FormCollection values)
        :base(values)
        {
        }
        public override StartCongressVotingParameters CreateVotingParameters()
        {
            return new ChangeArticleTaxVotingParameters(NewTax);
        }
    }
}