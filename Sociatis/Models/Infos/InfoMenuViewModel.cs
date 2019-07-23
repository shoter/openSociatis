using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Infos
{
    public class InfoMenuViewModel
    {
        public List<InfoItemViewModel> Items { get; set; } = new List<InfoItemViewModel>();
        public InfoMenuViewModel AddItem(InfoItemViewModel item)
        {
            Items.Add(item);
            return this;
        }
    }
}