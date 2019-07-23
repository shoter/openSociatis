using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Sociatis.Models.Base
{
    public class ViewModelBase
    {
        /// <summary>
        /// Gets the select list based on the ienumerable containing objects which have integer ID property
        /// and string Name property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The list items to convert.</param>
        /// <param name="firstEmpty">if set to <c>true</c> [first empty].</param>
        /// <returns></returns>
        protected static List<SelectListItem> CreateSelectList<T>(IEnumerable<T> items, bool includeEmptySelectItem = true, string emptySelectItemText = "Select", string defaultValue = "")
        {
            var retList = new List<SelectListItem>();
            if (includeEmptySelectItem)
            {
                retList.Add(new SelectListItem() { Text = String.Format("-- {0} --", emptySelectItemText), Value = defaultValue });
            }
            retList.AddRange(items.Select(a => new SelectListItem() { Text = ((dynamic)a).Name, Value = ((dynamic)a).ID.ToString() }));
            return retList;
        }


        /// <summary>
        /// Gets the select list based on the list and specified members that would be responsible
        /// for text and the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list of objects to be converted.</param>
        /// <param name="textProperty">The text property.</param>
        /// <param name="valueProperty">The value property.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="includeEmptySelectItem">if set to <c>true</c> [include empty select item].</param>
        /// <returns></returns>
        protected static List<SelectListItem> CreateSelectList<T>(IEnumerable<T> list, Func<T, string> textProperty,
            Func<T, int> valueProperty, bool includeEmptySelectItem = true, string emptySelectItemText = "Select", string defaultValue = "")
        {
            // So what's going on here
            // We create empty list 
            var retList = new List<SelectListItem>();
            // If the includeEmptySelectItem == true - then add the -- Select-- item to the list
            if (includeEmptySelectItem)
            {
                retList.Add(new SelectListItem() { Text = String.Format("-- {0} --", emptySelectItemText), Value = defaultValue });
            }
            // and finally we add the elements from the input list
            retList.AddRange(list.Select(a => new SelectListItem()
            {
                Text = textProperty(a),
                Value = valueProperty(a).ToString()
            })
            // I have added order by here already
            // So now instead of adding order by everywhere - I'd suggest using this GetSelectList method
            .OrderBy(a => a.Text));
            return retList;
        }

        protected static List<SelectListItem> CreateSelectList<TEnum>(Func<TEnum, object> textProperty)
        {
#if DEBUG
            if (!typeof(TEnum).IsEnum)
                throw new Exception("It must be enum!");
#endif
            List<SelectListItem> ret = new List<SelectListItem>();
            foreach (TEnum val in Enum.GetValues(typeof(TEnum)).Cast<TEnum>())
            {
                ret.Add(new SelectListItem()
                {
                    Text = textProperty(val).ToString(),
                    Value = Convert.ToInt32(val).ToString()
                });
            }

            return ret.OrderBy(r => r.Text).ToList();
        }
    }
}
