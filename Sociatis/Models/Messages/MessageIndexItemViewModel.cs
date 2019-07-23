using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis.Models.Messages
{
    public class MessageIndexItemViewModel
    {
        public int ThreadID { get; set; }
        public string Title { get; set; }
        public int Recipients { get; set; }
        public int Replies { get; set; }
        public int ReplyDay { get; set; }
        public DateTime ReplyDate { get; set; }
        public bool Unread { get; set; }

        public MessageIndexItemViewModel() { }

        public MessageIndexItemViewModel(MessageThread thread, bool unread)
        {
            ThreadID = thread.ID;
            Title = thread.Title;
            Unread = unread;

            Replies = thread.Messages.Count;

            var lastMessage = thread.Messages.Last();

            ReplyDay = lastMessage.Day;
            ReplyDate = lastMessage.Date;

            Recipients = thread.Recipients.Count;
        }

    }
}
