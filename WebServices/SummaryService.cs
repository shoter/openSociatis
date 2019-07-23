using Entities;
using Entities.enums;
using Entities.Models.Citizens;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public class SummaryService : ISummaryService
    {
        private readonly ICitizenRepository citizenRepository;
        public SummaryService(ICitizenRepository citizenRepository)
        {
            this.citizenRepository = citizenRepository;
        }

        public CitizenSummaryInfo GetCitizenSummaryInfo(int citizenID)
        {
            return citizenRepository.GetSummary(citizenID);
        }
    }
}
