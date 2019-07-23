using Common.EntityFramework;
using Entities.Items;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface IEquipmentRepository : IRepository<Equipment>
    {
        /// <summary>
        /// Returns equipmentItem from inventory
        /// </summary>
        /// <param name="equipmentID"></param>
        /// <param name="productID"></param>
        /// <param name="quality">quality of the product</param>
        /// <returns>Equipment if found in desired inventory. Null if not found</returns>
        /// <exception cref="ArgumentException">If equipment does not exists</exception>
        EquipmentItem GetEquipmentItem(int equipmentID, int productID, int quality);
        /// <summary>
        /// Returns list of items (with various quality) from equipment
        /// </summary>
        /// <param name="equipmentID"></param>
        /// <param name="productID"></param>
        /// <returns>list with desired items. List can be empty if products are not found</returns>
        /// <exception cref="ArgumentException">If equipment does not exists</exception>
        List<EquipmentItem> GetEquipmentItems(int equipmentID, int productID);

        /// <exception cref="ArgumentException">If equipment does not exists</exception>
        void AddEquipmentItem(int equipmentID, int productID, int quality, int amount = 1);

        /// <exception cref="ArgumentException">If equipment does not exists</exception>
        /// <exception cref="InvalidOperationException">If you try to remove more amount than there is in the DB</exception>
        void RemoveEquipmentItem(int equipmentID, int productID, int quality, int amount = 1);

        /// <exception cref="ArgumentException">If equipment does not exists</exception>
        /// <exception cref="InvalidOperationException">If you try to remove more amount than there is in the DB</exception>
        void RemoveEquipmentItem(int equipmentID, ItemBase item, int amount = 1);
        /// <summary>
        /// Returns null if item does not exist.
        /// </summary>
        /// <param name="itemID">equipment item ID to be found</param>
        /// <returns>item or null if not found</returns>
        EquipmentItem GetEquipmentItem(int itemID);
        /// <summary>
        /// Removes from database item with provided ID
        /// </summary>
        /// <param name="itemID">item ID that will be deleted</param>
        void RemoveEquipmentItem(int itemID);

        Equipment GetByEntityID(int entityID);
    }
}
