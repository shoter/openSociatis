using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Operations;
using Entities;
using WebServices.structs;
using Entities.Repository;
using Entities.enums;
using Entities.Models.Hospitals;
using WebServices.structs.Hospitals;
using WebServices.Helpers;
using System.Web.Mvc;
using Common.Extensions;

namespace WebServices
{
    public class HospitalService : BaseService, IHospitalService
    {
        private readonly IHospitalRepository hospitalRepository;
        private readonly IWalletService walletService;
        private readonly IEquipmentService equipmentService;
        private readonly IReservedEntityNameRepository reservedEntityNameRepository;
        private readonly IRegionRepository regionRepository;
        private readonly ICompanyService companyService;
        private readonly ITransactionsService transactionsService;
        private readonly IConstructionRepository constructionRepository;

        public HospitalService(IHospitalRepository hospitalRepository, IWalletService walletService, IEquipmentService equipmentService,
            IReservedEntityNameRepository reservedEntityNameRepository, IRegionRepository regionRepository, ICompanyService companyService,
            ITransactionsService transactionsService, IConstructionRepository constructionRepository)
        {
            this.hospitalRepository = hospitalRepository;
            this.walletService = Attach(walletService);
            this.equipmentService = Attach(equipmentService);
            this.reservedEntityNameRepository = reservedEntityNameRepository;
            this.regionRepository = regionRepository;
            this.companyService = Attach(companyService);
            this.transactionsService = Attach(transactionsService);
            this.constructionRepository = constructionRepository;
        }

        public void ReserveNamesForNationalHospitals()
        {
            var regionNames = regionRepository
                .Where(r => r.Hospital == null)
                .Select(r => r.Name)
                .ToList();

            var hospitalNames = regionNames
                .Select(name => $"{name} Hospital");

            reservedEntityNameRepository.AddIfTheyDoNotExist(hospitalNames.ToArray());
            ConditionalSaveChanges(reservedEntityNameRepository);
        }

        public string GenerateNameForHospital(Region region)
        {
            return $"{ region.Name} Hospital";

        }

        public MethodResult CanHealCitizen(Citizen citizen, Hospital hospital)
        {
            if (hospital == null)
                return new MethodResult("Hospital does not exist!");
            if (hospital.HealingEnabled == false)
                return new MethodResult("This hospital does not provide healing service!");

            if (citizen.UsedHospital)
                return new MethodResult("You cannot use hospital more than 1 time per day!");

            var healingPrice = GetPriceForHealing(hospital, citizen);

            if (healingPrice.Amount > 0)
            {

                if (walletService.HaveMoney(citizen.Entity.WalletID, healingPrice) == false)
                    return new MethodResult($"You need {healingPrice.Amount} {healingPrice.Currency.Symbol} to heal yourself in this hospital!");
            }

            if (HaveHealingItem(hospital) == false)
                return new MethodResult("Hospital does not have enough medical supplies to heal you!");

            if (citizen.HitPoints == 100)
                return new MethodResult("You do not need healing!");

            if (citizen.RegionID != hospital.Company.RegionID)
                return new MethodResult($"You are not in the same region as {hospital.Company.Entity.Name}!");

            return MethodResult.Success;
        }

        public bool HaveHealingItem(Hospital hospital)
        {
            return equipmentService.HaveItem(hospital.Company.Entity.Equipment, ProductTypeEnum.MedicalSupplies, hospital.Company.Quality, 1).isSuccess;
        }

        public void HealCitizen(Citizen citizen, Hospital hospital)
        {
            equipmentService.RemoveProductsFromEquipment(ProductTypeEnum.MedicalSupplies, 1, hospital.Company.Quality, hospital.Company.Entity.Equipment);
            HealingPrice healingPrice = hospitalRepository.GetHealingPrice(hospital.CompanyID, citizen.ID);
            if (healingPrice.Cost.HasValue)
                transactionsService.PayForHealing(hospital, citizen, healingPrice);

            HealCitizenProcess(citizen, hospital.Company.Quality);
            ConditionalSaveChanges(hospitalRepository);
        }

        public virtual void HealCitizenProcess(Citizen citizen,  int quality)
        {
            citizen.HitPoints += GetHealAmount(quality);
            citizen.HitPoints = Math.Min(100, citizen.HitPoints);
            citizen.UsedHospital = true;
        }

        public virtual int GetHealAmount(int quality)
        {
            return quality * 10;
        }

        public Money GetPriceForHealing(Hospital hospital, Citizen citizen)
        {
            HealingPrice healingPrice = hospitalRepository.GetHealingPrice(hospital.CompanyID, citizen.ID);
            var currency = Persistent.Currencies.GetById(healingPrice.CurrencyID);

            return new Money(currency, healingPrice.Cost ?? 0m);
        }

        public void SetHealingPriceForNationality(Hospital hospital, int countryID, decimal? price)
        {
            hospitalRepository.SetHealingPrice(hospital.CompanyID, countryID, price);
            ConditionalSaveChanges(hospitalRepository);
        }

        

        public MethodResult CanManageHospital(Entity entity, Hospital hospital)
        {
            if (hospital == null)
                return new MethodResult("Hospital does not exist!");

            if (companyService.DoesHaveRightTo(hospital.Company, entity, CompanyRightsEnum.PostMarketOffers) == false)
                return new MethodResult("You cannot do that!");

            return MethodResult.Success;
        }

        public void UpdateHospital(Hospital hospital, decimal? healingPrice, bool healingEnabled)
        {
            hospital.HealingPrice = healingPrice;
            hospital.HealingEnabled = healingEnabled;
            ConditionalSaveChanges(hospitalRepository);
        }

        public void UpdateHospitalNationalityOptions(Hospital hospital, IEnumerable<UpdateHospitalNationalityOption> options)
        {
            hospital.HospitalNationalityHealingOptions.Clear();
            foreach (var option in options)
            {
                if (option.Price <= 0)
                    option.Price = null;

                hospital.HospitalNationalityHealingOptions.Add(new HospitalNationalityHealingOption()
                {
                    CountryID = option.CountryID,
                    HospitalID = hospital.CompanyID,
                    HealingPrice = option.Price
                });
            }

            hospitalRepository.SaveChanges();
        }
    }
}
