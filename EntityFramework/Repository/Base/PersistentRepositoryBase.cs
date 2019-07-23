using Common;
using Common.EntityFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Common.EntityFramework.SingleChanges;

namespace Entities.Repository.Base
{
    public abstract class PersistentRepositoryBase<TEntity> : IRepository<TEntity>
        where TEntity : class, new()
    {
        protected IPersistentRepositoryStorage<TEntity> storage;

        public IQueryable<TEntity> Query => throw new NotImplementedException();

        public PersistentRepositoryBase(IPersistentRepositoryStorage<TEntity> storage)
        {
            this.storage = storage;

            Init();
            
        }

        public void Init()
        {
            storage.Clear();

            var context = new SociatisEntities();

            var entities = context.Set<TEntity>().ToList();

            foreach (var entity in entities)
            {
                var persistentEntity = new TEntity();

                PreLoadEntity(persistentEntity, entity);
                CleanNavigationalProperties(persistentEntity);
                LoadEntity(persistentEntity, entity);

                storage.Add(persistentEntity);
            }

            AfterLoad();
        }

        static protected void CleanNavigationalProperties<T>(T persistentEntity)
        {
            var properties = persistentEntity.GetType()
                .GetProperties();

            foreach (var propertyInfo in properties)
            {
                if (Utils.IsSimple(propertyInfo.PropertyType) == false)
                    propertyInfo.SetValue(persistentEntity, null);
            }
        }

        private void PreLoadEntity(TEntity persistentEntity, TEntity dbEntity)
        {
            var properties = persistentEntity.GetType()
                .GetProperties();

            foreach(var propertyInfo in properties)
            {
                if (Utils.IsSimple(propertyInfo.PropertyType))
                    propertyInfo.SetValue(persistentEntity, propertyInfo.GetValue(dbEntity));
            }
        }

        protected abstract void LoadEntity(TEntity persistentEntity, TEntity dbEntity);
        protected abstract void AfterLoad();

        protected void Load<T, TProp>(Expression<Func<T, TProp>> property, T persistentEntity, T dbEntity)
        {
            Load(property, persistentEntity, dbEntity, null);
        }
        protected void Load<T, TProp>(Expression<Func<T,TProp>> property, T persistentEntity, T dbEntity, Action<TProp, TProp> afterLoadAction)
        {
            var memberSelectorExpression = property.Body as MemberExpression;
            var propertyInfo = memberSelectorExpression.Member as PropertyInfo;

            var value = propertyInfo.GetValue(dbEntity);

            var type = value.GetType();

            if (type.IsGenericType)
            {
                var colInt = type.GetInterface("ICollection`1");

                if (colInt != null)
                {
                    var argType = type.GetGenericArguments()[0];


                    var count = (int)colInt.GetMethod("get_Count").Invoke(value, null);
                    Array collectionValues = Array.CreateInstance(argType, count);

                    colInt.GetMethod("CopyTo").Invoke(value, new object[] { collectionValues, 0 });

                    Type listType = typeof(List<>).MakeGenericType(new[] { argType });
                    IList list = (IList)Activator.CreateInstance(listType);

                    foreach (var colVal in collectionValues)
                    {
                        if (afterLoadAction != null)
                        {

                        }
                        CleanNavigationalProperties(colVal);
                        
                        list.Add(colVal);
                    }

                    propertyInfo.SetValue(persistentEntity, list);
                }
            }
            else
            {
                propertyInfo.SetValue(persistentEntity, value);
            }
        }

        

        public IEnumerable<TEntity> GetAll()
        {
            return storage.ToList();
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            return storage.Where(predicate.Compile());
        }

        public TEntity First()
        {
            return storage.First();
        }

        public TEntity First(Expression<Func<TEntity, bool>> predicate)
        {
            return storage.First(predicate.Compile());
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return storage.FirstOrDefault(predicate.Compile());
        }

        public virtual void Add(TEntity t)
        {
            throw new NotSupportedException();
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            throw new NotSupportedException();
        }

        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return storage.Any(predicate.Compile());
        }

        

        

        public virtual TEntity GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> predicate)
        {
            throw new NotImplementedException();
        }

        public void ReloadEntity<TAnyEntity>(TAnyEntity entity) where TAnyEntity : class
        {
            throw new NotSupportedException();
        }

        public void ReloadNavigationProperty<TElement>(TEntity entity, Expression<Func<TEntity, ICollection<TElement>>> navigationProperty) where TElement : class
        {
            throw new NotSupportedException();
        }

        public void Remove(TEntity t)
        {
            throw new NotSupportedException();
        }

        public void Remove(int id)
        {
            throw new NotSupportedException();
        }

        public void RemoveRange(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotSupportedException();
        }

        public void SaveChanges()
        {
            throw new NotSupportedException();
        }

        public void Update(TEntity t)
        {
            throw new NotSupportedException();
        }

        public void UpdateSingleField<TProp>(Expression<Func<TEntity, TProp>> expression, Action<TEntity> setUnique, TProp value)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            throw new NotImplementedException();
        }

        public IOrderedQueryable<TEntity> OrderByDescending<TValue>(Expression<Func<TEntity, TValue>> keySelector)
        {
            throw new NotImplementedException();
        }

        public IOrderedQueryable<TEntity> OrderBy<TValue>(Expression<Func<TEntity, TValue>> keySelector)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TSelect> Apply<TSelect>(IDomSelector<TEntity, TSelect> selector)
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public void RemoveSpecific<TSpecific>(TSpecific entity) where TSpecific : class, new()
        {
            throw new NotImplementedException();
        }

        public TEntity GetById(long id)
        {
            return GetById((int)id);
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public TEntity SingleOrDefault()
        {
            throw new NotImplementedException();
        }

        public TEntity Single()
        {
            throw new NotImplementedException();
        }

        public void SetTimeout(int seconds)
        {
            throw new NotImplementedException();
        }

        public void CreateMany<TOther>(IEnumerable<TOther> collection, Action<TOther, TEntity> SetUnique, params ISingleChangeExpression<TEntity>[] expressions)
        {
            throw new NotImplementedException();
        }

 
        public void UpdateMany<TOther>(IEnumerable<TOther> collection, Action<TOther, TEntity> SetUnique, params Func<TOther, ISingleChangeExpression<TEntity>>[] expressions)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            // :-)
        }

        public void Remove(long id)
        {
            throw new NotImplementedException();
        }

        public void Remove(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public TEntity FirstOrDefault()
        {
            throw new NotImplementedException();
        }

        public bool TryRemove(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<IGrouping<TKey, TEntity>> GroupBy<TKey>(Expression<Func<TEntity, TKey>> keySelector)
        {
            throw new NotImplementedException();
        }

        public void ReloadNavigationProperty<TElement>(TEntity entity, Expression<Func<TEntity, TElement>> navigationProperty) where TElement : class
        {
            throw new NotImplementedException();
        }

        public void Remove<TProp>(Action<TEntity> setUnique)
        {
            throw new NotImplementedException();
        }

        public void Remove<TSpecificEntity>(Action<TSpecificEntity> setUnique) where TSpecificEntity : class, new()
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetByIdAsync(int id)
        {
            return Task.FromResult(GetById(id));
        }

        public Task<TEntity> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> FirstOrDefaultAsync()
        {
            throw new NotImplementedException();
        }
    }
}
