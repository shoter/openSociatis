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

namespace Sociatis.Models.Congress
{
    public class ChangeOrganisationCompanyCostViewModel : CongressVotingViewModel
    {
        [DisplayName("New citizen company cost")]
        [Range(0, 10000)]
        public double CreationCost { get; set; }

        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeOrganisationCompanyCost;

        public ChangeOrganisationCompanyCostViewModel() { }
        public ChangeOrganisationCompanyCostViewModel(int countryID) :base(countryID) { }

        public ChangeOrganisationCompanyCostViewModel(CongressVoting voting)
        :base(voting)
        {
        }

        public ChangeOrganisationCompanyCostViewModel(FormCollection values)
        :base(values)
        {
        }
        public override StartCongressVotingParameters CreateVotingParameters()
        {
            return new ChangeOrganisationCompanyCostVotingParameters(CreationCost);
        }
    }
}