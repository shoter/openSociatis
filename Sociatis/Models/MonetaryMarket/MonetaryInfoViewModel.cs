using Entities.enums;
using Entities.Repository;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices.Helpers;

namespace Sociatis.Models.MonetaryMarket
{
    public class MonetaryInfoViewModel
    {
        public ImageViewModel Avatar { get; set; }
        public int MyOffersCount { get; set; }
        public int TransactionTodayCount { get; set; }
        public double? GoldTradedToday { get; set; }

        public MonetaryInfoViewModel()
        {
            Avatar = Images.MonetaryMarket.VM;

            var entity = SessionHelper.CurrentEntity;

            MyOffersCount = entity.MonetaryOffers.Count;

            var monetaryTransactionRepository = DependencyResolver.Current.GetService<IMonetaryTransactionRepository>();

            TransactionTodayCount = monetaryTransactionRepository.Where(t => t.Day == GameHelper.CurrentDay).Count();

            GoldTradedToday = monetaryTransactionRepository.Where(t => t.Day == GameHelper.CurrentDay &&
            (t.SellerCurrencyID == (int)CurrencyTypeEnum.Gold || t.BuyerCurrencyID == (int)CurrencyTypeEnum.Gold))
            .Select(t => (t.SellerCurrencyID == (int)CurrencyTypeEnum.Gold ? t.Amount : t.Amount * t.Rate))
            .DefaultIfEmpty(0)
            .Sum(x => (double)x);

        }
    }
}