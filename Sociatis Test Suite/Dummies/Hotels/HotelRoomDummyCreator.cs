using Common.utilities;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Helpers;
using Sociatis_Test_Suite.Extensions;
namespace Sociatis_Test_Suite.Dummies.Hotels
{
    public class HotelRoomDummyCreator : IDummyCreator<HotelRoom>
    {
        private static UniqueIDGenerator uniqueID = new UniqueIDGenerator();
        private Hotel hotel;

        private HotelRoom room;
        public HotelRoomDummyCreator(Hotel hotel)
        {
            this.hotel = hotel;
            room = create();
        }

        private HotelRoom create()
        {
            return new HotelRoom()
            {
                Hotel = hotel,
                HotelID = hotel.ID,
                ID = uniqueID,
                Quality = 1
            };
        }

        public HotelRoomDummyCreator SetQuality(int quality)
        {
            room.Quality = quality;
            return this;
        }

        public HotelRoomDummyCreator SetInhabitant(Citizen citizen)
        {
            room.SetInhabitant(citizen);
            return this;
        }
        public HotelRoomDummyCreator SetRandomInhabitant()
        {
            return SetInhabitant(new CitizenDummyCreator().Create());
        }
        public HotelRoom Create()
        {
            var temp = room;
            room = create();
            return temp;
        }
    }
}
