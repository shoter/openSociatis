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
    public class ChangeOrganisationCreateCostViewModel : CongressVotingViewModel
    {
        [DisplayName("New organisation create cost")]
        [Range(0, 10000)]
        public double CreateCost { get; set; }

        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeOrganisationCreateCost;

        public ChangeOrganisationCreateCostViewModel() { }
        public ChangeOrganisationCreateCostViewModel(int countryID) : base(countryID) { }
        public ChangeOrganisationCreateCostViewModel(CongressVoting voting)
        :base(voting)
        {
        }

        public ChangeOrganisationCreateCostViewModel(FormCollection values)
        :base(values)
        {
            
        }
        public override StartCongressVotingParameters CreateVotingParameters()
        {
            return new ChangeOrganisationCreateCostVotingParameters(CreateCost);
        }
    }
}