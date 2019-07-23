using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class ChangeMinimalWageVotingParameters : StartCongressVotingParameters
    {
        public decimal NewMinimalWage { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.ChangeMinimalWage;

        public ChangeMinimalWageVotingParameters(decimal newVat)
        {
            this.NewMinimalWage = newVat;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = NewMinimalWage.ToString();
        }
    }
}
