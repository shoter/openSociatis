using Common.utilities;
using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Test_Suite.Dummies
{
    public class HouseFurnitureDummyCreator : IDummyCreator<HouseFurniture>
    {
        private static readonly UniqueIDGenerator uniqueID = new UniqueIDGenerator();
        private HouseFurniture houseFurniture;
        private readonly House house;

        public HouseFurnitureDummyCreator(House house)
        {
            this.house = house;

            initNew();
        }

        private void initNew()
        {
            houseFurniture = new HouseFurniture()
            {
                House = house,
                HouseID = house.ID,
                ID = (long)uniqueID,
                FurnitureTypeID = (int)FurnitureTypeEnum.Bed,
                Quality = 1,
            };
        }

        public HouseFurnitureDummyCreator SetFurnitureType(FurnitureTypeEnum furnitureType)
        {
            houseFurniture.FurnitureTypeID = (int)furnitureType;
            switch (furnitureType)
            {
                case FurnitureTypeEnum.Chest:
                    houseFurniture.HouseChest = new HouseChest()
                    {
                        Capacity = 20,
                        FurnitureID = houseFurniture.ID,
                        HouseFurniture = houseFurniture
                    };
                    break;
            }

            return this;
        }

        public HouseFurnitureDummyCreator SetQuality(int quality)
        {
            houseFurniture.Quality = quality;
            return this;
        }


        public HouseFurniture Create()
        {
            house.HouseFurnitures.Add(houseFurniture);
            var temp = houseFurniture;
            initNew();
            return temp;
        }
    }
}
