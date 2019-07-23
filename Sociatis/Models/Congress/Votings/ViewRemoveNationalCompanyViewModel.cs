using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entities.enums;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewRemoveNationalCompanyViewModel : ViewVotingBaseViewModel
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public bool Exists { get; set; }

        public ViewRemoveNationalCompanyViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
        :base(voting, isPlayerCongressman, canVote)
    {
            CompanyID = int.Parse(voting.Argument1);
            CompanyName = voting.Argument2;

            var companyRepository = DependencyResolver.Current.GetService<ICompanyRepository>();
            Exists = companyRepository.Any(c => c.ID == CompanyID);


        }
    }
}