using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.Extensions
{
    public class UploadFolders
    {
        static public string Content { get; set; } = "~/Content/";
        static public string UploadFolder { get; set; } = Content + "Upload/";

        static public string AvatarFolder { get; set; } = UploadFolder + "Avatars/";
        static public string ArticleFolder { get; set; } = UploadFolder + "Avatars/";
        public static string BugContentFolder { get; set; } = "~/Content/Bugs/upload/";
        public static string TempFoler { get; set; } = UploadFolder + "Temp/";
    }
}
