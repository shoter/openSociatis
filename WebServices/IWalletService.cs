using Common.Operations;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.structs;

namespace WebServices
{
    public interface IWalletService
    {
        Wallet CreateWallet(int entityID);
        void AddMoney(int walletID, Money money);

        bool HaveMoney(int walletID, params Money[] money);

        /// <summary>
        /// Never returns null
        /// </summary>
        WalletMoney GetWalletMoney(int walletID, int currencyID);
        List<WalletMoney> GetWalletMoney(Wallet wallet, List<int> currencyIDs);
        List<WalletMoney> GetWalletMoney(int walletID, List<int> currencyIDs);

        MethodResult CanAccessWallet(int walletID, int entityID);
        void TransferAllFounds(Wallet source, Wallet destination);
        void TransferAllFounds(List<Money> source, Wallet destination);
    }
}
