using Common.EntityFramework;
using Entities.Models.Hotels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class HotelRoomRepository : RepositoryBase<HotelRoom, SociatisEntities>, IHotelRoomRepository
    {
        public HotelRoomRepository(SociatisEntities context) : base(context)
        {
        }
        public List<HotelRoomModel> GetHotelRooms(int hotelID)
        {
            return (from room in Where(r => r.HotelID == hotelID)
                    join entity in context.Entities on room.InhabitantID equals entity.EntityID into possibleInhabitant
                    select new HotelRoomModel()
                    {
                        ID = room.ID,
                        InhabitantName = possibleInhabitant.Select(i => i.Name).FirstOrDefault(),
                        InhabitantID = room.InhabitantID,
                        InhabitantImgUrl = possibleInhabitant.Select(i => i.ImgUrl).FirstOrDefault(),
                        ToDay = room.StayingToDay,
                        Quality = room.Quality,
                    }).ToList();
        }
    }
}
