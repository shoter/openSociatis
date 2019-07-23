using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Weber.Html;

namespace WebServices.Html
{
    public class DevIssueLinkCreator
    {
        public static MvcHtmlString Create(DevIssue issue, string @class = null)
        {
            return LinkCreator.Create(
                name: issue.Name,
                action: "ViewIssue",
                controller: "DevIssue",
                routeValues: new { issueID = issue.ID });

        }
    }
}
