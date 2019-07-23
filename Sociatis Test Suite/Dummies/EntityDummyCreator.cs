using Common.utilities;
using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Test_Suite.Dummies
{
    public class EntityDummyCreator : IDummyCreator<Entity>
    {
        protected Entity entity;
        static protected UniqueIDGenerator idGenerator = new UniqueIDGenerator();

        public EntityDummyCreator()
        {
            createEntity(EntityTypeEnum.Citizen);
        }

        protected void createEntity(EntityTypeEnum entityType)
        {
            entity = new Entity();
            entity.EntityTypeID = (int)entityType;
            entity.EntityID = idGenerator.UniqueID;

            SetRandomName();
        }

        public void SetName(string name)
        {
            entity.Name = name;
        }

        public void SetRandomName()
        {
            SetName(RandomGenerator.GenerateString(10));
        }


        public Entity Create()
        {
            var _return = entity;
            createEntity((EntityTypeEnum)entity.EntityTypeID);
            return _return;
        }

    }
}
