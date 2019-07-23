using Common.Exceptions;
using DataTables.AspNet.Core;
using DataTables.AspNet.Mvc5;
using Entities;
using Entities.enums;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Code.Filters;
using Sociatis.Helpers;
using Sociatis.Models.MonetaryMarket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.Helpers;
using WebServices.structs;
using WebServices.structs.Params.MonetaryMarket;
using WebUtils.Attributes;

namespace Sociatis.Controllers
{
    [DayChangeAuthorize]
    public class MonetaryMarketController : ControllerBase
    {
        private readonly IMonetaryMarketService monetaryMarketService;
        private readonly IMonetaryOfferRepository monetaryOfferRepository;
        private readonly IMonetaryTransactionRepository monetaryTransactionRepository;
        private readonly ICountryRepository countryRepository;
        private readonly IWalletService walletService;

        public MonetaryMarketController(IMonetaryMarketService monetaryMarketService, IMonetaryOfferRepository monetaryOfferRepository,
            IMonetaryTransactionRepository monetaryTransactionRepository, ICountryRepository countryRepository, 
            IWalletService walletService, IPopupService popupService) : base(popupService)
        {
            this.monetaryMarketService = monetaryMarketService;
            this.monetaryOfferRepository = monetaryOfferRepository;
            this.monetaryTransactionRepository = monetaryTransactionRepository;
            this.countryRepository = countryRepository;
            this.walletService = walletService;
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Monetary-Market/")]
        public ActionResult Index()
        {
            var vm = new ViewMonetaryMarketViewModel();

            return View(vm);
        }
        [Route("Monetary-Market/sell-{sellCurrencyID:int}/buy-{buyCurrencyID:int}")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult IndexSpecified(int sellCurrencyID, int buyCurrencyID)
        {
            var vm = new ViewMonetaryMarketViewModel(sellCurrencyID, buyCurrencyID);

            return View("Index", vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpGet]
        public ActionResult Create()
        {
            var vm = new AddMonetaryOfferViewModel();

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        [AjaxOnly]
        public JsonResult RemoveOffer(int offerID)
        {
            try
            {
                var offer = monetaryOfferRepository.GetById(offerID);

                double tax = monetaryMarketService.CalcualteTax((double)offer.ValueOfSold, offer);

                if (offer.ValueOfSold == 0)
                    tax = 0;

                double moneyBack = (double)offer.OfferReservedMoney + (double)offer.TaxReservedMoney - tax;
                Currency usedCurrency = Persistent.Currencies.GetById(monetaryMarketService.GetUsedCurrencyID(offer));

                string msg = $"{moneyBack} {usedCurrency.Symbol} has been paid back to you";

                monetaryMarketService.OnlyRemoveOffer(offer);

                return JsonSuccess($"Offer was removed. {msg}");
            }
            catch(Exception e)
            {
                return JsonDebugOnlyError(e);
            }
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetRemoveCost(int offerID)
        {
            try
            {
                var offer = monetaryOfferRepository.GetById(offerID);
                var entity = SessionHelper.CurrentEntity;

                if (offer.SellerID != entity.EntityID)
                    throw new UserReadableException("You cannot access this offer!");

                if (offer.ValueOfSold == 0)
                {
                    return JsonData(new { Cost = "" });
                }

                var taxCost = monetaryMarketService.CalcualteTax((double)offer.ValueOfSold, offer);
                var currencyID = monetaryMarketService.GetUsedCurrencyID(offer);
                var currency = Persistent.Currencies.GetById(currencyID);

                

                return JsonData(new { Cost = string.Format("{0} {1}", taxCost, currency.Symbol) });

            }
            catch(Exception e)
            {
                return JsonDebugOnlyError(e);
            }


        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        [AjaxOnly]
        public JsonResult GetData(int sellCurrencyID, int buyCurrencyID)
        {
            try
            {
                var entityID = SessionHelper.CurrentEntity.EntityID;

                var history = monetaryTransactionRepository
                    .Where(t => t.BuyerCurrencyID == buyCurrencyID && t.SellerCurrencyID == sellCurrencyID && (t.Day == GameHelper.CurrentDay || t.Day == (GameHelper.CurrentDay - 1)))
                    .OrderBy(t => t.ID)
                    .Select(t => new
                    {
                        Time = t.Date,
                        Day = t.Day,
                        Value = t.Rate
                    })
                    .ToList()
                    .Select(t => new
                    {
                        Time = t.Time.ToString("HH:mm"),
                        Day = t.Day,
                        Value = t.Value
                    });

                var bestOffer = monetaryOfferRepository.GetActualBuySellOffer(sellCurrencyID, buyCurrencyID);

                var myOffers = monetaryOfferRepository.Where(b => b.SellerID == entityID && b.BuyCurrencyID == buyCurrencyID && b.SellCurrencyID == sellCurrencyID)
                    .OrderBy(o => o.Rate)
                    .Select(o =>
                    new
                    {
                        ID = o.ID,
                        Amount = o.Amount,
                        Rate = o.Rate,
                        Type = o.OfferTypeID
                    })
                    .ToList();

                return Json(new
                {
                    Plot = history,
                    Best = bestOffer,
                    My = myOffers,
                    SellSymbol = Persistent.Currencies.GetById(sellCurrencyID).Symbol,
                    BuySymbol = Persistent.Currencies.GetById(buyCurrencyID).Symbol
                });
            }
            catch(Exception e)
            {
                return JsonDebugOnlyError(e);
            }
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult Create(AddMonetaryOfferViewModel vm)
        {
            if(ModelState.IsValid)
            {
                

                if (vm.Amount <= 0)
                {
                    AddError("Amount cannot be lower or equal to 0!");
                    return RedirectToAction("IndexSpecified", new { buyCurrencyID = vm.BuyCurrencyID, sellCurrencyID = vm.SellCurrencyID });
                }

                if(vm.OfferType.HasValue == false)
                {
                    AddError("You need to select offer type!");
                    return RedirectToAction("IndexSpecified", new { buyCurrencyID = vm.BuyCurrencyID, sellCurrencyID = vm.SellCurrencyID });
                }

                if(vm.Rate <= 0)
                {
                    AddError("Rate cannot be lower or equal to 0!");
                    return RedirectToAction("IndexSpecified", new { buyCurrencyID = vm.BuyCurrencyID, sellCurrencyID = vm.SellCurrencyID });
                }

                var entity = SessionHelper.CurrentEntity;
                var cost = monetaryMarketService.GetMonetaryOfferCost(vm.SellCurrencyID.Value, vm.BuyCurrencyID.Value,
                vm.Amount, vm.Rate, (MonetaryOfferTypeEnum)vm.OfferType);

                if (walletService.HaveMoney(entity.WalletID, new Money(cost.Currency, (decimal)cost.Sum)) == false)
                {
                    AddError("You do not have enough money!");
                    return RedirectToAction("IndexSpecified", new { buyCurrencyID = vm.BuyCurrencyID, sellCurrencyID = vm.SellCurrencyID });
                }





                var param = new CreateMonetaryOfferParam()
                {
                    Amount = vm.Amount,
                    BuyCurrency = Persistent.Currencies.GetById(vm.BuyCurrencyID.Value),
                    SellCurency = Persistent.Currencies.GetById(vm.SellCurrencyID.Value),
                    OfferType = (MonetaryOfferTypeEnum)vm.OfferType,
                    Rate = vm.Rate,
                    Seller = SessionHelper.CurrentEntity
                };

                monetaryMarketService.CreateOffer(param);
                return RedirectToAction("IndexSpecified", new { buyCurrencyID = vm.BuyCurrencyID, sellCurrencyID = vm.SellCurrencyID });
            }

            return RedirectToAction("Index");
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Monetary-Market/Reserved-Money")]
        public ActionResult Reserved()
        {
            var entity = SessionHelper.CurrentEntity;

            var reservedMoney = monetaryOfferRepository.GetReservedMoney(entity.EntityID);

            var vm = new ViewReservedViewModel(reservedMoney);

            return View(vm);
        }

        [Route("Monetary-Market/My-Offers")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult MyOffers()
        {
            var entity = SessionHelper.CurrentEntity;

            var offers = monetaryOfferRepository.Where(o => o.SellerID == entity.EntityID)
                .OrderBy(o => o.BuyCurrencyID)
                .OrderBy(o => o.SellCurrencyID)
                .ToList();

            var vm = new MyOffersViewModel(offers);


            return View(vm);
        }

        [Route("Monetary-Market/Transactions")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult Transactions()
        {
            var entity = SessionHelper.CurrentEntity;

            var vm = new MonetaryTransactionsViewModel(entity);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        [AjaxOnly]
        public JsonResult CalculateOffer(int sellCurrencyID, int buyCurrencyID, int amount, double rate, int offerTypeID)
        {
            try
            {
                var cost = monetaryMarketService.GetMonetaryOfferCost(sellCurrencyID, buyCurrencyID, amount, rate, (MonetaryOfferTypeEnum)offerTypeID);


                string symbol = cost.Currency.Symbol;

                var jsonCost = new
                {
                    offerCost = string.Format("{0} {1}", cost.OfferCost, symbol),
                    taxCost = string.Format("{0} {1}", cost.TaxCost, symbol),
                    sum = string.Format("{0} {1}", cost.Sum, symbol),
                    offerTypeID = offerTypeID
                };

                return JsonData(jsonCost);

            }
            catch(Exception e)
            {
                return UndefinedJsonError(e);
            }
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Monetary-Market/TransactionsAjax")]
        [HttpPost]
        [AjaxOnly]
        public JsonResult TransactionsAjax(IDataTablesRequest request)
        {
            var entityID = SessionHelper.CurrentEntity.EntityID;
            var data = monetaryTransactionRepository.Where(o => o.SellerID == entityID || o.BuyerID == entityID);
            var dataCount = data.Count();

            if(string.IsNullOrWhiteSpace(request.Search.Value) == false)
            {
                data = data.Where(o => 
                   o.SellerCurrency.Symbol.Contains(request.Search.Value) 
                || o.BuyerCurrency.Symbol.Contains(request.Search.Value));
            }

            var dataFilteredCount = data.Count();

            if(request.Columns.First().Sort.Direction == SortDirection.Ascending)
                data = data.OrderBy(o => o.ID);
            else
                data = data.OrderByDescending(o => o.ID);


            data = data.Skip(request.Start).Take(request.Length);

            var dataPage = data.ToList().Select(o => new
            {
                TransactionType = o.BuyerID == entityID ? "Buy" : "Sell",
                BuyCurrency = o.BuyerCurrency.Symbol,
                SellCurrency = o.SellerCurrency.Symbol,
                Rate = string.Format("{0} {1}", o.Rate, o.SellerCurrency.Symbol),
                AmountRate = string.Format("{0} {1}", o.Rate * o.Amount, o.SellerCurrency.Symbol),
                Tax = string.Format("{0} {1}", o.BuyerID == entityID ? o.BuyerTax : o.SellerTax, o.BuyerID == entityID ? o.SellerCurrency.Symbol : o.BuyerCurrency.Symbol) , 
                Suma = string.Format("{0} {1}", (o.BuyerID == entityID ? o.Rate : 1) * o.Amount + (o.BuyerID == entityID ? o.BuyerTax : o.SellerTax), o.BuyerID == entityID ? o.SellerCurrency.Symbol : o.BuyerCurrency.Symbol),
                Amount = o.Amount,
                DateTime = "Day " + o.Day + " " + o.Date.ToShortTimeString()
            });

            // Response creation. To create your response you need to reference your request, to avoid
            // request/response tampering and to ensure response will be correctly created.
            var response = DataTablesResponse.Create(request, dataCount, dataFilteredCount, dataPage);

            // Easier way is to return a new 'DataTablesJsonResult', which will automatically convert your
            // response to a json-compatible content, so DataTables can read it when received.
            return new DataTablesJsonResult(response, JsonRequestBehavior.DenyGet);

        }
    }
}