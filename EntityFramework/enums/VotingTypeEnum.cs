using Entities.enums.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums
{
    public enum VotingTypeEnum
    {
        [AlwaysVotable]
        ChangeMinimumContractLength = 2,
        [AlwaysVotable]
        ChangeMaximumContractLength = 3,
        [AlwaysVotable]
        ChangeCongressVotingLength = 4,
        [AlwaysVotable]
        ChangePartyPresidentCadenceLength = 5,
        [AlwaysVotable]
        ChangeCongressCadenceLength = 6,
        [AlwaysVotable]
        ChangeNormalJobMarketFee = 7,
        [AlwaysVotable]
        ChangeContractJobMarketFee = 8,
        [AlwaysVotable]
        ChangeProductVAT = 9,
        [AlwaysVotable]
        ChangeProductImportTax = 10,
        [AlwaysVotable]
        ChangeProductExportTax = 11,
        [AlwaysVotable]
        ChangeArticleTax = 12,
        [AlwaysVotable]
        ChangeNewspaperCreateCost = 13,
        [AlwaysVotable]
        ChangeMarketOfferCost = 14,
        [AlwaysVotable]
        ChangeOrganisationCreateCost = 15,
        [AlwaysVotable]
        ChangePresidentCadenceLength = 16,
        [AlwaysVotable]
        ChangePartyCreateFee = 17,
        [AlwaysVotable]
        ChangeNormalCongressVotingWinPercentage = 18,
        [AlwaysVotable]
        ChangeCitizenCompanyCost = 19,
        [AlwaysVotable]
        ChangeOrganisationCompanyCost = 20,
        [AlwaysVotable]
        ChangeMonetaryTaxRate = 21,
        [AlwaysVotable]
        ChangeMinimumMonetaryTaxValue = 22,
        [AlwaysVotable]
        ChangeTreasureLawHolder = 23,
        [AlwaysVotable]
        ChangeCompanyCreationLawHolder = 24,
        [AlwaysVotable(false)]
        CreateNationalCompany = 25,
        [AlwaysVotable]
        RemoveNationalCompany = 26,
        [AlwaysVotable]
        AssignManagerToCompany = 27,
        [AlwaysVotable]
        TransferCashToCompany = 28,
        [AlwaysVotable]
        ChangeGreetingMessage = 29,
        [AlwaysVotable]
        ChangeCitizenStartingMoney = 30,
        [AlwaysVotable]
        PrintMoney = 31,
        [AlwaysVotable]
        ChangeMinimalWage = 32,
        [AlwaysVotable]
        BuildDefenseSystem = 33,
        [AlwaysVotable]
        ChangeHouseTax = 34,
        [AlwaysVotable]
        ChangeHotelTax = 35,
    }

    public static class VotingTypeEnumExtensions
    {
        public static string ToHumanReadable(this VotingTypeEnum votingType)
        {
            switch(votingType)
            {
                case VotingTypeEnum.ChangeMinimumContractLength:
                    return "Change minimum contract length";
                case VotingTypeEnum.ChangeMaximumContractLength:
                    return "Change maximum contract length";
                case VotingTypeEnum.ChangeCongressVotingLength:
                    return "Change congress voting length";
                case VotingTypeEnum.ChangePartyPresidentCadenceLength:
                    return "Change party president cadence length";
                case VotingTypeEnum.ChangeCongressCadenceLength:
                    return "Change congress cadence length";
                case VotingTypeEnum.ChangeNormalJobMarketFee:
                    return "Change normal job market fee";
                case VotingTypeEnum.ChangeContractJobMarketFee:
                    return "Change contract job market fee";
                case VotingTypeEnum.ChangeProductVAT:
                    return "Change product vat";
                case VotingTypeEnum.ChangeProductImportTax:
                    return "Change product import tax";
                case VotingTypeEnum.ChangeProductExportTax:
                    return "Change product export tax";
                case VotingTypeEnum.ChangeArticleTax:
                    return "Change article tax";
                case VotingTypeEnum.ChangeNewspaperCreateCost:
                    return "Change newspaper create cost";
                case VotingTypeEnum.ChangeMarketOfferCost:
                    return "Change market offer cost";
                case VotingTypeEnum.ChangeOrganisationCreateCost:
                    return "Change organisation create cost";
                case VotingTypeEnum.ChangePresidentCadenceLength:
                    return "Change president cadence length";
                case VotingTypeEnum.ChangePartyCreateFee:
                    return "Change party creation fee";
                case VotingTypeEnum.ChangeNormalCongressVotingWinPercentage:
                    return "Change normal congress voting win percentage";
                case VotingTypeEnum.ChangeCitizenCompanyCost:
                    return "Change citizen company cost";
                case VotingTypeEnum.ChangeOrganisationCompanyCost:
                    return "Change organisation company cost";
                case VotingTypeEnum.ChangeMonetaryTaxRate:
                    return "Change monetary tax rate";
                case VotingTypeEnum.ChangeMinimumMonetaryTaxValue:
                    return "Change minimum monetary tax value";
                case VotingTypeEnum.ChangeTreasureLawHolder:
                    return "Change who can see the treasury";
                case VotingTypeEnum.ChangeCompanyCreationLawHolder:
                    return "Change who can create national companies";
                case VotingTypeEnum.CreateNationalCompany:
                    return "Create national company";
                case VotingTypeEnum.RemoveNationalCompany:
                    return "Remove national company";
                case VotingTypeEnum.AssignManagerToCompany:
                    return "Assign manager to company";
                case VotingTypeEnum.TransferCashToCompany:
                    return "Transfer money to company";
                case VotingTypeEnum.ChangeGreetingMessage:
                    return "Change greeting message";
                case VotingTypeEnum.ChangeCitizenStartingMoney:
                    return "Change citizen starting money";
                case VotingTypeEnum.PrintMoney:
                    return "Print money";
                case VotingTypeEnum.ChangeMinimalWage:
                    return "Change minimal wage";
                case VotingTypeEnum.BuildDefenseSystem:
                    return "Build defense system";
                case VotingTypeEnum.ChangeHouseTax:
                    return "Change house tax";
                case VotingTypeEnum.ChangeHotelTax:
                    return "Change hotel tax";

            }
            throw new NotImplementedException();
        }
    }
}
