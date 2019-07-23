using Entities;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using Sociatis.Helpers;
using Sociatis.Models.Avatar;
using Sociatis.Models.Infos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebServices;
using WebServices.Helpers;
using WebServices.structs;

namespace Sociatis.Models.Companies
{
    public class CompanyInfoViewModel : BaseEntitySummaryViewModel
    {
        
        public ImageViewModel Avatar { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int? OwnerID { get; set; }
        public string OwnerName { get; set; }
        public int Stock { get; set; }
        public double Queue { get; set; }
        public int Quality { get; set; }

        public string RegionName { get; set; }
        public string CountryName { get; set; }
        public int RegionID { get; set; }
        public int CountryID { get; set; }
        public bool CanLeaveCompany { get; set; } = false;

        public string Fertility { get; set; } = "None";
        public bool IsRawCompany { get; set; }
        public bool IsEntityThisCompany { get; set; } = false;
        public CompanyRights CompanyRights { get; set; }

        public bool CanShowUpgradeButton { get; set; } = false;
        public bool IsUpgradeButtonDisabled { get; set; } = false;
        public string UpgradeButtonDisabledReason { get; set; }
        public double UpgradeCost { get; set; }
        public int UpgradeConstructionCost { get; set; }

        public CompanyTypeEnum CompanyTypeEnum { get; set; }

        public InfoMenuViewModel Menu { get; set; } = new InfoMenuViewModel();
        public AvatarChangeViewModel AvatarChange { get; set; }

        public CompanyInfoViewModel(Company company)
            :base(SessionHelper.Session)
        {
            var companyService = DependencyResolver.Current.GetService<ICompanyService>();
            var regionRepository = DependencyResolver.Current.GetService<IRegionRepository>();

            CompanyRights = companyService.GetCompanyRights(company, SessionHelper.CurrentEntity, SessionHelper.LoggedCitizen);

            var entity = company.Entity;
            IsEntityThisCompany = SessionHelper.CurrentEntity.EntityID == company.ID;

            Avatar = new ImageViewModel(entity.ImgUrl);
            CompanyID = entity.EntityID;
            CompanyName = entity.Name;
            Queue = (double)company.Queue;
            Stock = company.GetProducedProductItem().Amount;
            Quality = company.Quality;

            OwnerName = company.Owner?.Name;
            OwnerID = company.OwnerID;

            var region = company.Region;
            var country = region.Country;

            RegionName = region.Name;
            RegionID = region.ID;
            CountryName = country.Entity.Name;
            CountryID = country.ID;

            processWorkButton(company);

           
            CompanyTypeEnum = (CompanyTypeEnum)company.CompanyTypeID;

            IsRawCompany = company.CompanyTypeID == (int)CompanyTypeEnum.Producer;
            if (IsRawCompany)
            {
                ResourceTypeEnum resourceType = companyService.GetResourceTypeForProduct((ProductTypeEnum)company.ProductID);
                var resource = regionRepository.GetResourceForRegion(company.RegionID, resourceType);
                if (resource != null)
                    switch (resource.ResourceQuality)
                    {
                        case 1:
                            Fertility = "Low"; break;
                        case 2:
                            Fertility = "Medium"; break;
                        case 3:
                            Fertility = "High"; break;
                        case 4:
                            Fertility = "Abundant"; break;

                    }
            }

            if (CompanyRights.CanUpgradeCompany)
            {
                CanShowUpgradeButton = companyService.IsUpgradeAble(company);
                if (CanShowUpgradeButton)
                {
                    var canUpgradeResult = companyService.CanUpgradeCompany(company, SessionHelper.CurrentEntity, SessionHelper.LoggedCitizen);

                    if (canUpgradeResult.IsError)
                    {
                        IsUpgradeButtonDisabled = true;
                        UpgradeButtonDisabledReason = canUpgradeResult.ToString(",");

                    }
                    else
                    {
                        UpgradeCost = companyService.GetUpgradeCost(company);
                        UpgradeConstructionCost = companyService.GetUpgradeConstructionPointsNeeded(company);
                    }
                }

            }

            if (CompanyRights.CanSwitch)
            {
                AvatarChange = new AvatarChangeViewModel(company.ID);
            }

            createMenu(company);

        }

        private void processWorkButton(Company company)
        {
            if (company.CompanyEmployees.Any(ce => ce.CitizenID == SessionHelper.CurrentEntity.EntityID))
            {
                if (SessionHelper.CurrentEntity.EntityID == SessionHelper.LoggedCitizen.ID)
                {
                    var ceRepository = DependencyResolver.Current.GetService<ICompanyEmployeeRepository>();

                    var employee = ceRepository
                        .Where(ce => ce.CitizenID == SessionHelper.LoggedCitizen.ID)
                        .Include(ce => ce.JobContract)
                        .First();

                    CanLeaveCompany = true;

                    if (employee.JobContract != null)
                    {
                        var contract = employee.JobContract;

                        CanLeaveCompany = contract.AbusedByCompany;
                    }
                }
            }
        }

        private void createMenu(Company company)
        {
            if (CompanyRights.HaveAnyRights)
            {
                var manage = new InfoExpandableViewModel("Manage", "fa-cogs");

                if (CanShowUpgradeButton)
                {
                    if (IsUpgradeButtonDisabled)
                        manage.AddChild(new InfoDisabledActionViewModel("Upgrade", "fa-caret-square-o-up", UpgradeButtonDisabledReason));
                    else
                        manage.AddChild(new InfoCustomActionViewModel("Upgrade", "fa-caret-square-o-up", $"Sociatis.Company.Upgrade({CompanyID}, {UpgradeCost}, {UpgradeConstructionCost}, {Quality+1})"));

                }
                if (company.ProductID == (int)ProductTypeEnum.MedicalSupplies && CompanyRights.CanPostJobOffers)
                    manage.AddChild(new InfoActionViewModel("Manage", "Hospital", "Manage Hospital", "fa-medkit", new { hospitalID = company.ID }));

                if (CompanyRights.CanManageEquipment)
                    manage.AddChild(new InfoActionViewModel("Inventory", "Company", "Inventory", "fa-cubes", new { companyID = CompanyID }));
                if (CompanyRights.CanSeeWallet)
                    manage.AddChild(new InfoActionViewModel("Wallet", "Company", "Wallet", "fa-dollar", new { companyID = CompanyID }));

                var finances = new InfoExpandableActionViewModel("Finances", "fa-dollar")
                    .AddChild(new InfoActionViewModel("DaySummary", "CompanyFinance", "Day's summary", "", new { companyID = CompanyID, day = GameHelper.CurrentDay }))
                    .AddChild(new InfoActionViewModel("Overview", "CompanyFinance", "Overview", "", new { companyID = CompanyID }));

                manage.AddChild(finances);

                manage.AddChild(new InfoActionViewModel("Managers", "Company", "Managers", "fa-user-circle", new { companyID = CompanyID }));
                if (CompanyRights.HaveAnyRights && IsEntityThisCompany == false)
                    manage.AddChild(InfoActionViewModel.CreateEntitySwitch(CompanyID));



                Menu.AddItem(manage);
            }

            var market = new InfoExpandableViewModel("Market", "fa-shopping-cart");

            market.AddChild(new InfoActionViewModel("JobOffers", "Company", "Job Offers", "fa-file-o", new { companyID = CompanyID }))
               .AddChild(new InfoActionViewModel("MarketOffers", "Company", "Market Offers", "fa-usd", new { companyID = CompanyID }));

            Menu.AddItem(market);

            if (CanLeaveCompany)
                Menu.AddItem(new InfoActionViewModel("LeaveJob", "Company", "Leave Job", "fa-minus-square", FormMethod.Post, new { companyID = CompanyID }));

            if (SessionHelper.CurrentEntity.EntityID != CompanyID)
                Menu.AddItem(InfoExpandableViewModel.CreateExchange(CompanyID));

            var info = new InfoExpandableViewModel("Info", "fa-info");

            info.AddChild(new InfoActionViewModel("Management", "Company", "Company management", "fa-users", new { companyID = CompanyID }));

            Menu.AddItem(info);
            
        }
    }
}
