using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs
{
    public class CompanyRights
    {
        public bool this[CompanyRightsEnum rights]
        {
            get
            {
                switch (rights)
                {
                    case CompanyRightsEnum.ChangeAvatar:
                        return CanChangeAvatar;
                    case CompanyRightsEnum.ManageAndSeeEquipment:
                        return CanManageEquipment;
                    case CompanyRightsEnum.ManageWorkers:
                        return CanManageWorkers;
                    case CompanyRightsEnum.ManageManagers:
                        return CanManageManagers;
                    case CompanyRightsEnum.PostJobOffers:
                        return CanPostJobOffers;
                    case CompanyRightsEnum.PostMarketOffers:
                        return CanPostMarketOffers;
                    case CompanyRightsEnum.SeeWallet:
                        return CanSeeWallet;
                    case CompanyRightsEnum.Switch:
                        return CanSwitch;
                    case CompanyRightsEnum.UpgradeCompany:
                        return CanUpgradeCompany;
                }

                throw new NotImplementedException();
            }
            set
            {
                switch (rights)
                {
                    case CompanyRightsEnum.ChangeAvatar:
                        CanChangeAvatar = value; break;
                    case CompanyRightsEnum.ManageAndSeeEquipment:
                        CanManageEquipment = value; break;
                    case CompanyRightsEnum.ManageWorkers:
                        CanManageWorkers = value; break;
                    case CompanyRightsEnum.ManageManagers:
                        CanManageManagers = value; break;
                    case CompanyRightsEnum.PostJobOffers:
                        CanPostJobOffers = value; break;
                    case CompanyRightsEnum.PostMarketOffers:
                        CanPostMarketOffers = value; break;
                    case CompanyRightsEnum.SeeWallet:
                        CanSeeWallet = value; break;
                    case CompanyRightsEnum.Switch:
                        CanSwitch = value; break;
                    case CompanyRightsEnum.UpgradeCompany:
                        CanUpgradeCompany = value; break;
                    default:
                        throw new NotImplementedException();
                }

                
            }
        }
        public int Priority { get; set; }
        public bool IsManager { get; set; }
        public bool CanPostJobOffers { get; set; }
        public bool CanManageWorkers { get; set; }
        public bool CanPostMarketOffers { get; set; }
        public bool CanManageManagers { get; set; }
        public bool CanChangeAvatar { get; set; }
        public bool CanSeeWallet { get; set; }
        public bool CanManageEquipment { get; set; }
        public bool CanSwitch { get; set; }
        public bool CanUpgradeCompany { get; set; }
        public bool CanAcceptConstruction => CanSeeWallet || CanManageEquipment;

        public bool HaveAnyRights
        {
            get
            {
                return
                    CanChangeAvatar ||
                    CanManageManagers ||
                    CanPostJobOffers ||
                    CanPostMarketOffers ||
                    CanSeeWallet ||
                    CanManageEquipment ||
                    CanManageWorkers ||
                    CanSwitch ||
                    CanUpgradeCompany ||
                    IsManager;
            }
        }

        public CompanyRights(CompanyManager manager)
        {
            CanPostJobOffers = manager.PostJobOffers;
            CanManageWorkers = manager.ManageWorkers;
            CanPostMarketOffers = manager.PostMarketOffers;
            CanManageManagers = manager.ManageManagers;
            CanChangeAvatar = manager.ChangeAvatar;
            CanSeeWallet = manager.SeeWallet;
            CanManageEquipment = manager.ManageEquipment;
            CanSwitch = manager.Switch;
            CanUpgradeCompany = manager.UpgradeCompany;
            Priority = manager.Priority;
            IsManager = true;
        }

        public CompanyRights(bool fullRights, int? priority = null)
        {
            CanPostJobOffers = CanManageManagers =
                CanManageWorkers = CanPostMarketOffers =
                CanChangeAvatar = CanSeeWallet = 
                CanManageEquipment = CanSwitch = 
                CanUpgradeCompany = fullRights;
            IsManager = fullRights;
            if (fullRights)
                Priority = int.MaxValue;
            if (priority.HasValue)
                Priority = priority.Value;
        }

        public void FillEntity(ref CompanyManager manager)
        {
            manager.ChangeAvatar = CanChangeAvatar;
            manager.ManageEquipment = CanManageEquipment;
            manager.ManageManagers = CanManageManagers;
            manager.ManageWorkers = CanManageWorkers;
            manager.SeeWallet = CanSeeWallet;
            manager.PostMarketOffers = CanPostMarketOffers;
            manager.PostJobOffers = CanPostJobOffers;
            manager.Switch = CanSwitch;
            manager.UpgradeCompany = CanUpgradeCompany;
            manager.Priority = Priority;
        }

        public static CompanyRights FullRgihts => new CompanyRights(true);
    }
}
