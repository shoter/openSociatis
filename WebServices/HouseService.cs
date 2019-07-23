using Common.Extensions;
using Common.Operations;
using Entities;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using SociatisCommon.Errors;
using SociatisCommon.Rights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Houses;

namespace WebServices
{
    public class HouseService : BaseService, IHouseService
    {
        private readonly IHouseRepository houseRepository;
        private readonly HouseDayChangeProcessor houseDayChangeProcessor;
        private readonly IHouseFurnitureRepository houseFurnitureRepository;
        private readonly IWalletService walletService;
        private readonly IMarketService marketService;
        private readonly IHouseChestService houseChestService;
        public HouseService(IHouseRepository houseRepository, HouseDayChangeProcessor houseDayChangeProcessor, IWalletService walletService,
            IMarketService marketService, IHouseChestService houseChestService, IHouseFurnitureRepository houseFurnitureRepository)
        {
            this.houseRepository = houseRepository;
            this.houseDayChangeProcessor = houseDayChangeProcessor;
            this.walletService = walletService;
            this.marketService = marketService;
            this.houseChestService = houseChestService;
            this.houseFurnitureRepository = houseFurnitureRepository;
        }
        public House CreateHouseForCitizen(Citizen citizen)
        {
            var house = new House()
            {
                CitizenID = citizen.ID,
                Condition = 100m,
                RegionID = citizen.RegionID,
            };
            house.HouseFurnitures.Add(new HouseFurniture()
            {
                FurnitureTypeID = (int)FurnitureTypeEnum.Chest,
                Quality = 1,
                HouseChest = new HouseChest()
                {
                    Capacity = 20
                }
            });

            houseRepository.Add(house);
            ConditionalSaveChanges(houseRepository);

            return house;
        }

        public MethodResult CanCreateHouseForCitizen(Citizen citizen)
        {
            if (citizen == null)
                return new MethodResult("Citizen does not exist!");

            if (citizen.Houses.Any(h => h.RegionID == citizen.RegionID))
                return new MethodResult("You already have house in this region!");

            return MethodResult.Success;
        }

        public void ProcessDayChange(int newDay)
        {
            foreach (var house in houseRepository
                .Include(h => h.Citizen).ToList())
            {
                houseDayChangeProcessor.ProcessDayChange(house, newDay);
            }
        }

        public MethodResult CanViewHouse(House house, Entity entity)
        {
            if (house == null)
                return new MethodResult("House does not exist!");
            if (entity == null)
                return new MethodResult("Entity does not exist!");

            return MethodResult.Success;
        }

        public MethodResult CanModifyHouse(House house, Entity entity)
        {
            var result = CanViewHouse(house, entity);
            if (result.IsError)
                return result;

            if (house.CitizenID != entity.EntityID)
                return new MethodResult("It is not your house!");

            return MethodResult.Success;
        }

        public MethodResult CanBuyOffer(MarketOffer offer, int amount, House house, Entity entity)
        {
            if (offer == null)
                return new MethodResult(MarketOfferErrors.OfferNotExist);

            var productType = ((ProductTypeEnum)offer.ProductID);

            if (offer.Amount < amount)
                return new MethodResult(MarketOfferErrors.NotEnoughProducts);

            if (amount <= 0)
                return new MethodResult("Are you crazy?");

            if (productType != ProductTypeEnum.UpgradePoints && productType != ProductTypeEnum.ConstructionMaterials)
                return new MethodResult(MarketOfferErrors.NotAllowedProduct);

            if (offer.Company.GetCompanyType() != CompanyTypeEnum.Manufacturer)
                return new MethodResult("You cannot buy that offer!");
            
            var cost = marketService.GetOfferCost(offer, entity, amount);

            if (cost == null)
                return new MethodResult("You cannot buy that offer!");

            if (walletService.HaveMoney(entity.WalletID, cost.ClientPriceMoney) == false)
                return new MethodResult(MarketOfferErrors.NotEnoughMoney);

            if (productType == ProductTypeEnum.ConstructionMaterials)
            {
                var chest = houseFurnitureRepository.GetHouseChest(house.ID);

                if (houseChestService.GetUnusedSpace(chest) < amount)
                    return new MethodResult("You do not have enough space in chest!");
            }

           

            return MethodResult.Success;
        }

        public MethodResult CanUpgradeFurniture(House house, FurnitureTypeEnum furnitureType)
        {
            if (house == null)
                return new MethodResult("House does not exist!");
            var furniture = houseFurnitureRepository.GetFurniture(house.ID, furnitureType);
            if (furniture == null)
                return new MethodResult("Furniture does not exist!");


            var furnitureObject = HouseFurnitureObjectFactory.CreateHouseFurniture(furniture);

            if (furnitureObject.CanUpgrade() == false)
                return new MethodResult("You cannot further upgrade this furniture");


            var cost = furnitureObject.GetUpgradeCost();
            var chest = houseFurnitureRepository.GetHouseChest(house.ID);

            if (houseChestService.HasItem(chest, ProductTypeEnum.UpgradePoints, 1, cost) == false)
                return new MethodResult($"You do not have {cost} Upgrade Points to upgrade {furnitureObject.ToHumanReadable()}.");

            return MethodResult.Success;
        }

        public MethodResult CanBuildFurniture(House house, FurnitureTypeEnum furnitureType, Entity entity)
        {
            var result = CanModifyHouse(house, entity);
            if (result.IsError)
                return result;

            if (houseFurnitureRepository.IsFurnitureBuilt(house.ID, furnitureType))
                return new MethodResult($"{furnitureType.ToHumanReadable().FirstUpper()} is already built!");

            var cost = HouseFurnitureObject.GetUpgradeCost(furnitureType, 1);
            var chest = houseFurnitureRepository.GetHouseChest(house.ID);

            if (houseChestService.HasItem(chest, ProductTypeEnum.ConstructionMaterials, 1, cost) == false)
                return new MethodResult($"You do not have {cost} Construction materials to create {furnitureType.ToHumanReadable()}.");

            return MethodResult.Success;
        }

        public void BuildFurniture(House house, FurnitureTypeEnum furnitureType)
        {
            var furniture = new HouseFurniture()
            {
                House = house,
                FurnitureTypeID = (int)furnitureType,
                Quality = 1
            };

            houseFurnitureRepository.Add(furniture);
            ConditionalSaveChanges(houseFurnitureRepository);
        }

        public void UpgradeFurniture(House house, FurnitureTypeEnum furnitureType)
        {
            var furniture = houseFurnitureRepository.GetFurniture(house.ID, furnitureType);
            var furnitureObject = HouseFurnitureObjectFactory.CreateHouseFurniture(furniture);

            payForUpgrade(house, furnitureObject);
            furnitureObject.Upgrade();

            houseRepository.SaveChanges();
        }

        private void payForUpgrade(House house, IHouseFurnitureObject furnitureObject)
        {
            var cost = furnitureObject.GetUpgradeCost();
            var chest = houseFurnitureRepository.GetHouseChest(house.ID);
            houseChestService.RemoveItem(chest, ProductTypeEnum.UpgradePoints, 1, cost);
        }

        public HouseRights GetHouseRights(House house, Entity entity)
        {
            return new HouseRights(
                CanViewHouse(house, entity).isSuccess,
                CanModifyHouse(house, entity).isSuccess
                );
        }
    }
}
