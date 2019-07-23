using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class MessageRepository : RepositoryBase<Message, SociatisEntities>, IMessageRepository
    {
        public IQueryable<MailboxMessage> MailboxMessages => context.MailboxMessages.AsQueryable();
        public MessageRepository(SociatisEntities context) : base(context)
        {
        }

        public void Add(MessageThread thread)
        {
            context.Set<MessageThread>().Add(thread);
        }

        public MessageThread GetThreadByID(int threadID)
        {
            return context.Set<MessageThread>().First(t => t.ID == threadID);
        }

        IQueryable<MessageThread> IMessageRepository.Where(Expression<Func<MessageThread, bool>> predicate)
        {
            return context.MessageThreads.Where(predicate);
        }

        IQueryable<MessageThread> Where(Expression<Func<MessageThread, bool>> predicate)
        {
            return context.MessageThreads.Where(predicate);
        }
    }
}
