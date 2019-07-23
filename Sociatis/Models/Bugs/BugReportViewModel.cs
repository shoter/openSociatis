using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Bugs
{
    public class BugReportViewModel
    {
        public string Content { get; set; }

        public HttpPostedFileBase BugImage { get; set; }

        public bool Sent { get; set; } = false;
    }
}