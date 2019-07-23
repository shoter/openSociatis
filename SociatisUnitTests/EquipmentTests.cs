using Entities.enums;
using Entities.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Sociatis_Test_Suite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SociatisUnitTests
{
    [TestClass]
    public class EquipmentTests
    {
        public EquipmentTests()
        {
            SingletonInit.Init();
        }
        [TestMethod]
        public void TestAddingSimpleItem()
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(1)))
            {
                IEquipmentRepository repository = Ninject.Current.Get<IEquipmentRepository>();

                var entity = Factory.CreateTemporaryEntity();

                var equipment = entity.Equipment;

                repository.AddEquipmentItem(equipment.ID, (int)ProductTypeEnum.Bread, 3, 25);
                repository.SaveChanges();
                var item = repository.GetEquipmentItem(equipment.ID, (int)ProductTypeEnum.Bread, 3);

                Assert.IsTrue(item != null);
                Assert.IsTrue(item.Quality == 3);
                Assert.IsTrue(item.ProductID == (int)ProductTypeEnum.Bread);

                var items = repository.GetEquipmentItems(equipment.ID, (int)ProductTypeEnum.Bread);

                Assert.IsTrue(items.Count == 1);
                Assert.IsTrue(items[0].Quality == 3);
                Assert.IsTrue(items[0].ProductID == (int)ProductTypeEnum.Bread);
            }
        }

        [TestMethod]
        public void TestRemoveItem()
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(1)))
            {
                IEquipmentRepository repository = Ninject.Current.Get<IEquipmentRepository>();

                var entity = Factory.CreateTemporaryEntity();

                var equipment = entity.Equipment;

                repository.AddEquipmentItem(equipment.ID, (int)ProductTypeEnum.Bread, 3, 25);

                repository.RemoveEquipmentItem(equipment.ID, (int)ProductTypeEnum.Bread, 3, 25);

                var item = repository.GetEquipmentItem(equipment.ID, (int)ProductTypeEnum.Bread, 3);

                Assert.IsTrue(item == null);

                var items = repository.GetEquipmentItems(equipment.ID, (int)ProductTypeEnum.Bread);

                Assert.IsTrue(items.Count == 0);
            }
        }

        [TestMethod]
        public void RemoveAmountItem()
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(1)))
            {
                IEquipmentRepository repository = Ninject.Current.Get<IEquipmentRepository>();

                var entity = Factory.CreateTemporaryEntity();

                var equipment = entity.Equipment;

                repository.AddEquipmentItem(equipment.ID, (int)ProductTypeEnum.Bread, 3, 25);

                repository.RemoveEquipmentItem(equipment.ID, (int)ProductTypeEnum.Bread, 3, 20);

                var item = repository.GetEquipmentItem(equipment.ID, (int)ProductTypeEnum.Bread, 3);

                Assert.IsTrue(item.Amount == 5);

                var items = repository.GetEquipmentItems(equipment.ID, (int)ProductTypeEnum.Bread);

                Assert.IsTrue(items.Count == 1);
                Assert.IsTrue(items[0].Amount == 5);
            }
        }

    }
}
