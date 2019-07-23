using Common;
using Common.Extensions;
using Common.Operations;
using Common.Transactions;
using Entities.enums;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Helpers;
using Sociatis.Models.Gifts;
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
    public class GiftController : ControllerBase
    {
        private readonly IEntityRepository entityRepository;
        private readonly IWalletRepository walletRepository;
        private readonly IEquipmentRepository equipmentRepository;
        private readonly IGiftService giftService;
        private readonly ITransactionScopeProvider transactionScopeProvider;

        public GiftController(IPopupService popupService, IEntityRepository entityRepository, IWalletRepository walletRepository,
            IEquipmentRepository equipmentRepository, IGiftService giftService, ITransactionScopeProvider transactionScopeProvider) : base(popupService)
        {
            this.entityRepository = entityRepository;
            this.walletRepository = walletRepository;
            this.equipmentRepository = equipmentRepository;
            this.giftService = giftService;
            this.transactionScopeProvider = transactionScopeProvider;
        }

        [Route("Gift/{destinationID:int}")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult Gift(int destinationID)
        {
            var destination = entityRepository.GetById(destinationID);
            string destinationName = destination?.Name;

            var entity = SessionHelper.CurrentEntity;

            int[] disallowedItems = (new ProductTypeEnum[]
                {
                    ProductTypeEnum.SellingPower,
                    ProductTypeEnum.MedicalSupplies,
                    ProductTypeEnum.UpgradePoints
                }).Cast<int>().ToArray<int>();

            var equipmentItems = equipmentRepository.Where(eq => eq.ID == entity.EquipmentID)
                .SelectMany(eq => eq.EquipmentItems)
                .Where(ei => disallowedItems.Contains(ei.ProductID) == false)
                .OrderBy(e => e.Quality)
                .OrderBy(e => e.Product.Name)
                .ToList();

            

            var walletMoneys = walletRepository.Where(wallet => wallet.ID == entity.WalletID)
                .OrderBy(e => e.ID)
                .SelectMany(wallet => wallet.WalletMoneys).ToList();


            var vm = new GiftViewModel(walletMoneys, equipmentItems, destinationID, destinationName);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [AjaxOnly]
        [HttpPost]
        public JsonResult SendMoney(int destinationID, int currencyID, decimal amount)
        {
            try
            {
                var source = SessionHelper.CurrentEntity;
                var destination = entityRepository.GetById(destinationID);
                var currency = Persistent.Currencies.GetById(currencyID);

                using (var trs = transactionScopeProvider.CreateTransactionScope())
                {
                    MethodResult result = giftService.CanSendMoneyGift(source, destination, currency, amount);
                    if (result.IsError)
                        return JsonError(result);

                    giftService.SendMoneyGift(source, destination, currency, amount);
                    trs.Complete();
                }
                    return JsonSuccess("Money successfully sent!");
                
            }
            catch (Exception e)
            {
                return JsonError("Undefined error");
            }
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [AjaxOnly]
        [HttpPost]
        public JsonResult WillGiftUseFuel(int destinationID)
        {
            try
            {
                var source = SessionHelper.CurrentEntity;
                var destination = entityRepository.GetById(destinationID);

                bool result = giftService.WillGiftUseFuel(source, destination);

                return JsonData(result);
            }
            catch (Exception e)
            {
                return JsonError("Undefined error!");
            }
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [AjaxOnly]
        [HttpPost]
        public JsonResult CanReceiveProductGifts(int destinationID)
        {
            try
            {
                var source = SessionHelper.CurrentEntity;
                var destination = entityRepository.GetById(destinationID);

                MethodResult result = giftService.CanReceiveProductGifts(source, destination);

                if (result.IsError)
                    return JsonData(new { result= false, error= result.Errors[0]});

                return JsonData(new { result = true, error = "" });
            }
            catch (Exception e)
            {
                return JsonError("Undefined error!");
            }
        }
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [AjaxOnly]
        [HttpPost]
        public JsonResult SendProduct(int destinationID, int productID, int quality, int amount)
        {
            try
            {
                var source = SessionHelper.CurrentEntity;
                var destination = entityRepository.GetById(destinationID);
                var productType = (ProductTypeEnum)productID;

                MethodResult result = giftService.CanSendProductGift(source, destination, productType, quality, amount);
                if (result.IsError)
                    return JsonError(result);

                giftService.SendProductGift(source, destination, productType, quality, amount);
                return JsonSuccess($"{productType.ToHumanReadable().FirstUpper()} successfully sent!");
            }
            catch (Exception e)
            {
                return JsonError("Undefined error");
            }
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [AjaxOnly]
        [HttpPost]
        public JsonResult GetFuelCost(int destinationID, int productID, int quality, int amount)
        {
            try
            {
                var source = SessionHelper.CurrentEntity;
                var destination = entityRepository.GetById(destinationID);
                var productType = (ProductTypeEnum)productID;

                var fuelCost = giftService.GetNeededFuelToSendGift(source, destination, productType, quality, amount);
                return JsonData(fuelCost);
            }
            catch (Exception e)
            {
                return JsonError("Undefined error");
            }
        }


            public JsonResult GetPossibleDestinations(Select2Request request)
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