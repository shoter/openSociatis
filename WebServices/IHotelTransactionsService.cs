using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.structs.Hotels;

namespace WebServices
{
    public interface IHotelTransactionsService
    {
        void PayForRoomRents(Hotel hotel, Citizen citizen, HotelCost cost);
    }
}
