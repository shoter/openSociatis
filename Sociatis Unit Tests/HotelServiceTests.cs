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
    public class HotelServiceTests
    {
        private Mock<HotelService> mockHotelService;
        private HotelService hotelService => mockHotelService.Object;

        private HotelDummyCreator hotelDummyCreator = new HotelDummyCreator();
        private CitizenDummyCreator citizenDummyCreator = new CitizenDummyCreator();
        public HotelServiceTests()
        {
            mockHotelService = new Mock<HotelService>(Mock.Of<IEntityService>(), Mock.Of<IEntityRepository>());
            mockHotelService.CallBase = true;
            SingletonInit.Init();
        }

        
    }
}
