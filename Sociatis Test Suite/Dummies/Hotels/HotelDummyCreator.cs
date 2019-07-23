using Common.utilities;
using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Test_Suite.Dummies.Hotels
{
    public class HotelDummyCreator : EntityDummyCreator, IDummyCreator<Hotel>
    {
        private Hotel hotel
        {
            get => entity.Hotel;
            set
            {
                value.ID = entity.EntityID;
                value.Entity = entity;
                entity.Hotel = value;
            }
        }

        private CitizenDummyCreator citizenDummyCreator = new CitizenDummyCreator();
        private HotelRoomDummyCreator hotelRoomDummyCreator;

        public HotelDummyCreator()
        {
            hotel = create();
        }

        private Hotel create()
        {
            base.createEntity(EntityTypeEnum.Hotel);
            hotel = new Hotel()
            {
                Condition = 100m,
                Owner = citizenDummyCreator.Create().Entity,
                HotelRooms = new List<HotelRoom>(),
                HotelPrice = new HotelPrice()
                {
                    PriceQ1 = 1,
                    PriceQ2 = 2,
                    PriceQ3 = 3,
                    PriceQ4 = 4,
                    PriceQ5 = 5
                },
            };

            hotel.HotelPrice.Hotel = hotel;
            hotel.HotelPrice.HotelID = hotel.ID;

            hotelRoomDummyCreator = new HotelRoomDummyCreator(hotel);

            for (int i = 0; i < 5; ++i)
                hotel.HotelRooms.Add(hotelRoomDummyCreator
                    .SetQuality(i).Create());

            hotel.OwnerID = hotel.Owner.EntityID;

            return hotel;
        }
        public new Hotel Create()
        {
            var temp = hotel;
            hotel = create();
            return temp;
        }
    }
}
