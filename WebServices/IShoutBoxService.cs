using Common.Operations;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface IShoutBoxService
    {
        ShoutboxMessage SendMessage(string message, Entity author, ShoutboxChannel channel);
        ShoutboxMessage SendMessage(string message, Entity author, ShoutboxMessage parent);

        MethodResult CanSendMessage(string content, ShoutboxChannel channel, Entity author);

        /// <summary>
        /// Create national and global channels if they do not exist
        /// </summary>
        void AutoCreateChannels();
        void MergeSameNameChannels();
    }
}
