using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class ChangeProductExportTaxVotingParameters : StartCongressVotingParameters
    {
        public ProductTypeEnum ProductType { get; set; }
        public double ExportTaxRate { get; set; }
        /// <summary>
        /// -1 means all
        /// </summary>
        public int ForeignCountryID { get; set; }
        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.ChangeProductExportTax;

        public ChangeProductExportTaxVotingParameters(ProductTypeEnum productType, double exportTaxRate, int foreignCountryID)
        {
            ExportTaxRate = exportTaxRate;
            ProductType = productType;
            ForeignCountryID = foreignCountryID;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = ((int)ProductType).ToString();
            voting.Argument2 = ExportTaxRate.ToString();
            voting.Argument3 = ForeignCountryID.ToString();
        }
    }
}
