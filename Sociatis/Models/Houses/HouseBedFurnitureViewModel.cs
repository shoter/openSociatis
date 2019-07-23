using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using SociatisCommon.Rights;
using WebServices.Houses;

namespace Sociatis.Models.Houses
{
    public class HouseBedFurnitureViewModel : HouseBaseFurnitureViewModel
    {
        public int MaximumHealedHP { get; set; }
        public int HealedHP { get; set; }
        public int MaximumHealedHpNextLevel { get; set; }

        public HouseBedFurnitureViewModel(HouseFurniture furniture, HouseRights rights) : base(furniture, rights)
        {
            MaintainceCost = HouseBedObject.CalculateDecay(furniture.Quality);
            MaximumHealedHP = HouseBedObject.CalculateHealedHP(furniture.Quality, 100m);
            HealedHP = HouseBedObject.CalculateHealedHP(furniture.Quality, furniture.House.Condition);
            if (CanUpgrade)
                MaximumHealedHpNextLevel = HouseBedObject.CalculateHealedHP(furniture.Quality + 1, 100m);
        }
    }
}
