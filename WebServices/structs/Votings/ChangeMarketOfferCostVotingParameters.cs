using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class ChangeMarketOfferCostVotingParameters : StartCongressVotingParameters
    {
        public double OfferCost { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.ChangeMarketOfferCost;

        public ChangeMarketOfferCostVotingParameters(double offerCost)
        {
            OfferCost = offerCost;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = OfferCost.ToString();
        }
    }
}
