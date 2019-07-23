using Entities;
using Sociatis.Controllers;
using Sociatis.Models.Entit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebUtils;
using WebUtils.Extensions;
using WebUtils.Forms.Select2;

namespace Sociatis.Models.Messages
{
    public class ViewThreadViewModel
    {
        public int ThreadID { get; set; }
        public string ThreadName { get; set; }
        public PagingParam Paging { get; set; } = new PagingParam();
        public List<ViewMessageViewModel> Messages { get; set; } = new List<ViewMessageViewModel>();
        public List<SmallEntityAvatarViewModel> Recipients { get; set; } = new List<SmallEntityAvatarViewModel>();
        public Select2AjaxViewModel AddRecipientSelector { get; set; }

        public ViewThreadViewModel()
        {
            AddRecipientSelector = Select2AjaxViewModel.Create<MessageController>(c => c.GetEligibleRecipients(null), "RecipientID", null, string.Empty);
        }
        
        public ViewThreadViewModel(MessageThread thread, int pageSize, int page = 0)
        {
            InitPagingParams(thread, pageSize, page);

            Initialize(thread, pageSize);
        }

        public void Initialize(MessageThread thread, int pageSize)
        {
            ThreadName = thread.Title;
            ThreadID = thread.ID;

            thread.Messages
            .OrderBy(m => m.Date)
            .Apply(Paging)
            .ToList().
            ForEach(m => Messages.Add(new ViewMessageViewModel(m)));

            foreach(var recipient in thread.Recipients)
            {
                Recipients.Add(new SmallEntityAvatarViewModel(recipient));
            }
        }

        public void InitPagingParams(MessageThread thread, int pageSize, int page)
        {
            Paging.PageNumber = page;
            Paging.RecordsPerPage = pageSize;
        }
    }
}