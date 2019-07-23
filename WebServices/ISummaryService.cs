using Entities.Models.Citizens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface ISummaryService
    {
        CitizenSummaryInfo GetCitizenSummaryInfo(int citizenID);
    }
}
