using Common.Operations;
using Entities;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebServices.structs;


namespace WebServices
{
    public class WalletService : BaseService, IWalletService
    {
        Entities.Repository.IWalletRepository walletRepository;
        IEntityRepository entityRepository;
        ICurrencyRepository currencyRepository;

        public WalletService(Entities.Repository.IWalletRepository walletsRepository, IEntityRepository entitiesRepository, ICurrencyRepository currencyRepository)
        {
            this.walletRepository = walletsRepository;
            this.entityRepository = entitiesRepository;
            this.currencyRepository = currencyRepository;
        }
        public void AddMoney(int walletID, Money money)
        {
            var wallet = walletRepository.GetById(walletID);
            var existingMoney = wallet.WalletMoneys.FirstOrDefault(wm => wm.CurrencyID == money.Currency.ID);

            if (existingMoney == null)
            {
                WalletMoney walletMoney = createWalletMoney(walletID, money);
                wallet.WalletMoneys.Add(walletMoney);
            }
            else
                existingMoney.Amount += money.Amount;
            ConditionalSaveChanges(walletRepository);
        }

        private static WalletMoney createWalletMoney(int walletID, Money money)
        {
            return new WalletMoney()
            {
                Amount = money.Amount,
                CurrencyID = money.Currency.ID,
                WalletID = walletID
            };
        }

        public Wallet CreateWallet(int entityID)
        {
            Wallet wallet = new Wallet();
            var entity = entityRepository.GetById(entityID);
            wallet.Entities.Add(entity);
            walletRepository.Add(wallet);
            walletRepository.SaveChanges();

            return wallet;

        }

        public WalletMoney GetWalletMoney(int walletID, int currencyID)
        {
            var wallet = walletRepository.FirstOrDefault(w => w.ID == walletID);

            WalletMoney money = wallet.WalletMoneys.FirstOrDefault(wm => wm.CurrencyID == currencyID);

            if (money == null)
            {
                var currency = currencyRepository.First(c => c.ID == currencyID);
                return new WalletMoney()
                {
                    Amount = 0,
                    Currency = currency,
                    CurrencyID = currency.ID,
                    Wallet = wallet,
                    WalletID = walletID
                };
            }
            return money;
        }

        public bool HaveMoney(int walletID, params Money[] moneys)
        {
            var wallet = walletRepository.FirstOrDefault(w => w.ID == walletID);
            foreach (var money in moneys)
            {
                if (money.Amount <= 0)
                    continue;

                WalletMoney walletMoney = wallet.WalletMoneys.FirstOrDefault(wm => wm.CurrencyID == money.Currency.ID);

                if (walletMoney == null)
                    return false;
                else if (walletMoney.Amount < money.Amount)
                    return false;
            }
            return true;
        }

        public List<WalletMoney> GetWalletMoney(int walletID, List<int> currencyIDs)
        {
            var wallet = walletRepository.First(w => w.ID == walletID);

            return GetWalletMoney(wallet, currencyIDs);


        }

        public List<WalletMoney> GetWalletMoney(Wallet wallet, List<int> currencyIDs)
        {
            var moneys = currencyRepository
                .Where(c => currencyIDs.Contains(c.ID))
                .Select(c =>
                new
                {
                    Currency = c,
                    Money = c.WalletMoneys.FirstOrDefault(wm => wm.WalletID == wallet.ID)

                }
                )
                .ToList();

            List<WalletMoney> _return = new List<WalletMoney>();

            foreach (var money in moneys)
            {
                if (money.Money == null)
                {
                    _return.Add(new WalletMoney()
                    {
                        Amount = 0,
                        Currency = money.Currency,
                        CurrencyID = money.Currency.ID,
                        Wallet = wallet,
                        WalletID = wallet.ID
                    });
                }
                else
                {
                    _return.Add(money.Money);
                }
            }

            return _return;
        }

        public MethodResult CanAccessWallet(int walletID, int entityID)
        {
            var walletEntity = walletRepository.Where(w => w.ID == walletID).Select(w => w.Entities.FirstOrDefault()).FirstOrDefault();

            if (walletEntity != null)
                return CanAccessEntityWallet(walletEntity, entityID);

            return MethodResult.Failure;
        }

        public MethodResult CanAccessEntityWallet(Entity entity, int entityID)
        {
            if (entity.EntityID == entityID)
                return MethodResult.Success;

            if (entity.Is(EntityTypeEnum.Hotel))
            {
                var hotelService = DependencyResolver.Current.GetService<IHotelService>();
                var currentEntity = entityRepository.GetById(entityID);

                if (hotelService.GetHotelRigths(entity.Hotel, currentEntity).CanUseWallet)
                    return MethodResult.Success;
            }

            return MethodResult.Failure;
        }

        public void TransferAllFounds(Wallet source, Wallet destination)
        {
            foreach (var wm in source.WalletMoneys.ToList())
            {
                var money = new Money(wm.CurrencyID, wm.Amount);
                
                AddMoney(destination.ID, money);

                money.Amount = -money.Amount;
                AddMoney(source.ID, money);
            }
            walletRepository.SaveChanges();
        }

        public void TransferAllFounds(List<Money> source, Wallet destination)
        {
            foreach (var m in source.ToList())
            {
                var money = new Money(m.Currency.ID, m.Amount);

                AddMoney(destination.ID, money);
            }
            walletRepository.SaveChanges();
        }
    }
}
