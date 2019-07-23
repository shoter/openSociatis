using Entities;
using Entities.enums;
using Sociatis.Models.Entit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.DevIssues
{
    public class DevIssueCommentViewModel
    {
        public SmallEntityAvatarViewModel Avatar { get; set; }
        public int Day { get; set; }
        public string Hour { get; set; }
        public string Content { get; set; }
        public string Visibility { get; set; }

        public DevIssueCommentViewModel(DevIssueComment comment)
        {
            Avatar = new SmallEntityAvatarViewModel(comment.Citizen.Entity);
            Day = comment.Day;
            Hour = comment.Time.ToShortTimeString();
            Content = comment.Content;
            Visibility = ((VisibilityOptionEnum)comment.VisibilityOptionID).ToString();
        }
    }
}