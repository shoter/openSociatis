using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class ChangePartyCreateFeeVotingParameters : StartCongressVotingParameters
    {
        public double CreationFee { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.ChangePartyCreateFee;

        public ChangePartyCreateFeeVotingParameters(double taxRate)
        {
            CreationFee = taxRate;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = CreationFee.ToString();
        }
    }
}
