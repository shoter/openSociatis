using Common.Operations;
using Entities;
using Entities.enums;
using SociatisCommon.Rights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface IHouseService
    {
        House CreateHouseForCitizen(Citizen citizen);
        MethodResult CanCreateHouseForCitizen(Citizen citizen);

        MethodResult CanViewHouse(House house, Entity entity);
        MethodResult CanModifyHouse(House house, Entity entity);

        void ProcessDayChange(int newDay);

        MethodResult CanBuyOffer(MarketOffer offer, int amount, House house, Entity entity);

        HouseRights GetHouseRights(House house, Entity entity);

        MethodResult CanUpgradeFurniture(House house, FurnitureTypeEnum furnitureType);
        void UpgradeFurniture(House house, FurnitureTypeEnum furnitureType);

        MethodResult CanBuildFurniture(House house, FurnitureTypeEnum furnitureType, Entity entity);
        void BuildFurniture(House house, FurnitureTypeEnum furnitureType);

       
    }
}
