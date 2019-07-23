using Common.Exceptions;
using Common.Operations;
using Entities;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebServices.enums;
using WebServices.Extensions;
using WebServices.Helpers;

namespace WebServices
{
    public class UploadService : IUploadService
    {
        protected readonly IUploadRepository uploadRepository;

        public UploadService(IUploadRepository uploadRepository)
        {
            this.uploadRepository = uploadRepository;
        }
        public virtual MethodResult CanUpload(HttpPostedFileBase file, UploadLocationEnum location)
        {
            if (file == null || file.ContentLength == 0)
            {
                return new MethodResult("File is empty or does not exist");
            }

            if (file.ContentLength >= 5_000_000)
                return new MethodResult("File is too big! Upload something smaller than 5MB");

            return MethodResult.Success;
        }

        public Upload Upload(HttpPostedFileBase file, UploadLocationEnum location, Citizen citizen)
        {
            var filePath = GetUniqueFilePath(location, ".png");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            file.SaveAs(filePath);

            var fileName = Path.GetFileName(filePath);
            var upload = new Upload()
            {
                UploadedByCitizenID = citizen.ID,
                Day = GameHelper.CurrentDay,
                Time = DateTime.Now,
                UploadLocationID = (int)location,
                Filename = fileName,

            };

            uploadRepository.Add(upload);
            uploadRepository.SaveChanges();

            return upload;
        }




        /// <returns>Virtual path to the file with extension</returns>
        public MethodResult<string> UploadImage(HttpPostedFileBase file, UploadLocationEnum location, bool enableSizeValidation = true)
        {
            MethodResult<string> result = MethodResult<string>.Success;
            if (file == null || file.ContentLength == 0)
            {
                throw new UserReadableException("File is empty or does not exist");
            }

            var tempPath = Path.GetTempFileName();
            file.SaveAs(tempPath);

            Image img = Image.FromFile(tempPath);

            if (enableSizeValidation)
                result.Merge(checkIfImageIsCorrect(img));

            if (result.IsError)
                return result;

            var fileName = GetUniqueFilePath(location, ".png");
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            var imageFile = File.Create(fileName);


            img.Save(imageFile, ImageFormat.Png);
            img.Dispose();
            File.Delete(tempPath);
            imageFile.Close();


            result.ReturnValue = "\\" + fileName.Replace(HttpContext.Current.Request.ServerVariables["APPL_PHYSICAL_PATH"], string.Empty);
            return result;
        }

        private static MethodResult checkIfImageIsCorrect(Image img)
        {
            MethodResult result = MethodResult.Success;
            if (img.Size.Width > 450 || img.Size.Height > 450)
            {
                result.AddError("Picture can have max dimensions of 450x450");
            }

            if (Math.Abs(1.0 - img.Size.Width / (double)img.Size.Height) > 0.5)
            {
                result.AddError("Picture have wrong ratio!");
            }
            return result;
        }

        public void RemoveTemporaryFiles()
        {
            var dirPath = HttpContext.Current.Server.MapPath(UploadFolders.TempFoler);
            if (Directory.Exists(dirPath))
            {
                var files = Directory.GetFiles(dirPath);
                foreach (var file in files)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception) { }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        /// <param name="extension">Without dot</param>
        /// <returns></returns>
        public string GetUniqueFilePath(UploadLocationEnum location, string extension)
        {
            string filename = "";
            if (extension.StartsWith(".") == false)
                extension = "." + extension;
            do
            {
                filename = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
                filename += extension;
                filename = GetFilePathForLocation(filename, location);
            } while (File.Exists(filename) == true);

            return filename;
        }

        public string GetFilePathForLocation(Upload upload)
        {
            return GetFilePathForLocation(upload.Filename, (UploadLocationEnum)upload.UploadLocationID);
        }

        public string GetFilePathForLocation(string fileName, UploadLocationEnum location)
        {
            var locationPath = location.ToPath();
            return Path.Combine(HttpContext.Current.Server.MapPath(locationPath), fileName);
        }

        public string GetRelativePathForLocation(Upload upload)
        {
            return GetRelativePathForLocation(upload.Filename, (UploadLocationEnum)upload.UploadLocationID);
        }

        public string GetRelativePathForLocation(string filename, UploadLocationEnum location)
        {
            var locationPath = location.ToPath().Replace("~", "");
            //by removing ~ we will start with /content/ not ~/content/

            return Path.Combine(locationPath, filename);
        }
    }
}
