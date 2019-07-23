using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebServices.Helpers
{
    public class VirtualFileHelper
    {
        public static void Delete(string virtualPath)
        {
            var path = AbsolutePath(virtualPath);
            File.Delete(path);
        }

        public static bool Exists(string virtualpath)
        {
            var path = AbsolutePath(virtualpath);
            return File.Exists(path);
        }

        private static string AbsolutePath(string virtualPath)
        {
            return  HttpContext.Current.Server.MapPath(virtualPath);
        }
    }
}
