using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.enums
{
    public enum PopupMessageType
    {
        Info,
        Warning,
        Error,
        Success
    }

    public static class PopupMessageTypeExtension
    {
        /// <summary>
        /// returns CSS class associated with this message type.
        /// </summary>
        /// <param name="message">message type</param>
        /// <returns>CSS class</returns>
        public static string GetMessageClass(this PopupMessageType message)
        {
            switch (message)
            {
                case PopupMessageType.Error:
                    return "error";
                case PopupMessageType.Info:
                    return "info";
                case PopupMessageType.Warning:
                    return "warning";
                case PopupMessageType.Success:
                    return "success";
            }
            return "";
        }

    }
}
