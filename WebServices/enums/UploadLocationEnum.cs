using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Extensions;

namespace WebServices.enums
{
    public enum UploadLocationEnum
    {
        Avatars = 1,
        Articles = 2,
        Bugs = 3,
        Temp = 4,

    }

    public static class UploadLocationEnumExtensions
    {
        public static string ToPath(this UploadLocationEnum location)
        {
            switch(location)
            {
                case UploadLocationEnum.Avatars:
                    {
                        return UploadFolders.AvatarFolder;
                    }
                case UploadLocationEnum.Articles:
                    {
                        return UploadFolders.ArticleFolder;
                    }
                case UploadLocationEnum.Bugs:
                    {
                        return UploadFolders.BugContentFolder;
                    }
                case UploadLocationEnum.Temp:
                    {
                        return UploadFolders.TempFoler;
                    }
            }
            throw new ArgumentException("Location does not exist!");
        }
    }

}
