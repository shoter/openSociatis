using Entities;
using Entities.Extensions;
using Entities.Repository;
using Sociatis.Models.Organisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sociatis.Validators.Organisations
{
    public class OrganisationCreationValidator : Validator<CreateOrganisationViewModel>
    {
        private readonly ICurrencyRepository currencyRepository;
        private readonly IEntityRepository entityRepository;
        private readonly IOrganisationRepository organisationRepository;
        public OrganisationCreationValidator(ModelStateDictionary ModelState, ICurrencyRepository currencyRepository, IEntityRepository entityRepository,
            IOrganisationRepository organisationRepository) : base(ModelState)
        {
            this.currencyRepository = currencyRepository;
            this.entityRepository = entityRepository;
            this.organisationRepository = organisationRepository;
        }

        public bool Validate(CreateOrganisationViewModel model, Entity entity)
        {
            var wallet = entity.Wallet;
            var countryMoney = wallet.GetMoney(model.CountryFee.CurrencyID, currencyRepository.GetAll());
            var adminMoney = wallet.GetMoney(model.AdminFee.CurrencyID, currencyRepository.GetAll());

            if (countryMoney.Amount < model.CountryFee.Quantity)
                AddError("Not enough money", () => model.CountryFee);
            if (adminMoney.Amount < model.AdminFee.Quantity)
                AddError("Not enough gold", () => model.AdminFee);

            var existingCompany = organisationRepository.FirstOrDefault(o => o.Entity.Name == model.OrganisationName);

            if (existingCompany != null)
                AddError("You cannot create organisation with this name!", () => model.OrganisationName);

            return IsValid;
        }
    }
}