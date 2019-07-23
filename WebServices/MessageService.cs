using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using WebServices.BigParams.messages;
using WebServices.Helpers;
using System.Data.Entity;

namespace WebServices
{
    public class MessageService : IMessageService
    {
        private IMessageRepository messageRepository;
        private IEntityRepository entityRepository;

        public MessageService(IMessageRepository messageRepository, IEntityRepository entityRepository)
        {
            this.messageRepository = messageRepository;
            this.entityRepository = entityRepository;

            
        }

        public void AddRecipient(Entity entity, MessageThread thread)
        {
            thread.Recipients.Add(entity);
            messageRepository.SaveChanges();
        }

        public bool IsMemberOfThread(Entity entity, MessageThread thread)
        {
            return thread.Recipients.Any(r => r.EntityID == entity.EntityID);
        }

        public MessageThread CreateNewThread(List<int> recipients, string threadName)
        {
            var entities = entityRepository.Where(e => recipients.Contains(e.EntityID)).ToList();

            List<MailboxMessage> messages = new List<MailboxMessage>();

            foreach(var entity in entities)
            {
                messages.Add(new MailboxMessage()
                {
                    Viewers_EntityID = entity.EntityID,
                    Unread = true
                });
            }

            var thread = new MessageThread()
            {
                Recipients = entities,
                MailboxMessages = messages,
                Title = threadName
            };

            messageRepository.Add(thread);
            messageRepository.SaveChanges();

            return thread;
        }

        public void MarkAsRead(int messageID, int entityID)
        {
            var messageRepository = new MessageRepository(new SociatisEntities());

            var message = messageRepository.MailboxMessages
                .Where(mb => mb.MessageThreads1_ID == messageID && mb.Viewers_EntityID == entityID)
                .ToList().FirstOrDefault();

            if (message != null)
            {
                message.Unread = false;
                messageRepository.SaveChanges();
            }


        }

        public Message SendMessage(SendMessageParams pars)
        {
            var message = new Message()
            {
                AuthorID = pars.AuthorID,
                Content = pars.Content,
                Date = pars.Date,
                Day = pars.Day,
                ThreadID = pars.ThreadID
            };

            messageRepository.Add(message);
            messageRepository.SaveChanges();

            return message;
        }

        public bool CanReadMessage(MessageThread thread, int entityID)
        {
            if (entityRepository.GetById(entityID) == null)
                return false;

            if (thread.Recipients.Any(e => e.EntityID == entityID))
                return true;

            return false;
        }
    }
}
