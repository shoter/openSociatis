using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using Entities.Items;
using Common.EntityFramework;

namespace Entities.Repository
{
    public class EquipmentRepository : RepositoryBase<Equipment, SociatisEntities>, IEquipmentRepository
    {
        public EquipmentRepository(SociatisEntities context) : base(context)
        {        }

        public List<EquipmentItem> GetEquipmentItems(int equipmentID, int productID)
        {
            var equipment = DbSet.Find(equipmentID);

            if (equipment == null)
                throw new ArgumentException();

            return equipment
                .EquipmentItems
                .Where(ei => ei.ProductID == productID)
                .ToList();
        }

        public EquipmentItem GetEquipmentItem(int equipmentID, int productID, int quality)
        {
            var equipment = DbSet.Find(equipmentID);

            if (equipment == null)
                throw new ArgumentException();

            return equipment
                .EquipmentItems
                .FirstOrDefault(ei => ei.ProductID == productID && ei.Quality == quality);
        }

        public EquipmentItem GetEquipmentItem(int itemID)
        {
            return context.EquipmentItems.FirstOrDefault(ei => ei.ID == itemID);
        }

        public void AddEquipmentItem(int equipmentID, int productID, int quality, int amount = 1)
        {
            Contract.Requires(amount >= 1);
            Contract.Requires(quality >= 1 && quality <= 5);

            var equipment = DbSet.Find(equipmentID);

            if (equipment == null)
                throw new ArgumentException();

            var equipmentItem = equipment.EquipmentItems
                .FirstOrDefault(ei => ei.Quality == quality && ei.ProductID == productID);

            if(equipmentItem != null)
            {
                equipmentItem.Amount += amount;
            }
            else
            {
                EquipmentItem item = new EquipmentItem()
                {
                    Amount = amount,
                    ProductID = productID,
                    Quality = quality,
                };

                equipment.EquipmentItems.Add(item);
            }           
        }

        public override Equipment GetById(int id)
        {
            return DbSet.FirstOrDefault(e => e.ID == id);
        }

        public Equipment GetByEntityID(int entityID)
        {
            return (from entity in context.Entities.Where(e => e.EntityID == entityID)
                    join equipment in context.Equipments on entity.EquipmentID equals equipment.ID
                    select equipment).FirstOrDefault();
        }

        public void RemoveEquipmentItem(int equipmentID, int productID, int quality, int amount = 1)
        {
            Contract.Requires(amount >= 1);
            Contract.Requires(quality >= 1 && quality <= 5);

            var equipment = DbSet.Find(equipmentID);

            if (equipment == null)
                throw new ArgumentException();

            var equipmentItem = equipment.EquipmentItems
                .First(ei => ei.Quality == quality && ei.ProductID == productID);

            equipmentItem.Amount -= amount;

            if (amount < 0)
                throw new InvalidOperationException("Amount is lower than 0!");
            else if(equipmentItem.Amount == 0)
            {
                context.EquipmentItems.Remove(equipmentItem);
            }
        }

        public void RemoveEquipmentItem(int equipmentID, ItemBase item, int amount = 1)
        {
            RemoveEquipmentItem(equipmentID, (int)item.ProductType, item.Quality, amount);
        }

        public void RemoveEquipmentItem(int itemID)
        {
            var item = GetEquipmentItem(itemID);
            context.EquipmentItems.Remove(item);
        }
    }
}
