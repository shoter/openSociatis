using Entities;
using Entities.enums;
using Entities.Repository;
using Sociatis.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices.structs.Votings;
using WebUtils.Forms.Select2;

namespace Sociatis.Models.Congress
{
    public class AssignManagerToCompanyViewModel : CongressVotingViewModel
    {
        [DisplayName("Company")]
        public int CompanyID { get; set; }
        [DisplayName("Citizen")]
        public int CitizenID { get; set; }

        public List<SelectListItem> Companies { get; set; }
        public Select2AjaxViewModel Citizens { get; set; }

        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.AssignManagerToCompany;

        public AssignManagerToCompanyViewModel()
        {
            loadSelectLists();
        }
        public AssignManagerToCompanyViewModel(int countryID) : base(countryID)
        {
            loadSelectLists();
        }
        public AssignManagerToCompanyViewModel(CongressVoting voting)
        : base(voting)
        {
            loadSelectLists();
        }

        public AssignManagerToCompanyViewModel(FormCollection values)
        : base(values)
        {
            loadSelectLists();
        }


        public override StartCongressVotingParameters CreateVotingParameters()
        {
            return new AssignManagerToCompanyVotingParameters(CompanyID, CitizenID);
        }

        private void loadSelectLists()
        {
            loadCompanies();
            Citizens  = Select2AjaxViewModel.Create<CitizenController>(c => c.GetCitizens(null), nameof(CitizenID), null, "");
            Citizens.ID = nameof(CitizenID);
        }



        private void loadCompanies()
        {
            var companyRepository = DependencyResolver.Current.GetService<ICompanyRepository>();

            Companies = CreateSelectList(
                companyRepository.GetNationalCompanies(CountryID)
                .Select(c => new
                {
                    Name = c.Entity.Name,
                    ID = c.ID
                }).ToList(),
                c => c.Name,
                c => c.ID);
        }
    }

}