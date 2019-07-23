using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Code.Json.Shoutbox
{
    public class ShoutboxRequestJson
    {
        public int ChannelID { get; set; }
        public int? LastMessageID { get; set; }
        public int PageSize { get; set; }
    }
}