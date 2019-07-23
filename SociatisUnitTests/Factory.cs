using Entities;
using Entities.enums;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;

namespace SociatisUnitTests
{
    public static class Factory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns>Returns raw entity. Does not create any associated type with it like citizen</returns>
        public static Entity CreateTemporaryEntity(EntityTypeEnum entityType = EntityTypeEnum.Citizen)
        {
            IEntityService entityService = Ninject.Current.Get<IEntityService>();

            return entityService.CreateEntity("Test", entityType);
        }
    }
}
