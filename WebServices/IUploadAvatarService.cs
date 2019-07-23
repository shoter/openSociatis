using Common.Operations;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebServices.enums;
using WebServices.structs.Avatars;

namespace WebServices
{
    public interface IUploadAvatarService : IUploadService
    {
        MethodResult CanUpload(HttpPostedFileBase file, UploadLocationEnum location, bool isTemporary = false);
        Upload CropUploadAndMoveToAnotherLocation(Upload upload, CropRectangle crop, UploadLocationEnum newLocation);

        MethodResult CanCropUpload(Upload upload, Entity entity, Entity currentEntity, Citizen currentCitizen, CropRectangle crop);
    }
}
