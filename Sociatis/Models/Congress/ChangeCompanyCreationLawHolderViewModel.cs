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
    public class ChangeCompanyCreationLawHolderViewModel : CongressVotingViewModel
    {
        [DisplayName("Who can create national companies?")]
        public int LawHolderID { get; set; }

        public List<SelectListItem> LawHolderSelect { get; set; } = CreateSelectList<LawAllowHolderEnum>(law => law.ToHumanReadableString());

        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeCompanyCreationLawHolder;

        public ChangeCompanyCreationLawHolderViewModel() { }
        public ChangeCompanyCreationLawHolderViewModel(int countryID) : base(countryID) { }
        public ChangeCompanyCreationLawHolderViewModel(CongressVoting voting)
        : base(voting)
        {
        }

        public ChangeCompanyCreationLawHolderViewModel(FormCollection values)
        : base(values)
        {
        }


        public override StartCongressVotingParameters CreateVotingParameters()
        {
            return new ChangeCompanyCreationLawHolderVotingParameters((LawAllowHolderEnum)LawHolderID);
        }
    }
}