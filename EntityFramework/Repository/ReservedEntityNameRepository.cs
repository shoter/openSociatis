using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class ReservedEntityNameRepository : RepositoryBase<ReservedEntityName, SociatisEntities>, IReservedEntityNameRepository
    {
        public ReservedEntityNameRepository(SociatisEntities context) : base(context)
        {
        }

        public void Add(string name)
        {
            Add(new ReservedEntityName()
            {
                Name = name
            });
        }

        public void AddIfTheyDoNotExist(params string[] names)
        {
            var existing = Where(res => names.Contains(res.Name))
                .Select(r => r.Name)
                .ToList();

            var unique = names.Distinct();

            foreach (var name in unique)
            {
                if (existing.Contains(name))
                    continue;

                Add(name);
            }
        }

        public void Remove(string name)
        {
            var entities = Where(ent => ent.Name == name);
            foreach (var entity in entities)
                Remove(entity);
        }
    }
}
