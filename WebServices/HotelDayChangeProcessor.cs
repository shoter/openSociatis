using Entities;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public class HotelDayChangeProcessor
    {

        private readonly IEquipmentService equipmentService;
        private readonly IEquipmentRepository equipmentRepository;

        private Hotel hotel;
        public int NewDay { get; private set; }

        public HotelDayChangeProcessor(IEquipmentService equipmentService, IEquipmentRepository equipmentRepository)
        {
            this.equipmentService = equipmentService;
            this.equipmentRepository = equipmentRepository;
        }
        public void ProcessHotel(int newDay, Hotel hotel)
        {
            Init(newDay, hotel);

            foreach (var room in hotel.HotelRooms.Where(r => r.InhabitantID.HasValue))
            {
                ProcessSleepInRoom(hotel, room);
            }

            foreach (var room in hotel.HotelRooms.ToList())
            {
                DecayHotelRoom(hotel, room);
                ClearRoomIfAble(room);
            }

            UseConstructionMaterialsToRepairHotel(hotel);
        }

        public void UseConstructionMaterialsToRepairHotel(Hotel hotel)
        {
            int neededConstructionMaterials = GetNeededConstructionMaterialsToFullyRepair(hotel.Condition);
            var item = equipmentRepository.GetEquipmentItem(hotel.Entity.EquipmentID.Value, (int)ProductTypeEnum.ConstructionMaterials, 1);

            if (((item?.Amount) ?? 0) == 0)
                return;

            int usedConstructionMaterials = Math.Min(item.Amount, neededConstructionMaterials);
            equipmentRepository.RemoveEquipmentItem(item.EquipmentID, item.ProductID, 1, usedConstructionMaterials);

            UseConstructionMaterialsOnHotel(hotel, usedConstructionMaterials);
        }

        public void Init(int newDay, Hotel hotel)
        {
            this.NewDay = newDay;
            this.hotel = hotel;
        }

        virtual public void UseConstructionMaterialsOnHotel(Hotel hotel, int constructionMaterialsCount)
        {
            hotel.Condition += constructionMaterialsCount;
            hotel.Condition = Math.Min(100m, hotel.Condition);
        }

        virtual public int GetNeededConstructionMaterialsToFullyRepair(decimal condition)
        {
            return (int)
                 Math.Ceiling(Math.Abs(condition - 100m));
        }

        virtual public void ProcessSleepInRoom(Hotel hotel, HotelRoom room)
        {
            int hp = GetHealedHP(room.Quality, hotel.Condition);
            room.Inhabitant.AddHealth(hp);
        }

        virtual public void DecayHotelRoom(Hotel hotel, HotelRoom room)
        {
            hotel.Condition -= CalculateRoomDecay(room.Quality,
                used: room.InhabitantID.HasValue);
            hotel.Condition = Math.Max(0m, hotel.Condition);
        }

        virtual public void ClearRoomIfAble(HotelRoom room)
        {
            if (CanClearRoom(room))
            {
                ClearRoom(room);
            }
        }

        virtual public void ClearRoom(HotelRoom room)
        {
            room.StayingToDay = null;
            room.StayingFromDay = null;
            room.InhabitantID = null;
        }

        virtual public bool CanClearRoom(HotelRoom room)
        {
            return room.InhabitantID.HasValue && NewDay >= room.StayingToDay.Value;
        }


        virtual public decimal CalculateRoomDecay(int quality, bool used)
        {
            decimal decay = 0m;
            switch (quality)
            {
                case 1:
                    decay = 0.1m;
                    break;
                case 2:
                    decay = 0.2m;
                    break;
                case 3:
                    decay = 0.3m;
                    break;
                case 4:
                    decay = 0.4m;
                    break;
                case 5:
                    decay = 0.5m;
                    break;
            }

            if (used == false)
                return decay / 10m;
            return decay;
        }

        virtual public int GetHealedHP(int roomQuality, decimal hotelCondition)
        {
            double conditionModifier = CalculateConditionModifier(hotelCondition);

            int bonus = 0;
            if (hotelCondition == 100m)
                bonus = 1;

            return (int)Math.Ceiling((roomQuality * 2) * conditionModifier) + bonus;
        }

        virtual public double CalculateConditionModifier(decimal hotelCondition)
        {
            return (double)(hotelCondition / 100m);
        }
    }
}
