using Common.Exceptions;
using Entities;
using Entities.enums;
using Entities.Repository;
using Sociatis.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.Helpers;
using WebServices.structs.Votings;

namespace Sociatis.Models.Congress
{
    public class CreateNationalCompanyViewModel : CongressVotingViewModel
    {
        [DisplayName("Product")]
        public int ProductID { get; set; }
        [DisplayName("Region")]
        public int RegionID { get; set; }
        [DisplayName("Company name")]
        public string CompanyName { get; set; }

        public MoneyViewModel GoldNeeded { get; set; } = new MoneyViewModel(GameHelper.Gold, ConfigurationHelper.Configuration.CompanyCountryFee);
        public MoneyViewModel CountryGold { get; set; }

        public List<SelectListItem> Regions { get; set; }
        public List<SelectListItem> Functions { get; set; }
        public bool CanSeeTreasury { get; set; } = false;

        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.CreateNationalCompany;

        public CreateNationalCompanyViewModel() { }
        public CreateNationalCompanyViewModel(int countryID) : base(countryID) { initialize(countryID); }
        public CreateNationalCompanyViewModel(CongressVoting voting)
        : base(voting)
        {
            initialize(voting.CountryID);
        }

        public CreateNationalCompanyViewModel(FormCollection values)
        : base(values)
        {
            initialize(CountryID);
        }

        private void initialize(int countryID)
        {
            loadCountryList(countryID);

            var entityRepository = DependencyResolver.Current.GetService<IEntityRepository>();
            var walletService = DependencyResolver.Current.GetService<IWalletService>();
            var countryRepository = DependencyResolver.Current.GetService<ICountryRepository>();
            

            CanSeeTreasury = countryRepository.GetCountryPolicySetting(countryID, p => p.TreasuryVisibilityLawAllowHolderID) != (int)LawAllowHolderEnum.President;

            if (CanSeeTreasury)
            {
                var countryEntity = entityRepository.GetById(countryID);
                var gold = walletService.GetWalletMoney(countryEntity.WalletID, GameHelper.Gold.ID);

                CountryGold = new MoneyViewModel(gold);
            }
        }

        private void loadCountryList(int countryID)
        {
            Functions = ProductTypeEnumUtils.GetFunctionsList();

            var regionRepository = DependencyResolver.Current.GetService<IRegionRepository>();
            var regions = regionRepository.Where(r => r.CountryID == countryID)
                .Select(r => new
                {
                    ID = r.ID,
                    Name = r.Name
                }).ToList();

            Regions = CreateSelectList(regions, r => r.Name, r => r.ID, false);
        }


        public override StartCongressVotingParameters CreateVotingParameters()
        {
            validate();
            return new CreateNationalCompanyVotingParameters(CompanyName, RegionID, ProductID);
        }

        private void validate()
        {
            if(ProductID == (int)ProductTypeEnum.MedicalSupplies)
                CompanyName = "";

            var regionRepository = DependencyResolver.Current.GetService<IRegionRepository>();
            var countryRepository = DependencyResolver.Current.GetService<ICountryRepository>();
            var entityService = DependencyResolver.Current.GetService<IEntityService>();
            var walletService = DependencyResolver.Current.GetService<IWalletService>();
            var companyService = DependencyResolver.Current.GetService<ICompanyService>();

            var companyRepository = DependencyResolver.Current.GetService<ICompanyRepository>();

            if (ProductID == (int)ProductTypeEnum.Development)
            {
                if (companyRepository.Any(c => c.ProductID == ProductID && c.RegionID == RegionID))
                    throw new UserReadableException("There is already national company in this region that is producing infrastructure!");
            }
            else if (ProductID == (int)ProductTypeEnum.MedicalSupplies)
            {
                if (regionRepository.Any(r => r.ID == RegionID && r.Hospital != null))
                    throw new UserReadableException("There is already national hospital in this region!");
            }

            var region = regionRepository.GetById(RegionID);
            var country = countryRepository.GetById(CountryID);

            if (region.CountryID != CountryID)
                throw new UserReadableException("This region does not belong to your country!");
            if (Enum.IsDefined(typeof(ProductTypeEnum), ProductID) == false)
                throw new UserReadableException("This product type is not defined");
            if (entityService.IsNameTaken(CompanyName) && ProductID != (int)ProductTypeEnum.MedicalSupplies)
                throw new UserReadableException("This name is already taken!");

            var companyCreationCost = companyService.GetCompanyCreationCost(region, EntityTypeEnum.Country);

            foreach (var money in companyCreationCost)
                if (walletService.HaveMoney(country.Entity.WalletID, money) == false)
                    throw new UserReadableException($"Your country does not have enough {money.Currency.Symbol}");
        }
    }
}