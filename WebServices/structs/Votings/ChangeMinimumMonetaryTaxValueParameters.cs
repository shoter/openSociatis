using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class ChangeMinimumMonetaryTaxValueParameters : StartCongressVotingParameters
    {
        public double TaxValue { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.ChangeMinimumMonetaryTaxValue;

        public ChangeMinimumMonetaryTaxValueParameters(double taxValue)
        {
            TaxValue = taxValue;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = TaxValue.ToString();
        }
    }
}
