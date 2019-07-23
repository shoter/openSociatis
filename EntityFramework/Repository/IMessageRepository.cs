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
    public interface IMessageRepository : IRepository<Message>
    {
        IQueryable<MailboxMessage> MailboxMessages { get; }
        void Add(MessageThread thread);

        IQueryable<MessageThread> Where(Expression<Func<MessageThread, bool>> predicate);

        MessageThread GetThreadByID(int threadID);
    }
}
