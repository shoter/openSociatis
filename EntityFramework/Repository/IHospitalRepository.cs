using Common.EntityFramework;
using Entities.Models.Hospitals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface IHospitalRepository : IRepository<Hospital>
    {
        HealingPrice GetHealingPrice(int hospitalID, int citizenID);
        void SetHealingPrice(int hospitalID, int countryID, decimal? price);
        void AddNationalityHealingOption(int hospitalID, int countryID, decimal? price);
        HospitalManage GetHospitalManageInfo(int hospitalID);
    }
}
