using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Sociatis.Models.Infos
{
    public class InfoExpandableActionViewModel : InfoActionViewModel
    {
        public List<InfoActionViewModel> Children { get; set; } = new List<InfoActionViewModel>();
        public InfoExpandableActionViewModel(string name, string icon) : base("", "", name, icon) { }

        public InfoExpandableActionViewModel AddChild(InfoActionViewModel child)
        {
            child.ClassField.RemoveClass("action");
            Children.Add(child);
            return this;
        }
    }
}
