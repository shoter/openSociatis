using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Infrastructure.Repository.Base
{
    public interface IRepository<T> 
        where T : class
    {
        IEnumerable<T> Get();
        T GetById(int id);
        void Create(T t);
        void Delete(int id);
        void Update(T t);
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
        void SaveChanges();
    }
}
