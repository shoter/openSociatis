using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class ConfigurationRepository : RepositoryBase<ConfigurationTable, SociatisEntities>, IConfigurationRepository
    {
        public ConfigurationRepository(SociatisEntities context) : base(context)
        {
        }

        public ConfigurationTable GetConfiguration()
        {
             return DbSet.First();
        }

        public int GetCurrentDay()
        {
            return GetConfiguration().CurrentDay;
        }

        public DateTime GetLastDayChangeTime()
        {
            return GetConfiguration().LastDayChange;
        }
    }
}
