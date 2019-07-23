using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewChangeProductVATViewModel : ViewVotingBaseViewModel
    {
        public double NewVat { get; set; }
        public string ProductName { get; set; }


        public ViewChangeProductVATViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
        : base(voting, isPlayerCongressman, canVote)
        {
            ProductName = ((ProductTypeEnum)int.Parse(voting.Argument1)).ToHumanReadable();
            NewVat = double.Parse(voting.Argument2);
        }
    }
}