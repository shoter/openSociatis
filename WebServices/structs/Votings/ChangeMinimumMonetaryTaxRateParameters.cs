using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class ChangeMinimumMonetaryTaxRateParameters : StartCongressVotingParameters
    {
        public double TaxRate { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.ChangeMonetaryTaxRate;

        public ChangeMinimumMonetaryTaxRateParameters(double taxRate)
        {
            TaxRate = taxRate;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = TaxRate.ToString();
        }
    }
}
