using Entities;
using Entities.Models.Hospitals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.enums;
using WebServices.structs;

namespace WebServices
{
    public interface ITransactionsService
    {
        TransactionResult MakeTransaction(Transaction transaction, bool useSqlTransaction = true);
        TransactionResult PayForBattleLoss(Entities.Country attacker, Entities.Country defender, double goldTaken);
        TransactionResult PayForWar(Entities.Country attacker, Entities.Country defender, IWarService warService);
        TransactionResult PayForJobOffer(Entities.Company company, Entities.Country receiver, int currencyID, decimal price);
        TransactionResult PayForArticle(Entities.Article article, Entities.Entity payer, int currencyID, double articlePrice, double tax);
        TransactionResult PayForNewspaperCreation(Entities.Newspaper newspaper, Entities.Entity payer);
        TransactionResult TransferMoneyFromCountryToCompany(Money money, Company destination, Country source);

        TransactionResult PayForResistanceWar(Citizen attacker, Region defenderRegion, IWarService warService);
        TransactionResult PayForResistanceBattle(Citizen attacker, Region defenderRegion, IWarService warService);
        TransactionResult MakeGift(Entity source, Entity destination, Money money);
        TransactionResult UgradeCompany(Company company, double goldCost);

        TransactionResult PayForHealing(Hospital hospital, Citizen citizen, HealingPrice healingPrice);

        TransactionResult PayForMPPOffer(Country proposing, Country other, decimal goldCost);
        TransactionResult GetGoldForDeclineMPP(Country proposing, Country other, decimal goldCost);

        TransactionResult PrintMoney(Country country, Money money);
    }
}
