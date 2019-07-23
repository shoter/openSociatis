using Entities;
using Entities.enums;
using Entities.Extensions;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Congress.Votings
{
    public static class CongressViewVotingViewModelChooser
    {
        private static Dictionary<VotingTypeEnum, Type> viewModelTypes = new Dictionary<VotingTypeEnum, Type>();
        static CongressViewVotingViewModelChooser()
        {

            add<ViewChangeMinimumContractLength>(VotingTypeEnum.ChangeMinimumContractLength);
            add<ViewChangeMaximumContractLength>(VotingTypeEnum.ChangeMaximumContractLength);
            add<ViewChangeCongressVotingLengthViewModel>(VotingTypeEnum.ChangeCongressVotingLength);
            add<ViewChangePartyPresidentCadenceLengthViewModel>(VotingTypeEnum.ChangePartyPresidentCadenceLength);
            add<ViewChangeCongressCadenceLengthViewModel>(VotingTypeEnum.ChangeCongressCadenceLength);
            add<ViewChangeNormalJobMarketFeeViewModel>(VotingTypeEnum.ChangeNormalJobMarketFee);
            add<ViewChangeContractJobMarketFeeViewModel>(VotingTypeEnum.ChangeContractJobMarketFee);
            add<ViewChangeProductVATViewModel>(VotingTypeEnum.ChangeProductVAT);
            add<ViewChangeProductImportTaxViewModel>(VotingTypeEnum.ChangeProductImportTax);
            add<ViewChangeProductExportTaxViewModel>(VotingTypeEnum.ChangeProductExportTax);
            add<ViewChangeArticleTaxViewModel>(VotingTypeEnum.ChangeArticleTax);
            add<ViewChangeNewspaperCreateCostViewModel>(VotingTypeEnum.ChangeNewspaperCreateCost);
            add<ViewChangeMarketOfferCostViewModel>(VotingTypeEnum.ChangeMarketOfferCost);
            add<ViewChangeOrganisationCreateCostViewModel>(VotingTypeEnum.ChangeOrganisationCreateCost);
            add<ViewChangePresidentCadenceLengthViewModel>(VotingTypeEnum.ChangePresidentCadenceLength);
            add<ViewChangePartyCreateFeeViewModel>(VotingTypeEnum.ChangePartyCreateFee);
            add<ViewChangeNormalCongressVotingWinPercentageViewModel>(VotingTypeEnum.ChangeNormalCongressVotingWinPercentage);
            add<ViewChangeCitizenCompanyCostViewModel>(VotingTypeEnum.ChangeCitizenCompanyCost);
            add<ViewChangeOrganisationCompanyCostViewModel>(VotingTypeEnum.ChangeOrganisationCompanyCost);
            add<ViewChangeMinimumMonetaryTaxRateViewModel>(VotingTypeEnum.ChangeMonetaryTaxRate);
            add<ViewChangeMinimumMonetaryTaxValueViewModel>(VotingTypeEnum.ChangeMinimumMonetaryTaxValue);
            add<ViewChangeTreasureLawHolderViewModel>(VotingTypeEnum.ChangeTreasureLawHolder);
            add<ViewChangeCompanyCreationLawHolderViewModel>(VotingTypeEnum.ChangeCompanyCreationLawHolder);
            add<ViewCreateNationalCompanyViewModel>(VotingTypeEnum.CreateNationalCompany);
            add<ViewRemoveNationalCompanyViewModel>(VotingTypeEnum.RemoveNationalCompany);
            add<ViewAssignManagerToCompanyViewModel>(VotingTypeEnum.AssignManagerToCompany);
            add<ViewTransferCashToCompanyViewModel>(VotingTypeEnum.TransferCashToCompany);
            add<ViewChangeGreetingMessageViewModel>(VotingTypeEnum.ChangeGreetingMessage);
            add<ViewChangeCitizenStartingMoneyViewModel>(VotingTypeEnum.ChangeCitizenStartingMoney);
            add<ViewPrintMoneyViewModel>(VotingTypeEnum.PrintMoney);
            add<ViewChangeMinimalWageViewModel>(VotingTypeEnum.ChangeMinimalWage);
            add<ViewBuildDefenseSystemViewModel>(VotingTypeEnum.BuildDefenseSystem);
            add<ViewChangeHouseTaxViewModel>(VotingTypeEnum.ChangeHouseTax);
            add<ViewChangeHotelTaxViewModel>(VotingTypeEnum.ChangeHotelTax);
        }

        private static void add<T>(VotingTypeEnum votingType)
            where T: class
        {
            var type = typeof(T);
#if DEBUG
            if (viewModelTypes.ContainsKey(votingType))
                throw new Exception("We have that type!(" + votingType.ToString() + ")");
            
#endif
            
            viewModelTypes.Add(votingType, type);
        }


        private static ViewVotingBaseViewModel instantiate(VotingTypeEnum votingType, CongressVoting voting, bool isPlayerCongressman, bool canVote)
        {
#if DEBUG
            if (viewModelTypes.Count != Enum.GetValues(typeof(VotingTypeEnum)).Length)
                throw new Exception("Something is propably wrong here :D");
#endif

            var type = viewModelTypes[votingType];
            var ctor = type.GetConstructor(new[] { typeof(CongressVoting), typeof(bool), typeof(bool) });

            return (ViewVotingBaseViewModel)ctor.Invoke(new object[] { voting, isPlayerCongressman, canVote });
        }

        static public ViewVotingBaseViewModel Instantiate(Entities.CongressVoting voting, ICongressVotingService congressVotingService)
        {
            bool isPlayerCongressman = false;
            bool canVote = false;
            var currentEntity = SessionHelper.CurrentEntity;

            if(currentEntity.Citizen != null)
            {
                var citizen = currentEntity.Citizen;
                if (citizen?.Congressmen.Any(c => c.CountryID == voting.CountryID) == true)
                    isPlayerCongressman = true;

                canVote = congressVotingService.CanVote(citizen, voting);
            }

            return instantiate(voting.GetVotingType(), voting, isPlayerCongressman, canVote);
        }
    }
}