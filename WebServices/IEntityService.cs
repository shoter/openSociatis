using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface IEntityService
    {
        Entity CreateEntity(string name, EntityTypeEnum entityType);
        bool CanChangeInto(Entity currentEntity, Entity desiredEntity, Citizen loggedCitizen);
        bool Exists(string name);
        bool IsNameTaken(string name);
        bool IsSpecialName(string name);
    }
}
