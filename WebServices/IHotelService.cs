using Common.Operations;
using Entities;
using Entities.Models.Hotels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.structs;
using WebServices.structs.Hotels;

namespace WebServices
{
    public interface IHotelService
    {
        Hotel BuildHotel(string name, Region region, Entity owner);
        void ProcessDayChange(int newDay);
        HotelCost CalculateRoomCost(Hotel room, int quality, int nights);

        int CalculateConstructionCostForNewRoom(Hotel hotel, int quality);
        HotelRights GetHotelRigths(Hotel hotel, Entity entity);
        int CalculateConstructionCostForNewRoom(int quality, int roomCount);

        MethodResult CanRentRoom(Hotel hotel, int quality, int nights, Entity entity);
        void RentRoom(Hotel hotel, int quality, int nights, Citizen citizen);
        MethodResult CanBuildNewRoom(Hotel hotel, int quality, Entity entity);
        void BuildRoom(Hotel hotel, int quality);

        void SetPrices(int hotelID, decimal? priceQ1, decimal? priceQ2, decimal? priceQ3, decimal? priceQ4, decimal? priceQ5);
        MethodResult CanRemoveRoom(HotelRoom room, Entity entity);
        void RemoveRoom(HotelRoom room);
        MethodResult CanSeeWallet(Hotel hotel, Entity entity);

        MethodResult CanMakeDeliveries(Hotel hotel, Entity entity);
        MethodResult CanManageManager(Hotel hotel, Entity currentEntity, HotelRights managerRights);
        MethodResult CanAddManager(Hotel hotel, Entity currentEntity, Citizen newManager);

        void AddManager(Hotel hotel, Entity currentEntity, Citizen newManager);
        void UpdateManager(HotelManager manager, HotelRights newRights);
        MethodResult CanUpdateManager(HotelManager manager, Entity currentEntity, HotelRights newRights);
    }
}
