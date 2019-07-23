using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewChangeProductExportTaxViewModel : ViewVotingBaseViewModel
    {
        public double NewExportTax { get; set; }
        public string ProductName { get; set; }
        public string ForeignCountryName { get; set; }


        public ViewChangeProductExportTaxViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
        :base(voting, isPlayerCongressman, canVote)
    {
            ProductName = ((ProductTypeEnum)int.Parse(voting.Argument1)).ToHumanReadable();
            NewExportTax = double.Parse(voting.Argument2);
            int foreignCountryID = int.Parse(voting.Argument3);

            if (foreignCountryID == -1)
                ForeignCountryName = "all countries";
            else
                ForeignCountryName = Persistent.Countries.GetById(foreignCountryID).Entity.Name;
        }
    }
}