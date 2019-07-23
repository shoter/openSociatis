using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class DevIssueExtensions
    {
        public static VisibilityOptionEnum GetVisibilityOption(this DevIssue issue)
        {
            return (VisibilityOptionEnum)issue.VisibilityOptionID;
        }
    }
}
