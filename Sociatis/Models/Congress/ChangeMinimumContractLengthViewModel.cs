using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities;
using Entities.enums;
using System.ComponentModel;
using System.Web.Mvc;
using WebServices.structs.Votings;
using System.ComponentModel.DataAnnotations;
using WebServices;
using Common.Exceptions;

namespace Sociatis.Models.Congress
{
    public class ChangeMinimumContractLengthViewModel : CongressVotingViewModel
    {
        [DisplayName("New length")]
        [Range(Constants.ContractMinimumLength, Constants.ContractMaximumLength)]
        public int NewLength { get; set; }
        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeMinimumContractLength;

        public ChangeMinimumContractLengthViewModel() { }
        public ChangeMinimumContractLengthViewModel(int countryID) : base(countryID) { }
        public ChangeMinimumContractLengthViewModel(CongressVoting voting)
        {
            NewLength = int.Parse(voting.Argument1);
        }
        public ChangeMinimumContractLengthViewModel(FormCollection values)
            : base(values)
        {
        }

        public override StartCongressVotingParameters CreateVotingParameters()
        {
            var congressVotingService = DependencyResolver.Current.GetService<ICongressVotingService>();

            congressVotingService.CanCreateVotingWithMinimumContractLength(CountryID, NewLength).ThrowUserReadableExceptionIfError();

            return new ChangeMinimumContractLengthParameters(NewLength);
        }
    }
}