using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class ChangeHotelTaxVotingParameters : StartCongressVotingParameters
    {
        public decimal Tax { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.ChangeHotelTax;

        public ChangeHotelTaxVotingParameters(decimal tax)
        {
            this.Tax = tax;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = Tax.ToString();

        }
    }
}
