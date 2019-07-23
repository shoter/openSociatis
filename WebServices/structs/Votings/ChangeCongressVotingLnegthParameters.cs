using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class ChangeCongressVotingLnegthParameters : StartCongressVotingParameters
    {
        public int NewCongressVotingLength { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.ChangeCongressVotingLength;

        public ChangeCongressVotingLnegthParameters(int newCongressVotingLength)
        {
            this.NewCongressVotingLength = newCongressVotingLength;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = NewCongressVotingLength.ToString();
        }
    }
}
