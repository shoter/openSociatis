using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebServices.structs.Issues
{
    public class CreateIssueArgs
    {
        public string Content { get; set; }
        public VisibilityOptionEnum VisibilityOption { get; set; }
        public string Name { get; set; }

        public List<HttpPostedFileBase> UploadedFiles { get; set; }

    }
}
