using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebUtils.Mvc;

namespace Sociatis.Code
{
    public class SelectLists
    {
        public static List<SelectListItem> Countries { get; private set; }
        public static List<SelectListItem> Qualities { get; private set; }

        static SelectLists()
        {
            Countries = new CustomSelectList()
                .AddSelect()
                .AddItems(Persistent.Countries.GetAll(),
                c => c.ID,
                c => c.Entity.Name);

            Qualities = new CustomSelectList()
                .AddNumbers(1, 5);
        }
    }
}