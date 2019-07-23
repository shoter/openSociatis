using Common.EntityFramework;
using Entities.enums;
using Entities.Models.Wallet;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class WalletRepository : RepositoryBase<Wallet, SociatisEntities>, IWalletRepository
    {
        public WalletRepository(SociatisEntities context) : base(context)
        {
        }

        public ICollection<WalletMoney> GetMoney(int walletID)
        {
            Wallet wallet = GetById(walletID);
            return wallet.WalletMoneys.ToList();
        }

        public List<WalletMoney> GetMoneyForEntity(int entityID)
        {
            return context.Entities.Where(entity => entity.EntityID == entityID)
                .Select(entity => entity.Wallet)
                .SelectMany(entity => entity.WalletMoneys)
                .ToList();
        }

        public bool HaveMoney(int walletID, CurrencyTypeEnum currency, double amount)
        {
            return context.WalletMoneys
                .Any(wm => wm.CurrencyID == (int)currency && wm.Amount >= (decimal)amount && wm.WalletID == walletID);
        }

        public int? GetWalletIDForEntity(int entityID)
        {
            return Where(w => w.Entities.Any(e => e.EntityID == entityID)).Select(w => w.ID).FirstOrDefault();
        }

        public decimal GetMoneyAmount(int walletID, int currencyID)
        {
            return Where(w => w.ID == walletID)
                .SelectMany(w => w.WalletMoneys)
                .Where(wm => wm.CurrencyID == currencyID)
                .Select(wm => (decimal?)wm.Amount).FirstOrDefault() ?? 0m;
        }
    }
}
