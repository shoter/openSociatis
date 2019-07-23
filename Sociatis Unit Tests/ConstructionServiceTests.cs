
using Entities.enums;
using Entities.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class ConstructionServiceTests
    {
        private ConstructionService constructionService => mockConstructionService.Object;
        private Mock<ConstructionService> mockConstructionService;

        public ConstructionServiceTests()
        {
            mockConstructionService = new Mock<ConstructionService>(Mock.Of<IDefenseSystemService>(),
                Mock.Of<IConstructionRepository>(),
                Mock.Of<ICompanyService>());
        }


    }
}
