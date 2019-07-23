using Common.Operations;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Helpers;
using Sociatis.Models.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Controllers
{
    public class EquipmentController : ControllerBase
    {
        IEquipmentRepository equipmentRepository;
        IEquipmentService equipmentService;

        public EquipmentController(IEquipmentRepository equipmentRepository, IEquipmentService equipmentService, IPopupService popupService) : base(popupService)
        {
            this.equipmentRepository = equipmentRepository;
            this.equipmentService = equipmentService;
        }
        // GET: Equipment
        public ActionResult DisplayEquipment(int equipmentID)
        {
            var equipment = equipmentRepository.GetById(equipmentID);

            var vm = new EquipmentViewModel(equipment);

            return PartialView(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult DropSingleItem(int itemID)
        {
            var item = equipmentRepository.GetEquipmentItem(itemID);

            if(item == null)
                return RedirectBackWithError("Item does not exist!");

            var ownerEntity = item.Equipment.Entities.First();

            if (ownerEntity.EntityID != SessionHelper.CurrentEntity.EntityID)
                return RedirectBackWithError("You cannot remove this item!");
            var message = string.Format("You successfully dropped 1 {0} Q{1}", item.Product.Name, item.Quality);
            equipmentRepository.RemoveEquipmentItem(ownerEntity.EquipmentID.Value, item.ProductID, item.Quality, amount: 1);
            equipmentRepository.SaveChanges();

            AddSuccess(message);
            return RedirectBack();
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult DropAllItem(int itemID)
        {
            var item = equipmentRepository.GetEquipmentItem(itemID);

            if (item == null)
                return RedirectBackWithError("Item does not exist!");

            var ownerEntity = item.Equipment.Entities.First();

            if (ownerEntity.EntityID != SessionHelper.CurrentEntity.EntityID)
                return RedirectBackWithError("You cannot remove this item!");
            var message = string.Format("You successfully dropped {0} Q{1}", item.Product.Name, item.Quality);

            equipmentRepository.RemoveEquipmentItem(itemID);
            equipmentRepository.SaveChanges();

            AddSuccess(message);
            return RedirectBack();
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult DropItem(int itemID, int amount)
        {
            var item = equipmentRepository.GetEquipmentItem(itemID);

            if (item == null)
                return RedirectBackWithError("Item does not exist!");

            MethodResult<bool> result;
            if (result = equipmentService.CanHaveAccessToEquipment(item.Equipment, SessionHelper.CurrentEntity, SessionHelper.LoggedCitizen) == false)
                return RedirectToHomeWithError(result);

            var message = string.Format("You successfully dropped {0} Q{1}", item.Product.Name, item.Quality);
            

            equipmentService.RemoveProductsFromEquipment((ProductTypeEnum)item.ProductID, amount, item.Quality, item.Equipment);
            AddSuccess(message);
            return RedirectBack();
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult UseItem(int itemID)
        {
            var item = equipmentRepository.GetEquipmentItem(itemID);
            if (item == null)
                return RedirectToHomeWithError("Item not found!");
            var entity = SessionHelper.CurrentEntity;

            if (item.EquipmentID != entity.EquipmentID)
                return RedirectToHomeWithError("You are not an owner of this item!");

            MethodResult<bool> err = null;

            if((err = equipmentService.CanUseEquipmentItem(item, entity)))
            {
                if(item.GetProductType() == ProductTypeEnum.MovingTicket)
                {
                    return RedirectToAction("Travel", "Citizen");
                }

                var result = equipmentService.UseEquipmentItem(item, entity);

                if (result.IsError)
                    return RedirectBackWithError(result as MethodResult);

                AddInfo(result.ReturnValue);
                return RedirectBack();
            }


            return RedirectToHomeWithError(err);
        }
    }
}