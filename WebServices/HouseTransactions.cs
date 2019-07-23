using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.enums;
using WebServices.enums;
using WebServices.structs;

namespace WebServices
{
    public class HouseTransactions : IHouseTransactions
    {
        private readonly ITransactionsService transactionsService;

        public HouseTransactions(ITransactionsService transactionsService)
        {
            this.transactionsService = transactionsService;
        }
        public TransactionResult PayForHouseBuy(Money money, Citizen owner, Citizen buyer, House house)
        {
            var sourceEntity = buyer.Entity;
            var destinationEntity = owner.Entity;

            var transaction = new Transaction()
            {
                Arg1 = "House Buy From Citizen",
                Arg2 = string.Format("{0}({1}) bought for house from {2}({3})", sourceEntity.Name, sourceEntity.EntityID, destinationEntity.Name, destinationEntity.EntityID),
                SourceEntityID = buyer.ID,
                DestinationEntityID = owner.ID,
                Money = money,
                TransactionType = TransactionTypeEnum.HouseBuy
            };

            return transactionsService.MakeTransaction(transaction);
        }
    }
}
