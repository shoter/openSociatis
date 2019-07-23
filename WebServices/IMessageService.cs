using Entities;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.BigParams.messages;

namespace WebServices
{
    public interface IMessageService
    {
        MessageThread CreateNewThread(List<int> recipients, string threadName);

        Message SendMessage(SendMessageParams pars);

        bool CanReadMessage(MessageThread thread, int entityID);

        void MarkAsRead(int messageID, int entityID);

        bool IsMemberOfThread(Entity entity, MessageThread thread);

        void AddRecipient(Entity entity, MessageThread thread);

    }
}
