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
    public class ChangeHotelTaxViewModel : CongressVotingViewModel
    {
        [DisplayName("New hotel tax")]
        [Range(0, 10000)]
        public decimal NewTax { get; set; }

        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeHotelTax;

        public ChangeHotelTaxViewModel() { }
        public ChangeHotelTaxViewModel(int countryID) : base(countryID) { }

        public ChangeHotelTaxViewModel(CongressVoting voting)
        :base(voting)
        {
        }

        public ChangeHotelTaxViewModel(FormCollection values)
        :base(values)
        {
        }
        public override StartCongressVotingParameters CreateVotingParameters()
        {
            return new ChangeHotelTaxVotingParameters(NewTax);
        }
    }
}