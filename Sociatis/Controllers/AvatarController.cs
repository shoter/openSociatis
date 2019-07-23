using Entities.enums;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.enums;
using WebServices.structs.Avatars;
using WebUtils.Attributes;

namespace Sociatis.Controllers
{
    public class AvatarController : ControllerBase
    {
        private readonly IUploadAvatarService uploadAvatarService;
        private readonly IUploadRepository uploadRepository;
        private readonly IEntityRepository entityRepository;

        public AvatarController(IPopupService popupService, IUploadAvatarService uploadAvatarService, IUploadRepository uploadRepository,
            IEntityRepository entityRepository) : base(popupService)
        {
            this.uploadAvatarService = uploadAvatarService;
            this.uploadRepository = uploadRepository;
            this.entityRepository = entityRepository;
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public JsonResult UploadPicture(HttpPostedFileBase file)
        {
            try
            {
                var result = uploadAvatarService.CanUpload(file, UploadLocationEnum.Temp, true);
                if (result.IsError)
                    return JsonError(result);
                var upload = uploadAvatarService.Upload(file, UploadLocationEnum.Temp, SessionHelper.LoggedCitizen);


                return JsonData(new
                {
                    uploadID = upload.ID,
                    uploadPath = uploadAvatarService.GetRelativePathForLocation(upload),
                });
            }
            catch (Exception ex)
            {
                return UndefinedJsonError(ex);
            }

        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [AjaxOnly]
        [HttpPost]
        public JsonResult CropPicture(int entityID, int uploadID, double x, double y, double width, double height)
        {
            try
            {
                var upload = uploadRepository.GetById(uploadID);
                var entity = entityRepository.GetById(entityID);
                var currentEntity = SessionHelper.CurrentEntity;
                var citizen = SessionHelper.LoggedCitizen;
                var crop = new CropRectangle((int)x, (int)y, (int)width, (int)height);

                var result = uploadAvatarService.CanCropUpload(upload, entity, currentEntity, citizen, crop);
                if (result.IsError)
                    return JsonError(result);

                var newUpload = uploadAvatarService.CropUploadAndMoveToAnotherLocation(upload, crop, UploadLocationEnum.Avatars);
                entity.ImgUrl = uploadAvatarService.GetRelativePathForLocation(newUpload);

                uploadRepository.SaveChanges();
                return JsonData(new
                {
                    msg = "Avatar changed!",
                    img = entity.ImgUrl
                });
            }
            catch (Exception e)
            {
                return UndefinedJsonError(e);
            }
        }

    }
}