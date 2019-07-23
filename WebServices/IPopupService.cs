using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.enums;
using WebServices.Models;

namespace WebServices
{
    public interface IPopupService
    {
        List<PopupMessageViewModel> Popups { get; }

        void AddMessage(PopupMessageViewModel msg);
        void AddMessage(string content, PopupMessageType msgType);
        void AddInfo(string content);
        void AddSuccess(string content);
    }
}
