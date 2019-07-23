using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository.Base
{
    public interface IPersistentRepositoryStorage<TEntity>
        where TEntity : class
    {
        void Add(TEntity entity);

        IQueryable<TEntity> Where(Func<TEntity, bool> predicate);

        TEntity First();
        TEntity First(Func<TEntity, bool> predicate);
        TEntity FirstOrDefault(Func<TEntity, bool> predicate);
        void Clear();
        List<TEntity> ToList();
        bool Any(Func<TEntity, bool> predicate);

    }
}
