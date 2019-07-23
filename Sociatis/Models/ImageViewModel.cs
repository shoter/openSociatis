using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Sociatis.Models
{
    public class ImageViewModel
    {
        public string Url { get; set; }
        public bool AlwaysShow { get; set; } = false;
        public string ImgClass { get; set; }
        public bool Exists
        {
            get
            {
                if (AlwaysShow)
                    return true;
                if (string.IsNullOrWhiteSpace(Url))
                    return false;

                var path = HostingEnvironment.MapPath(Url);
                return File.Exists(path);
            }
        }

        public ImageViewModel(string url, bool alwaysShow = false, string imgClass = "")
        {
            this.Url = url;
            this.AlwaysShow = alwaysShow;
            this.ImgClass = imgClass;
        }

        public ImageViewModel(Image image)
            :this(image.Path)
        {

        }

        public ImageViewModel()
        {

        }
    }
}
