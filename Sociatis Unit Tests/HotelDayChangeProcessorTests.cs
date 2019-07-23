using Entities;
using Entities.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sociatis_Test_Suite;
using Sociatis_Test_Suite.Dummies;
using Sociatis_Test_Suite.Dummies.Hotels;
using Sociatis_Test_Suite.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;
using WebServices.Helpers;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class HotelDayChangeProcessorTests
    {
        private HotelDayChangeProcessor processor => mockProcessor.Object;
        private readonly Mock<HotelDayChangeProcessor> mockProcessor;

        private readonly HotelDummyCreator hotelDummyCreator;
        private readonly CitizenDummyCreator citizenDummyCreator;

        private Mock<IEquipmentService> equipmentService = new Mock<IEquipmentService>();
        private Mock<IEquipmentRepository> equipmentRepository = new Mock<IEquipmentRepository>();

        public HotelDayChangeProcessorTests()
        {
            mockProcessor = new Mock<HotelDayChangeProcessor>(equipmentService.Object, equipmentRepository.Object);
            mockProcessor.CallBase = true;

            SingletonInit.Init();

            hotelDummyCreator = new HotelDummyCreator();
            citizenDummyCreator = new CitizenDummyCreator();
        }

        [TestMethod]
        public void CalculateConditionModifier_goodRange_test()
        {
            for (decimal d = 0m; d < 100m; d += 0.01m)
            {
                var modifier = processor.CalculateConditionModifier(d);
                Assert.IsTrue(modifier >= 0);
                Assert.IsTrue(modifier <= 1);
            }
        }

        [TestMethod]
        public void DecayHotelRoom_UnusedRoom_DecayUsed()
        {
            mockProcessor.Setup(x => x.CalculateRoomDecay(1, It.IsAny<bool>())).Returns(1m);

            var hotel = hotelDummyCreator.Create();
            var room = hotel.HotelRooms.First();
            room.Quality = 1;


            processor.DecayHotelRoom(hotel, room);

            mockProcessor.Verify(x => x.CalculateRoomDecay(
                1,
                false), Times.Once);
            Assert.AreEqual(99m, hotel.Condition);
        }

        [TestMethod]
        public void ProcessSleepInRoom_InhabitantInside_IsHealed()
        {
            mockProcessor.Setup(x => x.ProcessSleepInRoom(It.IsAny<Hotel>(), It.IsAny<HotelRoom>()));

            var hotel = hotelDummyCreator.Create();
            var room = hotel.HotelRooms.First();

            room.Quality = 1;
            room.SetInhabitant(citizenDummyCreator.Create());

            processor.ProcessSleepInRoom(hotel, room);

            mockProcessor.Verify(x => x.ProcessSleepInRoom(hotel, room), Times.Once);
        }

        [TestMethod]
        public void DecayHotelRoom_TryToMakeLessThan0Condition_ConditionHasMinimumValueOf0()
        {
            mockProcessor.Setup(x => x.CalculateRoomDecay(1, It.IsAny<bool>())).Returns(12345m);

            var hotel = hotelDummyCreator.Create();


            var room = hotel.HotelRooms.First();

            room.Quality = 1;
            room.SetInhabitant(citizenDummyCreator.Create());

            processor.DecayHotelRoom(hotel, room);

            Assert.AreEqual(0, hotel.Condition);
        }

        [TestMethod]
        public void UseConstructionMaterialsOnHotel_NormalValue_HotelHealed()
        {
            var hotel = hotelDummyCreator.Create();
            hotel.Condition = 85m;
            processor.UseConstructionMaterialsOnHotel(hotel, 10);

            Assert.AreEqual(95m, hotel.Condition);
        }

        [TestMethod]
        public void UseConstructionMaterialsOnHotel_BigValue_ConditionEqualTo100()
        {
            var hotel = hotelDummyCreator.Create();
            hotel.Condition = 85m;
            processor.UseConstructionMaterialsOnHotel(hotel, 12345);

            Assert.AreEqual(100m, hotel.Condition);
        }

        [TestMethod]
        public void HealInhabitant_UsingOuterFunctionAndHealing()
        {
            mockProcessor.Setup(x => x.GetHealedHP(1, It.IsAny<decimal>())).Returns(1);

            var hotel = hotelDummyCreator.Create();
            var room = hotel.HotelRooms.First();

            room.Quality = 1;
            room.SetInhabitant(citizenDummyCreator.Create());
            var citizen = room.Inhabitant;
            citizen.HitPoints = 90;

            processor.ProcessSleepInRoom(hotel, room);

            mockProcessor.Verify(x => x.GetHealedHP(1, It.IsAny<decimal>()), Times.Once);
            Assert.AreEqual(91, citizen.HitPoints);
        }



        [TestMethod]
        public void ClearRoomIfAble_Clear_IfCanClearRoom()
        {
            mockProcessor.Setup(x => x.CanClearRoom(It.IsAny<HotelRoom>())).Returns(true);
            mockProcessor.Setup(x => x.ClearRoom(It.IsAny<HotelRoom>()));
            var hotel = hotelDummyCreator.Create();
            var room = hotel.HotelRooms.First();

            processor.ClearRoomIfAble(room);

            mockProcessor.Verify(x => x.ClearRoom(It.IsAny<HotelRoom>()), Times.Once);
        }

        [TestMethod]
        public void GetHealedHP_IdealCondition_BonusHP()
        {
            for (int q = 1; q <= 5; ++q)
            {
                var notPerfect = processor.GetHealedHP(1, 90m);
                var perfect = processor.GetHealedHP(1, 100m);

                Assert.IsTrue(perfect > notPerfect);
            }
        }

        [TestMethod]
        public void ClearRoomIfAble_NoClear_IfCannotClearRoom()
        {
            mockProcessor.Setup(x => x.CanClearRoom(It.IsAny<HotelRoom>())).Returns(false);
            mockProcessor.Setup(x => x.ClearRoom(It.IsAny<HotelRoom>()));
            var hotel = hotelDummyCreator.Create();
            var room = hotel.HotelRooms.First();

            processor.ClearRoomIfAble(room);

            mockProcessor.Verify(x => x.ClearRoom(It.IsAny<HotelRoom>()), Times.Never);
        }

        [TestMethod]
        public void ClearRoomIfAble_NoTraceOfInhabitant()
        {
            var hotel = hotelDummyCreator.Create();
            var room = hotel.HotelRooms.First();
            room.SetInhabitant(citizenDummyCreator.Create());

            processor.ClearRoom(room);

            Assert.AreEqual(null, room.InhabitantID);
            Assert.AreEqual(null, room.StayingFromDay);
            Assert.AreEqual(null, room.StayingToDay);
        }

        [TestMethod]
        public void CalculateRoomDecay_NotUsed_LessDecay()
        {
            for (int i = 1; i <= 5; ++i)
            {
                decimal used = processor.CalculateRoomDecay(i, used: true);
                decimal notUsed = processor.CalculateRoomDecay(i, used: false);

                Assert.IsTrue(notUsed < used);
            }
        }
    }
}
