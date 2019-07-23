using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.structs.MonetaryMarket;
using WebServices.structs.Params.MonetaryMarket;

namespace WebServices
{
    public interface IMonetaryMarketService
    {
        MonetaryOffer CreateOffer(CreateMonetaryOfferParam param);
        double CalculateTax(double overallPrice, double taxRate, double minimumTax);
        double CalcualteTax(double overallPrice, MonetaryOffer offer);
        int GetUsedCurrencyID(MonetaryOfferTypeEnum offerType, int sellCurrencyID, int buyCurrencyID);
        int GetUsedCurrencyID(MonetaryOffer offer);
        MonetaryOfferCost GetMonetaryOfferCost(int sellCurrencyID, int buyCurrencyID, int amount, double rate, MonetaryOfferTypeEnum offerType);
        void OnlyRemoveOffer(MonetaryOffer offer);
    }
}
