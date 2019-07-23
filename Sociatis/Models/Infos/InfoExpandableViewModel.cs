using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sociatis.Models.Infos
{
    public class InfoExpandableViewModel : InfoItemViewModel
    {
        public List<InfoActionViewModel> Children { get; private set; } = new List<InfoActionViewModel>();
        public InfoExpandableViewModel(string name, string icon) : base(name, icon) { }
        public InfoExpandableViewModel AddChild(InfoActionViewModel action)
        {
            Children.Add(action);
            action.ClassField.RemoveClass("action");
            return this;
        }

        public static InfoExpandableViewModel CreateExchange(int entityID)
        {
            var exp = new InfoExpandableViewModel("Exchange", "fa-dollar");

            exp.AddChild(new InfoActionViewModel("Gift", "Gift", "Gift", "fa-gift", new { destinationID = entityID }))
                .AddChild(new InfoActionViewModel("StartTrade", "Trade", "Start Trade", "fa-dollar", FormMethod.Post, new { entityID = entityID }));

            return exp;
        }
    }
}