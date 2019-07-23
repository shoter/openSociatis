using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Operations;
using Entities;
using Entities.enums;
using Entities.Extensions;
using Entities.Models.Hotels;
using Entities.Repository;
using WebServices.Helpers;
using WebServices.structs;
using WebServices.structs.Hotels;

namespace WebServices
{
    public class HotelService : BaseService, IHotelService
    {
        private readonly IEntityService entityService;
        private readonly IEntityRepository entityRepository;
        private readonly IHotelRepository hotelRepository;
        private readonly ICountryRepository countryRepository;
        private readonly IWalletService walletService;
        private readonly IHotelRoomRepository hotelRoomRepository;
        private readonly IHotelTransactionsService hotelTransactionsService;
        private readonly IEquipmentService equipmentService;
        private readonly IHotelManagerRepository hotelManagerRepository;
        private readonly IEquipmentRepository equipmentRepository;

        public HotelService(IEntityService entityService, IEntityRepository entityRepository, IHotelRepository hotelRepository, ICountryRepository countryRepository,
            IWalletService walletService, IHotelRoomRepository hotelRoomRepository, IHotelTransactionsService hotelTransactionsService,
            IEquipmentService equipmentService, IHotelManagerRepository hotelManagerRepository, IEquipmentRepository equipmentRepository)
        {
            this.entityService = entityService;
            this.entityRepository = entityRepository;
            this.hotelRepository = hotelRepository;
            this.countryRepository = countryRepository;
            this.walletService = walletService;
            this.hotelRoomRepository = hotelRoomRepository;
            this.hotelTransactionsService = hotelTransactionsService;
            this.equipmentService = equipmentService;
            this.hotelManagerRepository = hotelManagerRepository;
            this.equipmentRepository = equipmentRepository;
        }
        public Hotel BuildHotel(string name, Region region, Entity owner)
        {
            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                var entity = entityService.CreateEntity(name, EntityTypeEnum.Hotel);
                entity.Equipment.ItemCapacity = 3500;

                entity.Hotel = new Hotel()
                {
                    Condition = 100m,
                    HotelPrice = new HotelPrice(),
                    RegionID = region.ID,
                    Owner = owner
                };

                List<HotelRoom> hotelRooms = new List<HotelRoom>();
                for (int i = 0; i < 5; ++i)
                {
                    hotelRooms.Add(new HotelRoom()
                    {
                        Quality = 1
                    });
                }

                entity.Hotel.HotelRooms = hotelRooms;

                entityRepository.SaveChanges();
                trs.Complete();
                return entity.Hotel;
            }
        }

        

        

        public void ProcessDayChange(int newDay)
        {
            var hotels = hotelRepository.GetAll();

            foreach (var hotel in hotels)
            {
                var processor = new HotelDayChangeProcessor(equipmentService, equipmentRepository);

                processor.ProcessHotel(newDay, hotel);
            }
        }

        virtual public void BuildRoom(Hotel hotel, int quality)
        {
            var room = new HotelRoom()
            {
                Hotel = hotel,
                Quality = quality
            };

            var cost = CalculateConstructionCostForNewRoom(hotel, quality);
            equipmentService.RemoveProductsFromEquipment(ProductTypeEnum.ConstructionMaterials, cost, 1, hotel.Entity.Equipment);

            hotelRoomRepository.Add(room);
            ConditionalSaveChanges(hotelRoomRepository);
        }

        virtual public int CalculateConstructionCostForNewRoom(Hotel hotel, int quality)
        {
            return CalculateConstructionCostForNewRoom(quality, hotel.HotelRooms.Count);
        }

        virtual public MethodResult CanBuildNewRoom(Hotel hotel, int quality, Entity entity)
        {
            if (hotel == null)
                return new MethodResult("Hotel does not exist!");
            if (entity == null)
                return new MethodResult("You do not exist O_o");

            var rights = GetHotelRigths(hotel, entity);
            if (rights.CanBuildRooms == false)
                return new MethodResult("You cannot build rooms in this hotel!");

            if (quality < 1 || quality > 5)
                return new MethodResult("Wrong quality!");

            var matPrice = CalculateConstructionCostForNewRoom(hotel, quality);

            if (equipmentService.HaveItem(hotel.Entity.Equipment, ProductTypeEnum.ConstructionMaterials, 1, matPrice).IsError)
                return new MethodResult($"You do not have {matPrice} construction materials to do that!");

            var roomsInfo = hotelRepository.GetHotelRoomInfo(hotel.ID);
            if (roomsInfo.Count > 0)
            {
                int minQ = roomsInfo.Select(r => r.Quality).Min(),
                    maxQ = roomsInfo.Select(r => r.Quality).Max();

                if (Math.Abs(quality - minQ) >= 3 || Math.Abs(quality - maxQ) >= 3)
                    return new MethodResult("New room cannot have bigger quality diffrence to existing ones than 3!");
            }



            return MethodResult.Success;
        }

        virtual public MethodResult CanRemoveRoom(HotelRoom room, Entity entity)
        {
            if (room == null)
                return new MethodResult("Room does not exist!");
            if (entity == null)
                return new MethodResult("You do not exist!");

            if (room.InhabitantID.HasValue)
                return new MethodResult("You cannot remove rooms occupied by someone!");

            var rights = GetHotelRigths(room.Hotel, entity);

            if (rights.CanBuildRooms == false)
                return new MethodResult("You are not allowed to do that!");

            return MethodResult.Success;
        }

        virtual public MethodResult CanSeeWallet(Hotel hotel, Entity entity)
        {
            if (hotel == null)
                return new MethodResult("Hotel does not exist!");

            if (entity == null)
                return new MethodResult("You do not exist!");

            var rights = GetHotelRigths(hotel, entity);

            if (rights.CanUseWallet == false)
                return new MethodResult("You cannot do that!");

            return MethodResult.Success;
        }

        virtual public void RemoveRoom(HotelRoom room)
        {
            hotelRoomRepository.Remove(room);
            ConditionalSaveChanges(hotelRoomRepository);
        }

        virtual public HotelRights GetHotelRigths(Hotel hotel, Entity entity)
        {
            if (entity.EntityID == hotel.ID)
                return HotelRights.FullRights;

            if (entity.Is(EntityTypeEnum.Citizen) == false)
                return HotelRights.NoRights;

            if (hotel.OwnerID == entity.EntityID)
                return HotelRights.FullRights;

            var manager = hotel.HotelManagers.FirstOrDefault(m => m.CitizenID == entity.EntityID);

            if (manager == null)
                return HotelRights.NoRights;

            return new HotelRights(manager);
        }

        virtual public MethodResult CanMakeDeliveries(Hotel hotel, Entity entity)
        {
            if (hotel == null)
                return new MethodResult("Hotel does not exist!");
            if (entity == null)
                return new MethodResult("You do not exist O_O?!");

            var rights = GetHotelRigths(hotel, entity);

            if (rights.CanMakeDeliveries == false)
                return new MethodResult("You cannot do that!");

            return MethodResult.Success;
        }

        

        virtual public int CalculateConstructionCostForNewRoom(int quality, int roomCount)
        {
            return roomCount * 1 + quality * 5;
        }

        virtual public HotelCost CalculateRoomCost(Hotel hotel, int quality, int nights)
        {
            decimal hotelTax = GetHotelTax(hotel);
            decimal? nightPrice = GetNightPrice(hotel, quality);
            if (nightPrice.HasValue == false)
                return null;

            var currency = Persistent.Countries.GetCountryCurrency(hotel.Region.CountryID.Value);
            return new HotelCost()
            {
                BasePrice = nights * nightPrice.Value,
                Tax = hotelTax,
                Currency = currency
            };
        }

        virtual public decimal? GetNightPrice(Hotel hotel, int quality)
        {
            var prices = hotel.HotelPrice;

            switch (quality)
            {
                case 1:
                    return prices.PriceQ1;
                case 2:
                    return prices.PriceQ2;
                case 3:
                    return prices.PriceQ3;
                case 4:
                    return prices.PriceQ4;
                case 5:
                    return prices.PriceQ5;
            }

            throw new NotImplementedException();
        }

        virtual public decimal GetHotelTax(Hotel hotel)
        {
            int countryID = hotel.Region.CountryID.Value;
            return countryRepository.GetCountryPolicySetting(countryID, p => p.HotelTax);

        }

        public virtual MethodResult CanManageManager(Hotel hotel, Entity currentEntity, HotelRights managerRights)
        {
            if (hotel == null)
                return new MethodResult("Hotel does not exist!");

            var rights = GetHotelRigths(hotel, currentEntity);

            if (rights.CanManageManagers == false)
                return new MethodResult("You cannot manage managers here!");

            if (managerRights.Priority >= rights.Priority)
                return new MethodResult("You cannot manage manager with same or higher priority than you!");

            return MethodResult.Success;
        }

        public virtual void AddManager(Hotel hotel, Entity currentEntity, Citizen newManager)
        {
            var rights = GetHotelRigths(hotel, currentEntity);

            var manager = new HotelManager()
            {
               CanBuildRooms = false,
               CanMakeDeliveries = false,
               CanManageEquipment = false,
               CanManageManagers = false,
               CanSetPrices = false,
               CanSwitchInto = false,
               CanUseWallet = false,
               Citizen = newManager,
               Hotel = hotel,
               Priority = 0
            };

            hotelManagerRepository.Add(manager);
            hotelManagerRepository.SaveChanges();
        }

        public virtual MethodResult CanAddManager(Hotel hotel, Entity currentEntity, Citizen newManager)
        {
            if (hotel == null)
                return new MethodResult("Hotel does not exist!");
            if (newManager == null)
                return new MethodResult("That citizen does not exist!");

                var rights = GetHotelRigths(hotel, currentEntity);

            if (rights.CanManageManagers == false)
                return new MethodResult("You cannot add new managers");

            if (hotel.OwnerID == newManager.ID)
                return new MethodResult("You cannot add owner as a manger!");

            if (hotel.HotelManagers.Any(m => m.CitizenID == newManager.ID))
                return new MethodResult("This citizen is already a manager in the hotel");

            return MethodResult.Success;
        }

        public virtual MethodResult CanUpdateManager(HotelManager manager, Entity currentEntity, HotelRights newRights)
        {
            var hotel = manager.Hotel;
            var oldRights = new HotelRights(manager);
            var result = CanManageManager(hotel, currentEntity, oldRights);
            if (result.IsError)
                return result;

            var currentRights = GetHotelRigths(hotel, currentEntity);
            if (newRights.Priority >= currentRights.Priority)
                return new MethodResult("You cannot set priority equal or higher to your actual priority!");

            return MethodResult.Success;
        }

        public virtual void UpdateManager(HotelManager manager, HotelRights newRights)
        {
            newRights.Update(manager);
            hotelManagerRepository.SaveChanges();
        }



        public virtual MethodResult CanRentRoom(Hotel hotel, int quality, int nights, Entity entity)
        {
            if (hotel == null)
                return new MethodResult("Hotel does not exist!");

            if (entity.Is(EntityTypeEnum.Citizen) == false)
                return new MethodResult("You must be a citizen to rent a room!");

            if (entity.Citizen.RegionID != hotel.RegionID)
                return new MethodResult("You must be in the same region to rent a room!");

            if (nights <= 0)
                return new MethodResult("You must rent room for at least 1 night!");

            if (hotel.HotelPrice.GetRoomPrice(quality).HasValue == false || hotel.HotelRooms.Any(r => r.Quality == quality) == false)
                return new MethodResult("Hotel does not offer those rooms");

            if (hotel.HotelRooms.Any(r => r.Quality == quality && r.InhabitantID.HasValue == false) == false)
                return new MethodResult("There is no free room with that quality!");

            var cost = CalculateRoomCost(hotel, quality, nights);
            if (walletService.HaveMoney(entity.WalletID, new Money(cost.Currency, cost.TotalCost)) == false)
                return new MethodResult("You do not have enough money to rent a room!");

            if (entity.Citizen.HotelRooms.Any(r => r.Hotel.RegionID == hotel.RegionID))
                return new MethodResult("You are renting a room at this moment in this region!");

            return MethodResult.Success;
        }

        public virtual void RentRoom(Hotel hotel, int quality, int nights, Citizen citizen)
        {
            var room = GetFreeRoom(hotel, quality);
            room.InhabitantID = citizen.ID;
            room.StayingFromDay = GameHelper.CurrentDay;
            room.StayingToDay = GameHelper.CurrentDay + nights;

            var cost = CalculateRoomCost(hotel, quality, nights);

            hotelTransactionsService.PayForRoomRents(hotel, citizen, cost);

            hotelRoomRepository.SaveChanges();
        }

        public virtual HotelRoom GetFreeRoom(Hotel hotel, int quality)
        {
            return hotel.HotelRooms
                .Where(r => r.Quality == quality && r.InhabitantID.HasValue == false)
                .First();
        }

        public virtual void SetPrices(int hotelID, decimal? priceQ1, decimal? priceQ2, decimal? priceQ3, decimal? priceQ4, decimal? priceQ5)
        {
            var price = hotelRepository.GetGotelPrice(hotelID);
            price.PriceQ1 = priceQ1;
            price.PriceQ2 = priceQ2;
            price.PriceQ3 = priceQ3;
            price.PriceQ4 = priceQ4;
            price.PriceQ5 = priceQ5;

            ConditionalSaveChanges(hotelRepository);
        }
    }
}
