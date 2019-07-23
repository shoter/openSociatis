using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebUtils;
using WebUtils.Extensions;

namespace Sociatis.Models.Messages
{
    public class MessageIndexViewModel
    {
        public List<MessageIndexItemViewModel> Messages { get; set; } = new List<MessageIndexItemViewModel>();
        public PagingParam PagingParams { get; set; } = new PagingParam();

        public MessageIndexViewModel(IQueryable<MailboxMessage> threads)
        {

            var list = threads.OrderByDescending(t => t.MessageThreads1_ID)
                .Apply(PagingParams)
                .Select(mb => new
                {
                    Unread = mb.Unread,
                    Thread = mb.MessageThread
                }).ToList();

            foreach(var item in list)
            {
                var vm = new MessageIndexItemViewModel(item.Thread, item.Unread);
                Messages.Add(vm);
            }
        }
    }
}
