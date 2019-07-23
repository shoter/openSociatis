using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Congress
{
    public class ActualLawViewModel 
    {
        public decimal CitizenFee { get; set; }
        public decimal CitizenCompanyCost { get; set; }
        public decimal OrganisationCompanyCost { get; set; }
        public decimal NormalJobMarketFee { get; set; }
        public decimal ContractJobMarketFee { get; set; }
        public int MinimumContractLength { get; set; }
        public int MaximumContractLength { get; set; }
        public double NormalCongressVotingWinPercentage { get; set; }
        public decimal PartyFoundingFee { get; set; }
        public int PartyPresidentCadenceLength { get; set; }
        public int CongressCadenceLength { get; set; }
        public int CongressVotingLength { get; set; }
        public int PresidentCadenceLength { get; set; }
        public decimal OrganisationCreateCost { get; set; }
        public decimal MarketOfferCost { get; set; }
        public decimal NewspaperCreateCost { get; set; }
        public double ArticleTax { get; set; }
        public string CurrencySymbol { get; set; }
        public double MonetaryTaxRate { get; set; }
        public double MinimumMonetaryTax { get; set; }
        public decimal MinimalWage { get; set; }
        public LawAllowHolderEnum TreasuryLawAllowHolder { get; set; }
        public LawAllowHolderEnum NationalCompanyBuildLawAllowHolder { get; set; }

        public ActualLawViewModel(CountryPolicy policy)
        {
            CurrencySymbol = Persistent.Countries.GetCountryCurrency(policy.CountryID).Symbol;

            CitizenFee = policy.CitizenFee;
            CitizenCompanyCost = policy.CitizenCompanyCost;
            OrganisationCompanyCost = policy.OrganisationCompanyCost;
            NormalJobMarketFee = policy.NormalJobMarketFee;
            ContractJobMarketFee = policy.ContractJobMarketFee;
            MinimumContractLength = policy.MinimumContractLength;
            MaximumContractLength = policy.MaximumContractLength;
            NormalCongressVotingWinPercentage = (double)policy.NormalCongressVotingWinPercentage;
            PartyFoundingFee = policy.PartyFoundingFee;
            PartyPresidentCadenceLength = policy.PartyPresidentCadenceLength;
            CongressCadenceLength = policy.CongressCadenceLength;
            CongressVotingLength = policy.CongressVotingLength;
            PresidentCadenceLength = policy.PresidentCadenceLength;
            OrganisationCreateCost = policy.OrganisationCreateCost;
            MarketOfferCost = policy.MarketOfferCost;
            NewspaperCreateCost = policy.NewspaperCreateCost;
            ArticleTax = (double)policy.ArticleTax;
            MonetaryTaxRate = (double)policy.MonetaryTaxRate;
            MinimumMonetaryTax = (double)policy.MinimumMonetaryTax;
            TreasuryLawAllowHolder = (LawAllowHolderEnum)policy.TreasuryVisibilityLawAllowHolderID;
            NationalCompanyBuildLawAllowHolder = (LawAllowHolderEnum)policy.CountryCompanyBuildLawAllowHolder;
            MinimalWage = policy.MinimalWage;
        }
    }
}