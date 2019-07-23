using Common.Operations;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebServices.enums;

namespace WebServices
{
    public interface IUploadService
    {
        Upload Upload(HttpPostedFileBase file, UploadLocationEnum locaton, Citizen citizen);
        MethodResult<string> UploadImage(HttpPostedFileBase file, UploadLocationEnum location, bool enableSizeValidation = true);
        void RemoveTemporaryFiles();

        string GetFilePathForLocation(string fileName, UploadLocationEnum location);

        string GetRelativePathForLocation(Upload upload);

        string GetRelativePathForLocation(string filename, UploadLocationEnum location);
    }
}
