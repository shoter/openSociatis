using Common.EntityFramework;
using Entities.enums;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface IWalletRepository : IRepository<Wallet>
    {
        ICollection<WalletMoney> GetMoney(int walletID);
        bool HaveMoney(int walletID, CurrencyTypeEnum currency, double amount);
        List<WalletMoney> GetMoneyForEntity(int entityID);
        int? GetWalletIDForEntity(int entityID);
        decimal GetMoneyAmount(int walletID, int currencyID);
    }
}
