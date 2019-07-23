using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices.Models;

namespace Sociatis.Helpers
{
    public static class TempDataHelper
    {
        /// <summary>
        /// It gets messageList from tempData. It creates new empty list if it is not found in TempData
        /// </summary>
        /// <param name="tempData"></param>
        /// <returns></returns>
        public static List<PopupMessageViewModel> GetMessages(this TempDataDictionary tempData)
        {
            object messages = tempData["Messages"];
            if(messages != null)
            {
                return messages as List<PopupMessageViewModel>;
            }
            var messageList = new  List<PopupMessageViewModel>();
            tempData["Messages"] = messageList;
            return messageList;
        }



        /// <summary>
        /// Add message which will be displayed after next HTTP Request
        /// You can add multiple messages which will be stacked on the page
        /// </summary>
        /// <param name="message">message to display.</param>
        ///
        public static void AddMessage(this TempDataDictionary tempData, PopupMessageViewModel message)
        {
            var messages = GetMessages(tempData);
            messages.Add(message);
        }
    }
}