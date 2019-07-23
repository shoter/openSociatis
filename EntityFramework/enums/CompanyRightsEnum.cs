using Common.utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums
{
    public enum CompanyRightsEnum
    {
        PostJobOffers,
        PostMarketOffers,
        ManageWorkers,
        ChangeAvatar,
        ManageManagers,
        SeeWallet,
        ManageAndSeeEquipment,
        Switch,
        UpgradeCompany


    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class CompanyRightsEnumExtensions
    {
        public static string ToHumanReadable(this CompanyRightsEnum right)
        {
            return StringUtils.PascalCaseToWord(right.ToString());
        }
        public static bool GetRight(this Entities.CompanyManager manager, CompanyRightsEnum companyRights)
        {
            switch(companyRights)
            {
                case CompanyRightsEnum.ChangeAvatar:
                    return manager.ChangeAvatar;
                case CompanyRightsEnum.ManageManagers:
                    return manager.ManageManagers;
                case CompanyRightsEnum.ManageWorkers:
                    return manager.ManageWorkers;
                case CompanyRightsEnum.PostJobOffers:
                    return manager.PostJobOffers;
                case CompanyRightsEnum.PostMarketOffers:
                    return manager.PostMarketOffers;
                case CompanyRightsEnum.SeeWallet:
                    return manager.SeeWallet;
                case CompanyRightsEnum.ManageAndSeeEquipment:
                    return manager.ManageEquipment;
                case CompanyRightsEnum.Switch:
                    return manager.Switch;
                case CompanyRightsEnum.UpgradeCompany:
                    return manager.UpgradeCompany;
            }

            throw new NotImplementedException();
        }
    }
}
