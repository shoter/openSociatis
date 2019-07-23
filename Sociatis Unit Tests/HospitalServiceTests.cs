using Entities;
using Entities.enums;
using Entities.Models.Hospitals;
using Entities.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sociatis_Test_Suite.Dummies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class HospitalServiceTests
    {
        private Mock<HospitalService> mockHospitalService;
        private HospitalService hospitalService => mockHospitalService.Object;
        private Mock<IEquipmentService> equipmentService = new Mock<IEquipmentService>();
        private Mock<IHospitalRepository> hospitalRepository = new Mock<IHospitalRepository>();
        private Mock<ITransactionsService> transactionsService = new Mock<ITransactionsService>();

        public HospitalServiceTests()
        {
            mockHospitalService = new Mock<HospitalService>(hospitalRepository.Object, Mock.Of<IWalletService>(),
                equipmentService.Object, Mock.Of<IReservedEntityNameRepository>(), Mock.Of<IRegionRepository>(), Mock.Of<ICompanyService>(),
                transactionsService.Object, Mock.Of<IConstructionRepository>());
            mockHospitalService.CallBase = true;
        }


        [TestMethod]
        public void HealCitizen_simple_test()
        {
            var hospital = new HospitalDummyCreator().Create();
            var citizen = new CitizenDummyCreator().Create();
            var healingPrice = new HealingPrice() { Cost = 12.34m, CurrencyID = 1 };
            hospitalRepository.Setup(x => x.GetHealingPrice(It.IsAny<int>(), It.IsAny<int>())).Returns(healingPrice);
            transactionsService.Setup(x => x.PayForHealing(It.IsAny<Hospital>(), It.IsAny<Citizen>(), It.IsAny<HealingPrice>()));
            equipmentService.Setup(x => x.RemoveProductsFromEquipment(It.IsAny<ProductTypeEnum>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Equipment>()));

            hospitalService.HealCitizen(citizen, hospital);


            transactionsService.Verify(x => x.PayForHealing(
                It.Is<Hospital>(h => h == hospital),
                It.Is<Citizen>(c => c == citizen),
                It.Is<HealingPrice>(h => h == healingPrice)), Times.Once);

            equipmentService.Verify(e => e.RemoveProductsFromEquipment(
                It.Is<ProductTypeEnum>(t => t == ProductTypeEnum.MedicalSupplies),
                It.Is<int>(amount => amount == 1),
                It.Is<int>(quality => quality == hospital.Company.Quality),
                It.Is<Equipment>(eq => eq == hospital.Company.Entity.Equipment)), Times.Once);
        }

        [TestMethod]
        public void HealCitizen_freeHealing_test()
        {
            var hospital = new HospitalDummyCreator().Create();
            var citizen = new CitizenDummyCreator().Create();
            var healingPrice = new HealingPrice() { Cost = null, CurrencyID = 1 };
            hospitalRepository.Setup(x => x.GetHealingPrice(It.IsAny<int>(), It.IsAny<int>())).Returns(healingPrice);

            hospitalService.HealCitizen(citizen, hospital);

            transactionsService.Verify(x => x.PayForHealing(
                It.IsAny<Hospital>(),
                It.IsAny<Citizen>(),
                It.IsAny<HealingPrice>()), Times.Never);
        }

        public void HealCitizen_quality_test()
        {
            mockHospitalService.Setup(x => x.GetHealAmount(It.IsAny<int>())).Returns(10);
            var citizen = new CitizenDummyCreator().Create();
            citizen.HitPoints = 50;

            hospitalService.HealCitizenProcess(citizen, 1234);

            mockHospitalService.Verify(x => x.GetHealAmount(It.Is<int>(q => q == 1234)), Times.Once);

            Assert.AreEqual(60, citizen.HitPoints);
            Assert.IsTrue(citizen.UsedHospital);
        }

        public void HealCitizen_overflow_test()
        {
            mockHospitalService.Setup(x => x.GetHealAmount(It.IsAny<int>())).Returns(10);
            var citizen = new CitizenDummyCreator().Create();
            citizen.HitPoints = 99;

            hospitalService.HealCitizenProcess(citizen, 1234);

            mockHospitalService.Verify(x => x.GetHealAmount(It.Is<int>(q => q == 1234)), Times.Once);

            Assert.AreEqual(100, citizen.HitPoints);
            Assert.IsTrue(citizen.UsedHospital);
        }
    }
}
