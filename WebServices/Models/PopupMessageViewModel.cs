using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.enums;

namespace WebServices.Models
{
    public class PopupMessageViewModel
    {
        public string Content { get; set; }
        public PopupMessageType MessageType { get; set; }

        public PopupMessageViewModel(string content, PopupMessageType messageType = PopupMessageType.Info)
        {
            Content = content;
            MessageType = messageType;
        }

        public PopupMessageViewModel() { }
    }
}
