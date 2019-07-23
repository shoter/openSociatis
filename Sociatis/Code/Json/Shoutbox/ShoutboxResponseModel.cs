using Entities;
using Sociatis.Models.Shoutbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Code.Json.Shoutbox
{
    public class ShoutboxResponseModel : JsonSuccessModel
    {
        public List<ShoutboxMessageViewModel> Messages { get; set; }

        public ShoutboxResponseModel(IEnumerable<ShoutboxMessage> messages)
        {
            Messages = messages.Select(msg => new ShoutboxMessageViewModel(msg)).ToList();
        }
    }
}