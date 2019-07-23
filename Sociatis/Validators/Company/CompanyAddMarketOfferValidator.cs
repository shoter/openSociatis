using Entities.Repository;
using Sociatis.Models.Companies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sociatis.Validators.Company
{
    public class CompanyAddMarketOfferValidator : Validator<CompanyAddMarketOfferViewModel>
    {
        private readonly ICountryRepository countryRepository;

        public CompanyAddMarketOfferValidator(ModelStateDictionary ModelState, ICountryRepository countryRepository) : base(ModelState)
        {
            this.countryRepository = countryRepository;
        }

        public bool Validate(CompanyAddMarketOfferViewModel model)
        {
            if(model.Price < 0.01m)
            {
                AddError("Price must be equal or higher than 0.01!", () => model.Price);
            }

            if (model.CountryID > 0)
            {
                var countryExists = countryRepository.Any(c => c.ID == model.CountryID);

                if (countryExists == false)
                {
                    AddError("Country does not exists!", () => model.CountryID);
                }
            }

            return IsValid;
        }
    }
}