using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class SingleChangeRepository: IDisposable
    {
        private SociatisEntities _entities = new SociatisEntities();

        public void UpdateSingleField<TEntity, TProp>(Expression<Func<TEntity, TProp>> expression, Action<TEntity> setUnique, TProp value)
            where TEntity : class, new()
        {
            var entity = new TEntity();
            setUnique(entity);

            var memberSelectorExpression = expression.Body as MemberExpression;
            var property = memberSelectorExpression.Member as PropertyInfo;
            property.SetValue(entity, value, null);


            _entities.Set<TEntity>().Attach(entity);
            _entities.Entry(entity).Property(expression).IsModified = true;

        }

        public void Dispose()
        {
            if(_entities != null)
            {
                _entities.SaveChanges();
                _entities = null;
            }
            
        }
    }
}
