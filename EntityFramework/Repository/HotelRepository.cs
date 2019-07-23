using Common.EntityFramework;
using Common.Expressions;
using Entities.Models.Hotels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class HotelRepository : RepositoryBase<Hotel, SociatisEntities>, IHotelRepository
    {
        public HotelRepository(SociatisEntities context) : base(context)
        {
        }

        public Equipment GetEquipmentForHotel(int hotelID)
        {
            return (from hotel in Where(h => h.ID == hotelID)
                    join entity in context.Entities on hotel.ID equals entity.EntityID
                    join equipment in context.Equipments on entity.EquipmentID equals equipment.ID
                    select equipment)
                    .First();
        }
        public HotelInfo GetHotelInfo(int hotelID, int currentEntityID)
        {
            var info = (from hotel in Where(h => h.ID == hotelID)
                        join entity in context.Entities on hotel.ID equals entity.EntityID
                        join region in context.Regions on hotel.RegionID equals region.ID
                        join country in context.Countries on region.CountryID equals country.ID
                        join room in context.HotelRooms on hotel.ID equals room.HotelID into rooms
                        join manager in context.HotelManagers.Where(m => m.CitizenID == currentEntityID) on hotel.ID equals manager.HotelID into manager
                        select new HotelInfo()
                        {
                            HotelID = hotelID,
                            Condition = hotel.Condition,
                            HotelName = entity.Name,
                            ImgUrl = entity.ImgUrl,
                            IsOwner = hotel.OwnerID == currentEntityID,
                            RegionID = region.ID,
                            CountryID = country.ID,
                            RegionName = region.Name,
                            CountryName = country.Entity.Name,
                            OwnerID = hotel.OwnerID,
                            OwnerName = hotel.Owner.Name,
                            OwnerImgUrl = hotel.Owner.ImgUrl,
                            HotelRights = new HotelRights()
                            {
                                CanBuildRooms = manager.Select(m => (bool?)m.CanBuildRooms).FirstOrDefault() ?? false,
                                CanMakeDeliveries = manager.Select(m => (bool?)m.CanMakeDeliveries).FirstOrDefault() ?? false,
                                CanSetPrices = manager.Select(m => (bool?)m.CanSetPrices).FirstOrDefault() ?? false,
                                CanUseWallet = manager.Select(m => (bool?)m.CanUseWallet).FirstOrDefault() ?? false,
                                CanSwitchInto = manager.Select(m => (bool?)m.CanSwitchInto).FirstOrDefault() ?? false,
                                CanManageManagers = manager.Select(m => (bool?)m.CanManageManagers).FirstOrDefault() ?? false,
                                CanManageEquipment = manager.Select(m => (bool?)m.CanManageEquipment).FirstOrDefault() ?? false,
                                Priority = manager.Select(m => (int?)m.Priority).FirstOrDefault() ?? 0,

                            },
                            HotelRoomInfos = rooms.GroupBy(r => r.Quality).Select(r => new HotelRoomInfo()
                            {
                                Count = r.Count(),
                                Quality = r.Key
                            })
                        }).FirstOrDefault();

            if (info?.IsOwner == true || hotelID == currentEntityID)
                info.HotelRights = HotelRights.FullRights;

            return info;
        }
        public HotelManagerModel GetManager(int hotelID, int managerID)
        {
            return (from manager in context.HotelManagers.Where(m => m.CitizenID == managerID && m.HotelID == hotelID)
                    join entity in context.Entities on manager.CitizenID equals entity.EntityID
                    select new HotelManagerModel()
                    {
                        ImgURL = entity.ImgUrl,
                        ManagerID = managerID,
                        ManagerName = entity.Name,
                        HotelRights = new HotelRights()
                        {
                            CanBuildRooms = manager.CanBuildRooms,
                            CanMakeDeliveries = manager.CanMakeDeliveries,
                            CanManageEquipment = manager.CanManageEquipment,
                            CanManageManagers = manager.CanManageManagers,
                            CanSetPrices = manager.CanSetPrices,
                            CanSwitchInto = manager.CanSwitchInto,
                            CanUseWallet = manager.CanUseWallet,
                            Priority = manager.Priority
                        }
                    }).FirstOrDefault();
        }

        public List<HotelManagerModel> GetManagers(int hotelID)
        {
            return (from manager in context.HotelManagers.Where(m => m.HotelID == hotelID)
                    join entity in context.Entities on manager.CitizenID equals entity.EntityID
                    select new HotelManagerModel()
                    {
                        ManagerID = manager.CitizenID,
                        ManagerName = entity.Name,
                        ImgURL = entity.ImgUrl,
                        HotelRights = new HotelRights()
                        {
                            CanBuildRooms = manager.CanBuildRooms,
                            CanMakeDeliveries = manager.CanMakeDeliveries,
                            CanManageEquipment = manager.CanManageEquipment,
                            CanManageManagers = manager.CanManageManagers,
                            CanSetPrices = manager.CanSetPrices,
                            CanSwitchInto = manager.CanSwitchInto,
                            CanUseWallet = manager.CanUseWallet,
                            Priority = manager.Priority
                        }
                        }).ToList();
        }

        public HotelMain GetHotelMain(int hotelID)
        {
            return (from hotel in Where(h => h.ID == hotelID)
                    join room in context.HotelRooms.Where(r => r.InhabitantID.HasValue == false) on hotel.ID equals room.HotelID into rooms
                    join region in context.Regions on hotel.RegionID equals region.ID
                    join country in context.Countries on region.CountryID equals country.ID
                    join currency in context.Currencies on country.CurrencyID equals currency.ID
                    join price in context.HotelPrices on hotel.ID equals price.HotelID
                    select new HotelMain()
                    {
                        CurrencySymbol = currency.Symbol,
                        Rooms = rooms.GroupBy(r => r.Quality).Select(r => new HotelMainRoom()
                        {
                            Quality = r.Key,
                            Price =
                            r.Key == 1 ? price.PriceQ1 :
                            r.Key == 2 ? price.PriceQ2 :
                            r.Key == 3 ? price.PriceQ3 :
                            r.Key == 4 ? price.PriceQ4 :
                            price.PriceQ5
                        })
                    }).FirstOrDefault();
        }

        public HotelPrice GetGotelPrice(int hotelID)
        {
            return context.HotelPrices.Where(h => h.HotelID == hotelID).FirstOrDefault();
        }

        public List<HotelRoomInfo> GetHotelRoomInfo(int hotelID)
        {
            return (from room in context.HotelRooms.Where(r => r.HotelID == hotelID)
                    group room by room.Quality into rooms
                    select new HotelRoomInfo()
                    {
                        Count = rooms.Count(),
                        Quality = rooms.Key
                    }).ToList();
        }

        public void RemoveManager(int hotelID, int managerID)
        {
            Remove<HotelManager>(m => { m.CitizenID = managerID; m.HotelID = hotelID; });
        }




    }
}
