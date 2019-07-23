using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class ChangeArticleTaxVotingParameters : StartCongressVotingParameters
    {
        public double TaxRate { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.ChangeArticleTax;

        public ChangeArticleTaxVotingParameters(double taxRate)
        {
            TaxRate = taxRate;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = TaxRate.ToString();
        }
    }
}
