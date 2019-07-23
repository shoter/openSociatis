using Sociatis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Helpers
{
    public class Image
    {
        public string Path { get; set; }
        public Image(string path)
        {
            Path = path;
        }

        public override string ToString()
        {
            return Path;
        }

        public static implicit operator string(Image image)
        {
            return image.ToString();
        }

        public ImageViewModel VM
        {
            get
            {
                return new ImageViewModel(this);
            }
        }
    }
}