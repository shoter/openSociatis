using Entities;
using Entities.enums;
using Entities.Repository;
using Sociatis.Helpers;
using Sociatis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Controllers
{
    public class WalletController : ControllerBase
    {
        private readonly IWalletService walletService;
        private readonly IWalletRepository walletRepository;

        public WalletController(IWalletService walletService, IWalletRepository walletRepository, IPopupService popupService) : base(popupService)
        {
            this.walletService = walletService;
            this.walletRepository = walletRepository;
        }

        public ActionResult MoneyPrintout(List<MoneyViewModel> vm)
        {
            if (vm == null)
                vm = TempData["MoneyList"] as List<MoneyViewModel>;

            return PartialView("MoneyPrintout", vm);
        }

        public ActionResult WalletShortInformation(int walletID, int mainCurrencyID)
        {
            List<MoneyViewModel> vm = new List<MoneyViewModel>();

            List<int> currencies = new List<int>();
            currencies.Add(mainCurrencyID);
            currencies.Add((int)CurrencyTypeEnum.Gold);

            var moneys = walletService.GetWalletMoney(walletID, currencies);

            foreach(var money in moneys)
            {
                vm.Add(new MoneyViewModel(money));
            }

            return MoneyPrintout(vm);
        }

        public JsonResult GetWalletCurrencyAmount(int walletID, int currencyID)
        {
            try
            {
                var result = walletService.CanAccessWallet(walletID, SessionHelper.CurrentEntity.EntityID);
                if (result.IsError)
                    return JsonError(result);

                decimal amount = walletRepository.GetMoneyAmount(walletID, currencyID);

                return JsonData(amount);
            }
            catch (Exception e)
            {
                return JsonDebugOnlyError(e);
            }
        }

        public ActionResult MakeDelivery(int constructionID, int offerID, int amount)
        {
            AddSuccess($"{constructionID}<br/>{offerID}<br/>{amount}");
            return RedirectBack();
        }
    }
}