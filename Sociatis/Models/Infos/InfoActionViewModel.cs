using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUtils.Html;

namespace Sociatis.Models.Infos
{
    public class InfoActionViewModel : InfoItemViewModel
    {
        public string ActionName { get; private set; }
        public string ControllerName { get; private set; }
        public FormMethod FormMethod { get; private set; }
        public object RouteValues { get; private set; }
        public ClassField ClassField { get; set; } = new ClassField("action");
        


        public InfoActionViewModel(string action, string controller, string name, string icon, FormMethod formMethod, object routeValues) :base(name, icon)
        {
            ActionName = action;
            ControllerName = controller;
            FormMethod = formMethod;
            RouteValues = routeValues;
        }
        public InfoActionViewModel(string action, string controller, string name, string icon, FormMethod formMethod) : this(action, controller, name, icon, formMethod, null) { }
        public InfoActionViewModel(string action, string controller, string name, string icon) : this(action, controller, name, icon, FormMethod.Get) { }

        public InfoActionViewModel(string action, string controller, string name, string icon, object routeValues) : this(action, controller, name, icon, FormMethod.Get, routeValues) { }

        public static InfoActionViewModel CreateEntitySwitch(int entityID)
        {
            return new InfoActionViewModel("Switch", "Entity", "Switch", "fa-location-arrow", new { ID = entityID });
        }

    }
}