using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entities.enums;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewAssignManagerToCompanyViewModel : ViewVotingBaseViewModel
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int CitizenID { get; set; }
        public string CitizenName { get; set; }

        public ViewAssignManagerToCompanyViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
        :base(voting, isPlayerCongressman, canVote)
    {
            CompanyID = int.Parse(voting.Argument1);
            CitizenID = int.Parse(voting.Argument2);

            var entityRepository = DependencyResolver.Current.GetService<IEntityRepository>();

            var company = entityRepository.GetById(CompanyID);
            var citizen = entityRepository.GetById(CitizenID);

            CompanyName = company.Name;
            CitizenName = citizen.Name;
        }
    }
}