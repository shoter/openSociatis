using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.enums;
using Entities.Extensions;

namespace WebServices.Houses
{
    public class HouseBedObject : HouseFurnitureObject
    {
        private bool used = false;
        public HouseBedObject(HouseFurniture furniture) : base(furniture)
        {
        }

        public override decimal CalculateDecay()
        {
            if (used == false)
                return 0m;
            return CalculateDecay(Quality);
        }

        public static decimal CalculateDecay(int quality)
        {
            return quality * 0.05m;
        }

        public override void Process(int newDay)
        {
            if (HouseEntity.RegionID == HouseEntity.Citizen.RegionID)
            {
                if (HouseEntity.Citizen.HotelRooms.Any(r => r.Hotel.RegionID == HouseEntity.Citizen.RegionID))
                    return;

                int hpAdd = CalculateHealedHP();
                HouseEntity.Citizen.AddHealth(hpAdd);
                used = true;
            }
        }

        public int CalculateHealedHP()
        {
            return CalculateHealedHP(Quality, HouseEntity.Condition);
        }

        public static int CalculateHealedHP(int quality, decimal condition)
        {
            var hpAdd = quality * 2;
            if (condition >= 100m)
                hpAdd += 1;
            return hpAdd;
        }

        public static int GetUpgradeCost(int quality)
        {
            decimal mod = 0.1m;
            switch (quality)
            {
                case 1:
                    return (int)(10 * mod);
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
