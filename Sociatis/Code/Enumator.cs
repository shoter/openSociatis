using Common.EntityFramework;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Sociatis.Code
{
    public class Enumator<TEntity, TEnum>
        where TEntity: class,new()
    {
        private readonly IRepository<TEntity> repository;

        public Enumator(IGenericRepository repository)
        {
            this.repository = repository.GetRepository<TEntity>();
        }

        public void CreateNewIfAble<TName, TValue>(Action<TEntity, TName> setName, Action<TEntity, TValue> setValue,
            Func<TEnum, TName> getName, Func<TEnum, TValue> getValue,
            Func<TEntity, TEnum, bool> isTheSame)
        {
            var all = repository.GetAll();

            foreach (TEnum val in Enum.GetValues(typeof(TEnum)).Cast<TEnum>())
            {
                if (all.Any(entity => isTheSame(entity, val)))
                    continue;

                var name = getName(val);
                var value = getValue(val);

                TEntity newEntity = new TEntity();
                setName(newEntity, name);
                setValue(newEntity, value);

                repository.Add(newEntity);
            }

            repository.SaveChanges();
        }

        public void CreateNewIfAble()
        {
            var all = repository.GetAll();

            foreach (TEnum val in Enum.GetValues(typeof(TEnum)).Cast<TEnum>())
            {
                if (all.Any(entity => ((dynamic)entity).ID == (int)((dynamic)val)))
                    continue;

                string name = val.ToString();
                int value = (int)((dynamic)val);

                TEntity newEntity = new TEntity();
                ((dynamic)newEntity).Name = name;
                ((dynamic)newEntity).ID = value;

                repository.Add(newEntity);
            }

            repository.SaveChanges();
        }
    }
}