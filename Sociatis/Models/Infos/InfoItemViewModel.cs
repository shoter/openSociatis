using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebUtils.Html;

namespace Sociatis.Models.Infos
{
    public abstract class InfoItemViewModel
    {

        public string Name { get; set; }
        public string Icon { get; set; }

        public InfoItemViewModel(string name, string icon)
        {
            Name = name;
            Icon = icon;
        }
    }
}