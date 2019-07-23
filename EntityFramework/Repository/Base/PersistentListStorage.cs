using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository.Base
{
    public class PersistentListStorage<TEntity> : IPersistentRepositoryStorage<TEntity>
        where TEntity : class
    {
        private List<TEntity> entities = new List<TEntity>();

        public void Add(TEntity entity)
        {
            entities.Add(entity);
        }

        public void Clear()
        {
            entities.Clear();
        }

        public TEntity First()
        {
            return entities.First();
        }

        public TEntity First(Func<TEntity, bool> predicate)
        {
            return entities.First(predicate);
        }

        public TEntity FirstOrDefault(Func<TEntity, bool> predicate)
        {
            return entities.FirstOrDefault(predicate);
        }

        public List<TEntity> ToList()
        {
            return entities.ToList();
        }

        public IQueryable<TEntity> Where(Func<TEntity, bool> predicate)
        {
            return entities.Where(predicate).AsQueryable();
        }

        public bool Any(Func<TEntity, bool> predicate)
        {
            return entities.Any(predicate);
        }
    }
}
