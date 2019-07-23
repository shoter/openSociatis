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

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class ShoutBoxServiceTests
    {
        private IShoutBoxService shoutBoxService;
        private Mock<IShoutboxMessageRepository> shoutboxMessageRepository = new Mock<IShoutboxMessageRepository>();

        public ShoutBoxServiceTests()
        {
            shoutBoxService = new ShoutBoxService(shoutboxMessageRepository.Object, Mock.Of<ICountryRepository>(), Mock.Of<IShoutboxChannelRepository>(), Mock.Of<IConfigurationRepository>());
            SingletonInit.Init();
        }

        [TestMethod]
        public void AddMessageWithChannelTest()
        {
            string message = "Test 123";
            var author = new EntityDummyCreator().Create();
            var channel = new ShoutboxChannelDummyCreator().SetName("Test Channel").Create();


            shoutBoxService.SendMessage(message, author, channel);

            shoutboxMessageRepository.Verify(x => x.SaveChanges(), Times.Once());
            shoutboxMessageRepository.Verify(x => x.Add(It.Is<ShoutboxMessage>(msg => 
            msg.Message == message &&
            msg.AuthorID == author.EntityID &&
            msg.ChannelID == channel.ID   
            )));
        }

        [TestMethod]
        public void AddMessageWithParentTest()
        {
            string message = "Test 123";
            var author = new EntityDummyCreator().Create();
            var parentAuthor = new EntityDummyCreator().Create();
            var channel = new ShoutboxChannelDummyCreator().SetName("Test Channel").Create();

            var parent = new ShoutboxMessageDummyCreator().SetAuthor(parentAuthor).Create(channel);


            shoutBoxService.SendMessage(message, author, parent);

            shoutboxMessageRepository.Verify(x => x.SaveChanges(), Times.Once());
            shoutboxMessageRepository.Verify(x => x.Add(It.Is<ShoutboxMessage>(msg =>
            msg.Message == message &&
            msg.AuthorID == author.EntityID &&
            msg.ChannelID == channel.ID &&
            msg.ParentID == parent.ID
            )));
        }

        [TestMethod]
        public void CanSendMessageNoChannelTest()
        {
            string message = "DUPA";
            var author = new EntityDummyCreator().Create();

            var result = shoutBoxService.CanSendMessage(message, null, author);

            Assert.IsFalse(result.isSuccess);
        }

        [TestMethod]
        public void CanSendMessageNoAuthorTest()
        {
            string message = "DUPA";
            var channel = new ShoutboxChannelDummyCreator().Create();

            var result = shoutBoxService.CanSendMessage(message, channel, null);

            Assert.IsFalse(result.isSuccess);
        }

        [TestMethod]
        public void CanSendMessageEmptyMessageTest()
        {
            string message = "              ";
            var author = new EntityDummyCreator().Create();
            var channel = new ShoutboxChannelDummyCreator().Create();

            var result = shoutBoxService.CanSendMessage(message, channel, author);

            Assert.IsFalse(result.isSuccess);
        }

        [TestMethod]
        public void CanSendMessageTest()
        {
            string message = "message";
            var author = new EntityDummyCreator().Create();
            var channel = new ShoutboxChannelDummyCreator().Create();

            var result = shoutBoxService.CanSendMessage(message, channel, author);

            Assert.IsTrue(result.isSuccess);
        }
    }
}
