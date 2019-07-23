using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Sociatis.Models.Congress
{
    public static class CongressVotingViewModelChooser
    {
        private static Dictionary<VotingTypeEnum ,Type> VotingViewModelsTypes = new Dictionary<VotingTypeEnum, Type>();
        static CongressVotingViewModelChooser()
        {
            Add<ChangeMinimumContractLengthViewModel>();
            Add<ChangeMaximumContractLengthViewModel>();
            Add<ChangeCongressVotingLengthViewModel>();
            Add<ChangePartyPresidentCadenceLengthViewModel>();
            Add<ChangeCongressCadenceLengthViewModel>();
            Add<ChangeNormalJobMarketFeeViewModel>();
            Add<ChangeContractJobMarketFeeViewModel>();
            Add<ChangeProductVATViewModel>();
            Add<ChangeProductExportTaxViewModel>();
            Add<ChangeProductImportTaxViewModel>();
            Add<ChangeArticleTaxViewModel>();
            Add<ChangeNewspaperCreateCostViewModel>();
            Add<ChangeMarketOfferCostViewModel>();
            Add<ChangeOrganisationCreateCostViewModel>();
            Add<ChangePresidentCadenceLengthViewModel>();
            Add<ChangePartyCreateFeeViewModel>();
            Add<ChangeNormalCongressVotingWinPercentageViewModel>();
            Add<ChangeOrganisationCompanyCostViewModel>();
            Add<ChangeCitizenCompanyCostViewModel>();
            Add<ChangeMinimumMonetaryTaxRateViewModel>();
            Add<ChangeMinimumMonetaryTaxValueViewModel>();
            Add<ChangeTreasureLawHolderViewModel>();
            Add<ChangeCompanyCreationLawHolderViewModel>();
            Add<CreateNationalCompanyViewModel>();
            Add<RemoveNationalCompanyViewModel>();
            Add<AssignManagerToCompanyViewModel>();
            Add<TransferCashToCompanyViewModel>();
            Add<ChangeGreetingMessageViewModel>();
            Add<ChangeCitizenStartingMoneyViewModel>();
            Add<PrintMoneyViewModel>();
            Add<ChangeMinimalWageViewModel>();
            Add<BuildDefenseSystemViewModel>();
            Add<ChangeHouseTaxViewModel>();
            Add<ChangeHotelTaxViewModel>();
        }

        private static void Add<T>()
            where T : CongressVotingViewModel, new()
        {
            T t = new T(); //duh
            VotingViewModelsTypes.Add(t.VotingType, typeof(T));
        }

        public static Type GetType(VotingTypeEnum votingType)
        {
            foreach (var type in VotingViewModelsTypes)
            {
                if (type.Key == votingType)
                {
                    return type.Value;
                }
            }

            throw new KeyNotFoundException("CongressVotingViewModel not found");
        }

        public static CongressVotingViewModel GetViewModel(VotingTypeEnum votingType, FormCollection values)
        {
            Type type = GetType(votingType);
            return (CongressVotingViewModel)Activator.CreateInstance(type, values);
        }

        public static CongressVotingViewModel GetViewModel(VotingTypeEnum votingType, CongressVoting voting)
        {
            Type type = GetType(votingType);
            return (CongressVotingViewModel)Activator.CreateInstance(type, voting);
        }

        public static CongressVotingViewModel GetViewModel(VotingTypeEnum votingType, int countryID)
        {
            Type type = GetType(votingType);
            ConstructorInfo ctor = type.GetConstructor(new[] { typeof(int) });
            return (CongressVotingViewModel)ctor.Invoke(new object[] { countryID });
        }
    }
}