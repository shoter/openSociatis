using Sociatis.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebUtils.Forms.Select2;

namespace Sociatis.Models.Messages
{
    public class SendMessageViewModel
    {
        [StringLength(40, MinimumLength = 1)]
        [Required]
        public string Title { get; set; }
        [StringLength(5000)]
        [Required]
        public string Content { get; set; }
        public string RecipientName { get; set; }
        public int? RecipientID { get; set; }

        public Select2AjaxViewModel RecipientSelector { get; set; }

        public SendMessageViewModel()
        {
            RecipientSelector = Select2AjaxViewModel.Create<MessageController>(c => c.GetEligibleRecipients(null), "RecipientID", RecipientID, RecipientName);
            RecipientSelector.ID = "RecipientID";
        }



    }
}