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
    public class ChangeNormalCongressVotingWinPercentageViewModel : CongressVotingViewModel
    {
        [DisplayName("New voting win percentage")]
        [Range(Constants.MinimalVotingPercentage , Constants.MaximalVotingPercentage)]
        public double WinPercentage { get; set; }

        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeNormalCongressVotingWinPercentage;

        public ChangeNormalCongressVotingWinPercentageViewModel() { }
        public ChangeNormalCongressVotingWinPercentageViewModel(int countryID) : base(countryID) { }

        public ChangeNormalCongressVotingWinPercentageViewModel(CongressVoting voting)
        :base(voting)
        {
        }

        public ChangeNormalCongressVotingWinPercentageViewModel(FormCollection values)
        :base(values)
        {
        }
        public override StartCongressVotingParameters CreateVotingParameters()
        {
            return new ChangeNormalCongressVotingWinPercentageVotingParameters(WinPercentage);
        }
    }
}