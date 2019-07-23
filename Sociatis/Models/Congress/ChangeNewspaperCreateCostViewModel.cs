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
    public class ChangeNewspaperCreateCostViewModel : CongressVotingViewModel
    {
        [DisplayName("New newspaper create cost")]
        [Range(0, 10000)]
        public double NewCreateCost { get; set; }




        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeNewspaperCreateCost;
        public ChangeNewspaperCreateCostViewModel() { }
        public ChangeNewspaperCreateCostViewModel(int countryID) : base(countryID) { }
        public ChangeNewspaperCreateCostViewModel(CongressVoting voting)
        :base(voting)
        {
        }

        public ChangeNewspaperCreateCostViewModel(FormCollection values)
        :base(values)
        {
        }
        public override StartCongressVotingParameters CreateVotingParameters()
        {
            return new ChangeNewspaperCreateCostVotingParameters(NewCreateCost);
        }
    }
}