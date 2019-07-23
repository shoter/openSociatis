using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUtils;

namespace Sociatis.Helpers
{
    public static class PagingParamHtmlHelper
    {
        /// <summary>
        /// Important notice. It automatically creates row.
        /// </summary>
        /// <param name="pagingParam"></param>
        /// <returns></returns>
        public static MvcHtmlString Render(PagingParam pagingParam)
        {
            var row = new TagBuilder("div");

            row.AddCssClass("row");

            var PageNumberInput = new TagBuilder("input");
            PageNumberInput.MergeAttribute("type", "hidden");
            PageNumberInput.MergeAttribute("name", "PagingParam.PageNumber");
            PageNumberInput.MergeAttribute("value", pagingParam.PageNumber.ToString());

            row.InnerHtml += PageNumberInput;

            row.InnerHtml += CreateEmptyField();

            var pagingDiv = new TagBuilder("div");

            pagingDiv.AddCssClass("small-6 columns pagingParamContainer no-bullet");

            int linkFields = 0;
            for(int i = Math.Max(pagingParam.PageNumber - 2, 1); i <= Math.Min(pagingParam.PageNumber + 2, pagingParam.PageCount); ++i)
            {
                pagingDiv.InnerHtml += CreateLinkField(i.ToString(), i == pagingParam.PageNumber);
                linkFields++;
            }

            

            if(pagingParam.PageNumber + 2 < pagingParam.PageCount)
            {
                pagingDiv.InnerHtml += CreateDots();

                pagingDiv.InnerHtml += CreateLinkField(pagingParam.PageCount.ToString(), false);
                linkFields++;
            }

            if (linkFields <= 1)
                pagingDiv.AddCssClass("smallAmount");

            row.InnerHtml += pagingDiv.ToString();

            row.InnerHtml += CreateEmptyField();

            return MvcHtmlString.Create(row.ToString(TagRenderMode.Normal));
        }

        private static string CreateLinkField(string text, bool current)
        {
            var div = new TagBuilder("div");

            var pageTag = new TagBuilder(current ? "span" : "a");

            pageTag.InnerHtml = text;

            pageTag.AddCssClass("pagingLink");

            if (current)
                pageTag.AddCssClass("current");
            else
            {
                pageTag.MergeAttribute("onclick", string.Format("Sociatis.PagingTo({0}, this)", text));
            }

            div.InnerHtml += pageTag.ToString();

            return div.ToString();
        }

        private static string CreateDots()
        {
            var div = new TagBuilder("div");

            div.InnerHtml = "...";

            return div.ToString();
        }

        private static string CreateEmptyField()
        {
            var field = new TagBuilder("div");

            field.AddCssClass("small-3");
            field.AddCssClass("columns");

            field.InnerHtml = "&nbsp;";

            return field.ToString();
        }
    }
}