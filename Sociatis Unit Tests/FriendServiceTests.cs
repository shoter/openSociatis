using Entities;
using Entities.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sociatis_Test_Suite;
using Sociatis_Test_Suite.Dummies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;
using WebServices.Models;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class FriendServiceTests
    {
        FriendService friendService;
        Mock<IFriendRepository> friendRepository = new Mock<IFriendRepository>();
        Mock<IWarningService> warningService = new Mock<IWarningService>();
        Mock<IPopupService> popupService = new Mock<IPopupService>();
        public FriendServiceTests()
        {
            SingletonInit.Init();
            friendService = new FriendService(friendRepository.Object, warningService.Object, popupService.Object);
        }
        [TestMethod]
        public void CanAcceptReuqestTest()
        {
            var request = new FriendRequestDummyCreator().Create();
            var someRandomCitizen = new CitizenDummyCreator().Create();


            Assert.IsTrue(friendService.CanAcceptReuqest(request.SecondCitizen, request).isSuccess);
            Assert.IsFalse(friendService.CanAcceptReuqest(request.ProposerCitizen, request).isSuccess);


            Assert.IsFalse(friendService.CanAcceptReuqest(someRandomCitizen, request).isSuccess);
        }

        [TestMethod]
        public void CanDeclineRequestTest()
        {
            var request = new FriendRequestDummyCreator().Create();
            var someRandomCitizen = new CitizenDummyCreator().Create();


            Assert.IsTrue(friendService.CanDeclineRequest(request.SecondCitizen, request).isSuccess);
            Assert.IsFalse(friendService.CanDeclineRequest(request.ProposerCitizen, request).isSuccess);


            Assert.IsFalse(friendService.CanDeclineRequest(someRandomCitizen, request).isSuccess);
        }

        [TestMethod]
        public void CanRemoveRequestTest()
        {
            var request = new FriendRequestDummyCreator().Create();
            var someRandomCitizen = new CitizenDummyCreator().Create();


            Assert.IsFalse(friendService.CanRemoveRequest(request.SecondCitizen, request).isSuccess);
            Assert.IsTrue(friendService.CanRemoveRequest(request.ProposerCitizen, request).isSuccess);


            Assert.IsFalse(friendService.CanRemoveRequest(someRandomCitizen, request).isSuccess);
        }

        [TestMethod]
        public void AcceptFriendRequestTest()
        {
            var request = new FriendRequestDummyCreator().Create();

            friendService.AcceptRequest(request);

            friendRepository.Verify(m => m.Add(It.IsAny<Friend>()), Times.Once);
            friendRepository.Verify(m => m.RemoveFriendshipRequest(It.Is<FriendRequest>(r => r == request)), Times.Once);
            warningService.Verify(m => m.AddWarning(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void DeclineFriendRequestTest()
        {
            var request = new FriendRequestDummyCreator().Create();

            friendService.DeclineRequest(request);

            friendRepository.Verify(m => m.Add(It.IsAny<Friend>()), Times.Never);
            friendRepository.Verify(m => m.RemoveFriendshipRequest(It.Is<FriendRequest>(r => r == request)), Times.Once);
            warningService.Verify(m => m.AddWarning(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void RemoveFriendRequestTest()
        {
            var request = new FriendRequestDummyCreator().Create();

            friendService.RemoveRequest(request);

            friendRepository.Verify(m => m.Add(It.IsAny<Friend>()), Times.Never);
            friendRepository.Verify(m => m.RemoveFriendshipRequest(It.Is<FriendRequest>(r => r == request)), Times.Once);
            warningService.Verify(m => m.AddWarning(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }


    }
}
