using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class CreateNationalCompanyVotingParameters : StartCongressVotingParameters
    {
        public string CompanyName { get; set; }
        public int RegionID { get; set; }
        public int ProductID { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.CreateNationalCompany;

        public CreateNationalCompanyVotingParameters(string companyName, int regionID, int productID)
        {
            CompanyName = companyName;
            RegionID = regionID;
            ProductID = productID;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = CompanyName;
            voting.Argument2 = RegionID.ToString();
            voting.Argument3 = ProductID.ToString();
        }
    }

}
