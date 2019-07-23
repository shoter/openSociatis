using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Entities.Repository;
using Moq;
using WebServices;
using Entities;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using WebServices.BigParams.messages;
using WebServices.Helpers;
using Sociatis_Test_Suite;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class MessageServiceTests
    {

        private Mock<IMessageRepository> messageRepository = new Mock<IMessageRepository>();
        private Mock<IEntityRepository> entityRepository = new Mock<IEntityRepository>();
        private MessageService messageService;
        public MessageServiceTests()
        {
            SingletonInit.Init();

            ICollection<Entity> recipients = new List<Entity>()
            {
                new Entity() { EntityID = 2 },
                new Entity() { EntityID = 3 },
                new Entity() { EntityID = 4 }
            };

            messageRepository.Setup(x => x.Where(It.IsAny<Expression<Func<MessageThread, bool>>>())).Returns<Expression<Func<MessageThread, bool>>>
                (r =>
               new List<MessageThread>()
               {
                    new MessageThread()
                    {
                        ID = 1,
                        Recipients = recipients
                    },
                    new MessageThread()
                    {
                        ID = 2,
                        Recipients = recipients
                    }
               }.AsQueryable()
                );

            entityRepository.Setup(x => x.GetById(It.Is<int>(num => num == 5))).Returns<int>(id => new Entity() { EntityID = 5 });
            entityRepository.Setup(x => x.GetById(It.Is<int>(num => num == 2))).Returns<int>(id => new Entity() { EntityID = 2 });
            entityRepository.Setup(x => x.GetById(It.Is<int>(num => num == 1))).Returns<int>(id => new Entity() { EntityID = 1 });

            messageService = new MessageService(messageRepository.Object, entityRepository.Object);
        }

        [TestMethod]
        public void CreateThreadTest()
        {
            string threadTitle = "Title";
            List<int> recipients = new List<int> { 1, 2, 3 };
            
            MessageThread thread = null;
            messageRepository.Setup(x => x.Add(It.IsAny<MessageThread>()))
                .Callback<MessageThread>(r => thread = r);


            entityRepository.Setup(x => x.Where(It.IsAny<Expression<Func<Entity, bool>>>())).Returns<Expression<Func<Entity, bool>>>
                (r =>
               new List<Entity>()
               {
                    new Entity() { EntityID = 1 },
                    new Entity() { EntityID = 2 },
                    new Entity() { EntityID = 3 }
               }.AsQueryable()
                );

            messageService.CreateNewThread(recipients, threadTitle);

            Assert.AreEqual(threadTitle, thread.Title);
            
            foreach(var recipient in thread.Recipients)
            {
                Assert.IsTrue(recipients.Contains(recipient.EntityID));
            }

            foreach (var viewer in thread.MailboxMessages)
            {
                Assert.IsTrue(recipients.Contains(viewer.Viewers_EntityID));
            }
        }
        
        [TestMethod]
        public void SendMessageTest()
        {
            Message message = null;
            messageRepository.Setup(x => x.Add(It.IsAny<Message>())).Callback<Message>(m => message = m);
            

           

            SendMessageParams param = new SendMessageParams()
            {
                AuthorID = 1,
                Content = "Siema Heniek",
                Date = DateTime.Now,
                ThreadID = 10,
                Day = GameHelper.CurrentDay
            };

            messageService.SendMessage(param);

            Assert.AreEqual(message.AuthorID, param.AuthorID);
            Assert.AreEqual(message.Content, param.Content);
            Assert.AreEqual(message.Date, param.Date);
            Assert.AreEqual(message.ThreadID, param.ThreadID);
            Assert.AreEqual(message.Day, GameHelper.CurrentDay);

        }

        [TestMethod]
        public void CanReadMessageTestNotExistingUser()
        {
            var thread = messageRepository.Object.Where(x => true).First();

            Assert.IsFalse(messageService.CanReadMessage(thread, 5));
            
        }

        [TestMethod]
        public void CanReadMessageTest()
        {
            var thread = messageRepository.Object.Where(x => true).First();

            Assert.IsTrue(messageService.CanReadMessage(thread, 2));
        }

        [TestMethod]
        public void CanReadMessageTestNoAccess()
        {
            var thread = messageRepository.Object.Where(x => true).First();

            Assert.IsFalse(messageService.CanReadMessage(thread, 1));
        }
    }
}
