using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewChangeTreasureLawHolderViewModel : ViewVotingBaseViewModel
    {
        public LawAllowHolderEnum LawHolder { get; set; }


        public ViewChangeTreasureLawHolderViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
        :base(voting, isPlayerCongressman, canVote)
    {
            LawHolder = (LawAllowHolderEnum)Enum.Parse(typeof(LawAllowHolderEnum), voting.Argument1);
        }
    }

}