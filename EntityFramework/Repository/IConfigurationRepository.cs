using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface IConfigurationRepository : IRepository<ConfigurationTable>
    {
        ConfigurationTable GetConfiguration();
        int GetCurrentDay();
        DateTime GetLastDayChangeTime();
    }
}
