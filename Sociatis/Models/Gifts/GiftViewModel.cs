using Entities;
using Entities.enums;
using Entities.Extensions;
using Sociatis.Controllers;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebUtils.Forms.Select2;

namespace Sociatis.Models.Gifts
{
    public class GiftViewModel
    {
        public Select2AjaxViewModel DestinationSelector { get; set; }
        public List<MoneyGiftViewModel> MoneyGifts { get; set; }
        public List<ProductGiftViewModel> ProductGifts { get; set; }
        public bool CanSendProducts { get; set; } = false;

        public GiftViewModel(List<WalletMoney> walletMoneys, List<EquipmentItem> equipmentItems, int? currentDestinationID, string currentDestinationName)
        {
            MoneyGifts = walletMoneys.Select(wm => new MoneyGiftViewModel(wm)).ToList();
            ProductGifts = equipmentItems.Select(ei => new ProductGiftViewModel(ei)).ToList();

            DestinationSelector = Select2AjaxViewModel.Create<GiftController>(c => c.GetPossibleDestinations(null), "DestinationID", currentDestinationID, currentDestinationName);
            DestinationSelector.OnChange = "Sociatis.Gifts.OnDestinationChange";
            DestinationSelector.ID = "DestinationID";

            if (SessionHelper.CurrentEntity.Is(EntityTypeEnum.Citizen, EntityTypeEnum.Company))
                CanSendProducts = true;
        }
        public GiftViewModel(List<WalletMoney> walletMoneys, List<EquipmentItem> equipmentItems) : this(walletMoneys, equipmentItems, null, null) { }

    }
}