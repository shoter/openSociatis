using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewBuildDefenseSystemViewModel : ViewVotingBaseViewModel
    {
        public int Quality { get; set; }
        public string RegionName { get; set; }
        public int RegionID { get; set; }
        public double GoldCost { get; set; }

        public ViewBuildDefenseSystemViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
    : base(voting, isPlayerCongressman, canVote)
    {
            var defenseSystemService = DependencyResolver.Current.GetService<IDefenseSystemService>();

            RegionID = int.Parse(voting.Argument1);
            Quality = int.Parse(voting.Argument3);

            RegionName = Persistent.Regions.GetById(RegionID).Name;
            GoldCost = (double)defenseSystemService.GetGoldCostForStartingConstruction(Quality);
        }


    }
}