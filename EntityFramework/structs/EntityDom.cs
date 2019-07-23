using Entities.enums;
using Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.structs
{
    public class EntityDom
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public EntityTypeEnum EntityType { get; set; }


        public EntityDom(Entity entity)
        {
            ID = entity.EntityID;
            Name = entity.Name;
            EntityType = entity.GetEntityType();
        }
    }
}
