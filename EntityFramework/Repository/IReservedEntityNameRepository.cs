using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface IReservedEntityNameRepository : IRepository<ReservedEntityName>
    {
        void Add(string name);
        void Remove(string name);

        void AddIfTheyDoNotExist(params string[] names);
    }
}
