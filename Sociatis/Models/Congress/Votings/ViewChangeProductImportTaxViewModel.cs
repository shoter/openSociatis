using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewChangeProductImportTaxViewModel : ViewVotingBaseViewModel
    {
        public double NewImportTax { get; set; }
        public string ProductName { get; set; }
        public string ForeignCountryName { get; set; }


        public ViewChangeProductImportTaxViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
        :base(voting, isPlayerCongressman, canVote)
    {
            ProductName = ((ProductTypeEnum)int.Parse(voting.Argument1)).ToHumanReadable();
            NewImportTax = double.Parse(voting.Argument2);
            int foreignCountryID = int.Parse(voting.Argument3);

            if (foreignCountryID == -1)
                ForeignCountryName = "all countries";
            else
                ForeignCountryName = Persistent.Countries.GetById(foreignCountryID).Entity.Name;
        }
    }
}