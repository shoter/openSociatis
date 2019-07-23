using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.DevIssues
{
    public class DevIssueLabelViewModel
    {
        public string Classname { get; set; }
        public string Text { get; set; }

        public DevIssueLabelViewModel(DevIssueLabelType label)
        {
            DevIssueLabelTypeEnum type = (DevIssueLabelTypeEnum)label.ID;
            Classname = GetClassname(type);
            Text = type.ToString();
        }

        public static string GetClassname(DevIssueLabelTypeEnum type)
        {
            switch (type)
            {
                case DevIssueLabelTypeEnum.Bug:
                    return "red";
                case DevIssueLabelTypeEnum.Feature:
                    return "green";
                case DevIssueLabelTypeEnum.Support:
                    return "blue";
                case DevIssueLabelTypeEnum.Resolved:
                    return "green";
                default:
                    return "blue";
            }
        }
    }
}