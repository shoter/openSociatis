using Common.EntityFramework;
using Entities.Models.Citizens;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface ICitizenRepository : IRepository<Citizen>
    {
        Citizen GetByName(string name);
        CitizenSummaryInfo GetSummary(int citizenID);

        bool IsEmailAddressAllowed(string address);
        bool IsEmailAddressUsed(string address);

        /// <summary>
        /// Can return null
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Can return null</returns>
        Citizen GetByEmail(string email);

        Citizen GetByCredentials(string username, string password);

        int GetActivePlayerCount(int currentDay);
    }
}
