using Common.EntityFramework;
using Entities.Models.Hotels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface  IHotelRoomRepository : IRepository<HotelRoom>
    {
        List<HotelRoomModel> GetHotelRooms(int hotelID);
    }
}
