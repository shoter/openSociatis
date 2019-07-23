using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace WebServices.Houses
{
    public class HouseChestObject : HouseFurnitureObject
    {
        public int Capacity { get; set; }
        public HouseChest ChestEntity { get; set; }
        public HouseChestObject(HouseFurniture furniture) : base(furniture)
        {
            Capacity = furniture.HouseChest.Capacity;
            ChestEntity = furniture.HouseChest;
        }

        public override decimal CalculateDecay()
        {
            return Quality * 0.01m + 0.01m;
        }

        public override void Process(int newDay)
        {
            return;
        }

        public override void AfterUpgrade(int newQuality)
        {
            ChestEntity.Capacity = GetCapacity(newQuality);
        }

        public static int GetCapacity(int quality)
        {
            switch (quality)
            {
                case 1:
                    return 20;
                case 2:
                    return 35;
                case 3:
                    return 70;
                case 4:
                    return 125;
                case 5:
                    return 200;
            }
            throw new ArgumentException();
        }

        public static int GetUpgradeCost(int quality)
        {
            decimal mod = 0.1m;
            switch (quality)
            {
                case 1:
                    return (int)(0 * mod);
                case 2:
                    return (int)(50 * mod);
                case 3:
                    return (int)(150 * mod);
                case 4:
                    return (int)(350 * mod);
                case 5:
                    return (int)(1000 * mod);
            }
            throw new ArgumentException();
        }

        public override int GetUpgradeCost()
        {
            return HouseChestObject.GetUpgradeCost(Quality + 1);
        }
    }
}
