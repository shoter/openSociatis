using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class ChangeOrganisationCompanyCostVotingParameters : StartCongressVotingParameters
    {
        public double CreationCost { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.ChangeOrganisationCompanyCost;

        public ChangeOrganisationCompanyCostVotingParameters(double creationCost)
        {
            CreationCost = creationCost;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = CreationCost.ToString();
        }
    }
}
