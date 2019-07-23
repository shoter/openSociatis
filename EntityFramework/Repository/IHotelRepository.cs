using Common.EntityFramework;
using Entities.Models.Hotels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface IHotelRepository : IRepository<Hotel>
    {
        HotelInfo GetHotelInfo(int hotelID, int currentEntityID);
        HotelMain GetHotelMain(int hotelID);
        HotelPrice GetGotelPrice(int hotelID);
        List<HotelRoomInfo> GetHotelRoomInfo(int hotelID);
        Equipment GetEquipmentForHotel(int hotelID);
        List<HotelManagerModel> GetManagers(int hotelID);
        HotelManagerModel GetManager(int hotelID, int managerID);
        void RemoveManager(int hotelID, int managerID);
    }
}
