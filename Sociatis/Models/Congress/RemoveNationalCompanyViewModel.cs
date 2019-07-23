using Common.Exceptions;
using Entities;
using Entities.enums;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices.structs.Votings;

namespace Sociatis.Models.Congress
{
    public class RemoveNationalCompanyViewModel : CongressVotingViewModel
    {
        [DisplayName("Company")]
        public int CompanyID { get; set; }
        public List<SelectListItem> Companies { get; set; }


        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.RemoveNationalCompany;

        public RemoveNationalCompanyViewModel()
        {
            loadCompanies();
        }
        public RemoveNationalCompanyViewModel(int countryID) : base(countryID)
        {
            loadCompanies();
        }
        public RemoveNationalCompanyViewModel(CongressVoting voting)
        :base(voting)
        {
            loadCompanies();
        }

        public RemoveNationalCompanyViewModel(FormCollection values)
        : base(values)
        {
            loadCompanies();
        }


        public void loadCompanies()
        {
            var companyRepository = DependencyResolver.Current.GetService<ICompanyRepository>();
            var companies = companyRepository.GetNationalCompanies(CountryID)
                .Select(company => new
                {
                    Name = company.Entity.Name,
                    ID = company.ID
                }).ToList();

            Companies = CreateSelectList(companies, c => c.Name, c => c.ID);

        }
        public override StartCongressVotingParameters CreateVotingParameters()
        {
            validate();
            return new RemoveNationalCompanyVotingParameters(CompanyID);
        }

        private void validate()
        {
            var companyRepository = DependencyResolver.Current.GetService<ICompanyRepository>();
            var company = companyRepository.GetById(CompanyID);
            if (company?.Region?.CountryID != CountryID)
                throw new UserReadableException("Company is not your national company!");
        }
    }
}