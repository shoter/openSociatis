using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Common.Operations;
using WebServices.enums;
using System.IO;
using System.Drawing;
using Entities.Repository;
using Entities;
using WebServices.structs.Avatars;
using WebServices.Helpers;
using Entities.Extensions;
using Entities.enums;

namespace WebServices
{
    public class UploadAvatarService : UploadService, IUploadAvatarService
    {
        private readonly ICompanyService companyService;
        private readonly INewspaperService newspaperService;
        private readonly IPartyService partyService;
        public UploadAvatarService(IUploadRepository uploadRepository, ICompanyService companyService, INewspaperService newspaperService, IPartyService partyService) : base(uploadRepository)
        {
            this.companyService = companyService;
            this.newspaperService = newspaperService;
            this.partyService = partyService;
        }
        public static string[] AllowedExtension = new string[]{
            "jpg",
            "gif",
            "tiff",
            "jpeg",
            "png"
            };

        public MethodResult CanUpload(HttpPostedFileBase file, UploadLocationEnum location, bool isTemporary = false)
        {
            var result = base.CanUpload(file, location);
            if (result.IsError)
                return result;

            var extension = Path.GetExtension(file.FileName).Replace(".", "").ToLower();
            if (AllowedExtension.Contains(extension) == false)
                return new MethodResult("You cannot use this extension!");

            if (isTemporary == false)
            {
                result = ChackIfAvatarIsCorrectImage(file);
                if (result.IsError)
                    return result;
            }

            return MethodResult.Success;
        }

        public static MethodResult ChackIfAvatarIsCorrectImage(HttpPostedFileBase file)
        {
            var tempPath = Path.GetTempFileName();
            file.SaveAs(tempPath);
            MethodResult result = MethodResult.Success;

            using (var img = Image.FromFile(tempPath))
            {
                if (img.Size.Width != 150 || img.Size.Height != 150)
                {
                    result.AddError("Final picture must have dimensions of 150x150 pixels!");
                }
            }

            return result;
        }

        public MethodResult CanCropUpload(Upload upload, Entity entity, Entity currentEntity, Citizen currentCitizen, CropRectangle crop)
        {
            if (upload.UploadedByCitizenID != currentCitizen.ID)
                return new MethodResult("This is not your upload!");


            switch (entity.GetEntityType())
            {
                case EntityTypeEnum.Citizen:
                    if (entity.EntityID != currentEntity.EntityID)
                        return new MethodResult("You cannot do that!");
                    break;
                case EntityTypeEnum.Company:
                    {
                        var rights = companyService.GetCompanyRights(entity.Company, currentEntity, currentCitizen);
                        if (rights.CanSwitch == false)
                            return new MethodResult("You cannot do that!");
                        break;
                    }
                case EntityTypeEnum.Newspaper:
                    {
                        var rights = newspaperService.GetNewspaperRights(entity.Newspaper, currentEntity, currentCitizen);
                        if (rights != NewspaperRightsEnum.Full)
                            return new MethodResult("You cannot do that!");
                        break;
                    }
                case EntityTypeEnum.Party:
                    {
                        var role = partyService.GetPartyRole(currentCitizen, entity.Party);
                        if(role < PartyRoleEnum.Manager)
                            return new MethodResult("You cannot do that!");
                        break;
                    }
            }

           
            if (crop.X < 0 || crop.Y < 0)
                return new MethodResult("Wrong crop!");

            using (var image = Image.FromFile(GetFilePathForLocation(upload)))
            {

                if (crop.X + crop.Width > image.Width || crop.Y + crop.Height > image.Height)
                    return new MethodResult("Wrong crop!");
            }

            return MethodResult.Success;

        }

        public Upload CropUploadAndMoveToAnotherLocation(Upload upload, CropRectangle crop, UploadLocationEnum newLocation)
        {
            Bitmap bitmap;
            Upload newUpload;
            using (var image = Image.FromFile(GetFilePathForLocation(upload)))
            {
                bitmap = cropImage(image, crop);
            }

            using (var resized = new Bitmap(bitmap, new Size(crop.Width, crop.Height)))
            {
                var path = GetUniqueFilePath(newLocation, Path.GetExtension(upload.Filename).Replace(".", ""));
                resized.Save(path);
                newUpload = new Entities.Upload()
                {
                    UploadedByCitizenID = upload.UploadedByCitizenID,
                    UploadLocationID = (int)newLocation,
                    Time = DateTime.Now,
                    Day = GameHelper.CurrentDay,
                    Filename = Path.GetFileName(path)
                };

                uploadRepository.Add(newUpload);
                uploadRepository.SaveChanges();

            }
            bitmap.Dispose();

            return newUpload;
        }


        public static Bitmap cropImage(Image image, CropRectangle crop)
        {
            var rect = new Rectangle(crop.X, crop.Y, crop.Width, crop.Height);

            Bitmap bmpImage = new Bitmap(image);
            return bmpImage.Clone(rect, bmpImage.PixelFormat);
        }

    }
}
