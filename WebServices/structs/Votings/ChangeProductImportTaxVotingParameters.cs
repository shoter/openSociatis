using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class ChangeProductImportTaxVotingParameters : StartCongressVotingParameters
    {
        public ProductTypeEnum ProductType { get; set; }
        public double ImportTaxRate { get; set; }
        /// <summary>
        /// -1 means all
        /// </summary>
        public int ForeignCountryID { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.ChangeProductImportTax;

        public ChangeProductImportTaxVotingParameters(ProductTypeEnum productType, double importTaxRate, int foreignCountryID)
        {
            ImportTaxRate = importTaxRate;
            ProductType = productType;
            ForeignCountryID = foreignCountryID;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = ((int)ProductType).ToString();
            voting.Argument2 = ImportTaxRate.ToString();
            voting.Argument3 = ForeignCountryID.ToString();
        }
    }
}
