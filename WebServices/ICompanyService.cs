using Common.Operations;
using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.BigParams.Company;
using WebServices.structs;
using WebServices.structs.Companies;

namespace WebServices
{
    public interface ICompanyService
    {
        List<ManageableCompanyInfo> GetManageableCompanies(Entity entity);
        Company CreateCompany(string name, ProductTypeEnum productType, int regionID, int ownerID);
        CompanyEmployee EmployCitizen(EmployCitizenParameters pars);
        /// <summary>
        /// does NOT return null if entity does not have rights
        /// </summary>
        CompanyRights GetCompanyRights(Company company, Entity currentEntity, Citizen loggedCitizen);
        bool DoesHaveRightTo(Company company, Entity currentEntity, Citizen loggedCitizen, CompanyRightsEnum right);
        bool DoesHaveRightTo(Company company, Entity currentEntity, CompanyRightsEnum right);
        bool IsWorkingAtCompany(int citizenID, int companyID);
        bool CanLeaveJob(CompanyEmployee employee);
        void LeaveJob(int citizenID, int companyID);
        MethodResult Work(int citizenID, int companyID);
        MethodResult CanWork(Citizen citizen, Company company);

        double GetProductionPoints(Citizen citizen, Company company);
        int GetHitPointsLostFromWork(ProductTypeEnum prodctType, int quality = 1);
        bool DoesHaveEnoughRawToWork(Citizen citizen, Company company);
        List<ProductRequirement> GetNeededResourcesForWork(Citizen citizen, Company company);
        WorkStatistics GetWorkStatistics(int citizenID);
        int GetNeededSpaceForWork(Citizen citizen, Company company);
        double GetTodaySalary(Citizen citizen, CompanyEmployee employee);

        bool DoesCompanyHaveEnoughMoneyForSalary(Citizen citizen, Company company);
        
        void InformAboutNewEmployee(Company company, CompanyEmployee employee);

        void InformAboutNotEnoughSpaceForWork(Company company, CompanyEmployee employee, int neededSpace);
        void InformAboutNotEnoughSalaryForWork(Company company, CompanyEmployee employee, double neededSalary);

        void InformAboutNotEnoughRawToWork(Company company, CompanyEmployee employee, List<ProductRequirement> requirements);
        MethodResult CanStartWorkAt(int jobOfferID, Citizen citizen);
        void ProcessDayChange(int newDay);
        ResourceTypeEnum GetResourceTypeForProduct(ProductTypeEnum productType);
        IEnumerable<Money> GetCompanyCreationCost(Region region, EntityTypeEnum creator);
        Company CreateCompanyForCountry(string name, ProductTypeEnum productType, int regionID, Country country, bool payForCreation = true);
        void AddManager(Company company, Citizen citizen, CompanyRights rights);

        MethodResult CanUpgradeCompany(Company company, Entity entity, Citizen loggedCitizen);
        void UpgradeCompany(Company company);
        double GetUpgradeCost(Company company);
        bool IsUpgradeAble(Company company);

        MethodResult CanAddManager(Company company, Entity addingEntity, Entity addedEntity, CompanyRights addedRights);
        MethodResult CanModifyManager(Company company, Entity modifyingEntity, CompanyManager manager, CompanyRights newRights);
        MethodResult CanSeeManagers(Company company, Entity entity);

        void ModifyManager(CompanyManager manager, CompanyRights rights);
        MethodResult CanRemoveManager(CompanyManager manager, Entity byWho);
        void RemoveManager(CompanyManager manager);
        decimal CalculateCreatedDevelopementForCreatedItems(decimal producedItems);

        int GetUpgradeConstructionPointsNeeded(Company company);
        void RemoveJobOffersThatDoesNotMeetMinimalWage(decimal minimalWage, int countryID);


        bool CanUseRawMultiplier(Company company);
        bool CanUseQualityModifier(Company company);
        double GetProductionPoints(ProductTypeEnum producedProduct, int hitPoints, double skill, double distance, int quality, int resourceQuality, double regionDevelopment, int peopleCount);

        IEnumerable<CompanyTypeEnum> GetCompaniesCreatableByPlayers();
        IEnumerable<ProductTypeEnum> GetCompaniesProductsCreatableByPlayers();
    }
}
