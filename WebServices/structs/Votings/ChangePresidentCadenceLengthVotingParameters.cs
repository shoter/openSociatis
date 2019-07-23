using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class ChangePresidentCadenceLengthVotingParameters : StartCongressVotingParameters
    {
        public int CadenceLength { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.ChangePresidentCadenceLength;

        public ChangePresidentCadenceLengthVotingParameters(int cadenceLength)
        {
            CadenceLength = cadenceLength;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = CadenceLength.ToString();
        }
    }
}
