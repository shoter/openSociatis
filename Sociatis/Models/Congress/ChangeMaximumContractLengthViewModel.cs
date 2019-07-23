using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices.structs.Votings;
using WebServices;

namespace Sociatis.Models.Congress
{
    public class ChangeMaximumContractLengthViewModel : CongressVotingViewModel
    {
        [DisplayName("New length")]
        [Range(Constants.ContractMinimumLength, Constants.ContractMaximumLength)]
        public int NewLength { get; set; }
        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeMaximumContractLength;

        public ChangeMaximumContractLengthViewModel() { }
        public ChangeMaximumContractLengthViewModel(int countryID) : base(countryID) { }
        public ChangeMaximumContractLengthViewModel(CongressVoting voting)
        {
            NewLength = int.Parse(voting.Argument1);
        }
        public ChangeMaximumContractLengthViewModel(FormCollection values)
            : base(values)
        {
        }
        public override StartCongressVotingParameters CreateVotingParameters()
        {
            var congressVotingService = DependencyResolver.Current.GetService<ICongressVotingService>();

            congressVotingService.CanCreateVotingWithMaximumContractLength(CountryID, NewLength).ThrowUserReadableExceptionIfError();

            return new ChangeMaximumContractLengthParameters(NewLength);
        }
    }
}