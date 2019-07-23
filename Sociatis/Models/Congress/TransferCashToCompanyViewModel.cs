using Common.Exceptions;
using Common.Operations;
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
using WebServices;
using WebServices.structs;
using WebServices.structs.Votings;
using WebUtils.Forms.Select2;

namespace Sociatis.Models.Congress
{
    public class TransferCashToCompanyViewModel : CongressVotingViewModel
    {
        [DisplayName("Currency")]
        public int CurrencyID { get; set; }
        [DisplayName("Company")]
        public int CompanyID { get; set; }
        [DisplayName("Amount")]
        public decimal Amount { get; set; }

        public Select2AjaxViewModel Companies { get; set; }
        public List<SelectListItem> Currencies { get; set; }



        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.TransferCashToCompany;

        public TransferCashToCompanyViewModel()
        {
            loadSelectLists(CountryID);
        }
        public TransferCashToCompanyViewModel(int countryID) : base(countryID)
        {
            loadSelectLists(CountryID);
        }
        public TransferCashToCompanyViewModel(CongressVoting voting)
        : base(voting)
        {
            loadSelectLists(CountryID);
        }

        public TransferCashToCompanyViewModel(FormCollection values)
        : base(values)
        {
            loadSelectLists(CountryID);
        }

        void loadSelectLists(int countryID)
        {
            var entityRepository = DependencyResolver.Current.GetService<IEntityRepository>();
            var currencyIDs = entityRepository.Where(entity => entity.EntityID == countryID)
                .Select(e => e.Wallet)
                .SelectMany(w => w.WalletMoneys)
                .Select(money => money.CurrencyID)
                .ToList();
            var currencies = Persistent.Currencies.Where(currency => currencyIDs.Contains(currency.ID)).ToList();


            Currencies = CreateSelectList(currencies, c => c.Symbol, c => c.ID);
            Companies = Select2AjaxViewModel.Create<CongressController>(x => x.GetTransferMoneyToCompanies(null, 0), nameof(CompanyID), null, "");
            Companies.ID = "CompanyID";
            Companies.Data = "formatTransferData";

        }


        public override StartCongressVotingParameters CreateVotingParameters()
        {
            validate();
            return new TransferCashToCompanyVotingParameters(CompanyID, CurrencyID, Amount);
        }

        private void validate()
        {
            var companyRepository = DependencyResolver.Current.GetService<ICompanyRepository>();
            var countryRepository = DependencyResolver.Current.GetService<ICountryRepository>();

            var company = companyRepository.GetById(CompanyID);
            var country = countryRepository.GetById(CountryID);

            var money = new Money(CurrencyID, Amount);

            var congressVotingService = DependencyResolver.Current.GetService<ICongressVotingService>();

            MethodResult result = congressVotingService.CanTransferCashToCompany(money, company, country);
            if (result.IsError)
                throw new UserReadableException(result.Errors[0].ToString()); 
        }
    }
}