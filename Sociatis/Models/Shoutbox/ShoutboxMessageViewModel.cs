using Common.utilities;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Shoutbox
{
    public class ShoutboxMessageViewModel
    {
        public long MessageID { get; set; }
        public int AuthorID { get; set; }
        public string AuthorImageURL { get; set; }
        public string AuthorName { get; set; }
        public string Message { get; set; }
        public int Day { get; set; }
        public long TimeUTCMilliseconds { get; set; }

        public ShoutboxMessageViewModel(ShoutboxMessage message)
        {
            MessageID = message.ID;
            AuthorID = message.AuthorID;
            AuthorImageURL = message.Author.ImgUrl;
            AuthorName = message.Author.Name;
            Message = message.Message;
            Day = message.Day;
            TimeUTCMilliseconds = new JavascriptDate(message.Time).JavascriptMilliseconds;
        }
    }
}