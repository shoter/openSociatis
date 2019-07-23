using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class AssignManagerToCompanyVotingParameters : StartCongressVotingParameters
    {
        public int CompanyID { get; set; }
        public int CitizenID { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.AssignManagerToCompany;

        public AssignManagerToCompanyVotingParameters(int companyID, int citizenID)
        {
            CompanyID = companyID;
            CitizenID = citizenID;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = CompanyID.ToString();
            voting.Argument2 = CitizenID.ToString();
        }
    }
}
