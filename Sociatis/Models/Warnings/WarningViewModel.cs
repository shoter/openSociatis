using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Warnings
{
    public class WarningViewModel
    {
        public string Message { get; set; }
        public bool Unread { get; set; }
        public int Day { get; set; }
        public string Time { get; set; }
        public int WarningID { get; set; }

        public WarningViewModel(Entities.Warning warning)
        {
            Message = warning.Message;
            Unread = warning.Unread;
            WarningID = warning.ID;

            Day = warning.Day;
            Time = warning.DateTime.ToShortTimeString();
        }
    }
}