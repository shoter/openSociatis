using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class ChangeMaximumContractLengthParameters : StartCongressVotingParameters
    {
        public int NewLength { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.ChangeMaximumContractLength;

        public ChangeMaximumContractLengthParameters(int newLength)
        {
            this.NewLength = newLength;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = NewLength.ToString();
        }
    }
}
