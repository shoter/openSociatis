using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class BuildDefenseSystemVotingsParameters : StartCongressVotingParameters
    {
        public int RegionID { get; set; }
        public int Quality { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.BuildDefenseSystem;

        public BuildDefenseSystemVotingsParameters(int regionID, int quality)
        {
            this.RegionID = regionID;
            this.Quality = quality;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = RegionID.ToString();
            voting.Argument3 = Quality.ToString();

        }
    }

}
