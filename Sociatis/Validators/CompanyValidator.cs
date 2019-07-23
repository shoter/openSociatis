using Entities;
using Sociatis.Models.Companies;
using Sociatis.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Extensions;
using Entities.Repository;
using System.Web.Mvc;
using WebServices;
using Entities.enums;

namespace Sociatis.Validators
{
    public class CompanyValidator : Validator<CreateCompanyViewModel>
    {
        ICurrencyRepository currencyRepository;
        IEntityRepository entityRepository;
        IWalletService walletService;
        ICompanyRepository companyRepository;
        IJobOfferService jobOfferService;
        IEntityService entityService;

        public CompanyValidator(ModelStateDictionary modelState)
            :base(modelState)
        {
            this.currencyRepository = DependencyResolver.Current.GetService<ICurrencyRepository>();
            this.entityRepository = DependencyResolver.Current.GetService<IEntityRepository>();
            this.walletService = DependencyResolver.Current.GetService<IWalletService>();
            this.companyRepository = DependencyResolver.Current.GetService<ICompanyRepository>();
            this.jobOfferService = DependencyResolver.Current.GetService<IJobOfferService>();
            this.entityService = DependencyResolver.Current.GetService<IEntityService>();
        }
        public void Validate(CreateCompanyViewModel model, Entity entity, ValidatorAction action = ValidatorAction.Undefined)
        {
            var wallet = entity.Wallet;
            var countryMoney = wallet.GetMoney(model.CountryFee.CurrencyID, currencyRepository.GetAll());
            var adminMoney = wallet.GetMoney(model.AdminFee.CurrencyID, currencyRepository.GetAll());

            if (countryMoney.Amount < model.CountryFee.Quantity)
                AddError("Not enough money", () => model.CountryFee);
            if (adminMoney.Amount < model.AdminFee.Quantity)
                AddError("Not enough gold", () => model.AdminFee);


            if (entityService.IsNameTaken(model.CompanyName))
                AddError("Name is already used!", () => model.CompanyName);

            if (entityService.IsSpecialName(model.CompanyName))
            {
                AddError("This is special name. You cannot use it", () => model.CompanyName);
            }

            if (model.ProducedProductID == (int)ProductTypeEnum.Development)
                AddError("You cannot produce company which produce this kind of product!");
        }

        public void Validate(CreateNormalJobOfferViewModel model, Entity entity)
        {
            var company = companyRepository.GetById(model.CompanyID);
            var wallet = company.Entity.Wallet;
            var policy = company.Region.Country.CountryPolicy;
            var countryID = company.Region.CountryID.Value;
            var currency = Persistent.Countries.GetCountryCurrency(countryID);

            if (model.PostOfferOnJobMarket && !wallet.HaveMoney(model.JobMarketFee.CurrencyID, (double)model.JobMarketFee.Quantity * model.Amount))
                AddError("Not enough money" , () => model.JobMarketFee);

            if (jobOfferService.IsCompliantWithMinimalWage((decimal)(model.Salary??0), countryID) == false)
                AddError($"Salary is lower than actual minimal wage! ({policy.MinimalWage} {currency.Symbol})");
        }

        public void Validate(CreateContractJobOfferViewModel model, Entity entity)
        {
            var company = companyRepository.GetById(model.CompanyID);
            var country = company.Entity.GetCurrentRegion().Country;
            var countryPolicy = country.CountryPolicy;
            var wallet = company.Entity.Wallet;
            var currency = Persistent.Countries.GetCountryCurrency(country);

            if (model.PostOfferOnJobMarket && !wallet.HaveMoney(model.JobMarketFee.CurrencyID, (double)model.JobMarketFee.Quantity * model.Amount))
                AddError("Not enough money", () => model.JobMarketFee);

            if(model.Length > countryPolicy.MaximumContractLength)
            {
                AddError(string.Format("Your contract's length is too long. In {0} contracts can have length from {1} to {2} days", country.Entity.Name, countryPolicy.MinimumContractLength, countryPolicy.MaximumContractLength), () => model.Length);
            }

            if (model.Length < countryPolicy.MinimumContractLength)
            {
                AddError(string.Format("Your contract's length is too short. In {0} contracts can have length from {1} to {2} days", country.Entity.Name, countryPolicy.MinimumContractLength, countryPolicy.MaximumContractLength), () => model.Length);
            }

            if (jobOfferService.IsCompliantWithMinimalWage((decimal)(model.MinimumSalary??0), company.Region.CountryID.Value) == false)
                AddError($"Minimum salary is lower than actual minimal wage! ({countryPolicy.MinimalWage} {currency.Symbol})");


        }
    }
}
