using Entities;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models
{
    public class BaseEntitySummaryViewModel
    {
        public string SwitchBackEntityName { get; set; }

        public BaseEntitySummaryViewModel(Session session)
        {
            if (SessionHelper.SwitchStack.Count == 1)
                SwitchBackEntityName = "";
            else
                SwitchBackEntityName = SessionHelper.SwitchStack.ElementAt(1).Name;
        }
    }
}