using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public class PrintMoneyVotingParameters : StartCongressVotingParameters
    {
        public int MoneyPrinted { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.PrintMoney;

        public PrintMoneyVotingParameters(int moneyToPrint)
        {
            this.MoneyPrinted = moneyToPrint;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            voting.Argument1 = MoneyPrinted.ToString();
        }
    }
}
