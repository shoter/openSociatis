using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class RemoveNationalCompanyVotingParameters : StartCongressVotingParameters
    {
        public int CompanyID { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.RemoveNationalCompany;

        public RemoveNationalCompanyVotingParameters(int companyID)
        {
            this.CompanyID = companyID;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = CompanyID.ToString();
        }
    }
}
