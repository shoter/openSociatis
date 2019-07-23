using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.enums;

namespace WebServices.structs.Votings
{
    public class ChangeProductVatVotingParameters : StartCongressVotingParameters
    {
        public ProductTypeEnum ProductType { get; set; }
        public double VatRate { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.ChangeProductVAT;

        public ChangeProductVatVotingParameters(ProductTypeEnum productType, double vatRate)
        {
            VatRate = vatRate;
            ProductType = productType;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = ((int)ProductType).ToString();
            voting.Argument2 = VatRate.ToString();
        }
    }
}
