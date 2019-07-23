using Entities;
using Entities.enums;
using Entities.Extensions;
using Microsoft.Ajax.Utilities;
using Sociatis.Helpers;
using Sociatis.Models.Base;
using Sociatis.Models.Entit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices.Helpers;

namespace Sociatis.Models.DevIssues
{
    public class DevIssueViewModel : ViewModelBase
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public PlayerTypeEnum PlayerType { get; set; }
        public int Day { get; set; }
        public string Time { get; set; }
        public SmallEntityAvatarViewModel Avatar { get; set; }
        public int ID { get; set; }
        public List<DevIssueUploadedFileViewModel> Files { get; set; }
        public List<DevIssueCommentViewModel> Comments { get; set; }

        public List<SelectListItem> VisiblityOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> LabelTypes { get; set; } = new List<SelectListItem>();

        public string Score { get; set; }
        public bool CanVote { get; set; }
        public bool CanUnvote { get; set; }
        public string VoteClassname { get; set; }


        public List<DevIssueLabelViewModel> Labels { get; set; } = new List<DevIssueLabelViewModel>();

        public List<int> labelIDs { get; set; } = new List<int>();

        public DevIssueViewModel(DevIssue issue)
        {
            ID = issue.ID;
            Name = issue.Name;
            Content = issue.Content;

            PlayerType = SessionHelper.LoggedCitizen.GetPlayerType();
            Day = issue.Day;
            Time = issue.Time.ToShortTimeString();
            Avatar = new SmallEntityAvatarViewModel(issue.Citizen.Entity);
            Avatar.Classname = "avatar";

            Files = issue.DevIssueUploadedFiles.ToList()
                .Select(f => new DevIssueUploadedFileViewModel(f)).ToList();



            var actualLabels = issue.DevIssueLabelTypes.ToList();


            foreach (var label in Enum.GetValues(typeof(DevIssueLabelTypeEnum)).Cast<DevIssueLabelTypeEnum>())
            {
                var item = new SelectListItem()
                {
                    Text = label.ToString(),
                    Value = ((int)label).ToString(),
                    Selected = actualLabels.Any(l => l.ID == (int)label)
                };

                LabelTypes.Add(item);
            }

            foreach (var visibility in Enum.GetValues(typeof(VisibilityOptionEnum)).Cast<VisibilityOptionEnum>())
            {
                var item = new SelectListItem()
                {
                    Text = visibility.ToString(),
                    Value = ((int)visibility).ToString(),
                    Selected = issue.VisibilityOptionID == (int)visibility
                };

                VisiblityOptions.Add(item);
            }

            foreach (var label in actualLabels)
            {
                labelIDs.Add(label.ID);
                Labels.Add(new DevIssueLabelViewModel(label));
            }

            Score = ScoreHelper.ToString(issue.DevIssueVotes.Sum(v => v.Score));
            CanVote = issue.DevIssueVotes.Any(v => v.CitizenID == SessionHelper.LoggedCitizen.ID) == false;
            CanUnvote = !CanVote;

            loadComments(issue);
        }

        private void loadComments(DevIssue issue)
        {
            VisibilityOptionEnum playerVisiblity = VisibilityOptionEnum.Everyone;
            var citizen = SessionHelper.LoggedCitizen;
            if (citizen.ID == issue.CreatedByID)
                playerVisiblity = VisibilityOptionEnum.Author;
            if (citizen.GetPlayerType() == PlayerTypeEnum.Moderator)
                playerVisiblity = VisibilityOptionEnum.Moderators;
            if (citizen.GetPlayerType() >= PlayerTypeEnum.Admin)
                playerVisiblity = VisibilityOptionEnum.Admins;

            Comments = issue.DevIssueComments.Where(c => c.VisibilityOptionID <= (int)playerVisiblity).ToList()
               .Select(c => new DevIssueCommentViewModel(c)).ToList();
        }
    }
}