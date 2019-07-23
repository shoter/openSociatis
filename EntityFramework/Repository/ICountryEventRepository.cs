using Common.EntityFramework;
using Entities.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface ICountryEventRepository : IRepository<CountryEvent>
    {
        List<EventModel> GetCountryEvents(int countryID, IEnumerable<Country> countries, int? count);
    }
}
