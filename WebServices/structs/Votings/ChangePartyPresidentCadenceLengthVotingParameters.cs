using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class ChangePartyPresidentCadenceLengthVotingParameters : StartCongressVotingParameters
    {
        public int NewCadenceLength { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.ChangePartyPresidentCadenceLength;

        public ChangePartyPresidentCadenceLengthVotingParameters(int newCadenceLength)
        {
            this.NewCadenceLength = newCadenceLength;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = NewCadenceLength.ToString();
        }
    }
}
