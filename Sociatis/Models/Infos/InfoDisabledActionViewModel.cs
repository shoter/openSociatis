using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Infos
{
    public class InfoDisabledActionViewModel : InfoActionViewModel
    {
        public string Tooltip { get; set; }
        public InfoDisabledActionViewModel(string name, string icon, string tooltip) : base("", "", name, icon)
        {
            Tooltip = tooltip;
            ClassField.AddClass("disabled");
            if (tooltip != null)
                ClassField.AddClass("has-tip");
        }

        public InfoDisabledActionViewModel(string name, string icon) : this(name, icon, null) { }
    }
}