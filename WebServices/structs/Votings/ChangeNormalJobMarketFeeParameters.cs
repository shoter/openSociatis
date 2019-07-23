using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class ChangeNormalJobMarketFeeParameters : StartCongressVotingParameters
    {
        public double JobFee { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.ChangeNormalJobMarketFee;

        public ChangeNormalJobMarketFeeParameters(double jobFee)
        {
            this.JobFee = jobFee;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = JobFee.ToString();
        }
    }
}
