using Common;
using Common.Operations;
using Common.Transactions;
using Entities;
using Entities.enums;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Helpers;
using Sociatis.Models.Trades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebUtils.Attributes;
using WebUtils.Forms.Select2;

namespace Sociatis.Controllers
{
    public class TradeController : ControllerBase
    {
        private readonly ITradeService tradeService;
        private readonly IEntityRepository entityRepository;
        private readonly ITradeRepository tradeRepository;
        private readonly ITradeProductRepository tradeProductRepository;
        private readonly ITradeMoneyRepository tradeMoneyRepository;
        private readonly ITransactionScopeProvider transactionScopeProvider;

        public TradeController(IPopupService popupService, ITradeService tradeService, IEntityRepository entityRepository, ITradeRepository tradeRepository,
            ITradeProductRepository tradeProductRepository, ITradeMoneyRepository tradeMoneyRepository, ITransactionScopeProvider transactionScopeProvider) : base(popupService)
        {
            this.tradeService = tradeService;
            this.entityRepository = entityRepository;
            this.tradeRepository = tradeRepository;
            this.tradeProductRepository = tradeProductRepository;
            this.tradeMoneyRepository = tradeMoneyRepository;
            this.transactionScopeProvider = transactionScopeProvider;
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Trade/StartTrade")]
        [HttpPost]
        public ActionResult StartTrade(int entityID)
        {
            var source = SessionHelper.CurrentEntity;
            var destination = entityRepository.GetById(entityID);

            MethodResult result = tradeService.CanStartTrade(source, destination);
            if (result.IsError)
                return RedirectBackWithError(result);

            var trade = tradeService.StartTrade(source, destination);

            return RedirectToAction("View", "Trade", new { tradeID = trade.ID });
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Trade/StartTrade")]
        [HttpGet]
        public ActionResult StartTrade()
        {
            var vm = new StartTradeViewModel();

            return View(vm);
        }


        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Trade/{tradeID:int}")]
        public ActionResult View(long tradeID)
        {
            var trade = tradeRepository.GetById(tradeID);
            var entity = SessionHelper.CurrentEntity;

            MethodResult result = tradeService.CanHaveAccess(entity, trade);
            if (result.IsError)
                return RedirectToHomeWithError(result);

            var possibleItems = tradeService.GetItemsForTrade(entity, trade)
                .OrderBy(i => ((ProductTypeEnum)i.ProductID).ToHumanReadable())
                .ThenBy(i => i.Quality);
               

            var possibleMoney = tradeService.GetMoneyForTrade(entity, trade)
                .OrderBy(i => i.CurrencyID);

            var vm = new TradeViewModel(trade, possibleItems, possibleMoney, tradeService);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult RemoveProduct(long tradeID, int productID, int quality, int entityID)
        {
            var product = tradeProductRepository.SingleOrDefault(p =>
            p.TradeID == tradeID &&
            p.ProductID == productID &&
            p.Quality == quality &&
            p.EntityID == entityID);

            var entity = SessionHelper.CurrentEntity;
            var trade = tradeRepository.GetById(tradeID);

            var result = tradeService.CanRemoveProduct(product, entity, trade);
            if (result.IsError)
                return RedirectBackWithError(result);

            tradeService.RemoveProduct(product, trade);

            AddSuccess("Product removed!");
            return RedirectToAction("View", new { tradeID = tradeID });
                
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult RemoveMoney(long tradeID, int currencyID, int entityID)
        {
            var money = tradeMoneyRepository.SingleOrDefault(p =>
            p.TradeID == tradeID &&
            p.CurrencyID == currencyID &&
            p.EntityID == entityID);

            var entity = SessionHelper.CurrentEntity;
            var trade = tradeRepository.GetById(tradeID);

            var result = tradeService.CanRemoveMoney(money, entity, trade);
            if (result.IsError)
                return RedirectBackWithError(result);

            tradeService.RemoveMoney(money, trade);

            AddSuccess("Money removed!");
            return RedirectToAction("View", new { tradeID = tradeID });

        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [AjaxOnly]
        [HttpPost]
        public JsonResult AddProduct(int tradeID, int productID, int quality, int amount)
        {
            try
            {
                var trade = tradeRepository.GetById(tradeID);
                ProductTypeEnum productType = (ProductTypeEnum)productID;
                var entity = SessionHelper.CurrentEntity;
                using (var trs = transactionScopeProvider.CreateTransactionScope())
                {
                    MethodResult result = tradeService.CanAddProduct(productType, quality, amount, entity, trade);
                    if (result.IsError)
                        return JsonError(result);

                    tradeService.AddProduct(productType, quality, amount, entity, trade);
                    trs.Complete();
                }
                return JsonSuccess("Product has been added!");
            }
            catch (Exception e)
            {
                return UndefinedJsonError(e);
            }
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult AcceptTrade(int tradeID)
        {
            var trade = tradeRepository.GetById(tradeID);
            var entity = SessionHelper.CurrentEntity;

            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                var result = tradeService.CanAcceptTrade(entity, trade);
                if (result.IsError)
                    return RedirectBackWithError(result);

                tradeService.AcceptTrade(entity, trade);
                trs.Complete();
            }
            return RedirectToAction("View", new { tradeID = tradeID });
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult CancelTrade(int tradeID)
        {
            var trade = tradeRepository.GetById(tradeID);
            var entity = SessionHelper.CurrentEntity;

            var result = tradeService.CanCancelTrade(entity, trade);
            if (result.IsError)
                return RedirectBackWithError(result);

            tradeService.CancelTrade(trade, entity);
            return RedirectToAction("View", new { tradeID = tradeID });
        }


        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [AjaxOnly]
        [HttpPost]
        public JsonResult AddMoney(int tradeID, int currencyID, decimal amount)
        {
            try
            {
                var trade = tradeRepository.GetById(tradeID);
                Currency currency = Persistent.Currencies.GetById(currencyID);
                var entity = SessionHelper.CurrentEntity;

                using (var trs = transactionScopeProvider.CreateTransactionScope())
                {
                    MethodResult result = tradeService.CanAddMoney(currency, amount, entity, trade);
                    if (result.IsError)
                        return JsonError(result);

                    tradeService.AddMoney(currency, amount, entity, trade);
                    trs.Complete();
                }
                return JsonSuccess("Money has been added!");
            }
            catch (Exception e)
            {
                return UndefinedJsonError(e);
            }
        }

        public JsonResult GetEntitiesToTrade(Select2Request request)
        {
            string search = request.Query.Trim().ToLower();

            var query = entityRepository
                .Where(entity => entity.Name.ToLower().Contains(search))
                .OrderBy(entity => entity.Name)
                .Select(entity => new Select2Item()
                {
                    id = entity.EntityID,
                    text = entity.Name
                });

            return Select2Response(query, request);
        }
    }
}