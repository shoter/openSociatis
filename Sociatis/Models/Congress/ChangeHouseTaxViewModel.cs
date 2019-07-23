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
    public class ChangeHouseTaxViewModel : CongressVotingViewModel
    {
        [DisplayName("New house tax")]
        [Range(0, 10000)]
        public decimal NewTax { get; set; }

        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeHouseTax;

        public ChangeHouseTaxViewModel() { }
        public ChangeHouseTaxViewModel(int countryID) : base(countryID) { }

        public ChangeHouseTaxViewModel(CongressVoting voting)
        : base(voting)
        {
        }

        public ChangeHouseTaxViewModel(FormCollection values)
        : base(values)
        {
        }
        public override StartCongressVotingParameters CreateVotingParameters()
        {
            return new ChangeHouseTaxVotingParameters(NewTax);
        }
    }
}