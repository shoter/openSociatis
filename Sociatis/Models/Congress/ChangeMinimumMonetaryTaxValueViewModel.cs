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
    public class ChangeMinimumMonetaryTaxValueViewModel : CongressVotingViewModel
    {
        [DisplayName("New tax value")]
        [Range(0, 100000)]
        public double NewTaxValue { get; set; }

        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeMinimumMonetaryTaxValue;

        public ChangeMinimumMonetaryTaxValueViewModel() { }
        public ChangeMinimumMonetaryTaxValueViewModel(int countryID) : base(countryID) { }
        public ChangeMinimumMonetaryTaxValueViewModel(CongressVoting voting)
        : base(voting) { }

        public ChangeMinimumMonetaryTaxValueViewModel(FormCollection values)
        : base(values) { }


        public override StartCongressVotingParameters CreateVotingParameters()
        {
            return new ChangeMinimumMonetaryTaxValueParameters(NewTaxValue);
        }
    }
}
