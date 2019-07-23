using Common.EntityFramework;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class DevIssueRepository : RepositoryBase<DevIssue, SociatisEntities>, IDevIssueRepository
    {
        public DevIssueRepository(SociatisEntities context) : base(context)
        {
        }

        public List<DevIssueLabelType> GetLabels(params DevIssueLabelTypeEnum[] labelTypes)
        {
            List<int> labelIDs = labelTypes.Cast<int>().ToList();

            return context.DevIssueLabelTypes
                .Where(label => labelIDs.Contains(label.ID))
                .ToList();
        }
    }
}
