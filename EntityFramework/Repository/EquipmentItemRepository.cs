using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class EquipmentItemRepository : RepositoryBase<EquipmentItem, SociatisEntities>, IEquipmentItemRepository
    {
        public EquipmentItemRepository(SociatisEntities context) : base(context)
        {
        }

        public IQueryable<EquipmentItem> GetItemsForEquipment(int equipmentID)
        {
            return Where(i => i.EquipmentID == equipmentID);
        }
    }
}
