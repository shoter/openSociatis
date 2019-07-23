using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.Houses
{
    public abstract class HouseFurnitureObject : IHouseFurnitureObject
    {
        public long ID { get; set; }
        public int Quality { get; set; }
        public long HouseID { get; set; }
        public House HouseEntity { get; set; }
        public HouseFurniture FurnitureEntity { get; set; }
        public FurnitureTypeEnum FurnitureType { get; set; }
        public HouseFurnitureObject(HouseFurniture furniture)
        {
            ID = furniture.ID;
            FurnitureType = (FurnitureTypeEnum)furniture.FurnitureTypeID;
            Quality = furniture.Quality;
            HouseID = furniture.HouseID;
            HouseEntity = furniture.House;
            FurnitureEntity = furniture;
        }
        public void Upgrade()
        {
            FurnitureEntity.Quality++;
            AfterUpgrade(FurnitureEntity.Quality);
        }
        public abstract decimal CalculateDecay();
        public abstract void Process(int newDay);
        public virtual void AfterUpgrade(int newQuality) { } 
        public abstract int GetUpgradeCost();

        public virtual bool CanUpgrade()
        {
            return Quality <= 4;
        }

        public string ToHumanReadable()
        {
            return FurnitureType.ToHumanReadable();
        }

        public static int GetUpgradeCost(FurnitureTypeEnum furnitureType, int quality)
        {
            switch (furnitureType)
            {
                case FurnitureTypeEnum.Bed:
                    return HouseBedObject.GetUpgradeCost(quality);
                case FurnitureTypeEnum.Chest:
                    return HouseChestObject.GetUpgradeCost(quality);
            }
            throw new ArgumentException();
        }
    }
}
