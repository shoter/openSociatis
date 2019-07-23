using Common.EntityFramework;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface IDevIssueRepository : IRepository<DevIssue>
    {
        List<DevIssueLabelType> GetLabels(params DevIssueLabelTypeEnum[] labelTypes);
    }
}
