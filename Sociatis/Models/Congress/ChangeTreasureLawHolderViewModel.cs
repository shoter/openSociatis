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
    public class ChangeTreasureLawHolderViewModel : CongressVotingViewModel
    {
        [DisplayName("Who can see treasury")]
        public int LawHolderID { get; set; }

        public List<SelectListItem> LawHolderSelect { get; set; } = CreateSelectList<LawAllowHolderEnum>(law => law.ToHumanReadableString());

        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeTreasureLawHolder;

        public ChangeTreasureLawHolderViewModel() { }
        public ChangeTreasureLawHolderViewModel(int countryID) : base(countryID) { }
        public ChangeTreasureLawHolderViewModel(CongressVoting voting)
        :base(voting)
        {
        }

        public ChangeTreasureLawHolderViewModel(FormCollection values)
        :base(values)
        {
        }


        public override StartCongressVotingParameters CreateVotingParameters()
        {
            return new ChangeTreasureLawHolderVotingParameters((LawAllowHolderEnum)LawHolderID);
        }
    }
}