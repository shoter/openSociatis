using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Operations;
using Entities;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using SociatisCommon.Errors;
using WebServices.structs;

namespace WebServices
{
    public class SellHouseService : ISellHouseService
    {
        private readonly IHouseRepository houseRepository;
        private readonly IWalletService walletService;
        private readonly ISellHouseRepository sellHouseRepository;
        private readonly IHouseTransactions houseTransactions;

        public SellHouseService(IHouseRepository houseRepository, IWalletService walletService, ISellHouseRepository sellHouseRepository, IHouseTransactions houseTransactions)
        {
            this.houseRepository = houseRepository;
            this.walletService = walletService;
            this.sellHouseRepository = sellHouseRepository;
            this.houseTransactions = houseTransactions;
        }

        public void Buy(House house, Entity entity)
        {
            var money = GetBuyHouseMoney(house);
            houseTransactions.PayForHouseBuy(money, house.Citizen, entity.Citizen, house);

            house.CitizenID = entity.EntityID;
            sellHouseRepository.Remove(house.SellHouse);

            houseRepository.SaveChanges();
        }

        public MethodResult CanBuy(House house, Entity entity)
        {
            if (house == null)
                return new MethodResult(HouseErrors.HouseNotExist);
            if (entity == null)
                return new MethodResult(HouseErrors.EntityNotExist);

            if (entity.Is(EntityTypeEnum.Citizen) == false)
                return new MethodResult(HouseErrors.OnlyCitizenBuyHouse);

            if (house.SellHouse == null)
                return new MethodResult(HouseErrors.NotOnSell);

            if (HaveEnoughCashToBuy(house, entity) == false)
                return new MethodResult(HouseErrors.NotEnoughCash);

            if (houseRepository.HasHouseInRegion(entity.EntityID, house.RegionID))
                return new MethodResult(HouseErrors.AlreadyHaveHouse);

            return MethodResult.Success;
        }

        public virtual bool HaveEnoughCashToBuy(House house, Entity entity)
        {
            Money money = GetBuyHouseMoney(house);

            return walletService.HaveMoney(entity.WalletID, money);
        }

        private static Money GetBuyHouseMoney(House house)
        {
            var currency = Persistent.Countries.GetCountryCurrency(house.Region.CountryID.Value);
            var money = new Money()
            {
                Amount = house.SellHouse.Price,
                Currency = currency
            };
            return money;
        }

        public MethodResult CanSellHouse(House house, decimal price)
        {
            if (price <= 0)
                return new MethodResult("Price must be at least 0.01.");

            if (house.SellHouse != null)
                return new MethodResult("House is already at sell!");

            return MethodResult.Success;
        }

        public void SellHouse(House house, decimal price)
        {
            var sell = new SellHouse()
            {
                House = house,
                Price = price
            };

            sellHouseRepository.Add(sell);
            sellHouseRepository.SaveChanges();
        }
    }
}
