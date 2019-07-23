using Common.Operations;
using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.structs;
using WebServices.structs.Votings;

namespace WebServices
{
    public interface ICongressVotingService
    {
        CongressVoting StartVote(StartCongressVotingParameters parameters);
        bool IsCongressman(Citizen citizen, Country country);
        CongressVotingComment AddComment(CongressVoting voting, Citizen citizen, string message);

        CongressVote AddVote(CongressVoting voting, Citizen citizen, VoteTypeEnum voteType);

        bool HasVoted(Citizen citizen, CongressVoting voting);

        bool CanVote(Citizen citizen, CongressVoting voting);

        void ProcessDayChange(int newDay);

        void FinishVoting(CongressVoting voting);

        void ReserveMoneyForVoting(CongressVoting voting, params Money[] money);

        void UnreserveMoneyForVoting(CongressVoting voting);

        void RemoveFromGameMoneyInVoting(CongressVoting voting);

        void RejectVoting(CongressVoting voting, CongressVotingRejectionReasonEnum reason);

        MethodResult CanTransferCashToCompany(Money money, Company destination, Country source);

        CongressVotingRejectionReasonEnum? GetRejectionReasonForTransferCashToCompany(Money money, Company destination, Country source);

        double GetPrintCost(int moneyAmount);

        MethodResult CanCreateVotingWithMinimumContractLength(int countryID, int minimumLength);
        MethodResult CanCreateVotingWithMaximumContractLength(int countryID, int maximumLength);
    }
}
