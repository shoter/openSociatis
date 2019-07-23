using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Infos
{
    public class InfoCustomActionViewModel : InfoActionViewModel
    {
        public string OnClick { get; set; } = "";
        public InfoCustomActionViewModel(string name, string icon, string onClick) : base("", "", name, icon)
        {
            OnClick = onClick;
        }
    }
}