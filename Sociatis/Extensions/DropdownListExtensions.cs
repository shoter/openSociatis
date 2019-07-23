using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sociatis.Extensions
{
    public static class DropdownListExtensions
    {
        public static List<SelectListItem> Choose(this List<SelectListItem> list, int item)
        {
            var copyList = new List<SelectListItem>();

            list.ForEach(x => copyList.Add(x));

            copyList.First(x => x.Value == item.ToString()).Selected = true;

            return copyList;
        }
    }
}