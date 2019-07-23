using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class TransferCashToCompanyVotingParameters : StartCongressVotingParameters
    {
        public int CompanyID { get; set; }
        public int CurrencyID { get; set; }
        public decimal Amount { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.TransferCashToCompany;

        public TransferCashToCompanyVotingParameters(int companyID, int currencyID, decimal amount)
        {
            CompanyID = companyID;
            CurrencyID = currencyID;
            Amount = amount;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = Amount.ToString();
            voting.Argument2 = CompanyID.ToString();
            voting.Argument3 = CurrencyID.ToString();
        }
    }
}
