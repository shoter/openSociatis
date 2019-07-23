using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebServices.structs.Votings;

namespace Sociatis.Models.Congress
{
    public class ChangeMinimumMonetaryTaxRateViewModel : CongressVotingViewModel
    {
        [DisplayName("New tax value")]
        [Range(0, 100000)]
        public double NewTaxRate { get; set; }

        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeMonetaryTaxRate;

        public ChangeMinimumMonetaryTaxRateViewModel() { }
        public ChangeMinimumMonetaryTaxRateViewModel(int countryID) : base(countryID) { }
        public ChangeMinimumMonetaryTaxRateViewModel(CongressVoting voting)
        : base(voting) { }

        public ChangeMinimumMonetaryTaxRateViewModel(FormCollection values)
        : base(values) { }


        public override StartCongressVotingParameters CreateVotingParameters()
        {
            return new ChangeMinimumMonetaryTaxRateParameters(NewTaxRate);
        }
    }
}
