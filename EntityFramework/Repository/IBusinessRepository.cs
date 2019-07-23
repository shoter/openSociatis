using Entities.Models.Businesses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface IBusinessRepository
    {
        IEnumerable<Business> GetBusinesses(int citizenID);
    }
}
