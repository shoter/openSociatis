using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class CitizenExtensions
    {
        public static bool HasFood(this Citizen citizen)
        {
            return
                citizen.Entity
                .Equipment.EquipmentItems
                .Count(i => i.ProductID == (int)ProductTypeEnum.Bread) != 0;
        }

        public static EquipmentItem GetBestBread(this Citizen citizen)
        {
            return
                citizen.Entity
                .Equipment.EquipmentItems
                .Where(i => i.ProductID == (int)ProductTypeEnum.Bread)
                .OrderBy(i => i.Quality)
                .First();
        }

        public static double GetWorkSkill(this Citizen citizen, Company company)
        {
            return citizen.GetWorkSkill(company.GetWorkType());
        }
        /// <summary>
        /// Can return null. Returns citizen's house in the same region as where citizen is.
        /// </summary>
        public static House GetCurrentlyLivingHouse(this Citizen citizen)
        {
            return citizen.Houses.FirstOrDefault(h => h.RegionID == citizen.RegionID);
        }

        public static double GetWorkSkill(this Citizen citizen, WorkTypeEnum workType)
        {
            switch(workType)
            {
                case WorkTypeEnum.Construction:
                    {
                        return (double)citizen.Construction;
                    }
                case WorkTypeEnum.Manufacturing:
                    {
                        return (double)citizen.Manufacturing;
                    }
                case WorkTypeEnum.Raw:
                    {
                        return (double)citizen.Raw;
                    }
                case WorkTypeEnum.Selling:
                    {
                        return (double)citizen.Selling;
                    }
                case WorkTypeEnum.Any:
                    {
                        return (double)Math.Max(Math.Max(citizen.Construction, citizen.Selling), Math.Max(citizen.Manufacturing, citizen.Raw));
                    }
            }

            throw new ArgumentException("GetWorkSkill");
        }

        public static PlayerTypeEnum GetPlayerType(this Citizen citizen)
        {
            return (PlayerTypeEnum)citizen.PlayerTypeID;
        }

        public static Company GetCurrentJob(this Citizen citizen)
        {
            return citizen.CompanyEmployee?.Company;
        }

        public static bool Is(this Citizen citizen, PlayerTypeEnum playerType)
        {
            return citizen.GetPlayerType() == playerType;
        }

        public static bool HaveRightsOfAtLeast(this Citizen citizen, PlayerTypeEnum playerType)
        {
            return citizen.GetPlayerType() >= playerType;
        }

        public static void AddHealth(this Citizen citizen, int health)
        {
            citizen.HitPoints += health;

            if (citizen.HitPoints > 100)
                citizen.HitPoints = 100;
        }
    }
}
