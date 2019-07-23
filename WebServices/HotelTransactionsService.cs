using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.enums;
using WebServices.structs;
using WebServices.structs.Hotels;

namespace WebServices
{
    public class HotelTransactionsService : IHotelTransactionsService
    {
        private readonly ITransactionsService transactionsService;

        public HotelTransactionsService(ITransactionsService transactionsService)
        {
            this.transactionsService = transactionsService;
        }

        public void PayForRoomRents(Hotel hotel, Citizen citizen, HotelCost cost)
        {
            var destionationEntity = hotel.Entity;
            var sourceEntity = citizen.Entity;

            var citizenCost = new Money(cost.Currency, cost.BasePrice);

            var transaction = new Transaction()
            {
                Arg1 = "Hotel stay",
                Arg2 = string.Format("{0}({1}) paid for hotel stay in {2}({3})", sourceEntity.Name, sourceEntity.EntityID, destionationEntity.Name, destionationEntity.EntityID),
                SourceEntityID = citizen.ID,
                DestinationEntityID = hotel.ID,
                Money = citizenCost,
                TransactionType = TransactionTypeEnum.HotelStay
            };

            var result1 = transactionsService.MakeTransaction(transaction);

            var taxCost = new Money(cost.Currency, cost.Tax);

            var countryEntity = hotel.Region.Country.Entity;

            transaction = new Transaction()
            {
                Arg1 = "Hotel tax",
                Arg2 = string.Format("{0}({1}) paid hotel tax for {2}({3})", sourceEntity.Name, sourceEntity.EntityID, countryEntity.Name, countryEntity.EntityID),
                SourceEntityID = citizen.ID,
                DestinationEntityID = countryEntity.EntityID,
                Money = taxCost,
                TransactionType = TransactionTypeEnum.HotelTax
            };

            var result2= transactionsService.MakeTransaction(transaction);

            if (result1 != TransactionResult.Success || result2 != TransactionResult.Success)
                throw new Exception("No success. buu");
        }
    }
}
