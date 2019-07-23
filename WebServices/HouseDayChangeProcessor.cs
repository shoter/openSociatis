using Entities;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Houses;

namespace WebServices
{
    public class HouseDayChangeProcessor
    {
        private readonly IHouseFurnitureRepository houseFurnitureRepository;
        private readonly IHouseChestService houseChestService;

        public HouseDayChangeProcessor(IHouseFurnitureRepository houseFurnitureRepository, IHouseChestService houseChestService)
        {
            this.houseFurnitureRepository = houseFurnitureRepository;
            this.houseChestService = houseChestService;
        }
        public void ProcessDayChange(House house, int newDay)
        {
            var furnitures = getFurniture(house).ToList();

            processFurniture(newDay, furnitures);
            processDecay(house, furnitures);
            processRepair(house);
        }

        private void processRepair(House house)
        {
            int neededCM = CalculateNeededCM(house.Condition);
            if (neededCM > 0)
            {
                var chest = house.GetFurniture(FurnitureTypeEnum.Chest).HouseChest;
                var cm = houseChestService.GetChestItem(chest, ProductTypeEnum.ConstructionMaterials, 1);
                if (cm != null)
                {
                    int quantity = Math.Min(cm.Quantity, neededCM);
                    houseChestService.RemoveItem(chest, ProductTypeEnum.ConstructionMaterials, 1, quantity);
                }
            }
        }

        public int CalculateNeededCM(decimal condition)
        {
            if (condition >= 100m)
                return 0;

            return (int)Math.Ceiling(100m - condition);
        }

        private static void processDecay(House house, IEnumerable<IHouseFurnitureObject> furnitures)
        {
            foreach (var furniture in furnitures)
            {
                house.Condition -= furniture.CalculateDecay();
            }
        }

        private static void processFurniture(int newDay, IEnumerable<IHouseFurnitureObject> furnitures)
        {
            foreach (var furniture in furnitures)
            {
                furniture.Process(newDay);
            }
        }


        private IEnumerable<IHouseFurnitureObject> getFurniture(House house)
        {
            var furnitures = houseFurnitureRepository.
                Where(f => f.HouseID == house.ID)
                .Include(f => f.HouseChest)
                .Include(f => f.House)
                .ToList();

            foreach (var furniture in furnitures)
            {
                yield return HouseFurnitureObjectFactory.CreateHouseFurniture(furniture);
            }
        }

    }
}
