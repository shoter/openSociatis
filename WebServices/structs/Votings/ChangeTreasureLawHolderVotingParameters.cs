using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class ChangeTreasureLawHolderVotingParameters : StartCongressVotingParameters
    {
        public LawAllowHolderEnum LawHolder { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.ChangeTreasureLawHolder;

        public ChangeTreasureLawHolderVotingParameters(LawAllowHolderEnum lawHolder)
        {
            this.LawHolder = lawHolder;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = ((int)LawHolder).ToString();
        }
    }
}
