using Entities;
using Entities.enums;
using Entities.Repository;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateSociatisWorld.Creators
{
    /// <summary>
    /// It will not save changes to database. It will be used by other creators as way to fill in needed gaps for entity.
    /// </summary>
    public class EntityCreator
    {
        IEntityRepository entityRepository;
        Entity entity;

        public EntityCreator()
        {
            entityRepository = Ninject.Current.Get<IEntityRepository>();
            create();
        }

        public EntityCreator Set(string name, EntityTypeEnum entityType)
        {
            entity.Name = name;
            entity.EntityTypeID = (int)entityType;

            return this;
        }

        public Entity Get()
        {
            var _return = entity;
            create();
            return _return;
        }

        private void create()
        {
            entity = new Entity();
        }

    }
}
