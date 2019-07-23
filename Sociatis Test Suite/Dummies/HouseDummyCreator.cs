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
    public class HouseDummyCreator : IDummyCreator<House>
    {
        private static readonly UniqueIDGenerator uniqueID = new UniqueIDGenerator();
        private House house;

        public HouseDummyCreator()
        {
            initNewHouse();
        }

        private void initNewHouse()
        {
            house = new House()
            {
                Condition = 100m,
                ID = (long)uniqueID,
            };

            new HouseFurnitureDummyCreator(house)
                .SetFurnitureType(FurnitureTypeEnum.Chest)
                .Create();

            SetCitizen(new CitizenDummyCreator().Create())
                .SetRegion(house.Citizen.Region);
        }

        public HouseDummyCreator SetSellOffer(decimal price = 123m)
        {
            house.SellHouse = new SellHouse()
            {
                House = house,
                HouseID = house.ID,
                Price = price
            };
            return this;
        }

        public HouseDummyCreator SetCitizen(Citizen citizen)
        {
            house.Citizen = citizen;
            house.CitizenID = citizen.ID;
            citizen.Houses.Add(house);

            return this;
        }

        public HouseDummyCreator SetRegion(Region region)
        {
            house.Region = region;
            house.RegionID = region.ID;
            region.Houses.Add(house);

            return this;
        }


        public House Create()
        {
            var temp = house;
            initNewHouse();
            return temp;
        }
    }
}
