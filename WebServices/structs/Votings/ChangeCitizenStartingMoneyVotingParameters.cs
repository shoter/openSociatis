using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class ChangeCitizenStartingMoneyVotingParameters : StartCongressVotingParameters
    {
        public decimal NewFee { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.ChangeCitizenStartingMoney;

        public ChangeCitizenStartingMoneyVotingParameters(decimal newFee)
        {
            this.NewFee = newFee;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = NewFee.ToString();
        }
    }
}
