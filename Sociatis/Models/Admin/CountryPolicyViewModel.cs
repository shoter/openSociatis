using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Admin
{
    public class CountryPolicyViewModel
    {
        public string CountryName { get; set; }
        public int CountryID { get; set; }
        public double Vat { get; set; }
        public double ImportTax { get; set; }
        public double ExportTax { get; set; }
        public double CitizenFee { get; set; }
        public double CitizenCompanyCost { get; set; }
        public double OrganisationCompanyCost { get; set; }
        public double NormalJobMarketFee { get; set; }
        public double ContractJobMarketFee { get; set; }
        public int MinimumContractLength { get; set; }
        public int MaximumContractLength { get; set; }
        public double NormalCongressVotingWinPercentage { get; set; }
        public double PartFoundingFee { get; set; }
        public double PartyPresidentCadenceLength { get; set; }
        public double CongressCadenceLength { get; set; }
        public double CongressVotingLength { get; set; }

       public CountryPolicyViewModel(CountryPolicy policy)
        {
            CountryName = policy.Country.Entity.Name;
            CountryID = policy.Country.ID;
            CitizenFee = (double)policy.CitizenFee;
            CitizenCompanyCost = (double)policy.CitizenCompanyCost;
            OrganisationCompanyCost = (double)policy.OrganisationCompanyCost;
            NormalJobMarketFee = (double)policy.NormalJobMarketFee;
            ContractJobMarketFee = (double)policy.ContractJobMarketFee;
            MinimumContractLength = policy.MinimumContractLength;
            MaximumContractLength = policy.MaximumContractLength;
            NormalCongressVotingWinPercentage = (double)policy.NormalCongressVotingWinPercentage;
            PartFoundingFee = (double)policy.PartyFoundingFee;
            PartyPresidentCadenceLength = policy.PartyPresidentCadenceLength;
            CongressCadenceLength = policy.CongressCadenceLength;
            CongressVotingLength = policy.CongressVotingLength;
        }
    }
}