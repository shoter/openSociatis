using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class ChangeNormalCongressVotingWinPercentageVotingParameters : StartCongressVotingParameters
    {
        public double WinPercentage { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.ChangeNormalCongressVotingWinPercentage;

        public ChangeNormalCongressVotingWinPercentageVotingParameters(double winPercentage)
        {
            WinPercentage = winPercentage;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = WinPercentage.ToString();
        }
    }
}
