using Common.utilities;
using Entities;
using HeyRed.MarkdownSharp;
using Sociatis.Models.Entit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Helpers;

namespace Sociatis.Models.Messages
{
    public class ViewMessageViewModel
    {
        public SmallEntityAvatarViewModel AuthorAvatar { get; set; }
        public string Content { get; set; }
        public string Date { get; set; }
        public string DateTooltip { get; set; }

        public ViewMessageViewModel() { }

        public ViewMessageViewModel(Message message)
        {
            Content = new Markdown().Transform(message.Content);
            AuthorAvatar = new SmallEntityAvatarViewModel(message?.AuthorID, message?.Author?.Name, message?.Author?.ImgUrl);
            DateTooltip = string.Format("Day {0} {1}", message.Day, message.Date.ToString("HH:mm"));
            Date = AgoHelper.Ago(GameHelper.CurrentDay, DateTime.Now, message.Day, message.Date);
        }
    }
}
